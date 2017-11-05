using UnityEngine;
using System.Collections;

public class RotateLocally : MonoBehaviour {
    public float rotationSpeed = 720;
	
	void Update () {
        transform.Rotate(0, rotationSpeed*Time.deltaTime, 0);
	}
}
