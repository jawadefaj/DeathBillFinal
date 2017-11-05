using UnityEngine;
using System.Collections;

public class Work_Level1Phase3NuraLatePosSwitch : BaseWorker {

	public ThirdPersonController nura;
	public float executeAfter = 2f;
	public HUDSettings hudSettings;
	protected override void OnStart ()
	{
		finishWorkManually = true;
		StartCoroutine("IE_ExecuteMainWork");
		WorkFinished();
	}

	private IEnumerator IE_ExecuteMainWork()
	{
		yield return new WaitForSeconds(executeAfter);
		nura.GetAI().MoveStation();
		HUDManager.UpdateHUDSettings (hudSettings);
		FinishWorkManually();
	}
}
