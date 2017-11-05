using UnityEngine;
using System.Collections;

public class Work_MoveToGameEndingPoint : BaseWorker {

	public Transform target;
	public Transform rajakar;

	private Transform player; 

	protected override void OnStart ()
	{
		player = SneakyPlayerManager.instance.GetCurrentPlayer().transform;
		SneakyPlayerManager.instance.SwitchToAIPathFindingWalk(target,true);

		//IndicatorManager.instance.ShowIndicator(IndicatorType.BoxIndicator,rajakar);
	}

	protected override void OnUpdate ()
	{
		if(Vector3.SqrMagnitude(target.position-player.position)<2.5f)
			WorkFinished();
	}
}
