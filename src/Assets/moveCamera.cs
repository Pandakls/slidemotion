using UnityEngine;
using System.Collections;

public class moveCamera : MonoBehaviour {

	void Start ()
	{}
	
	void Update ()
	{
		// Store the input axes.
		float h = Input.GetAxisRaw ("Horizontal")*3.0f;
		float v = Input.GetAxisRaw ("Vertical")*3.0f;
		transform.Translate (new Vector3(h, v, 0.0f));
	}
}
