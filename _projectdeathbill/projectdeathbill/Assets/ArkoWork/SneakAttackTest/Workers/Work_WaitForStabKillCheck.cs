using UnityEngine;
using System.Collections;

public class Work_WaitForStabKillCheck : BaseWorker {

	protected override void OnUpdate ()
	{
		if(!SneakyPlayerManager.instance.IsStabKillingPending())
		{
			WorkFinished();
		}
	}
}
