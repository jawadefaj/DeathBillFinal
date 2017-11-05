using UnityEngine;
using System.Collections;

public class PhaseEndGameReset : BaseWorker {

	protected override void OnStart ()
	{
		KnifeAnimationManager.instance.ResetSettings();
		WorkFinished();
	}
}
