using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EmailViewer : MonoBehaviour {

	public Text uiText;

	// Use this for initialization
	void Start () {

		uiText.text = "initing";

		AndroidJavaClass UnityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject UnityPlayerActivity = UnityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

		AndroidJavaClass emailGetterClass = new AndroidJavaClass("org.portbliss.getemail.EmailGetter"); 
		string email = emailGetterClass.CallStatic<string>("getEmail", UnityPlayerActivity);

		uiText.text = email;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
