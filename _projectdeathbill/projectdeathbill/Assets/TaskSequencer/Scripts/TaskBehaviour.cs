using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using Portbliss.TaskSequencer;

public class TaskBehaviour : MonoBehaviour {

	private Action workOverCallback;
	private bool isWorking = false;
	private bool isActive = false;
    private bool hasStartedWorking = false;

	protected bool isCloneObject = false;

	void Awake () {
        if(!isCloneObject)
		    this.gameObject.SetActive(false);
		OnSceneAwake();
	}
	
	// Update is called once per frame
	void Update () {
        if(hasStartedWorking)
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

	public void StartWork(Action callback, WorkOptions _options)
	{
		//if this instance is running then create another instance of it
		if(isActive || isWorking)
		{
			TaskBehaviour go_bw = Instantiate(this) as TaskBehaviour;
			go_bw.isCloneObject = true;
			go_bw.StartWork(callback,_options);
			return;
		}
		//Debug.LogWarning(string.Format("{0} : {1} : {2}", Time.time,"WorkStart",this.gameObject.name));

        hasStartedWorking = false;
		isActive = true;
		workOverCallback = callback;
		//options = _options;
		this.gameObject.SetActive(true);

		StartCoroutine(WorkStarter(_options));
		StartCoroutine(WorkEnder(_options));

	}

	private IEnumerator WorkStarter(WorkOptions options)
	{
		if(options.delayStart)
		{
			yield return new WaitForSeconds(options.delayStartTime);
		}

		//now begin working
		InitVariables(this,options);
		if(isActive) isWorking = true;
        OnStart();
        hasStartedWorking = true;
	}

	private IEnumerator WorkEnder(WorkOptions options)
	{
		if(options.handleReleaseOption == WorkOptions.HandleReleaseOption.ImmediatelyAfterEnter)
		{
			//we want to pass the handel right now
			workOverCallback();

			StartCoroutine(TurnOffGameObject(options));
		}
		else if (options.handleReleaseOption == WorkOptions.HandleReleaseOption.OneFrameGapAtEnter)
		{
			yield return null;
			workOverCallback();

			StartCoroutine(TurnOffGameObject(options));
		}
		else if (options.handleReleaseOption == WorkOptions.HandleReleaseOption.AtWorkEnd)
		{
			//wait for work to finish
			do
			{
				yield return null;
			}while(isWorking == true || isActive == true);

            if (!options.keepAlive)
            {
                isActive = true;
                this.gameObject.SetActive(false);
            }
			
			workOverCallback();

		}
		else if (options.handleReleaseOption == WorkOptions.HandleReleaseOption.DelayAfterWorkEnd)
		{
			//wait for work to finish
			do
			{
				yield return null;
			}while(isWorking == true || isActive == true);

			yield return new WaitForSeconds(options.delayHandleReleaseTime);

            if (!options.keepAlive)
            {
                isActive = true;
                this.gameObject.SetActive(false);
            }
			
			workOverCallback();
		}
	}

	private IEnumerator TurnOffGameObject(WorkOptions options)
	{
		//wait for work to finish
		do
		{
			yield return null;
		}while(isWorking == true || isActive == true);

        if (!options.keepAlive)
        {
            this.gameObject.SetActive(false);
        }
	}

	private void InitVariables(System.Object obj, WorkOptions options)
	{

		for(int i=0;i<options.variableFields.Count;i++)
		{
			try
			{
				FieldInfo fi = obj.GetType().GetField(options.variableFields[i].fieldName);

                if(fi.FieldType == typeof(GameObject))
                    fi.SetValue(obj,options.variableFields[i].go_value);
                else if(fi.FieldType == typeof(Texture))
                    fi.SetValue(obj,options.variableFields[i].txt_value);
                else if(fi.FieldType == typeof(Sprite))
                    fi.SetValue(obj,options.variableFields[i].sprite_value);
                else
				    fi.SetValue(obj,options.variableFields[i].value);
			}
			catch(System.Exception ex)
			{
             	Debug.LogError(ex.Message);
			}
		}
			
	}


	protected void WorkFinished()
	{
		isWorking = false;
		isActive = false;	
	}

	protected void KillWorker()
	{
		isWorking = false;
		this.gameObject.SetActive(false);
	}
}
