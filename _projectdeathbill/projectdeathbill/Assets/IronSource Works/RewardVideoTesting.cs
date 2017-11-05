using UnityEngine;
using System.Collections;

public class RewardVideoTesting : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Supersonic.Agent.start();

        SupersonicEvents.onRewardedVideoInitSuccessEvent += RV_InitSuccessEvent;
        SupersonicEvents.onRewardedVideoInitFailEvent += RV_InitFailEvent; 

        SupersonicEvents.onVideoAvailabilityChangedEvent += VideoAvailabilityChangedEvent;



        InitInterestial();
	}
	
    void OnEnable () {
        

    }

    public void InitInterestial()
    {
        Supersonic.Agent.initRewardedVideo("56de4cdd", GetAdId());
        Debug.Log("user id "+ GetAdId());
    }


    void OnApplicationPause(bool isPause)
    {
        if (isPause)
            Supersonic.Agent.onPause();
        else
            Supersonic.Agent.onResume();
    }

    void RV_InitFailEvent(SupersonicError error)
    {
        Debug.Log("RV_InitFailEvent");
    }

    void RV_InitSuccessEvent()
    {
        Debug.Log("RV_InitSuccessEvent");
        Supersonic.Agent.loadInterstitial();
    }

    void VideoAvailabilityChangedEvent(bool isAvailable)
    {
        if (isAvailable)
        {
            Debug.Log("Ready and showing");
            Supersonic.Agent.showRewardedVideo();
        }
    }





    string GetAdId()
    {
        string advertisingID = "";
        bool limitAdvertising = false;

        AndroidJavaClass up = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject> ("currentActivity");
        AndroidJavaClass client = new AndroidJavaClass ("com.google.android.gms.ads.identifier.AdvertisingIdClient");
        AndroidJavaObject adInfo = client.CallStatic<AndroidJavaObject> ("getAdvertisingIdInfo",currentActivity);

        advertisingID = adInfo.Call<string> ("getId").ToString();   

        return advertisingID;
    }
}
