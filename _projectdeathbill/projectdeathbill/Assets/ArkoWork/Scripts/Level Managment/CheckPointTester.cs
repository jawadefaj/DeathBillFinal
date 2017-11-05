using UnityEngine;
using System.Collections;

public class CheckPointTester : MonoBehaviour {

	// Use this for initialization
	void Start () {
        int i;
        i = System.Convert.ToInt32('A');
        Debug.Log(i);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadChkPoint(int no)
	{
		LevelManager.LoadLevel(LevelID.Level01,no-1);
	}
}
