using UnityEngine;
using System.Collections;

public class Work_SwitchToAIWork : BaseWorker {

	protected override void OnStart ()
	{
		if(!SneakyPlayerManager.instance.IsStabKillingPending())
		{
			WorkFinished();
		}
		else
			SneakyPlayerManager.instance.SwitchToAIPathFindingWalk(null,true);
	}



	protected override void OnUpdate ()
	{

		if(!SneakyPlayerManager.instance.IsStabKillingPending())
		{
			//kill is done 

			SneakyPlayerManager.instance.GetCurrentPlayer().GetMovementController().StopWalk();
			SneakyPlayerManager.instance.AiPathFindingWalkDone();
			WorkFinished();

		}
	}
}
