using UnityEngine;
using System.Collections;

public class Work_SetStabEnemyTarget : BaseWorker {

	public Transform target;

	protected override void OnStart ()
	{
		//SneakyPlayerManager.instance.SetStabTarget(target);
		WorkFinished();
	}
}
