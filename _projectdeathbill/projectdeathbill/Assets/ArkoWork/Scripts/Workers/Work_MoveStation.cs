using UnityEngine;
using System.Collections;
using Portbliss.Station;

public class Work_MoveStation : BaseWorker {

	public bool waitForStationReach = true;
	public float executeAfter =0;

	protected override void OnStart ()
	{
		StartCoroutine(StartMove());
	}

	private IEnumerator StartMove()
	{
		yield return new WaitForSeconds(executeAfter);

		bool result = false;

		//move the curent player
		do
		{
			result = PlayerInputController.instance.GUI_MoveNextStation();
			yield return null;
		}while(!result);


		if(!waitForStationReach)
		{
			yield return null;
			PlayerInputController.instance.current_player.GetComponent<StationController>().OnStationReached += StationReachedNoWait;
			WorkFinished();
		}
		else
		{
			PlayerInputController.instance.current_player.GetComponent<StationController>().OnStationReached += StationReached;
		}

		//turn of hud
		HUDManager.instance.forceDisableRun_ShootGroup = true;
	}

	private void StationReached()
	{
		if(waitForStationReach)
		{
			PlayerInputController.instance.current_player.GetComponent<StationController>().OnStationReached -= StationReached;
			HUDManager.instance.forceDisableRun_ShootGroup = false;
		}

		WorkFinished();
	}

	private void StationReachedNoWait()
	{
		HUDManager.instance.forceDisableRun_ShootGroup = false;
	}
}

