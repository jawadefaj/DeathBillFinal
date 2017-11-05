using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeStamper : MonoBehaviour {
	public List<float> stampList = new List<float>();
	// Use this for initialization
	void Start () {
	
	}

	public bool ticking = false;
	float tickStartTime;
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.T)) {
			if (!ticking) {
				tickStartTime = Time.time;
				Debug.Log ("Timer "+(stampList.Count+1).ToString()+" started!");
				ticking = true;
			} else {
				stampList.Add( Time.time - tickStartTime);
				Debug.Log ("Time Stamp: " + stampList[stampList.Count - 1].ToString());
				ticking = false;
			}
		}
	}
}
