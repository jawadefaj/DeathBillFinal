using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DeviceEMailIdFinder : MonoBehaviour {

	public Text mailDisplay;

	// Use this for initialization
	void Start () {

		//get current context
		AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject unityPlayerActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

		//get a account manager object
		AndroidJavaClass accountManagerClass = new AndroidJavaClass("android.accounts.AccountManager");
		AndroidJavaObject accoutManagerObj = accountManagerClass.CallStatic<AndroidJavaClass>("get", new System.Object[] {unityPlayerActivity});

		mailDisplay.text = "done!";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
