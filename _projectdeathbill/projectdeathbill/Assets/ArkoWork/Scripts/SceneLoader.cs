using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour {

	private static string scene_name = "";

	public Image loaderImg;
	public GameObject[] loadingScenes;

	public static void LoadScene(string sceneName)
	{
		Time.timeScale = 1.0f;
		scene_name = sceneName;
		Application.LoadLevel(GameConstants.loadingScene);
	}

	// Use this for initialization
	IEnumerator Start () {

		int i = Random.Range(0,loadingScenes.Length);
		loadingScenes[i].SetActive(true);

		yield return new WaitForSeconds(2f);

		AsyncOperation levelloader = Application.LoadLevelAsync(scene_name);

		do{
			loaderImg.fillAmount = levelloader.progress;
			yield return null;
		} while(levelloader.isDone == false);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
