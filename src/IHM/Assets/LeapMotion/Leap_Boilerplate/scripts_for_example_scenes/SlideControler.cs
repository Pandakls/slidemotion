using UnityEngine;
using System.Collections;

public class SlideControler : MonoBehaviour {
	public GameObject[] slides;
	private LeapManager _leapManager;

	bool rightMoveRequired = false;
	bool leftMoveRequired = false;
	bool fingerRecognition = true;
	bool pageMoveRequired = false;

	float horizontalSpeed = 0.0f;
	float horizontalPosition = 0.0f;
	float previousHorizontalPosition = 0.0f;

	int actualSlide = 0;
	int slideNumber = 0;

	int sleepSlide = 0;
	int sleepRecog = 0;
	int sleepTime = 15;
	int state = 0;

	// Use this for initialization
	void Start () {
		_leapManager = (GameObject.Find("LeapManager") as GameObject).GetComponent(typeof(LeapManager)) as LeapManager;
	}
	
	// Update is called once per frame
	void Update () {
		previousHorizontalPosition = horizontalPosition;
		horizontalPosition = _leapManager.frontmostHand ().PalmPosition.x;
		horizontalSpeed = horizontalPosition - previousHorizontalPosition;

		if (sleepSlide <= 0){
			if (horizontalSpeed < -21) {
				rightMoveRequired = true;
				sleepSlide = 2*sleepTime;
			} else if (horizontalSpeed > 21) {
				leftMoveRequired = true;
				sleepSlide = 2*sleepTime;
			}
		}else {
			sleepSlide -=1;
		}

		if(sleepRecog <= 0 && horizontalSpeed < 0.5 && horizontalSpeed > -0.5){
			pageRecognition(); //Machine à etat
			sleepRecog = sleepTime;
		}else {
			sleepRecog --;
		}
		
		if(rightMoveRequired ){
			moveRight();
			Debug.Log("Move Right\n");
			rightMoveRequired = false;
		}
		if (leftMoveRequired ) {
			moveLeft();
			Debug.Log("Move Left\n");
			leftMoveRequired = false;
		}
		if (pageMoveRequired) {
			if(slideNumber > 0){
				moveTo (slideNumber);
				Debug.Log("Move to Page"+slideNumber+ "\n");
				pageMoveRequired = false;
			}
		}
	}

	void moveRight(){
		if (actualSlide + 1 < slides.GetLength(0)) {
			actualSlide += 1;
			moveTo(actualSlide);
		}
	}

	void moveLeft(){
		if (actualSlide > 0) {
			actualSlide -= 1;
			moveTo(actualSlide);
		}
	}

	void moveTo(int numSlide){
		actualSlide = numSlide;
		transform.Translate(-slides[actualSlide].transform.position.x, 0, 0);
	}

	void pageRecognition(){
		if (fingerRecognition){
			slideNumber = _leapManager.frontmostHand ().Fingers.Count;
			if(slideNumber != 0){
				fingerRecognition = false;
			}
		} else {
			Debug.Log("page recog :"+slideNumber+"\n");
			Debug.Log("State:"+state+"\n");
			switch (state){
			case 0:
			case 1:
			case 2:
				if(_leapManager.frontmostHand ().Fingers.Count == slideNumber){
					state += 1;
				}else{
					state = 0;
					fingerRecognition = true;
				}
				break;
			case 3:
				if(_leapManager.frontmostHand ().Fingers.Count != slideNumber){
					state = 0;
					fingerRecognition = true;
					pageMoveRequired = true;
				}
				break;
			default:
				break;
			}
		}
	}

}
