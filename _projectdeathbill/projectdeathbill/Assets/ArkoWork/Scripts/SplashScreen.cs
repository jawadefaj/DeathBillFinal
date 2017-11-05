using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour {

	public GameObject sponserContainer;
	public GameObject companyContainer;

	// Use this for initialization
	IEnumerator Start () {
		//sponserContainer.SetActive(true);

		//yield return new WaitForSeconds(1.5f);

		sponserContainer.SetActive(false);
		companyContainer.SetActive(true);

		//SceneLoader.LoadScene(GameConstants.mainMenu);
		yield return new WaitForSeconds(2f);

//		while(!GPGDataSaveManager.gpgDataLoadingClear)
//		{
//			yield return null;
//		}
		Application.LoadLevel(GameConstants.mainMenu);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
