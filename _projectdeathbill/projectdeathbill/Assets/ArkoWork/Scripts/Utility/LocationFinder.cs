using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum Country
{
	Bangladesh=0,
	Other =1,
	NotDetermined =2,
}

public class LocationFinder : MonoBehaviour {

	void Start()
	{
		if(GetCountry()== Country.Bangladesh) Debug.Log("Bangladesh");
		else Debug.Log("other");
	}

	public static Country GetCountry()
	{
		#if UNITY_EDITOR
		return Country.Bangladesh;
		#elif UNITY_ANDROID
			AndroidJavaClass UnityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
			AndroidJavaObject UnityPlayerActivity = UnityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

			AndroidJavaClass countryCodeClass = new AndroidJavaClass("org.portbliss.getemail.Utility"); 
			string countryCode = countryCodeClass.CallStatic<string>("getCountryCode", UnityPlayerActivity);

			if(string.Equals(countryCode.ToUpper(),"BD"))
			return Country.Bangladesh;
			else
			return Country.NotDetermined;
		#endif

	}

    public static string GetCountryRaw()
    {
        #if UNITY_EDITOR
        return "Editor";
        #elif UNITY_ANDROID
        AndroidJavaClass UnityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
        AndroidJavaObject UnityPlayerActivity = UnityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

        AndroidJavaClass countryCodeClass = new AndroidJavaClass("org.portbliss.getemail.Utility"); 
        string countryCode = countryCodeClass.CallStatic<string>("getCountryCode", UnityPlayerActivity);

        if(string.IsNullOrEmpty(countryCode))
        {
        return "CountryNotDetermined";
        }
        else
        {
            return countryCode;
        }
        #elif UNITY_IOS
        return "ios_country";
        #endif

    }
}
