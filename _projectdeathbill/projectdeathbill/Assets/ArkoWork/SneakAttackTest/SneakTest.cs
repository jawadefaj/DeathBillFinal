using UnityEngine;
using System.Collections;
using Portbliss.Station;
using SWS;

public class SneakTest : MonoBehaviour {

	public splineMove sm;
	private bool isSpeedUp = false;

	// Use this for initialization
	void Start () {
	

	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Space))
		{
			isSpeedUp = true;
		}
		if(Input.GetKeyDown(KeyCode.S))
		{
			sm.ChangeSpeed(5f);
			sm.StartMove();
			StartCoroutine(SpeedDown());
		}
	}

	IEnumerator SpeedDown()
	{
		do
		{
			if(isSpeedUp)
			{
				isSpeedUp = false;
				sm.ChangeSpeed(Mathf.Clamp(sm.speed+1f,0,8f));
			}
			else
			{
				sm.ChangeSpeed(Mathf.Clamp(sm.speed-5*Time.deltaTime,0,8f));
			}
			yield return null;
		}while(true);
	}
}
