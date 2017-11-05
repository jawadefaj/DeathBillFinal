using UnityEngine;
using UnityEditor;
using System.Collections;

public class PlayerPrefClearer {

	[MenuItem("Portbliss/Clear Player Pref")]
	public static void ClearPlayerPref()
	{
		PlayerPrefs.DeleteAll ();
		Debug.LogWarning ("Player pref data cleared!");
	}

    /*
	[MenuItem("Portbliss/Unlock Level 1")]
	public static void UnlockLevel1()
	{
		UserGameData udg = new UserGameData(SecurePlayerPrefs.GetString("gameData"));
		udg.UnlockStage (0, 0);
		udg.UnlockStage (0, 1);
		udg.UnlockStage (0, 2);
		udg.UnlockStage (0, 3);
		udg.UnlockStage (1, -1);
		udg.UnlockStage (1, 0);
	}*/
}
