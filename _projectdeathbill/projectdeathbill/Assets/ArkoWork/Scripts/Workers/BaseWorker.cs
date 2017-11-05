using System;
using UnityEngine;
using System.Collections;

public class BaseWorker : MonoBehaviour {

	private Action workOverCallback;
	private bool isWorking = false;
	protected bool finishWorkManually = false;

	void Awake () {
		this.gameObject.SetActive(false);
		OnSceneAwake();
	}
	
	// Update is called once per frame
	void Update () {
		if(isWorking)
			OnUpdate();
	}

	protected virtual void OnStart()
	{

	}

	protected virtual void OnSceneAwake()
	{

	}

	protected virtual void OnUpdate()
	{

	}

	public void StartWork(Action callback)
	{
		//Debug.LogWarning(string.Format("{0} : {1} : {2}", Time.time,"WorkStart",this.gameObject.name));

		workOverCallback = callback;
		this.gameObject.SetActive(true);
		OnStart();
		isWorking = true;
	}

	protected void WorkFinished()
	{
		//Debug.LogWarning(string.Format("{0} : {1} : {2}", Time.time,"WorkFinish",this.gameObject.name));
		if(!finishWorkManually)
		{
			isWorking = false;
			this.gameObject.SetActive(false);
		}
		if(workOverCallback!=null) workOverCallback();
	}

	protected void FinishWorkManually()
	{
		isWorking = false;
		this.gameObject.SetActive(false);
	}
}
