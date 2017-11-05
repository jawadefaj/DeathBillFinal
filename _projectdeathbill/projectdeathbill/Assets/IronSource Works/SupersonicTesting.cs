using UnityEngine;
using System.Collections;

public class SupersonicTesting : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Supersonic.Agent.start();

        SupersonicEvents.onInterstitialInitSuccessEvent += InterstitialInitSuccessEvent;
        SupersonicEvents.onInterstitialInitFailedEvent += InterstitialInitFailEvent; 
        SupersonicEvents.onInterstitialReadyEvent += InterstitialReadyEvent;
        SupersonicEvents.onInterstitialLoadFailedEvent += InterstitialLoadFailedEvent;
        SupersonicEvents.onInterstitialShowSuccessEvent += InterstitialShowSuccessEvent; 
        SupersonicEvents.onInterstitialShowFailedEvent += InterstitialShowFailEvent; 
        SupersonicEvents.onInterstitialClickEvent += InterstitialAdClickedEvent;
        SupersonicEvents.onInterstitialOpenEvent += InterstitialAdOpenedEvent;
        SupersonicEvents.onInterstitialCloseEvent += InterstitialAdClosedEvent;


        InitInterestial();
	}
	
    void OnEnable () {
        

    }

    public void InitInterestial()
    {
		Supersonic.Agent.initInterstitial("573e7c3d", GetAdId());
        Debug.Log("user id "+ GetAdId());
    }

   /* public void LoadInterestial()
    {

    }

    public void ShowInterestial()
    {

    }*/

    void OnApplicationPause(bool isPause)
    {
        if (isPause)
            Supersonic.Agent.onPause();
        else
            Supersonic.Agent.onResume();
    }

    void InterstitialInitFailEvent(SupersonicError error)
    {
        Debug.Log("InterstitialInitFailEvent");
    }

    void InterstitialInitSuccessEvent()
    {
        Debug.Log("InterstitialInitSuccessEvent");
        Supersonic.Agent.loadInterstitial();
    }

    void InterstitialReadyEvent()
    {
        Debug.Log("InterstitialReadyEvent");
        Supersonic.Agent.showInterstitial();
    }

    void InterstitialLoadFailedEvent(SupersonicError error)
    {
        Debug.Log("InterstitialLoadFailedEvent");
    }

    void InterstitialAdClickedEvent()
    {
        Debug.Log("InterstitialAdClickedEvent");
    }

    void InterstitialShowFailEvent(SupersonicError error)
    {
        Debug.Log("InterstitialShowFailEvent");
    }

    void InterstitialAdOpenedEvent()
    {
        Debug.Log("InterstitialAdOpenedEvent");
    }

    void InterstitialAdClosedEvent()
    {
        Debug.Log("InterstitialAdClosedEvent");
    }

    void InterstitialShowSuccessEvent()
    {
        Debug.Log("InterstitialShowSuccessEvent");
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
