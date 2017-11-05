using UnityEngine;
using System.Collections;

public class Work_Level2PhaseBPlayerSwitch : BaseWorker {

	public FighterRole selectPlayer;
	public float waitToExecute = 0;
	public float releaseHandleAfter = 0;

	protected override void OnStart ()
	{
		finishWorkManually = true;
		StartCoroutine(SwitchPlayer());
	}

	IEnumerator SwitchPlayer()
	{
		yield return new WaitForSeconds(waitToExecute);

		//go thru all player list
		for(int i=0;i<PlayerInputController.instance.players.Length;i++)
		{
			if(PlayerInputController.instance.players[i].fighterRole == selectPlayer)
			{
				PlayerInputController.instance.players[i].RemoveAI();
                PlayerInputController.instance.GM_SwitchToPlayer(selectPlayer);
			}
			else
			{
				PlayerInputController.instance.players[i].AttachAI(true);
			}
		}
		WorkFinished();
		yield return new WaitForSeconds(releaseHandleAfter);
		FinishWorkManually();
	}
}
