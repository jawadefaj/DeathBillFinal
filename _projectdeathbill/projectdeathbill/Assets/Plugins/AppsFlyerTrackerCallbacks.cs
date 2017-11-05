using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AppsFlyerTrackerCallbacks : MonoBehaviour {
	public GameObject text;
	// Use this for initialization
	void Start () {
		print ("AppsFlyerTrackerCallbacks on Start");
		text.GetComponent<Text> ().text += "on Start";
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void didReceiveConversionData(string conversionData) {
		print ("AppsFlyerTrackerCallbacks:: got conversion data = " + conversionData);
		text.GetComponent<Text> ().text += conversionData;
	}
	
	public void didReceiveConversionDataWithError(string error) {
		print ("AppsFlyerTrackerCallbacks:: got conversion data error = " + error);
		text.GetComponent<Text> ().text += error;
	}
	
	public void didFinishValidateReceipt(string validateResult) {
		print ("AppsFlyerTrackerCallbacks:: got didFinishValidateReceipt  = " + validateResult);
		text.GetComponent<Text> ().text += validateResult;
		
	}
	
	public void didFinishValidateReceiptWithError (string error) {
		print ("AppsFlyerTrackerCallbacks:: got idFinishValidateReceiptWithError error = " + error);
		text.GetComponent<Text> ().text += error;
		
	}
	
	public void onAppOpenAttribution(string validateResult) {
		print ("AppsFlyerTrackerCallbacks:: got onAppOpenAttribution  = " + validateResult);
		text.GetComponent<Text> ().text += validateResult;
		
	}
	
	public void onAppOpenAttributionFailure (string error) {
		print ("AppsFlyerTrackerCallbacks:: got onAppOpenAttributionFailure error = " + error);
		text.GetComponent<Text> ().text += error;
		
	}
	
	public void onInAppBillingSuccess () {
		print ("AppsFlyerTrackerCallbacks:: got onInAppBillingSuccess succcess");
		text.GetComponent<Text> ().text += "onInAppBillingSuccess succcess";
		
	}
	public void onInAppBillingFailure (string error) {
		print ("AppsFlyerTrackerCallbacks:: got onInAppBillingFailure error = " + error);
		text.GetComponent<Text> ().text += error;
	}
}
