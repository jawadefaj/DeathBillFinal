using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour {

	private float doorOpenTime = 1f;
	private bool openDoor = true;

	public void OpenDoor()
	{
		StopCoroutine("StartDoorTransition");
		openDoor = true;
		StartCoroutine("StartDoorTransition");
	}

	public void CloseDoor()
	{
		StopCoroutine("StartDoorTransition");
		openDoor = false;
		StartCoroutine("StartDoorTransition");
	}

	private IEnumerator StartDoorTransition()
	{
		float fromAngle = 0;
		float toAngle = 90;
		float timer = 0.0f; 
		float rate = 0;

		if (!openDoor)
		{
			fromAngle = 90;
			toAngle = 0;
		}
		rate = (toAngle-fromAngle)/doorOpenTime;

		while (timer <= doorOpenTime) {
			float t = 1.0f + Mathf.Pow((timer / doorOpenTime - 1.0f), 3.0f);
			this.transform.Rotate(Vector3.up,rate*Time.deltaTime,Space.World);
			timer += Time.deltaTime;
			yield return null;
		}

	}
}
