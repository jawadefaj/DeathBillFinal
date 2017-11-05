using UnityEngine;
using System.Collections;

public class Work_GameStarter : BaseWorker {

	public int level=1;
	public int phase=1;
	protected override void OnStart ()
	{
		GeneralManager.StartGame(level,phase);

        if (AnalyticsManager.instance != null)
            AnalyticsManager.instance.TrackLevelLoadEvent(level, phase);
        else
        {
            Debug.Log("Analytic manager instance not found!");
        }
		WorkFinished();
	}

}
