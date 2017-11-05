using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Work_Endurance_OptionalPlayerSetup : BaseWorker {

	protected override void OnStart ()
	{
		SetUpPlayers (EnduranceManager.ShortRangePlayers, EnduranceManager.ShortRangePlayer, FighterRole.Heavy);
		SetUpPlayers (EnduranceManager.MidRangePlayers, EnduranceManager.MidRangePlayer, FighterRole.Leader);
		SetUpPlayers (EnduranceManager.LongRangePlayers, EnduranceManager.LongRangePlayer, FighterRole.Sniper);

		WorkFinished ();
	}

	private void SetUpPlayers(List<FighterName> availablePlayers, FighterName selectedPlayer, FighterRole setRole)
	{
		//short range setup
		ThirdPersonController tpc;

		for (int i = 0; i < availablePlayers.Count; i++)
		{
			tpc = PlayerInputController.instance.GetPlayerByID (availablePlayers [i]);

			if (tpc != null)
			{
				if (availablePlayers [i] == selectedPlayer)
				{
					//this player will play
					tpc.fighterRole = setRole;
					tpc.gameObject.SetActive (true);
					//Debug.Log ("activating "+ tpc.fighterName.ToString());
				}
				else
				{
					//NOTE: If we have over laping list theb we need to do some work here
					//switch this player off
					tpc.fighterRole = FighterRole.NotActive;
					tpc.gameObject.SetActive (false);
					//Debug.Log ("deactivating "+ tpc.fighterName.ToString());
				}
			}
			else
			{
				Debug.LogError (string.Format("The player {0} does not exist in the player list",availablePlayers [i].ToString()));
			}
		}
	}
}
