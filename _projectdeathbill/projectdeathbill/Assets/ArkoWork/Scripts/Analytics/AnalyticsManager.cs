using UnityEngine;
using System.Collections;

public class AnalyticsManager : MonoBehaviour,IAnalyticsData {

    public static AnalyticsManager instance;

	private const string KEY_LAST_LEVEL = "last_level";
	private const string KEY_LAST_STAGE = "last_stage";
    private const string KEY_LAST_TRY_LEVELID = "last_try_level";
    private const string KEY_LAST_TRY_COUNT = "last_try";

	private long startTime=0;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        string country = LocationFinder.GetCountryRaw();
        string deviceModelName = SystemInfo.deviceModel;
        string graphicsDeviceName = SystemInfo.graphicsDeviceName;
        string graphicsDeviceVendor = SystemInfo.graphicsDeviceVendor;
        string systemmemorySize = SystemInfo.systemMemorySize.ToString() + " MB";
        TrackUserSpecifiqData(country,deviceModelName, graphicsDeviceName, graphicsDeviceVendor,systemmemorySize);
    }


	public void TrackLevelLoadEvent(int lvl, int stage)
	{
        //dispatch any previous hit
        int pre_lvl = PlayerPrefs.GetInt(KEY_LAST_LEVEL,-1);
        int pre_stage = PlayerPrefs.GetInt (KEY_LAST_STAGE, -1);
        int pre_try = PlayerPrefs.GetInt(KEY_LAST_TRY_COUNT, 1);

        if (pre_lvl >0 && pre_stage >0) {
            TrackGameLeaveEvent (pre_lvl, pre_stage,pre_try);
            //clear last play data
            PlayerPrefs.SetInt(KEY_LAST_LEVEL,-1);
            PlayerPrefs.SetInt (KEY_LAST_STAGE, -1);
            PlayerPrefs.Save ();
        }




		Debug.Log("Tracking level load event");
        UnityAnalyticsManager.instance.TrackLevelLoadEvent(lvl, stage);
		TapjoyManager.instance.TrackLevelLoadEvent(lvl, stage);    
		GoogleAnalyticsTracker.instance.TrackLevelLoadEvent(lvl, stage);


		//Start counting time
		TrackLevelPlayStartTimeEvent();

        //save new data for later use
        PlayerPrefs.SetInt(KEY_LAST_LEVEL,lvl);
        PlayerPrefs.SetInt (KEY_LAST_STAGE, stage);
        PlayerPrefs.Save ();
	}

    public void TrackLevelRestartEvent(int lvl, int stage, long invalidTime=0)
    {
        //track down play time and send restart hit
        long totalTime = (long)Time.time - startTime;
        startTime = 0;
        UnityAnalyticsManager.instance.TrackLevelRestartEvent(lvl, stage, totalTime);
        TapjoyManager.instance.TrackLevelRestartEvent(lvl, stage, totalTime);
        GoogleAnalyticsTracker.instance.TrackLevelRestartEvent(lvl, stage, totalTime);

        //increase number of try count
        string thisLevel = string.Format("L:{0} S:{1}",lvl,stage);
        string prevLevelTry = PlayerPrefs.GetString(KEY_LAST_TRY_LEVELID, "null");
        int lastTry = PlayerPrefs.GetInt(KEY_LAST_TRY_COUNT, 1);

        if (string.Equals(thisLevel, prevLevelTry))
        {
            //this is a old level. increase one;
            lastTry++;
            PlayerPrefs.SetInt(KEY_LAST_TRY_COUNT,lastTry);
        }
        else
        {
            //this is a new level try to recount
            lastTry =2;
            PlayerPrefs.SetString(KEY_LAST_TRY_LEVELID, thisLevel);
            PlayerPrefs.SetInt(KEY_LAST_TRY_COUNT, lastTry);
        }

        //clear any last play data
        PlayerPrefs.SetInt(KEY_LAST_LEVEL,-1);
        PlayerPrefs.SetInt(KEY_LAST_STAGE, -1);
        PlayerPrefs.Save();
    }

    public void TrackGameLeaveEvent(int lvl, int stage, long tryCount)
    {
        Debug.Log("Tracking Game leave event");
        UnityAnalyticsManager.instance.TrackGameLeaveEvent(lvl, stage, tryCount);
        TapjoyManager.instance.TrackGameLeaveEvent(lvl, stage, tryCount);
        GoogleAnalyticsTracker.instance.TrackGameLeaveEvent(lvl, stage, tryCount);
    }

	public void TrackLevelPlayStartTimeEvent ()
	{
		startTime = (long)Time.time;
	}

	public void TrackLevelPlayEndTimeEvent (int level, int stage)
	{
		long totalTime = (long)Time.time - startTime;
		startTime = 0;

        TrackLevelFinishTimeEvent (level, stage, totalTime);

		//clear last play data
		PlayerPrefs.SetInt(KEY_LAST_LEVEL,-1);
		PlayerPrefs.SetInt (KEY_LAST_STAGE, -1);
        PlayerPrefs.SetString(KEY_LAST_TRY_LEVELID, "null");
		PlayerPrefs.Save ();
	}

    public void TrackLevelFinishTimeEvent (int level, int stage, long time)
	{
        UnityAnalyticsManager.instance.TrackLevelFinishTimeEvent (level, stage, time);
        TapjoyManager.instance.TrackLevelFinishTimeEvent (level, stage, time);
        GoogleAnalyticsTracker.instance.TrackLevelFinishTimeEvent(level, stage, time);
	}

    public void TrackGameRating(int rate)
    {
        UnityAnalyticsManager.instance.TrackGameRating(rate);
        TapjoyManager.instance.TrackGameRating (rate);
        GoogleAnalyticsTracker.instance.TrackGameRating(rate);
    }

    public void TrackUserSpecifiqData(string countryName, string deviceModel, string graphicsDeviceName, string graphicsDeviceVendor, string systemMemorySize)
    {
        UnityAnalyticsManager.instance.TrackUserSpecifiqData(countryName, deviceModel,graphicsDeviceName,graphicsDeviceVendor,systemMemorySize);
        TapjoyManager.instance.TrackUserSpecifiqData(countryName, deviceModel,graphicsDeviceName,graphicsDeviceVendor,systemMemorySize);
        GoogleAnalyticsTracker.instance.TrackUserSpecifiqData(countryName, deviceModel,graphicsDeviceName,graphicsDeviceVendor,systemMemorySize);
    }

    public void TrackSceneEntryEvent(string sceneName)
    {
        UnityAnalyticsManager.instance.TrackSceneEntryEvent(sceneName);
        TapjoyManager.instance.TrackSceneEntryEvent(sceneName);
        GoogleAnalyticsTracker.instance.TrackSceneEntryEvent(sceneName);
    }


}
