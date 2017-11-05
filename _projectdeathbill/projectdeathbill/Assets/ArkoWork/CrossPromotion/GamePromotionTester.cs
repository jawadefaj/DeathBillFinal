using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GamePromotionTester : MonoBehaviour {

	public Button ShowAd;
	// Use this for initialization
	void Awake () {
	

	}

	void Start()
	{
		GamePromotionManager.instance.OnAdNewsPending += NewMsg;

		ShowAd.onClick.AddListener(()=> {GamePromotionManager.instance.ShowAd();});
		ShowAd.gameObject.SetActive(false);
	}

	void NewMsg()
	{
		Debug.Log("New notification pending");
		ShowAd.gameObject.SetActive(true);
	}
	// Update is called once per frame
	void Update () {
	
	}
}
