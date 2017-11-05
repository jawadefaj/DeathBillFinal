using UnityEngine;
using System.Collections;

public class Work_SneakyPlayerChange : BaseWorker {

	public FighterRole fighterId;

	protected override void OnStart ()
	{
		StartCoroutine(PlayerChange());
	}

	IEnumerator PlayerChange()
	{
		SneakyPlayerManager.instance.SwitchToPlayer(fighterId);
		yield return new WaitForSeconds(1.5f);
		WorkFinished();
	}
}
