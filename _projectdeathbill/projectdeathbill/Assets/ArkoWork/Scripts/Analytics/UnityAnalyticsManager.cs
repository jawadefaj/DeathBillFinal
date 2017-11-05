using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class UnityAnalyticsManager : MonoBehaviour, IAnalyticsData {

    public static UnityAnalyticsManager instance;

    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    public void TrackLevelLoadEvent(int level, int stage)
    {
        string txt = string.Format("L:{0} S:{1}",level,stage);

        Analytics.CustomEvent("LevelLoad", new Dictionary<string,object>
            {
                {"LevelID",txt}
            });
    }

    public void TrackLevelRestartEvent(int lvl, int stage, long tryTime)
    {
        string txt = string.Format("L:{0} S:{1}",lvl,stage);
        Analytics.CustomEvent("Restarted", new Dictionary<string,object>
            {
                {"LevelID",txt},
                {"LastTryTime",tryTime}
            });
    }

    public void TrackLevelFinishTimeEvent(int level, int stage, long time)
    {
        string txt = string.Format("L:{0} S:{1}",level,stage);

        Analytics.CustomEvent("LevelPlayTime", new Dictionary<string,object>
            {
                {"LevelID",txt},
                {"Time",time}
            });
    }

    public void TrackGameLeaveEvent(int level, int stage, long no_of_tries)
    {
        string txt = string.Format("L:{0} S:{1}",level,stage);

        Analytics.CustomEvent("GameLeave", new Dictionary<string,object>
            {
                {"LevelID",txt},
                {"TryCountBeforeLeave", no_of_tries}
            });
    }

    public void TrackGameRating(int rate)
    {
        Analytics.CustomEvent("GameRating", new Dictionary<string,object>
            {
                {"Rating",rate}
            });
    }

    public void TrackUserSpecifiqData(string countryName, string deviceModel, string graphicsDeviceName, string graphicsDeviceVendor, string systemMemorySize)
    {
        Analytics.CustomEvent("UserData", new Dictionary<string,object>
            {
                {"Country",countryName},
                {"DeviceModel",deviceModel},
                {"GraphicsDeviceName",graphicsDeviceName},
                {"GraphicsDeviceVendor",graphicsDeviceVendor},
                {"SystemMemorySize",systemMemorySize}
            });
    }

    public void TrackSceneEntryEvent(string sceneName)
    {
        Analytics.CustomEvent("SceneEntry", new Dictionary<string,object>
            {
                { "SceneName",sceneName }
            });
    }
}
