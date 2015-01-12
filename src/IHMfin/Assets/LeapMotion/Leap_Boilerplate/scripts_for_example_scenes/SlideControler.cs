using UnityEngine;
using System.Collections;
using Leap;

public class SlideControler : MonoBehaviour {
	public GameObject[] slides;
	public ParticleSystem particles;
	private LeapManager _leapManager;

	private Leap.Vector _palm; 

	bool rightMoveRequired = false;
	bool leftMoveRequired = false;
	bool fingerRecognition = true;
	bool pageMoveRequired = false;
	bool inMovement = false;

	float horizontalSpeed = 0.0f;
	float horizontalPosition = 0.0f;
	float previousHorizontalPosition = 0.0f;

	int actualSlide = 0;
	int thresholdSensibilitySliding = 12;

	int fingerNumberRecog = 0;
	int tempSlideNumber = 0;
	public int finalSlideNumber = 0;

	bool previousDirectionWasRight;
	int sleepSlide = 0;
	int sleepTimeSlideRight = 10;
	int sleepTimeSlideWrong = 40;

	int sleepRecog = 0;
	int sleepTimeRecog = 12;
	int state = 0;

	// Use this for initialization
	void Start () {
		_leapManager = (GameObject.Find("LeapManager") as GameObject).GetComponent(typeof(LeapManager)) as LeapManager;
		horizontalPosition = _leapManager.frontmostHand ().Fingers.Frontmost.TipPosition.x;
	}
	
	// Update is called once per frame
	void Update () {
		//Si les slides sont en mouvement aucune action n'est effectuée
		if(inMovement){
			moveTo(actualSlide);
			return;
		}
		
		_palm = _leapManager.frontmostHand ().PalmPosition;
		//Gestion des effets de bord
		if (_palm.x > 150 || _palm.x < -150 ||
		    _palm.y > 350 || _palm.y < 50 ||
		    _palm.z > 150 || _palm.z < -100 ){
			return;
		}

		previousHorizontalPosition = horizontalPosition;
		horizontalPosition = (2*Mathf.Sign(_leapManager.frontmostHand ().Fingers.Rightmost.TipPosition.x)
		                      *Mathf.Sqrt (Mathf.Abs(_leapManager.frontmostHand ().Fingers.Rightmost.TipPosition.x)) + _palm.x)/2 ;
		horizontalSpeed = horizontalPosition - previousHorizontalPosition;
		if (_leapManager.frontmostHand ().Fingers.Count == 1) {
			thresholdSensibilitySliding = 20;
		} else {
			thresholdSensibilitySliding = 10;
		}

		if (horizontalSpeed < -thresholdSensibilitySliding && (sleepSlide <= 0 || (previousDirectionWasRight && sleepSlide <= sleepTimeSlideWrong-sleepTimeSlideRight ))) {
			rightMoveRequired = true;
			previousDirectionWasRight = true;
			sleepSlide = sleepTimeSlideWrong;
		} else if (horizontalSpeed > thresholdSensibilitySliding && (sleepSlide <= 0 || (!previousDirectionWasRight && sleepSlide <= sleepTimeSlideWrong-sleepTimeSlideRight ))) {
			leftMoveRequired = true;
			previousDirectionWasRight = false;
			sleepSlide = sleepTimeSlideWrong;
		} else if ((_palm.y < 65 ) && sleepSlide <= 0 && finalSlideNumber !=0){
			pageMoveRequired = true;
		}
		sleepSlide -=1;


		if(horizontalSpeed < 0.05 && horizontalSpeed > -0.05 && (fingerRecognition || sleepRecog <= 0 ) 
		   && (_leapManager.frontmostHand ().Fingers.Leftmost.TipPosition.y < 160 || _leapManager.frontmostHand ().Fingers.Count > 1)){
			pageRecognition(); //Machine à etat
			sleepRecog = sleepTimeRecog;
		}else if(_leapManager.frontmostHand ().Fingers.Count == 0){
			sleepRecog = sleepTimeRecog;
			finalSlideNumber = 0;
			fingerRecognition = true;
		}else {
			sleepRecog --;
		}
		
		if(rightMoveRequired ){
			moveRight();
			Debug.Log("Move Right\n");
			rightMoveRequired = false;
			inMovement = true;
		}
		if (leftMoveRequired ) {
			moveLeft();
			Debug.Log("Move Left\n");
			leftMoveRequired = false;
			inMovement = true;
		}
		if (pageMoveRequired) {
			if(finalSlideNumber > 1){
				moveTo (Mathf.Min(slides.GetLength(0)-1, finalSlideNumber));
				Debug.Log("Move to Page : "+finalSlideNumber+ "\n");
				pageMoveRequired = false;
				finalSlideNumber = 0;
				inMovement = true;
			}
		}
	}

	void moveRight(){
		if (finalSlideNumber != 0) {
			moveTo (Mathf.Min(slides.GetLength(0)-1,actualSlide + finalSlideNumber));
			finalSlideNumber = 0;
		}else if (actualSlide + 1 < slides.GetLength(0)) {
			moveTo(actualSlide+1);
		}
		state = 0;
		fingerRecognition = true;
	}

	void moveLeft(){
		if (finalSlideNumber != 0) {
			moveTo (Mathf.Max(0,actualSlide - finalSlideNumber));
			finalSlideNumber = 0;
		}else if (actualSlide > 0) {
			moveTo(actualSlide-1);
		}
		state = 0;
		fingerRecognition = true;
	}

	void moveTo(int numSlide){
		if (particles) {
			particles.Clear (true);
		}
		actualSlide = numSlide;
		Vector3 newPosition = transform.position;
		newPosition.x -= slides [actualSlide].transform.position.x;
		transform.position = Vector3.MoveTowards(transform.position, newPosition, 5.0f);
		if (newPosition == transform.position){
			inMovement = false;
		}
	}

	void pageRecognition(){
		if (fingerRecognition){
			fingerNumberRecog = _leapManager.frontmostHand ().Fingers.Count;
			if (fingerNumberRecog != 0){
				fingerRecognition = false;
			}
		} else {
			Debug.Log("State:"+state+", ");
			Debug.Log("page recog :"+finalSlideNumber+"\n");
			switch (state){
			case 0:
			case 1:
			case 2:
				if(_leapManager.frontmostHand ().Fingers.Count == fingerNumberRecog){
					state += 1;
					tempSlideNumber = fingerNumberRecog;
				}else{
					state = 0;
					fingerRecognition = true;
				}
				break;
			case 3:
				if (finalSlideNumber == 0){
					finalSlideNumber = tempSlideNumber;
					tempSlideNumber = 0;
				}else if(finalSlideNumber == 5 && tempSlideNumber !=0){
					finalSlideNumber += tempSlideNumber;
					tempSlideNumber = 0;
				}else if (finalSlideNumber < 10 && tempSlideNumber !=0){
					finalSlideNumber *= 10;
					finalSlideNumber += tempSlideNumber;
					tempSlideNumber = 0;
				}else{
					finalSlideNumber += tempSlideNumber;
					tempSlideNumber = 0;
				}

				if(_leapManager.frontmostHand ().Fingers.Count != fingerNumberRecog){
					state = 0;
					fingerRecognition = true;
					if (_leapManager.frontmostHand ().Fingers.Count == 0){
						finalSlideNumber = 0;
					}
				}
				break;
			default:
				break;
			}

		}
	}
}
