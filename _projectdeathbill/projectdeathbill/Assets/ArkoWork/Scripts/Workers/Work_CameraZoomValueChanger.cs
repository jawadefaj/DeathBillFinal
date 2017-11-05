using UnityEngine;
using System.Collections;

public class Work_CameraZoomValueChanger : BaseWorker {

	public FighterRole fighterID;
	public float zoomCameraValue = 20f;

	protected override void OnStart ()
	{
		//CameraController.instance.zoomCameraValue = zoomCameraValue;
		PlayerInputController.instance.GetPlayerByRole(fighterID).assignedWeapon.personalZoomValue = zoomCameraValue;
		WorkFinished();
	}
}
