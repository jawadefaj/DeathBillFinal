using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

    public Transform cov1;
    public Transform cov2;

    public Transform movingCube;

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 50, 50), "cov2"))
            ImprovedCameraCntroller.instance.RequestCameraTransitMove(MovementPriority.Low, null,cov2.position, cov2.rotation, 1, 15);

        if (GUI.Button(new Rect(70, 10, 50, 50), "cov1"))
            ImprovedCameraCntroller.instance.RequestCameraTransitMove(MovementPriority.Normal, null,cov1.position, cov1.rotation, 1, 15);

        if (GUI.Button(new Rect(130, 10, 50, 50), "follow"))
            ImprovedCameraCntroller.instance.RequestFollowCameraMove(MovementPriority.High, null, IsMoving, movingCube);
        
        
    }

    public bool IsMoving()
    {
        return true;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
