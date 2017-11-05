using UnityEngine;
using System.Collections;

public class UserGameDataTester : MonoBehaviour {

	UserGameData ugd;
	// Use this for initialization
	void Start () {

		//make a dummy cloud game
		string fromCloud = "{\"k\":\"v\", \"lvl1a\":\"1\", \"lvl1b\":\"1\", \"lvl1c\":\"1\", \"lvl1d\":\"1\"}";

		//SecurePlayerPrefs.SetString("tag0","1123");

		ugd = UserGameData.LoadGame("tt");

		Debug.Log(ugd.GetGameData());

		PrintAllStatus();
	
	}
	

	public void PrintStatus(int lvl, int chk)
	{
		Debug.Log(string.Format("Level {0} ChkPoint {1} isOpen = {2}, Score = {3}",lvl,chk,ugd.IsCheckPointUnlocked(lvl-1,chk-1).ToString(),ugd.GetScore(lvl-1,chk-1)));
	}

	public void PrintAllStatus()
	{
		for(int i=0;i<4;i++)
			PrintStatus(1,i+1);

		for(int i=0;i<3;i++)
			PrintStatus(2,i+1);
	}
}
