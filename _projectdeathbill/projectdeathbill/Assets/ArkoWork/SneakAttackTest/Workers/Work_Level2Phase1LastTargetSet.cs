using UnityEngine;
using System.Collections;

public class Work_Level2Phase1LastTargetSet : BaseWorker {

	public AIPersonnel noobRajakar;
	protected override void OnStart ()
	{
		SneakyPlayerManager.instance.SetStabTarget(noobRajakar);
		WorkFinished();
	}
}
