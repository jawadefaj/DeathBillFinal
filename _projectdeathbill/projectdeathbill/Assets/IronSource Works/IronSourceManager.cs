using UnityEngine;
using System.Collections;

public class IronSourceManager : MonoBehaviour {

    public static IronSourceManager instance;

    private string uniqUserID = "";
    private string appKey
	{
		get
		{
			#if UNITY_ANDROID
			return "573e7c3d";
			#elif UNITY_IOS
			return "580c1d55";
			#else
			return "573e7c3d";
			#endif
		}
	}

    private bool isInterestitialReady = false;
    private bool isRewardVideoReady = false;

    void Awake()
    {
        DontDestroyOnLoad(this);
        instance = this;
    }

	void Start () {
        uniqUserID = GetAdId();

        Supersonic.Agent.start();
        Supersonic.Agent.initInterstitial(appKey, uniqUserID);
        Supersonic.Agent.initRewardedVideo(appKey, uniqUserID);

        Debug.Log("Iron Started to work");
	}
	
    void OnEnable()
    {
        SupersonicEvents.onInterstitialInitSuccessEvent += InterstitialInitSuccessEvent;
        SupersonicEvents.onInterstitialInitFailedEvent += InterstitialInitFailEvent; 

        SupersonicEvents.onInterstitialReadyEvent += InterstitialReadyEvent;
        SupersonicEvents.onInterstitialLoadFailedEvent += InterstitialLoadFailedEvent;
        SupersonicEvents.onInterstitialShowSuccessEvent += InterstitialShowSuccessEvent; 
        SupersonicEvents.onInterstitialShowFailedEvent += InterstitialShowFailEvent; 

        SupersonicEvents.onRewardedVideoInitSuccessEvent += RV_InitSuccessEvent;
        SupersonicEvents.onRewardedVideoInitFailEvent += RV_InitFailEvent; 

        SupersonicEvents.onVideoAvailabilityChangedEvent += VideoAvailabilityChangedEvent;
    }

    void OnDisable()
    {
        SupersonicEvents.onInterstitialInitSuccessEvent -= InterstitialInitSuccessEvent;
        SupersonicEvents.onInterstitialInitFailedEvent -= InterstitialInitFailEvent; 

        SupersonicEvents.onInterstitialReadyEvent -= InterstitialReadyEvent;
        SupersonicEvents.onInterstitialLoadFailedEvent -= InterstitialLoadFailedEvent;
        SupersonicEvents.onInterstitialShowSuccessEvent -= InterstitialShowSuccessEvent; 
        SupersonicEvents.onInterstitialShowFailedEvent -= InterstitialShowFailEvent; 

        SupersonicEvents.onRewardedVideoInitSuccessEvent -= RV_InitSuccessEvent;
        SupersonicEvents.onRewardedVideoInitFailEvent -= RV_InitFailEvent; 

        SupersonicEvents.onVideoAvailabilityChangedEvent -= VideoAvailabilityChangedEvent;
    }

    #region Interestitial Callbacks
    void InterstitialInitSuccessEvent()
    {
        Debug.Log("Interestitial successfully initialized");
        Supersonic.Agent.loadInterstitial();
    }

    void InterstitialInitFailEvent(SupersonicError error)
    {
        Debug.Log(string.Format("Interestitial initialization falied. Reason : {0}.", error.getDescription()));
    }

    void InterstitialReadyEvent()
    {
        Debug.Log("Interestitial is ready to show");
        isInterestitialReady = true;
    }

    void InterstitialLoadFailedEvent(SupersonicError error)
    {
        Debug.Log("InterstitialLoadFailedEvent");
        Supersonic.Agent.loadInterstitial();
    }

    void InterstitialShowSuccessEvent()
    {
        Debug.Log("Successfully interestitial is shown and loading a new one now");
        isInterestitialReady = false;
        Supersonic.Agent.loadInterstitial();
    }

    void InterstitialShowFailEvent(SupersonicError error)
    {
        Debug.Log(string.Format("Interestitial show falied. Reason : {0}. Loading new one", error.getDescription()));
        isInterestitialReady = false;
        Supersonic.Agent.loadInterstitial();
    }
    #endregion

    #region RewardVideo Callback
    void RV_InitSuccessEvent()
    {
        Debug.Log("Reward video successfully initialized");
    }

    void RV_InitFailEvent(SupersonicError error)
    {
        Debug.Log(string.Format("Reward video initialization falied. Reason : {0}.", error.getDescription()));
    }

    void VideoAvailabilityChangedEvent(bool isAvailable)
    {
        isRewardVideoReady = true;
    }

    #endregion

    #region Functionalities
    public static bool ShowInterestitial()
    {
        if (IronSourceManager.instance.isInterestitialReady)
        {
            IronSourceManager.instance.isInterestitialReady = false;
            Supersonic.Agent.showInterstitial();
            return true;
        }
        else
        {
            return false;
        }

    }

    public static bool ShowVideoAd()
    {
        if (IronSourceManager.instance.isRewardVideoReady)
        {
            IronSourceManager.instance.isRewardVideoReady = false;
            Supersonic.Agent.showRewardedVideo();
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void ShowSmartAd()
    {
        if (!ShowVideoAd())
        {
            if (!ShowInterestitial())
            {
                Debug.Log("Failed to show any kinds of ad");
            }
            else
            {
                Debug.Log("Showing interestitial");
            }
               
        }
        else
        {
            Debug.Log("Showing reqard video");
        }
    }
    #endregion
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
