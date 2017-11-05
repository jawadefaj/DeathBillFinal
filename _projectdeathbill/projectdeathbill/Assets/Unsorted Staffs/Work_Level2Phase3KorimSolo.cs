using UnityEngine;
using System.Collections;

public class Work_Level2Phase3KorimSolo : BaseWorker {

	public static Work_Level2Phase3KorimSolo instance;

	public static System.Action startingEvent;

	protected override void OnSceneAwake ()
	{
		instance = this;
	}

	protected override void OnStart ()
	{
		if (startingEvent != null)
			startingEvent ();
		//WorkFinished ();
	}

	public void FinishMyJob()
	{
		WorkFinished();
	}

}
