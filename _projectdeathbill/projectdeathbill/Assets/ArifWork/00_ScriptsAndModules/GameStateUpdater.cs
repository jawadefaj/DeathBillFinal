using UnityEngine;
using System.Collections;

public class GameStateUpdater : BaseWorker {
	public HUDSettings hudSettings;
	protected override void OnStart ()
	{
		HUDManager.UpdateHUDSettings (hudSettings);
		WorkFinished ();
	}
}
