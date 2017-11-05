using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIManagerModule : BaseWorker, AISpawner {
	
	public static List<AIManagerModule> nonMainAIMM_List = new List<AIManagerModule>();


	[SerializeField] bool workerRemainsAlive = false;
	[SerializeField] float actualExecutionDelay = 0f;
	[SerializeField] float controlReleaseDelay = 0f;
	[SerializeField] bool holdControl = true;

	[SerializeField] bool isMain = true;

	[SerializeField] public ZoneBlock zoneSet;
	[SerializeField] int totalSupply = 10;
	[SerializeField] int targetConecentration = 3;
	[SerializeField] float frequency = 1;
	[Range(0,1.0f)]
	[SerializeField] float timeDeviation= 0.4f;
	[Range(0,1.0f)]
	[SerializeField] float nadeChance= 0f;


	public bool useLocalInitData = false;
	public AIInitData localAIInitData;
	//public List<AIPersonnel> activeEnemyList = new List<AIPersonnel> ();
	[SerializeField] public List<AIPersonnel> reinforcementList = new List<AIPersonnel>();
	[SerializeField] AIMotionStates motionState = AIMotionStates.NORESTRICTION;


	public static AIManagerModule activeInstance;

	public int selfSpwnCount {
		get {
			if (reinforcementList != null)
				return reinforcementList.Count;
			else
				return 0;
		}
	}


	internal AITargetingParameters targetParams;
	protected override void OnSceneAwake()
	{
		targetParams = this.GetComponent<AITargetingParameters> ();
	}

	protected override void OnStart ()
	{	
		zoneSet.Clean ();
		if (holdControl) {
			//Debug.Log ("finish work manually flag was ignored!!");
			Handy.DoAfter (this,Init,actualExecutionDelay,null);
		} else {

			finishWorkManually = workerRemainsAlive;

			if (!workerRemainsAlive) {	
				Init ();
			} 
			else {
				Handy.DoAfter (this, ()=>{
					Init();
					FinishWorkManually();
				},actualExecutionDelay, ()=>{
					return PlayerInputController.instance.current_player.GetStationController().IsMoving();
				});
				FinishWorkAfterFixedDuration ();
			}

		}

	}
//	public void ResetAsMain()
//	{
//		isMain = true;
//		activeInstance = this;
//	}
	protected override void OnUpdate ()
	{
		TryFinish ();
	}
    string mainName;
	void Init()
	{
        mainName = this.transform.name;
        this.transform.name += "(Working...)";
        if (isMain)
        {
            activeInstance = this;
            //Debug.Log("active instance assigned!");
        }
        if(!isMain) nonMainAIMM_List.Add (this);
		finishWorkFlag = false;

		if (AIDataManager.instance.enemyBasicPrefab == null || zoneSet == null)
			Debug.LogError ("Missing initial data!");

		zoneSet.onWalkerAdded += OnAIAdded;
		AIInitData initData=null;
		if(!useLocalInitData)
			initData = AIDataManager.instance.aiInitData_Default;
		else 
			initData = localAIInitData;
		zoneSet.ActivateZoneBlock(AIDataManager.instance.enemyBasicPrefab, AIDataManager.instance.enemyGrenedierPrefab,nadeChance, totalSupply,targetConecentration,1/frequency,timeDeviation,initData.intelligence,this,isMain);
		

		if (holdControl) {
			zoneSet.allEnemiesClear += FinishWorkAfterFixedDuration;

		} 
		else {
			if(!workerRemainsAlive)
			FinishWorkAfterFixedDuration ();
		}
        zoneSet.allEnemiesClear += DeInit;
	}

	void OnAIAdded(GameObject go, Zone selectedZone)
	{
		AIPersonnel ai = go.GetComponent<AIPersonnel>();
		if (ai == null) Debug.LogError("AIpersonnel missing on created enemy");
		reinforcementList.Add (ai);
		AITargetUpdateState aitus = AITargetUpdateState.DYNAMIC;
		Transform targetPlayer = null;
		if (targetParams == null) {
			targetPlayer = AIDataManager.GetAnAITarget ();
		} else {
			if (targetParams.fighterList.Count == 0)
				Debug.LogError ("Fighter list on ai target param script not initialized");
			int Cnt = targetParams.fighterList.Count;
			int roll = Random.Range (0,Cnt);
			targetPlayer = targetParams.fighterList [roll].targetTransform;
			aitus = AITargetUpdateState.STATIC;
		}

		AIInitData initData=null;
		if(!useLocalInitData)
			initData = AIDataManager.instance.aiInitData_Default;
		else 
			initData = localAIInitData;
		ai.Init(selectedZone,targetPlayer, initData.damage,initData.accuracy,initData.intelligence, AIPersonnelBehaviourTypes.REINFORCEMENT, motionState ,aitus);
		if (!holdControl)
			ai.SetToWorthLess ();
		ai.spwnerRef = this;
	}

	public void RemoveReinforcementInfo(AIPersonnel ai)
	{
		if (reinforcementList.Contains (ai)) {
			reinforcementList.Remove (ai);
		} else {
			Debug.Log ("There was a prank call on removing reinforcementInfo");
		}
	
//		if (AIPatrolModule.getActiveCount == 0 && zoneSet.remainingWalkerSupply == 0 && reinforcementList.Count == 0) {
//			if (zoneSet.allEnemiesClear != null)
//				zoneSet.allEnemiesClear ();
//		}
		AIDataManager.OnEnemyDown (ai);
	}
	void DeInit()
	{ 
		if(activeInstance == this) activeInstance = null;
		if(holdControl) zoneSet.allEnemiesClear -= FinishWorkAfterFixedDuration;
        zoneSet.allEnemiesClear -= DeInit;
        Debug.Log("transform state: "+ transform.ToString());
        transform.name = mainName+"(Completed)";
		//Debug.Log ("deiniting "+this.gameObject.name);
	}

	void FinishWorkAfterFixedDuration(){
		Debug.Log ("will set finish flag");
		Handy.DoAfter (this,SetFinishFlag,controlReleaseDelay,null);
	}
	bool finishWorkFlag;
	void SetFinishFlag()
	{
		finishWorkFlag = true;
		TryFinish ();
	}
	void TryFinish()
	{
		if (finishWorkFlag && !SlowMotionBullet.instance.IsSlowMotionOn()) {
			finishWorkFlag = false;

			WorkFinished ();
		}
	}
}
