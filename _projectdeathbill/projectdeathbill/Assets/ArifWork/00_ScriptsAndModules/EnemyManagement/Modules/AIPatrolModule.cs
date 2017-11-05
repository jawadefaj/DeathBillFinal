using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIPatrolModule : BaseWorker, AISpawner {
	[SerializeField] bool workerRemainsAlive = false;
	[SerializeField] float actualExecutionDelay = 0f;
	[SerializeField] int spwnCount =2;
	[Range(0,1.0f)]
	[SerializeField] float nadeChance= 0f;
	[SerializeField] List<Zone> spwnZoneList;
	Zone selectedSpwnZone;
	[SerializeField] public List<AIPersonnel> patrolList = new List<AIPersonnel>();
	[SerializeField] bool startAlerted = false;
	[SerializeField] AIMotionStates motionState = AIMotionStates.SINGLEZONE;
	[SerializeField] bool randomRotation = true;
	public bool useLocalInitData = false;
	public AIInitData localAIInitData;
	public static AIPatrolModule activeInstance;

	public static int getActiveCount {
		get{
			if (activeInstance == null)
				return 0;
			else
				return activeInstance.patrolList.Count;
		}
	}
	public int selfSpwnCount {
		get {
			if (patrolList != null)
				return patrolList.Count;
			else
				return 0;
		}
	}

	AITargetingParameters targetParams;
	protected override void OnSceneAwake()
	{
		targetParams = this.GetComponent<AITargetingParameters> ();
	}


	protected override void OnStart ()
	{
		finishWorkManually = workerRemainsAlive;
		if (!workerRemainsAlive) {	
			Init ();
		} 
		else {
			Handy.DoAfter (this, ()=>{
				Init();
				FinishWorkManually();
			},actualExecutionDelay, ()=>{
				return !PlayerInputController.instance.current_player.GetStationController().IsMoving();
			});
		}
        this.transform.name += "(Completed)";
		WorkFinished ();
	}


	public void AlertActivePatrol()
	{
		foreach (AIPersonnel ai in patrolList) {
			ai.Alert (panic: false, reason: AIPersonnel.AlertReason.NULL);
		}
	}
	void Init()
	{
		AIDataManager.alertAvailableEnemies += AlertActivePatrol;
		activeInstance = this;
		GameObject tempGo;
		int randomChoice;
		Quaternion rotationChoice;
		int slotCount;

		foreach (Zone item in spwnZoneList) {
			item.filledSlotIndexes = new List<int> ();
		}

//		do {
//			selectedSpwnZone = spwnZoneList[Random.Range(0,spwnZoneList.Count)];
//		} while(selectedSpwnZone.bookedSlotCount>=selectedSpwnZone.maxCapacity);
//		slotCount = selectedSpwnZone.slots.Count;
		//selectedSpwnZone.filledSlotIndexes = new List<int> ();

		for (int i = 0; i < spwnCount; i++) 
		{
			do {
				selectedSpwnZone = spwnZoneList[Random.Range(0,spwnZoneList.Count)];
			} while(selectedSpwnZone.bookedSlotCount>=selectedSpwnZone.maxCapacity);
			slotCount = selectedSpwnZone.slots.Count;
			do 
			{
				randomChoice = Random.Range (0, slotCount);
			}
			while(selectedSpwnZone.filledSlotIndexes.Contains (randomChoice));
			if(randomRotation)
			{
				rotationChoice = Quaternion.Euler(0,Random.Range(0,360.0f),0);
			}
			else{
				rotationChoice =  selectedSpwnZone.slots [randomChoice].rotation;
			}
			GameObject chosenPref = null;
			if (Random.Range (0.0f, 1.0f) > nadeChance)
				chosenPref = AIDataManager.instance.enemyBasicPrefab;
			else
				chosenPref = AIDataManager.instance.enemyGrenedierPrefab;
			tempGo = Pool.Instantiate (chosenPref, selectedSpwnZone.slots[randomChoice].position, rotationChoice);
			selectedSpwnZone.filledSlotIndexes.Add(randomChoice);
			selectedSpwnZone.bookedSlotCount++;
			patrolList.Add (tempGo.GetComponent<AIPersonnel> ());
			tempGo.GetComponent<ZoneWalker> ().slotIndex = randomChoice;

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
			tempGo.GetComponent<AIPersonnel> ().Init (selectedSpwnZone, targetPlayer ,initData.damage,initData.accuracy,initData.intelligence, AIPersonnelBehaviourTypes.PATROL, motionState, aitus);
			tempGo.GetComponent<AIPersonnel> ().spwnerRef = this;
			if (startAlerted)
				tempGo.GetComponent<AIPersonnel> ().Alert (panic: false,reason: AIPersonnel.AlertReason.NULL);
		}
	}
	public void RemovePatrolInfo(AIPersonnel ai)
	{
		if (patrolList.Contains (ai)) {
			patrolList.Remove (ai);
			if (patrolList.Count == 0) {
				DeInit ();
			}
		} else {
			Debug.Log ("There was a prank call on removing patrolInfo");
		}
		AIDataManager.OnEnemyDown (ai);
	}
	void DeInit()
	{
		AIDataManager.alertAvailableEnemies -= AlertActivePatrol;
		activeInstance = null;
	}
	/*public List<Transform> spwnPoints = new List<Transform>();
	public List<AIPatrolNetWork> patrolNetworks = new List<AIPatrolNetWork>();

	void Start () {
		Init ();
	}

	void Init()
	{
		GameObject tempGo;
		foreach (Transform sp in spwnPoints) {
			tempGo = Pool.Instantiate (AIDataManager.instance.enemyBasicPrefab, sp.position, sp.rotation);
			tempGo.GetComponent<AIPersonnel> ().Init (null, AIDataManager.instance.targetList [0],0.3f,1.0f,true);
		}
	}

	#region Gizmo and auto fill area/slots
	void OnDrawGizmos() 
	{
		List<Transform> slotList = new List<Transform>();
		Color baseColor = new Color(0.2f,1,0.2f,1);
		foreach (Transform item in transform) 
		{
			if(item.gameObject.activeInHierarchy)
			{
				slotList.Add(item);
			}
		}
		foreach(Transform tr in slotList){
			Gizmos.color = baseColor;
			Gizmos.DrawSphere(tr.position, 0.25f);
		}
		this.spwnPoints = slotList;

		List<AIPatrolNetWork> aipnlist = new List<AIPatrolNetWork> ();
		for(int i=0;i<slotList.Count;i++) {
			AIPatrolNetWork aipn = new AIPatrolNetWork ();
			foreach (Transform wp in slotList[i]) 
			{
				SWS.PathManager pm = wp.GetComponent<SWS.PathManager> ();
				if (pm == null) 
				{
					Debug.LogError ("PathManager not found");
				}
				else 
				{
					aipn.paths.Add (pm);
					aipn.points.Add (pm.waypoints[0]);
				}
			}
			aipnlist.Add (aipn);
		}
		patrolNetworks = aipnlist;
	}
	#endregion


}
[System.Serializable]
public class AIPatrolNetWork
{
	public List<Transform> points = new List<Transform>();
	public List<SWS.PathManager> paths = new List<SWS.PathManager>();
*/
}