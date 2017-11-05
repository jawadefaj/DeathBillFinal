using UnityEngine;
using System.Collections;

public class AIRoofieReleaseModule : BaseWorker {
	bool wasInitialized = false;
	float startTime;
	bool targetShown = false;
	const float targetShowStartTime = 3f;
	protected override void OnStart ()
	{
		wasInitialized = true;
		foreach (AIPersonnel ai in AIRoofieModule.instance.roofieList) {
			ai.motionState = AIMotionStates.INTERZONEONLY;
			startTime = Time.time;
		}
	}
	protected override void OnUpdate()
	{
		if (wasInitialized) {
			if (!targetShown && Time.time > startTime + targetShowStartTime) {
				foreach (AIPersonnel ai in AIRoofieModule.instance.roofieList) {
					ai.canvasController.targetIcon.SetActive (true);
					targetShown = true;
				}
			}
			if (AIRoofieModule.instance == null) {
				wasInitialized = false;
				Debug.Log ("Roofie release worker finished!");
				WorkFinished ();
			}
			else if (AIRoofieModule.instance.roofieList.Count == 0) {
				wasInitialized = false;
				Debug.Log ("Roofie release worker finished!");

				WorkFinished ();
			}
		}
	}
}
