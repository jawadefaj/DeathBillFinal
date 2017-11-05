using UnityEngine;
using System.Collections;

public class Work_SneakyMoveNextStationAuto : BaseWorker {

	protected override void OnUpdate ()
	{
		if (SneakyPlayerManager.instance.MoveToNextStation())
		{
			SneakyPlayerManager.instance.GetCurrentPlayer().GetSneakyStationController().OnStationReached+=MoveComplete;
			HUDManager.instance.forceDisableRun_ShootGroup = true;
		}

	}

	private void MoveComplete()
	{
		SneakyPlayerManager.instance.GetCurrentPlayer().GetSneakyStationController().OnStationReached-=MoveComplete;
		HUDManager.instance.forceDisableRun_ShootGroup = false;
		WorkFinished();
	}
}
