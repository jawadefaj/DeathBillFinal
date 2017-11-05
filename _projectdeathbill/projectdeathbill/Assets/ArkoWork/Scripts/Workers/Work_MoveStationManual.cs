using UnityEngine;
using System.Collections;
using Portbliss.Station;

public class Work_MoveStationManual : BaseWorker {

	private StationController sc;

	protected override void OnStart ()
	{
		sc = null;
		StartCoroutine(StartMove());
	}

	protected override void OnUpdate ()
	{
		if(Input.GetKeyDown(KeyCode.P))
		{
			sc.SpeedUp();
		}
	}

	private IEnumerator StartMove()
	{
		bool result = false;

		do
		{
			result = PlayerInputController.instance.GUI_MoveNextStationManual();
			yield return null;
		}while(!result);

		sc = PlayerInputController.instance.current_player.GetComponent<StationController>();
		sc.OnStationReached += StationReached;

		yield return new WaitForSeconds (3.5f);
		HUDManager.TriggerToolTip (ToolTipType.Walk);
	}

	private void StationReached()
	{
		sc.OnStationReached -= StationReached;
		WorkFinished();
	}
}
