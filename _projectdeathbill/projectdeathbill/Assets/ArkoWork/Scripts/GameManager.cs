using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : BaseWorker {

	public List<WorkInfo> workList;

	private int currentIndex = 0;

	protected override void OnStart ()
	{
		currentIndex = 0;
		DoNextWork();
	}
	/*void Start () {
		currentIndex = startIndex;
		DoNextWork();
		//Debug.Log("");
	}*/
	
	void DoNextWork()
	{
		//no more work to do
		if(currentIndex == workList.Count) 
		{
			WorkFinished();
			return;
		}

		if(workList[currentIndex].isActive)
		{
			//Debug.Log("index "+ (currentIndex+1));

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

[Serializable]
public struct WorkInfo
{
	public BaseWorker work;
	public bool isActive;

	public void SetWork(BaseWorker bw)
	{
		this.work = bw;
	}

	public void SetActivity(bool value)
	{
		this.isActive = value;
	}
}
