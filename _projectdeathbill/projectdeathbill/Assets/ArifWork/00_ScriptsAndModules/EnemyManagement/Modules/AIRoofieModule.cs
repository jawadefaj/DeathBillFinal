using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIRoofieModule : BaseWorker, AISpawner {
	public static AIRoofieModule instance;
	[SerializeField] int spwnCount =2;
	[SerializeField] List<Zone> spwnZoneList;
	Zone selectedSpwnZone;
	[SerializeField] public List<AIPersonnel> roofieList = new List<AIPersonnel>();
	public bool useLocalInitData = false;
	public AIInitData localAIInitData;

	public int selfSpwnCount {
		get {
			return 1;
		}
	}

	protected override void OnStart ()
	{
		Init ();
		WorkFinished ();
	}
	void Init()
	{
		do {
			selectedSpwnZone = spwnZoneList[Random.Range(0,spwnZoneList.Count)];
		} while(selectedSpwnZone.bookedSlotCount>=selectedSpwnZone.maxCapacity);


		instance = this;
		GameObject tempGo;
		int slotCount = selectedSpwnZone.slots.Count;
		selectedSpwnZone.filledSlotIndexes = new List<int> ();
		int randomChoice;
		Quaternion rotationChoice;
		for (int i = 0; i < spwnCount; i++) {
			do 
			{
				randomChoice = Random.Range (0, slotCount);
			}
			while(selectedSpwnZone.filledSlotIndexes.Contains (randomChoice));
			rotationChoice =  selectedSpwnZone.slots [randomChoice].rotation;
			tempGo = Pool.Instantiate (AIDataManager.instance.enemyBasicPrefab, selectedSpwnZone.slots[randomChoice].position, rotationChoice);
			selectedSpwnZone.filledSlotIndexes.Add(randomChoice);
			selectedSpwnZone.bookedSlotCount++;
			roofieList.Add (tempGo.GetComponent<AIPersonnel> ());
			tempGo.GetComponent<ZoneWalker> ().slotIndex = randomChoice;
			AIInitData initData=null;
			if(!useLocalInitData)
				initData = AIDataManager.instance.aiInitData_Default;
			else 
				initData = localAIInitData;
			tempGo.GetComponent<AIPersonnel> ().Init (selectedSpwnZone, AIDataManager.GetAnAITarget(), initData.damage,initData.accuracy,initData.intelligence, AIPersonnelBehaviourTypes.ROOFIE, AIMotionStates.STATIONARY, AITargetUpdateState.DYNAMIC);
			tempGo.GetComponent<AIPersonnel> ().spwnerRef = this;
			tempGo.GetComponent<AIPersonnel> ().Alert (panic: false,reason: AIPersonnel.AlertReason.NULL);
		}
	}
	public void RemoveRoofieInfo(AIPersonnel ai)
	{
		if (roofieList.Contains (ai)) {
			roofieList.Remove (ai);
			ai.canvasController.targetIcon.SetActive (false);
			if (roofieList.Count == 0) {
				DeInit ();
			}
		} else {
			Debug.Log ("There was a prank call on removing roofieInfo");
		}
	}
	void DeInit()
	{
		//AIDataManager.alertAvailableEnemies -= AlertActivePatrol;
		instance = null;
	}
}
