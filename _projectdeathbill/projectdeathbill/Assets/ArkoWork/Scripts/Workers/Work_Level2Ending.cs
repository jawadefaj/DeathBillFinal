using UnityEngine;
using System.Collections;

public class Work_Level2Ending : BaseWorker {

	private Transform mainCam;
	public Transform camPos1;
	public Transform camPos2;
	public Transform camPos3;
	//public DetonatorTest bridgeDetonator;
	public GameObject normalBridge;
	public GameObject c4bridge;

	protected override void OnSceneAwake ()
	{
		mainCam = Camera.main.transform;

		Destroy(camPos1.gameObject.GetComponent<GUILayer>());
		Destroy(camPos1.gameObject.GetComponent<FlareLayer>());
		Destroy(camPos1.gameObject.GetComponent<Camera>());

		Destroy(camPos2.gameObject.GetComponent<GUILayer>());
		Destroy(camPos2.gameObject.GetComponent<FlareLayer>());
		Destroy(camPos2.gameObject.GetComponent<Camera>());

		Destroy(camPos3.gameObject.GetComponent<GUILayer>());
		Destroy(camPos3.gameObject.GetComponent<FlareLayer>());
		Destroy(camPos3.gameObject.GetComponent<Camera>());
	}

	protected override void OnStart ()
	{
		StartCoroutine(StartCine());
	}

	IEnumerator StartCine()
	{
		//turn of hud
		HUDManager.instance.forceDisableRun_ShootGroup = true;
		HUDManager.instance.gameObject.SetActive(false);

		//take out all controlles from player. this is highly mandatory
		PlayerInputController.instance.current_player.SetControll(false);

		yield return new WaitForSeconds(1f);

		mainCam.SetParent(null);

		//transit camera to cam pos 1
		mainCam.GetComponent<Camera>().fieldOfView = 60;
        ImprovedCameraCntroller.instance.RequestCameraTransitMove(MovementPriority.Normal, (bool isSuccess) =>
            {
                StartCoroutine(AftercamTransit());
            }, camPos1.position, camPos1.rotation, 2f,15f);
        
		/*CameraController.instance.TransitCamera(mainCam,camPos1,2f,15, () =>
			{
				StartCoroutine(AftercamTransit());
			}
		);*/
	}

	IEnumerator AftercamTransit()
	{
		PlayerInputController.instance.GetPlayerByRole(FighterRole.Support).AttachAI(true);
		PlayerInputController.instance.GetAiPlayer(FighterRole.Support).MoveStation();
		yield return new WaitForSeconds(1f);
		PlayerInputController.instance.GetPlayerByRole(FighterRole.Leader).AttachAI(true);
		PlayerInputController.instance.GetAiPlayer(FighterRole.Leader).MoveStation();

		yield return new WaitForSeconds(2f);

        PlayerInputController.instance.GetPlayerByRole(FighterRole.Sniper).AttachAI(true);
        PlayerInputController.instance.GetAiPlayer(FighterRole.Sniper).MoveStation();
		mainCam.transform.position = camPos2.position;
		mainCam.transform.rotation = camPos2.rotation;

		//switch bridge
		normalBridge.SetActive(false);
		c4bridge.SetActive(true);

		yield return new WaitForSeconds(1f);

		//explode bridge
		//bridgeDetonator.ExplodeBridge();

		yield return new WaitForSeconds(1.5f);

		mainCam.transform.position = camPos3.position;
		mainCam.transform.rotation = camPos3.rotation;

		yield return new WaitForSeconds(6f);

		//done
		HUDManager.instance.gameObject.SetActive(true);
		WorkFinished();
	}
}
