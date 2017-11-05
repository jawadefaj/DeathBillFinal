using UnityEngine;
using System.Collections;

public class Work_PlayerChange : BaseWorker {

	public FighterRole switchTo;

	protected override void OnStart ()
	{
		StartCoroutine(SwitchPlayer());
	}

	IEnumerator SwitchPlayer()
	{
		PlayerInputController.instance.GM_SwitchToPlayer(switchTo);
		yield return null;
		WorkFinished();
	}
}
