using UnityEngine;
using System.Collections;
using TapjoyUnity;



public class TapjoyManager : MonoBehaviour, IAnalyticsData {

	// Use this for initialization
    public static TapjoyManager instance;
	private const string CATEGORY = "DeathBill";

	void Awake(){
        DontDestroyOnLoad(this);
		instance = this;
	}
	// Update is called once per frame
	void Start () {
		if (!Tapjoy.IsConnected) {
			Tapjoy.Connect ();
		}
	}

    public void TrackLevelLoadEvent(int level, int stage){
        string txt = string.Format("L:{0} S:{1}",level,stage);
        Tapjoy.TrackEvent ( CATEGORY, "LevelStart", txt); 
    }

    public void TrackLevelRestartEvent(int lvl, int stage, long tryTime)
    {
        string txt = string.Format("L:{0} S:{1}",lvl,stage);
        Tapjoy.TrackEvent(CATEGORY, "Restarted", txt,null, tryTime);
    }

    public void TrackLevelFinishTimeEvent(int level, int stage, long time){
        string txt = string.Format("L:{0} S:{1}",level,stage);
        Tapjoy.TrackEvent (CATEGORY, "LevelFinishTiming", txt,null, time);
    }

    public void TrackGameLeaveEvent(int lvl, int stage,long no_of_tries)
    {
        string txt = string.Format("L:{0} S:{1}",lvl,stage);
        Tapjoy.TrackEvent (CATEGORY, "GameLeave", txt,null, no_of_tries);
    }

    public void TrackGameRating(int rate)
    {
        Tapjoy.TrackEvent (CATEGORY, "GameDescription", "Rating","Value", rate);
    }

    public void TrackUserSpecifiqData(string countryName, string deviceModel, string graphicsDeviceName, string graphicsDeviceVendor, string systemMemorySize)
    {
        Tapjoy.TrackEvent(CATEGORY,"UserDescription", "Country", countryName, 0);
        Tapjoy.TrackEvent(CATEGORY,"UserDescription", "DeviceModel", deviceModel, 0);
        Tapjoy.TrackEvent(CATEGORY,"UserDescription", "GraphicsDeviceName", graphicsDeviceName, 0);
        Tapjoy.TrackEvent(CATEGORY,"UserDescription", "GraphicsDeviceVendor", graphicsDeviceVendor, 0);
        Tapjoy.TrackEvent(CATEGORY,"UserDescription", "SystemMemorySize", systemMemorySize, 0);
    }

    public void TrackSceneEntryEvent(string sceneName)
    {
        Tapjoy.TrackEvent(CATEGORY,"SceneEntry", sceneName);
    }

}
