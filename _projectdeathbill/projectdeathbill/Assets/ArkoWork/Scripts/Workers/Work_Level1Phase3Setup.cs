using UnityEngine;
using System.Collections;
using Portbliss.Station;

public class Work_Level1Phase3Setup : BaseWorker {

	public ThirdPersonController korim;
	public ThirdPersonController nura;
	public ThirdPersonController taposh;
	public GameObject stuntmanTaposh;
	public GameObject treeClimbCamera;
	public GameObject _canvas;

	public Transform targetWaypoint_korim;
	public Transform targetwaypoint_nura;

	protected override void OnStart ()
	{
		finishWorkManually = true;

		//set up korim and nura for ai
		korim.AttachAI(false);
		nura.AttachAI(false);

		//something else??

		StartCoroutine(FinishWork());
	}

	IEnumerator FinishWork()
	{
		//turn off original taposh
		taposh.gameObject.gameObject.SetActive(false);



		yield return null;

		//turn off canvas
		_canvas.gameObject.SetActive(false);

		//set korim to a target waypoint
		korim.transform.position = targetWaypoint_korim.position;
		nura.transform.position = targetwaypoint_nura.position;
		korim.GetStationController().UpdateCurrentStationData();
		nura.GetStationController().UpdateCurrentStationData();

		//wait for some time to climb taposh
		yield return new WaitForSeconds(3.5f);

		_canvas.gameObject.SetActive(true);

		WorkFinished();

		yield return new WaitForSeconds(3f);

		//trun off nokol taposh
		stuntmanTaposh.SetActive(false);

		//turn on ashol taposh
		taposh.gameObject.SetActive(true);

		//destroy camera
		Destroy(treeClimbCamera);

		//switch to jamal
		PlayerInputController.instance.GM_SwitchToPlayer(FighterRole.Sniper);

		FinishWorkManually();

	}
}
