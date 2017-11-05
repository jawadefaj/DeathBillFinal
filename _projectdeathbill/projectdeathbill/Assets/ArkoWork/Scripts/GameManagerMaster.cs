using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManagerMaster : MonoBehaviour {

	public List<WorkInfo> workList;

	private int currentIndex = 0;

	// Use this for initialization
	void Start () {
		
		DoNextWork();
	}
	
	void DoNextWork()
	{
		//no more work to do
		if(currentIndex == workList.Count) return;

		if(workList[currentIndex].isActive)
		{
			//Debug.Log ("new task");
			//currentIndex++;
			workList[currentIndex++].work.StartWork(OnWorkOver);
		}
		else
		{
			currentIndex++;
			DoNextWork();
		}
	}

	void OnWorkOver()
	{
		DoNextWork();
	}
}
