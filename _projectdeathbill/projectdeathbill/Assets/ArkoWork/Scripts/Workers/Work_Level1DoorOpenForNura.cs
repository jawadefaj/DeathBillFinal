using UnityEngine;
using System.Collections;

public class Work_Level1DoorOpenForNura : BaseWorker {

	public DoorController doorForNura;
	public float executeAfter;

	protected override void OnStart ()
	{
		finishWorkManually = true;

		StartCoroutine(DoorOpen());

	}

	IEnumerator DoorOpen()
	{
		WorkFinished();
		yield return new WaitForSeconds(executeAfter);
		doorForNura.OpenDoor();
		FinishWorkManually();
	}
}
