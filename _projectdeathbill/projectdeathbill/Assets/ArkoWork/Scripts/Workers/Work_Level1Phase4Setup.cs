using UnityEngine;
using System.Collections;
using SWS;
using Portbliss.Station;

public class Work_Level1Phase4Setup : BaseWorker {

	public ThirdPersonController korim;
	public ThirdPersonController jamal;
	public Transform jamal_start_pos;
	public Transform korim_start_pos;
	public PathManager korim_newPath;

	protected override void OnStart ()
	{
		korim.RemoveAI();
		jamal.AttachAI(false);

		//initialize korim path
		korim.transform.position = korim_start_pos.position;
		StationController sc = korim.gameObject.GetComponent<StationController>();
		sc.SetPath(korim_newPath);
		sc.UpdateCurrentStationData();

		jamal.transform.position = jamal_start_pos.position;
		jamal.GetStationController().UpdateCurrentStationData();

		WorkFinished();
	}
}
