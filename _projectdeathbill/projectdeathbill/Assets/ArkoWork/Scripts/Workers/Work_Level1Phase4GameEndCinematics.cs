using UnityEngine;
using System.Collections;

public class Work_Level1Phase4GameEndCinematics : BaseWorker {

	public GameObject taposhStuntMan;
	public GameObject cineCamera;
	public GameObject fakaGuli;

	protected override void OnSceneAwake ()
	{
		//turon of nokol taposh
		taposhStuntMan.gameObject.SetActive(false);

		//turn off faka guli
		fakaGuli.SetActive(false);

		//remove cinecamera component
		Destroy(cineCamera.GetComponent<GUILayer>());
		Destroy(cineCamera.GetComponent<FlareLayer>());
		Destroy(cineCamera.GetComponent<Camera>());
        Destroy(cineCamera.GetComponent<AudioListener>());
	}

	protected override void OnStart ()
	{
		StartCoroutine(PlayCinematics());
	}

	IEnumerator PlayCinematics()
	{
		//turn of canvas
		HUDManager.instance.gameObject.SetActive(false);

		//Activate the camera
		GameObject _cam_go = new GameObject("cine camera");
		Camera _cam = _cam_go.AddComponent<Camera>();
		_cam.depth = 11;
		_cam_go.transform.position = cineCamera.transform.position;
		_cam_go.transform.rotation = cineCamera.transform.rotation;

		//activate nokol ptaposh
		taposhStuntMan.SetActive(true);

		//activate faka guli
		fakaGuli.SetActive(true);

		yield return new WaitForSeconds(3f);

		HUDManager.instance.gameObject.SetActive(true);
		WorkFinished();
	}
}
