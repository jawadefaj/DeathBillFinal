using UnityEngine;
using System.Collections;
using SWS;

public class SWSTest : MonoBehaviour {

	public splineMove spMove;
	public PathManager pm;

	// Use this for initialization
	void Start () {
		/*GameObject newPath = (GameObject) Instantiate(pm.gameObject,pm.gameObject.transform.position,Quaternion.identity);
		PathManager newPm = newPath.GetComponent<PathManager>();*/

		PathManager newPm = (PathManager) Instantiate(pm);

		Transform[] points = newPm.waypoints;
		newPm.waypoints = new Transform[] {points[2],points[3]};

		spMove.SetPath(newPm);
		spMove.StartMove();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
