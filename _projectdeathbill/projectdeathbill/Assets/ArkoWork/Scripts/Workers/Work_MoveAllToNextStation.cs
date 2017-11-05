using UnityEngine;
using System.Collections;
using Portbliss.Station;

public class Work_MoveAllToNextStation : BaseWorker {

	public bool waitForStationReach = true;

	protected override void OnStart ()
	{
		StartCoroutine(StartMove());
	}

	private IEnumerator StartMove()
	{
		bool result = false;

		do
		{
			result = PlayerInputController.instance.GUI_MoveNextStation();
			yield return null;
		}while(!result);

		//move all ai players
		for(int i=0;i<PlayerInputController.instance.aiPlayers.Count;i++)
		{
			PlayerInputController.instance.aiPlayers[i].GetAI().MoveStation();
		}


		if(!waitForStationReach)
		{
			yield return null;
			WorkFinished();
		}
		else
		{
			PlayerInputController.instance.current_player.GetComponent<StationController>().OnStationReached += StationReached;
			HUDManager.instance.forceDisableRun_ShootGroup = true;
		}
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
}
