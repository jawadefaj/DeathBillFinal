using UnityEngine;
using System.Collections;

public class TPCViewPortRelimitter : MonoBehaviour {

    public ThirdPersonController tpc;

    public float maxHorizontal = 45;
    public float minHorizontal = -45;
    public float maxVertical = 45;
    public float minVertical = -45;

    void Awake()
    {

        tpc.cameraMaxHorizontal = maxHorizontal;
        tpc.cameraMinHorizontal = minHorizontal;
        tpc.cameraMaxVertical = maxVertical;
        tpc.cameraMinVertical = minVertical;
    }
}
