using UnityEngine;
using System.Collections;

public class Work_SneakyMoveNextStationManual : BaseWorker {

	public static bool canWalk = false;
	private bool isMoving = false;
	protected override void OnUpdate ()
	{
		if(!isMoving)
		{
			if (SneakyPlayerManager.instance.MoveToNextStationManual())
			{
				isMoving = true;
				canWalk = true;
				if( SneakyPlayerManager.instance.OnWalkStateChanged!=null) SneakyPlayerManager.instance.OnWalkStateChanged(true);
				SneakyPlayerManager.instance.GetCurrentPlayer().GetSneakyStationController().OnStationReached+=MoveComplete;

				//show indicator
				IndicatorManager.instance.ShowIndicator(IndicatorType.PlaneIndicator, SneakyPlayerManager.instance.GetCurrentPlayer().GetSneakyStationController().GetStationTransform());
			}
		}

		/*if(isMoving)
		{
			if(Input.GetKeyDown(KeyCode.P))
			{
				SneakyPlayerManager.instance.GetCurrentPlayer().GetSneakyStationController().SpeedUp();
			}
		}*/
	}

	private void MoveComplete()
	{
		SneakyPlayerManager.instance.GetCurrentPlayer().GetSneakyStationController().OnStationReached-=MoveComplete;
		isMoving = false;
		canWalk = false;
		if( SneakyPlayerManager.instance.OnWalkStateChanged!=null) SneakyPlayerManager.instance.OnWalkStateChanged(false);

		//if we have a target then set indicator on it
		if(SneakyPlayerManager.instance.GetStabTarget()!=null)
		{
			//IndicatorManager.instance.ShowIndicator(IndicatorType.BoxIndicator,SneakyPlayerManager.instance.GetStabTarget());
			AIPersonnel ai = SneakyPlayerManager.instance.nextTargetPersonel;
			if (ai != null) {
				if (ai.enabled == true) {
					SneakyPlayerManager.instance.nextTargetPersonel.canvasController.targetIcon.SetActive (true);
				}
			}
				
		}

		//hide indicator
		IndicatorManager.instance.HideIndicator();

		WorkFinished();
	}
}
