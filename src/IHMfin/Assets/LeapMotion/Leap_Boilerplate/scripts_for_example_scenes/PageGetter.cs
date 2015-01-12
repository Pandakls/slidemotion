using UnityEngine;
using System.Collections;

public class PageGetter : MonoBehaviour {		
		
		private LeapManager _leapManager;
		private TextMesh text;
		
		private SlideControler slideControler;

		// Use this for initialization
		void Start () {
			text = gameObject.GetComponent(typeof(TextMesh)) as TextMesh;
			//_leapManager = (GameObject.Find("LeapManager") as GameObject).GetComponent(typeof(LeapManager)) as LeapManager;
			GameObject slides = GameObject.Find("Planes");
			slideControler = slides.GetComponent<SlideControler>();
		}
		
		// Update is called once per frame
		void Update () {
		int page = slideControler.finalSlideNumber;
		if (page != 0) {
			text.text = "Page :\n" + slideControler.finalSlideNumber;
		}else {
			text.text = "";
		}
		}
	}

