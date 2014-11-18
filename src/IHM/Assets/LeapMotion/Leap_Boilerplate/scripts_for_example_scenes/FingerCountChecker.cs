using UnityEngine;
using System.Collections;

public class FingerCountChecker : MonoBehaviour {


	private LeapManager _leapManager;
	private TextMesh text;

	//Player manipulation
	bool wasFive = false;
	bool pinch = false;
	bool isGrabbed = false;
	Leap.Vector initialPinchPosition;
	Leap.Vector initialHandOrientation;
	Leap.Vector movement;
	float angleOfRotation;
	//PlayerMovement playermovment

	// Use this for initialization
	void Start () {
		//Playermovement = player.getcomponent<PlayerMovement>();
		text = gameObject.GetComponent(typeof(TextMesh)) as TextMesh;
		_leapManager = (GameObject.Find("LeapManager") as GameObject).GetComponent(typeof(LeapManager)) as LeapManager;
	}
	
	// Update is called once per frame
	void Update () {
		text.text = "Finger Count: " + _leapManager.frontmostHand().Fingers.Count;

		if (_leapManager.frontmostHand ().Fingers.Count == 5){ // Before pinch
			wasFive = true;
			pinch = false;
			isGrabbed = false;
			//Save and compare initial pinch position
			initialPinchPosition = _leapManager.frontmostHand().PalmPosition;
			initialHandOrientation = initialPinchPosition - _leapManager.frontmostHand().Fingers.Rightmost.StabilizedTipPosition;
			initialHandOrientation.y = 0;
			initialHandOrientation = initialHandOrientation.Normalized;
		} else if (_leapManager.frontmostHand ().Fingers.Count == 0){ // out of screen
			pinch = false;
			wasFive = false;
			isGrabbed = false;
		} else if (wasFive && isPinchOnPlayer()){
			isGrabbed = true;
			wasFive = false;
			pinch = true;
		} else if (wasFive || pinch) { // pinch
			pinch = true;
			wasFive = false;
		}
		if (pinch && !isGrabbed) {
			text.text += "\npinched but missed";
		}
		if (isGrabbed) {
			text.text += "\nGrabbed!";
			//Compute movement from initial position
			movement = _leapManager.frontmostHand().PalmPosition - initialPinchPosition;
			//Compute rotation from initial position
			Leap.Vector finalHandOrientation = _leapManager.frontmostHand().PalmPosition - _leapManager.frontmostHand().Fingers.Rightmost.StabilizedTipPosition;
			finalHandOrientation.y = 0;
			finalHandOrientation = finalHandOrientation.Normalized;
			angleOfRotation =  Mathf.Acos(initialHandOrientation.x*finalHandOrientation.x + initialHandOrientation.y*finalHandOrientation.y);

			//PlayerMovement.Move (movement.x, movement.z);
			//PlayerMovement.Rotate(angleOfRotation);

			//Set new initial start for next iterations
			initialHandOrientation = finalHandOrientation;
			initialPinchPosition = _leapManager.frontmostHand().PalmPosition;

		}

	}

	bool isPinchOnPlayer () {
		return true;
	}
}
