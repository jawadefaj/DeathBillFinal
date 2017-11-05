using UnityEngine;
using System.Collections;

public class Work_GameEnder : BaseWorker {

	protected override void OnStart ()
	{
		//Game end tracking
		if (AnalyticsManager.instance != null)
			AnalyticsManager.instance.TrackLevelPlayEndTimeEvent(GeneralManager.instance.level, GeneralManager.instance.phase);
		else
		{
			Debug.Log("Analytic manager instance not found!");
		}

		//succesfully game end
		GeneralManager.EndGame(true);

		WorkFinished();
	}
}
