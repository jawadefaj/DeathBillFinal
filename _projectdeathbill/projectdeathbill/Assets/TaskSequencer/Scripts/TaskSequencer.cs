using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Portbliss.TaskSequencer
{
    public class TaskSequencer : TaskBehaviour {

    	public List<WorkInfo> workList;
    	public bool isMaster;

    	private int currentIndex = 0;

    	void Start()
    	{
    		if(isMaster)
    		{
    			currentIndex =0;
    			DoNextWork ();
    		}
    	}

    	protected override void OnSceneAwake ()
    	{
    		if(isMaster)
    			this.gameObject.SetActive (true);
    	}

    	protected override void OnStart ()
    	{
    		if(isMaster)
    		{
    			Debug.LogError ("A master sequencer is trying to start another master script. You should not have any master referenced to another master.");
    			return;
    		}
    		currentIndex = 0;
    		DoNextWork();
    	}
    	
    	void DoNextWork()
    	{
    		//no more work to do
    		if(currentIndex == workList.Count) 
    		{
    			if(!isMaster)
    				WorkFinished();
    			return;
    		}

    		if(workList[currentIndex].isActive)
    		{
    			//Debug.Log("index "+ (currentIndex+1));

    			workList[currentIndex].work.StartWork(OnWorkOver,workList[currentIndex++].workOptions);
    			//currentIndex++;
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
}

namespace Portbliss.TaskSequencer
{
    [Serializable]
    public struct WorkInfo
    {
    	public TaskBehaviour work;
    	public bool isActive;
    	public string title;

    	public WorkOptions workOptions;
    }
}

namespace Portbliss.TaskSequencer
{
    [System.Serializable]
    public struct WorkOptions
    {
    	public enum HandleReleaseOption
    	{
    		AtWorkEnd = 0,
    		ImmediatelyAfterEnter =1,
    		OneFrameGapAtEnter =2,
    		DelayAfterWorkEnd =3,
    	}

    	public bool keepAlive;
    	public bool delayStart ;
    	public float delayStartTime ;
    	public HandleReleaseOption handleReleaseOption ;
    	public float delayHandleReleaseTime;
    	public List<GM_FieldInfo> variableFields;

    	/*
    	public bool keepAlive = false;
    	public bool delayStart = false;
    	public float delayStartTime = 1f;
    	public HandleReleaseOption handleReleaseOption = HandleReleaseOption.AtWorkEnd;
    	public float delayHandleReleaseTime = 0f;
    	public List<FieldInfo> variableFields; 
    	 * */

    	public void Init()
    	{
    		Debug.Log("inti called");
    		variableFields = new List<GM_FieldInfo>();

    	}

    }
}

namespace Portbliss.TaskSequencer
{
    [System.Serializable]
    public struct GM_FieldInfo
    {
    	public enum SupportedType
    	{
    		s_int=0,
    		s_float=1,
    		s_string=2,
            s_gameObject=3,
    	}

    	public string fieldName;
    	public string value;
        public GameObject go_value;
        public Texture txt_value;
        public Sprite sprite_value;
    	public int stype;


        /// TO ADD MORE DATA TYPE SUPPORT
        /// 
        /// 1. Add the type of the data here. for example if we want to support the type of Sprite
        /// 
        ///         public Sprite sprite_value
        /// 
        /// 2. At GameManagerEditor.cs go to line 172
        /// 3. define the new type their and add a number for it
        /// 4. go to line 285. pick up the new serializedProperty you just created on step 1
        /// 5. In the If-Else block on down add editor property layout for your type
        /// 6. to correctly load your type on variable goto BaseWorker.cs at line 147
        /// 7. in the If-Else block add similar code for your desired type
    }
}
