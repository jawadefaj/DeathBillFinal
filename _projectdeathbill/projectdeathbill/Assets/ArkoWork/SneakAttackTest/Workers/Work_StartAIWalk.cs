using UnityEngine;
using System.Collections;

public class Work_StartAIWalk : BaseWorker {

	public bool useManualWalk = false;

	protected override void OnStart ()
	{
		SneakyPlayerManager.instance.SwitchToAIPathFindingWalk(SneakyPlayerManager.instance.GetCurrentPlayer().GetSneakyStationController().GetNextStationPoint(),useManualWalk);

		SneakyPlayerManager.instance.GetCurrentPlayer().GetMovementController().OnPointReached += AfterPointReached;

		if(!useManualWalk)
			HUDManager.instance.forceDisableRun_ShootGroup= true;
			
	}

	void AfterPointReached()
	{
		SneakyPlayerManager.instance.GetCurrentPlayer().GetSneakyStationController().UpdateStationPoint();
		SneakyPlayerManager.instance.GetCurrentPlayer().GetMovementController().StopWalk();
		SneakyPlayerManager.instance.AiPathFindingWalkDone();
		SneakyPlayerManager.instance.GetCurrentPlayer().GetMovementController().OnPointReached -= AfterPointReached;

		if(!useManualWalk)
			HUDManager.instance.forceDisableRun_ShootGroup = false;
		
		WorkFinished();
	}
}
