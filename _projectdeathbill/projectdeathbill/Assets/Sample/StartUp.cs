using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StartUp : MonoBehaviour {
	public GameObject text;

	// Use this for initialization
	void Start () {

		#if UNITY_IOS 

		UnityEngine.iOS.NotificationServices.RegisterForNotifications (UnityEngine.iOS.NotificationType.Alert |  UnityEngine.iOS.NotificationType.Badge |  UnityEngine.iOS.NotificationType.Sound);

		AppsFlyer.setAppsFlyerKey ("Z9hWmMNPVYB8rGNpVtv7VS");
		AppsFlyer.setAppID ("1171647185");
		AppsFlyer.setIsDebug (false);
		AppsFlyer.getConversionData ();
		AppsFlyer.trackAppLaunch ();

		#elif UNITY_ANDROID

		// if you are working without the manifest, you can initialize the SDK programatically.
		AppsFlyer.setAppsFlyerKey ("Z9hWmMNPVYB8rGNpVtv7VS");
		AppsFlyer.setIsDebug(false);

		// un-comment this in case you are not working with the android manifest file
		AppsFlyer.setAppID ("com.portbliss.deathbill"); 
		AppsFlyer.setGCMProjectNumber("1071135666043");

		#endif

	}

	private bool tokenSent_IOS = false;
	void Update()
	{
		#if UNITY_IOS
		if (!tokenSent_IOS) { // tokenSent needs to be defined somewhere (bool tokenSent = false)
			byte[] token = UnityEngine.iOS.NotificationServices.deviceToken;           
			if (token != null) {     
                AppsFlyer.registerUninstall (System.Text.Encoding.UTF8.GetString(token, 0, token.Length));
				tokenSent_IOS = true;
			}
		} 
		#endif
	}

}
