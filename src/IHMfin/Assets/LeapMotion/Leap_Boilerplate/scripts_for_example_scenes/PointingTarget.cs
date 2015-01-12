using UnityEngine;
using System.Collections;

public class PointingTarget : MonoBehaviour {

	private LeapManager _leapManager;
	public Material[] targets;
	public ParticleSystem particles;

	//Converstion from leap coordinates to unity coordinates
	float horizontalRatio = 36.0f / 230.0f;
	float verticalRatio = 24.0f / 180.0f;
	float horizontalOffset = 0.0f;
	float verticalOffset = 220.0f;

	float screenDepth = -150;

	int delayBeforWriting = 20;
	int actualDelay;

	Leap.Vector previousTip = new Leap.Vector();
	Leap.Vector previousPalm = new Leap.Vector();
	Leap.Vector tip;
	Leap.Vector palm;
	Leap.Vector stabilizedTip = new Leap.Vector();
	Leap.Vector stabilizedPalm = new Leap.Vector();

	// Use this for initialization
	void Start () {
		actualDelay = delayBeforWriting;
		_leapManager = (GameObject.Find("LeapManager") as GameObject).GetComponent(typeof(LeapManager)) as LeapManager;
		tip = _leapManager.frontmostHand ().Fingers.Leftmost.TipPosition;
		palm = _leapManager.frontmostHand ().PalmPosition;
		stabilizedTip = tip;
		stabilizedPalm = palm;
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log("tip : " + tip.x +" " + tip.y +" " + tip.z + "\n");
		if(_leapManager.frontmostHand ().Fingers.Count == 1){
			actualDelay--;
			previousTip.x = stabilizedTip.x;
			previousTip.y = stabilizedTip.y;
			previousTip.z = stabilizedTip.z;
			previousPalm.x = stabilizedPalm.x;
			previousPalm.y = stabilizedPalm.y;
			previousPalm.z = stabilizedPalm.z;
			tip = _leapManager.frontmostHand ().Fingers.Leftmost.TipPosition;
			palm = _leapManager.frontmostHand ().PalmPosition;

			//if(tip.DistanceTo(stabilizedTip)<50){
				stabilizedTip = (tip/4 + 3*previousTip/4);
				stabilizedPalm = (palm/4 + 3*previousPalm/4);
			//}

			Leap.Vector intersect = new Leap.Vector();
			intersect.z = screenDepth;
			intersect.x = - (stabilizedTip.x-stabilizedPalm.x+20)*(stabilizedPalm.z-intersect.z)/(stabilizedTip.z-stabilizedPalm.z) + stabilizedPalm.x-20;
			intersect.y = - (stabilizedTip.y-stabilizedPalm.y)*(stabilizedPalm.z-intersect.z)/(stabilizedTip.z-stabilizedPalm.z) + stabilizedPalm.y;


			float x =  (intersect.x - horizontalOffset) * horizontalRatio;
			float y =  (intersect.y - verticalOffset) * verticalRatio;

			Vector3 newPosition = transform.position;
			newPosition.x = x;
			newPosition.y = y;
			newPosition.z = 0;

//			Transform oldPosition = transform.position;
			transform.position = Vector3.MoveTowards(transform.position, newPosition, 5.0f);
			particles.transform.position = transform.position;


			//Debug.Log ("touch: " + touching.x + ", "+ touching.y + ".\n");
			//Target color depends on depth.
			int thresh_low = -50 ;
			int thresh_hight = 30;
			float minScale = 0.08f;
			float maxScale = 0.25f;

			if(stabilizedTip.z < thresh_low){
				renderer.sharedMaterial = targets[1]; //Green > thresh_hight
				if(actualDelay < 0){
					particles.emissionRate = 150;
				}
			}else if (stabilizedTip.z < thresh_hight){
				particles.emissionRate = 0;
				float scale = (thresh_hight-stabilizedTip.z)/(thresh_hight-thresh_low) * minScale +
					(stabilizedTip.z-thresh_low)/(thresh_hight-thresh_low) * maxScale ;
				Vector3 reScale = transform.localScale;
				reScale.x = scale;
				reScale.z = scale;
				transform.localScale = reScale;
				renderer.sharedMaterial = targets[0]; // thresh_low < resized grey < thresh_hight
			}

		}else {
			Vector3 newPosition = transform.position;
			newPosition.x = 0;
			newPosition.y = -33;
			newPosition.z = 0;
			transform.position = Vector3.MoveTowards(transform.position, newPosition, 5.0f);
			actualDelay = delayBeforWriting;
		}
	}
}



