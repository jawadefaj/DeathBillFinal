using UnityEngine;
using System.Collections;

public class HUDToggler : BaseWorker {
	public HUDSettings hudSettings1;
	public HUDSettings hudSettings2;
	protected override void OnStart ()
	{
		if (HUDManager.hudSettings == hudSettings1) {
			HUDManager.UpdateHUDSettings (hudSettings2);
		} else {
			HUDManager.UpdateHUDSettings (hudSettings1);
		}
		WorkFinished ();
	}
}
