#define SKIP4NOW
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SWS;

public class AIGeneratorModule : BaseWorker, AISpawner
{

	public int selfSpwnCount {
		get {
			if (dropList != null)
				return dropList.Count;
			else
				return 0;
		}
	}
	public static AIGeneratorModule activeInstance;

    internal ZoneBlock zoneSet;
	internal List<AIPersonnel> dropList = new List<AIPersonnel>();

	//settings=========================================

    public DeliveryOption deliveryMethod;
    public AIGenerationMode generationMode;
	public bool hasMortar;
	public int dropCount;
	[Range(0,1)]
	public float nadeChance;
	public bool useLocalInitData = false;
	public AIInitData localAIInitData;
	//public Zone spwnZone;

	public float troopDropInterval;
    public int SpwnSlots;
    void OnDrawGizmos()
    {
//        int i = 0;
//
//        foreach (Zone z in deliveryMethod.destZoneBlock.spwnList)
//        {
//            foreach (ZoneConnection zc in z.zoneConnections)
//            {
//                i += zc.endZone.maxCapacity;
//            }
//        }
//        SpwnSlots = i;
    }

	//public float troopDropIntervalDeviation;
	//=================================================
	//public List<AIPersonnel> generationList = new List<AIPersonnel>();

	protected override void OnStart ()
	{
		if (DeliveryManager.instance == null ) {
			Debug.LogError ("delivery manager not found!!");
		}
        if(deliveryMethod==null)deliveryMethod = MakeAChoice();
        if(deliveryMethod.destZoneBlock!=null) zoneSet = deliveryMethod.destZoneBlock;
		zoneSet.Clean ();
		RequestTroop ();
	}
	#region managerLikeModule
	void Init(MoverPack mPack)
	{
		activeInstance = this;	
		if (AIDataManager.instance == null || zoneSet == null)
			Debug.LogError ("Missing initial data!");
		zoneSet.onWalkerAdded += OnAIAdded;
        if (!mPack.isOfChopperType)
        {
            zoneSet.onAllEnemiesDeployed += () =>
            {
                DeliveryManager.RetrieveFromDelivery(mPack);
            };
        }
		AIInitData initData=null;
		if(!useLocalInitData)
			initData = AIDataManager.instance.aiInitData_Default;
		else 
			initData = localAIInitData;
		zoneSet.ActivateAsDropZoneBlock (
			primaryPrefab: AIDataManager.instance.enemyBasicPrefab,
			secondaryPrefab: AIDataManager.instance.enemyGrenedierPrefab,
			secondaryChance: nadeChance,
			totalSupply: dropCount,
			generationGap: troopDropInterval,
			intelligence: localAIInitData.intelligence,
			spwner: this
		);
		
		zoneSet.allEnemiesClear += DeInit;
		zoneSet.allEnemiesClear += WorkFinished;
	}
	void OnAIAdded(GameObject go, Zone selectedZone)//not for mortar
	{
		AIPersonnel ai = go.GetComponent<AIPersonnel>();
		if (ai == null) Debug.LogError("AIpersonnel missing on created enemy");
		dropList.Add (ai);
		AIInitData initData=null;
		if(!useLocalInitData)
			initData = AIDataManager.instance.aiInitData_Default;
		else 
			initData = localAIInitData;
        ai.Init (selectedZone,null, initData.damage, initData.accuracy,initData.intelligence, AIPersonnelBehaviourTypes.DROP,AIMotionStates.NORESTRICTION,AITargetUpdateState.ZONAL,false);
		ai.spwnerRef = this;
	}
	public void RemoveDropInfo(AIPersonnel ai)
	{
		if (dropList.Contains (ai)) {
			dropList.Remove (ai);
		} else {
			Debug.Log ("There was a prank call on removing drop Info");
		}
		AIDataManager.OnEnemyDown (ai);
	}
	void DeInit()
	{
		if(activeInstance == this) activeInstance = null;
		GeneralManager.instance.wavesCompleted++;
		zoneSet.allEnemiesClear -= WorkFinished;
		zoneSet.allEnemiesClear -= DeInit;
		//Debug.Log ("deiniting "+this.gameObject.name);
	}
	#endregion



    public DeliveryOption MakeAChoice()
    {
        switch(generationMode)
        {
            default:
                Debug.LogError("GenerationModeUnset");
                return null;
                break;
            case AIGenerationMode.LAND:
                return DeliveryManager.instance.landDeliveryOption_RNDM;
                break;  
            case AIGenerationMode.AIR:
                return DeliveryManager.instance.airDeliveryOption_RNDM;
                break;  
//          case AIGenerationMode.RANDOM:
//                break;
        }
    }
	public void RequestTroop()
	{
        DeliveryManager.StartDelivery (
            deliveryMethod,
			//(MoverPack moverPack) =>{StartCoroutine(AITroopCreate(moverPack));}
			(MoverPack moverPack)=>
            { 
                //Debug.Log("in start mech callback");
                if(moverPack.isOfChopperType)
                {
                    deliveryMethod.destDropArea.StartTheMachinary
                    (
                        howMany: dropCount,
                        interval: troopDropInterval,
                        variance: 0.15f,
                        chopperBody: moverPack.mover.transform,
                        OnFirstGuyDropped: ()=>{Init(moverPack);},
                        OnReadyToLeave: ()=>{ DeliveryManager.RetrieveFromDelivery(moverPack);}
                    );
                }
                else{
				    Init(moverPack);
                }
			},
			ClipID.engineTruck
		);
		if(hasMortar)StartCoroutine(RequestMortar());
	}

	const float timeGapBeforeMortartruck = 2f;   //time gap between carriers
	const float mortarDeployTime = 1f;        //carrier wait time before returning
	public IEnumerator RequestMortar()
	{
		yield return new WaitForSeconds(timeGapBeforeMortartruck);
        DeliveryManager.StartDelivery (DeliveryManager.instance.mortarDeliveryOption_RNDM, 
			(MoverPack moverPack) =>{StartCoroutine(AIMortarCreate(moverPack));},
			ClipID.engineJeep
		);
	}
	public IEnumerator AIMortarCreate(MoverPack moverPack)
	{
		yield return new WaitForSeconds(mortarDeployTime);
		Vector3 targetRotVec3 = Vector3.zero;
		Transform spwnPointMortar = moverPack.mover.transform.FindChild ("spwner");
		targetRotVec3.y = Mathf.Rad2Deg * Mathf.Atan2((AIDataManager.instance.mortarGeneralAimYPoint.position.x - spwnPointMortar.position.x), (AIDataManager.instance.mortarGeneralAimYPoint.position.z - spwnPointMortar.position.z));
		Quaternion targetRotation = Quaternion.Euler(targetRotVec3);

		GameObject go = Instantiate(AIDataManager.instance.enemyMortarPrefab, spwnPointMortar.position, targetRotation) as GameObject;
		AIPersonnel ai = go.GetComponent<AIPersonnel>();
		if (ai == null) Debug.LogError("AIpersonnel missing on created enemy");
		dropList.Add(ai);
		ai.Init(null,null,0,0,0,AIPersonnelBehaviourTypes.DROP,AIMotionStates.STATIONARY,AITargetUpdateState.ASNEEDED);
		ai.spwnerRef = this;
		DeliveryManager.RetrieveFromDelivery (moverPack);
	}
}

public enum AIGenerationMode
{
    UNSET,
    RANDOM,
    LAND, 
    AIR 
}