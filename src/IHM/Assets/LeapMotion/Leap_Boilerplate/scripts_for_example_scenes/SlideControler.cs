using UnityEngine;
using System.Collections;

public class SlideControler : MonoBehaviour {
	public GameObject[] slides;
	private LeapManager _leapManager;

	bool rightMoveRequired = false;
	bool leftMoveRequired = false;
	bool fingerRecognition = true;
	bool pageMoveRequired = false;
	bool inMovement = false;

	float horizontalSpeed = 0.0f;
	float horizontalPosition = 0.0f;
	float previousHorizontalPosition = 0.0f;

	int actualSlide = 0;

	int fingerNumberRecog = 0;
	int tempSlideNumber = 0;
	int finalSlideNumber = 0;

	int sleepSlide = 0;
	int sleepRecog = 0;
	int sleepTime = 10;
	int state = 0;

	// Use this for initialization
	void Start () {
		_leapManager = (GameObject.Find("LeapManager") as GameObject).GetComponent(typeof(LeapManager)) as LeapManager;
	}
	
	// Update is called once per frame
	void Update () {
		if(inMovement){
			moveTo(actualSlide);
			return;
		}
		previousHorizontalPosition = horizontalPosition;
		horizontalPosition = _leapManager.frontmostHand ().PalmPosition.x;
		horizontalSpeed = horizontalPosition - previousHorizontalPosition;

		if (sleepSlide <= 0){
			if (horizontalSpeed < -20) {
				rightMoveRequired = true;
				sleepSlide = 2*sleepTime;
			} else if (horizontalSpeed > 20) {
				leftMoveRequired = true;
				sleepSlide = 2*sleepTime;
			}
		}else {
			sleepSlide -=1;
		}

		if(horizontalSpeed < 0.4 && horizontalSpeed > -0.4 && (fingerRecognition || sleepRecog <= 0 )){
			pageRecognition(); //Machine à etat
			sleepRecog = sleepTime;
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
			if(finalSlideNumber > 0){
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
		actualSlide = numSlide;
		Vector3 newPosition = transform.position;
		newPosition.x -= slides [actualSlide].transform.position.x;
		transform.position = Vector3.MoveTowards(transform.position, newPosition, 6.0f);
		if (newPosition == transform.position){
			inMovement = false;
		}
	}

	void pageRecognition(){
		if (fingerRecognition){
			fingerNumberRecog = _leapManager.frontmostHand ().Fingers.Count;
			if(fingerNumberRecog != 0){
				fingerRecognition = false;
			}else if(finalSlideNumber != 0){
				pageMoveRequired = true;
			}
		} else {
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
				}

				if(_leapManager.frontmostHand ().Fingers.Count != fingerNumberRecog){
					state = 0;
					fingerRecognition = true;
					if (_leapManager.frontmostHand ().Fingers.Count == 0){
						pageMoveRequired = true;
					}
				}
				break;
			default:
				break;
			}
			Debug.Log("page recog :"+finalSlideNumber+"\n");
			Debug.Log("State:"+state+"\n");
		}
	}

}
