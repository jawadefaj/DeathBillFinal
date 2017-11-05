using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;


public class GoogleAnalyticsTracker : MonoBehaviour,IAnalyticsData {

	public static GoogleAnalyticsTracker instance;

	public GoogleAnalyticsV4 gav4;		//************************************

	/*private long survivalTime;

	private long timePlayedWith303=0;
	private long timePlayedWithFal=0;
	private long timePlayedWithStainGun=0;
	
	private float Gun303LastTime =0;
	private float GunFalLastTime =0;
	private float GunStainLastTime =0;

	private bool isTrackingSurvivalTime = false;*/

	void Awake()
	{
		instance = this;
		DontDestroyOnLoad(this);
	}

	void Start () {
		gav4.StartSession();		//*************************************************
		StartCoroutine(Dispatch_Hits());

		gav4.LogEvent("Status", "GameStartHit", "Sent From Splash Screen", 0);		//****************************************
	}

	void OnApplicationQuit()
	{
		gav4.StopSession();		//***************************************

		/*ReportTotalTimePlayedWithDifferentCharecter();

		//chekc if we had suddenly closed the game
		if(isTrackingSurvivalTime)
		{
			ReportRageQuitingEvent();
		}*/

		Debug.Log ("Application quit occur");
	}

    public void TrackLevelLoadEvent(int lvl, int stage)
    {
        string txt = string.Format("L:{0} S:{1}",lvl,stage);
        gav4.LogEvent("GameStat", "LevelLoad", txt, 0);
    }

    public void TrackLevelRestartEvent(int lvl, int stage, long tryTime)
    {
        string txt = string.Format("L:{0} S:{1}",lvl,stage);
        gav4.LogEvent("GameStat", "Restarted", txt, tryTime);
    }

    public void TrackLevelFinishTimeEvent (int level, int stage, long time)
    {
        string txt = string.Format("L:{0} S:{1}",level,stage);
        gav4.LogEvent("GameStat", "LevelFinishTiming", txt, time);
    }

    public void TrackGameLeaveEvent(int lvl, int stage,long no_of_tries)
    {
        string txt = string.Format("L:{0} S:{1}",lvl,stage);
        gav4.LogEvent("GameStat", "PlayerLeave", txt, no_of_tries);
    }

    public void TrackGameRating(int rate)
    {
        gav4.LogEvent("GameDescription", "Rating", "Value", rate);
    }
    public void TrackUserSpecifiqData(string countryName, string deviceModel, string graphicsDeviceName, string graphicsDeviceVendor, string systemMemorySize)
    {
        gav4.LogEvent("UserDescription", "Country", countryName, 0);
        gav4.LogEvent("UserDescription", "DeviceModel", deviceModel, 0);
        gav4.LogEvent("UserDescription", "GraphicsDeviceName", graphicsDeviceName, 0);
        gav4.LogEvent("UserDescription", "GraphicsDeviceVendor", graphicsDeviceVendor, 0);
        gav4.LogEvent("UserDescription", "SystemMemorySize", systemMemorySize, 0);
    }

    public void TrackSceneEntryEvent(string sceneName)
    {
        gav4.LogEvent("GameStat", "SceneEntry", sceneName, 0);
    }

	IEnumerator Dispatch_Hits()
	{
		yield return StartCoroutine(TestConnection());

		if(thereIsConnection)
		{
			Debug.Log ("Found internet connection and dispatching hit");
			gav4.DispatchHits();		//*********************************************
		}
		else
		{
			Debug.Log ("No internet connection found!");
		}
	}

	/*
	//Public Methods for external Call
	//-----------------------------------------------------------

    public void ReportGranadeClaim()
    {
        gav3.LogEvent("Inventory", "Nade Claim", "Nade Claimed", 1);
    }

    public void ReportCoverFireClaim()
    {
        gav3.LogEvent("Inventory", "Cover Claim", "Cover Claimed", 1);
    }

    public void ReportIlliteratePlayer()
    {
        gav3.LogEvent("Inventory", "No Claim", "Nothing Claimed", 1);
    }

	public void ReportAddClickButtonEvent()
	{
		gav3.LogEvent("Add Click","Clicked","Clicked on a add",1);
	}

	public void StartSurvivalTimeTracking()
	{
		survivalTime = (long)(Time.time);
		isTrackingSurvivalTime = true;
	}

	public void StopSurvivalTimeTracking(bool won, bool didComplt = true)
	{
		isTrackingSurvivalTime = false;
		survivalTime = ((long)(Time.time)-survivalTime);
		string timingLabel = won?"won":"lost";
		if(!didComplt) timingLabel = "DidntComplt";

		gav3.LogTiming("Survival Time",survivalTime*1000,timingLabel,"nothing");
        //Debug.Log("Survival time tracked down "+survivalTime*1000);
	}

	public void ReportGameFailEvent()
	{
		gav3.LogEvent("Game Stat","Failed","Camp Recaptured",1);
	}

	public void ReportGameWinEvent()
	{
		gav3.LogEvent("Game Stat","Won","Camp Defended",1);
	}

	public void ReportGameRestartEvent(string reasonToRestart)
	{
		gav3.LogEvent("Game Stat", "Restarted",reasonToRestart,1);
	}

	public void RespotAchievementUnlocked(string achievementName, long value)
	{
		gav3.LogEvent("Achievement", "Unlocked", achievementName,value);
	}

	public void ReportAverageFPS(int fps)
	{
		gav3.LogTiming("FPS Counter",fps*1000,"FPS1","FPS2");
		Debug.Log("Average fps reported as "+fps.ToString());
	}

	public void ReportStartingPlayedWith(FreedomFighter fighter)
	{
		if(fighter == FreedomFighter.Jamal)
		{
			Gun303LastTime = Time.time;
		}
		else if (fighter == FreedomFighter.Korim)
		{
			GunStainLastTime = Time.time;
		}
		else if (fighter == FreedomFighter.Nura)
		{
			GunFalLastTime = Time.time;
		}
	}

	public void ReportStoppedPlayedWith(FreedomFighter fighter)
	{
		if(fighter == FreedomFighter.Jamal)
		{
			timePlayedWith303 += (int)(Time.time-Gun303LastTime);
			Gun303LastTime=0;
		}
		else if (fighter == FreedomFighter.Korim)
		{
			timePlayedWithStainGun += (int)(Time.time-GunStainLastTime);
			GunStainLastTime=0;
		}
		else if (fighter == FreedomFighter.Nura)
		{
			timePlayedWithFal += (int)(Time.time-GunFalLastTime);
			GunFalLastTime=0;
		}
	}

	private void ReportRageQuitingEvent()
	{
		gav3.LogEvent("Quiting Game","Rage Quit","no label",1);
	}

	private void ReportTotalTimePlayedWithDifferentCharecter()
	{
		StopTrackingAllPlayers();

		gav3.LogTiming("Character Playing",timePlayedWith303*1000,"Jamal","303Rifel");
		gav3.LogTiming("Character Playing",timePlayedWithFal*1000,"Nura","FAL");
		gav3.LogTiming("Character Playing",timePlayedWithStainGun*1000,"Korim","StainGUn");

		Debug.Log ("Jamal : "+ timePlayedWith303);
		Debug.Log ("Korim : "+ timePlayedWithStainGun);
		Debug.Log ("Nura : "+ timePlayedWithFal);
	}

	public void StopTrackingAllPlayers()
	{
		//calculate any untracked player
		if(Gun303LastTime!=0) ReportStoppedPlayedWith(FreedomFighter.Jamal);
		if(GunStainLastTime!=0) ReportStoppedPlayedWith(FreedomFighter.Korim);
		if(GunFalLastTime!=0) ReportStoppedPlayedWith(FreedomFighter.Nura);
	}*/

	bool thereIsConnection = false;
	
	IEnumerator TestConnection()
	{
		
		
		float timeTaken = 0.0F;
		float maxTime = 2.0F;
		
		while ( true )
		{
			Ping testPing = new Ping("8.8.8.8");	//74.125.79.99
			
			timeTaken = 0.0F;
			
			while ( !testPing.isDone )
			{
				
				timeTaken += Time.deltaTime;
				
				
				if ( timeTaken > maxTime )
				{
					// if time has exceeded the max
					// time, break out and return false
					thereIsConnection = false;
					yield break;
				}
				
				yield return null;
			}
			if ( timeTaken <= maxTime ) 
			{
				thereIsConnection = true;
				yield break;
			}
			yield return null;
		}
	}



}
