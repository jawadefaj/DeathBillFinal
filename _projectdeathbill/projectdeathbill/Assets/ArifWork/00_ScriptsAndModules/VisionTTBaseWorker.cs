using UnityEngine;
using System.Collections;

public class VisionTTBaseWorker : BaseWorker {


	protected override void OnStart ()
	{
		HUDManager.TriggerToolTip (ToolTipType.Vision);
		WorkFinished ();
	}
}
