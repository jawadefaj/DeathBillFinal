using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIDataManager : MonoBehaviour{
	public const float enemyFriendlyFireDamageMultiplier = 0.3f;
	public static System.Action alertAvailableEnemies;
	public static float globalNadeThrowCallIgnoreTime=0;
	public static AIDataManager instance;
	public LayerMask aiBulletHitLayers;// = LayerMask.GetMask("Player","Obstackle","Terrain");

    [Range(0,1)]
    public float uiZoomRealness =1;
    public float uiZooomStandardDistance = 20;
	#if UNITY_EDITOR
	public List<AIPersonnel> activeEnemeies_FEDO;
    public List<AITarget> activeTargets_FEDO;
	void FixedUpdate()
	{
		activeEnemeies_FEDO = activeEnemyList;
        activeTargets_FEDO = EnemyTargets_readonly;
	}
	#endif


	static List<FighterRole> engagedfflist = new List<FighterRole>(); 
	public static List<FighterRole> EngagedAllyList()
	{
		engagedfflist.Clear ();
		foreach (AIPersonnel ai in activeEnemyList) {
			if (ai.status.moving || ai.enemyType == EnemyType.MORTAR)
				continue;
            //Debug.Log(ai.zwalker.currentZone);
			Zone z = ai.zwalker.currentZone;

			for (int i = 1; i < z.AdvantageAgainstFighters.Count; i++) {
                FighterRole ff = (FighterRole)i;
                if (z.CantSeeFighter(ff))
                    continue;

                if (!engagedfflist.Contains (ff))
                    engagedfflist.Add (ff);
			}
		}
		return engagedfflist;
	}

	#region active enemies
	public static System.Action EnemyCountLowEnoughAction;
	public static int EnemyCountLowEnoughRefValue=0;
	public static void CheckForEnemyLowEnoughEvent(AIPersonnel ai)
	{
		if (EnemyCountLowEnoughAction == null)
			return;
		if (AIManagerModule.activeInstance == null)
			return;
		if (activeEnemyCount + AIManagerModule.activeInstance.zoneSet.remainingWalkerSupply == EnemyCountLowEnoughRefValue) 
		{
			EnemyCountLowEnoughAction ();
		}
	}
	public static void OnEnemyDown(AIPersonnel ai)
	{
		CheckForEnemyLowEnoughEvent (ai);			
		if (AIDataManager.activeEnemyCount != 0)
			return;
		
		if (PlayerInputController.instance != null) {
			InGameSoundManagerScript.instance.PPDP_WaveClearCalls(PlayerInputController.instance.current_player.fighterName);
		}

		if (ai.personalityType == AIPersonnelBehaviourTypes.DROP) {
			AIGeneratorModule aigm = (ai.spwnerRef as AIGeneratorModule);
			if (aigm.zoneSet.remainingWalkerSupply != 0)
				return;
			if(aigm.selfSpwnCount==0)
				aigm.zoneSet.ForceStopZoneBlock ();
		} 
		else 
		{
			AIManagerModule aimm = AIManagerModule.activeInstance;
			if (ai.personalityType == AIPersonnelBehaviourTypes.REINFORCEMENT) {
				aimm = (ai.spwnerRef as AIManagerModule);
			}
			if (aimm != null) {
				if (aimm.zoneSet.remainingWalkerSupply != 0) {
					return;
				}
			} 
			else {
				if(ai.enemyType != EnemyType.RAJAKAR)
					Debug.LogError ("No Active ai manager module!! And no enemyleft!! now What?");
				return;
			}
			aimm.zoneSet.ForceStopZoneBlock ();
		}

	}

	public static int activeEnemyCount
	{
		get
		{
			int r = 0;
			int p = 0;
			int d = 0;
			if (AIManagerModule.activeInstance != null) {
				r = AIManagerModule.activeInstance.reinforcementList.Count;
			}
			if (AIPatrolModule.activeInstance != null) {
				p = AIPatrolModule.activeInstance.patrolList.Count;
			}
			if (AIGeneratorModule.activeInstance != null) {
				d = AIGeneratorModule.activeInstance.dropList.Count;
			}
			return r + p + d;
		}
	}
	public static List<AIPersonnel> activeEnemyList
	{
		get
		{
			ap.Clear ();
			if(AIManagerModule.activeInstance!=null)
			{
				ap.AddRange (AIManagerModule.activeInstance.reinforcementList);
			}
			if( AIPatrolModule.activeInstance!= null)
			{
				ap.AddRange (AIPatrolModule.activeInstance.patrolList);
			}
			if( AIGeneratorModule.activeInstance!= null)
			{
				ap.AddRange (AIGeneratorModule.activeInstance.dropList);
			}
			return ap;
		}
	}
    private static List<AIPersonnel> PDH_nonMainEnemyList = new List<AIPersonnel>();
    public static List<AIPersonnel> nonMainEnemyList
    {
        get
        {
            PDH_nonMainEnemyList.Clear();
            for (int i = 0; i < AIManagerModule.nonMainAIMM_List.Count; i++)
            {
                PDH_nonMainEnemyList.AddRange(AIManagerModule.nonMainAIMM_List[i].reinforcementList);
            }
            return PDH_nonMainEnemyList;
        }
    }
    public static bool areAllEnemiesAlert
    {
        get
        {
            foreach (AIPersonnel ai in activeEnemyList)
            {
                if (ai.status.unAlert)
                    return false;
            }
            return true;
        }
    }
	#endregion
	#region Targeting
	private static List<AIPersonnel> ap = new List<AIPersonnel>();
	private static List<AITarget> aitargetList = new List<AITarget> ();
	private static List<AITarget> aitargetListClone = new List<AITarget> ();
	public static List<AITarget> EnemyTargets_readonly{
		get{
			aitargetListClone.Clear ();
			foreach (AITarget t in aitargetList) {
                
				aitargetListClone.Add (t);
			}
			return aitargetListClone;
		}
	}

	public static float GetWeightFor(FighterRole freedomFighterID)
	{
		foreach (AITarget item in aitargetList) {
			if (item.fighterID == freedomFighterID)
				return item.weight;
		}
		return 0;
	}
	public static void SetAITargets(List<AITarget> targetList, float urgency)
	{
		float totalWeight = 0;
		AITarget aiT;
		for (int i = 0; i < targetList.Count; i++) {
			totalWeight += targetList [i].weight;
		}
		for (int i = 0; i < targetList.Count; i++) {
			aiT = targetList [i];
			aiT.weight /= totalWeight;
			targetList [i] = aiT;
		}
		aitargetList.Clear ();

		AITarget tempTarg;
		for (int i = 0; i < targetList.Count; i++) 
		{
			for (int j = i+1; j < targetList.Count; j++) 
			{
				if (targetList [j].weight > targetList [i].weight) 
				{
					tempTarg = targetList [i];
					targetList [i] = targetList [j];
					targetList [j] = tempTarg;
				}
			}
		}
		aitargetList = targetList;

		UrgentTargetUpdates (urgency);
	}
	public static void UrgentTargetUpdates(float urgency)
	{
		List<AIPersonnel> ailist = activeEnemyList;
		float rollOut;
		foreach (AIPersonnel item in ailist) {
			rollOut = Random.Range (0f, 1.0f);
			switch (item.targetUpdateState) {
			case AITargetUpdateState.STATIC:
			case AITargetUpdateState.ASNEEDED: 
				continue;
				break;
			case AITargetUpdateState.DYNAMIC:
				if (rollOut < urgency)
					item.targetPlayer = GetAnAITarget ();
				break;
			case AITargetUpdateState.ZONAL:
				if (rollOut < urgency) {
					Transform tr = GetZonalAITarget(item.zwalker.currentZone);
					if (lastHasReturnedCurrentPlayer)
						item.targetPlayer = tr;
				}
				break;
			}
		}
	}

	private static List<AITarget> tempTargList = new List<AITarget>();
	public static bool lastHasReturnedCurrentPlayer;
	public static Transform GetZonalAITarget(Zone zone)
	{
		//List<AITarget> tempTargList = new List<>
		float totalWeight =0;
		tempTargList.Clear();
		foreach (AITarget aitarg in aitargetList) {
            if (zone.CantSeeFighter(aitarg.fighterID))
				continue;
			if (aitarg.fighterID == PlayerInputController.instance.current_player.fighterRole) {
				totalWeight += aitarg.weight*instance.zonalTargetingCurrentPlayerPreference;
			} else {
				totalWeight += aitarg.weight;
			}

			tempTargList.Add (aitarg);
		}
        if (tempTargList.Count == 0)
        {
            Debug.LogError("TargetNotFound!!!!");
            tempTargList.Add(aitargetList[0]);
        }
		float roll = Random.Range (0.0f,0.99f);
		float collectiveWeight = 0;
		for (int i = 0; i < tempTargList.Count; i++) {
			if (tempTargList [i].fighterID == PlayerInputController.instance.current_player.fighterRole) {
				collectiveWeight += tempTargList [i].weight*instance.zonalTargetingCurrentPlayerPreference;
			} else {
				collectiveWeight += tempTargList [i].weight;
			}
			if(roll<collectiveWeight/totalWeight)
			{
				lastHasReturnedCurrentPlayer = (tempTargList [i].fighterID == PlayerInputController.instance.current_player.fighterRole);
				return tempTargList [i].target;
			}
		}
		Debug.Log ("fallen through");
		if (tempTargList.Count == 0) {
			return null;	
		} else {
			return tempTargList [tempTargList.Count - 1].target;
		}
	}
	public static Transform GetAnAITarget()
	{
		float roll = Random.Range (0.0f,0.99f);
		float collectiveWeight = 0;
		for (int i = 0; i < aitargetList.Count; i++) {
			collectiveWeight += aitargetList [i].weight;
			if(roll<collectiveWeight)
			{
				return aitargetList [i].target;
			}
		}
		Debug.Log ("fallen through");
		if (aitargetList.Count == 0) {
			return null;	
		} else {
			return aitargetList [aitargetList.Count - 1].target;
		}
	}
	public static Transform GetPrimaryTarget()
	{
		if (aitargetList.Count == 0) {
			Debug.LogError ("no ai target set");
			return null;
		}
		return aitargetList [0].target;
	}
	public static void SetTempTargetOnAllActivePersonnel(Transform target)
	{
		foreach (AIPersonnel ai in activeEnemyList) {
			ai.targetPlayer = target;
			Debug.Log ("Temporary target was set for  ai!");
		}
	}
	#endregion
	public AIInitData aiInitData_Default;
    //public List<GameObject> enemyOrdinaryPrefabs;
    public GameObject enemyBasicPrefab;
//    {
//        get
//        {
//            int N = enemyOrdinaryPrefabs.Count;
//            if (N == 0)
//            {
//                return null;
//            }
//            return enemyOrdinaryPrefabs[Random.Range(0,N)];
//        }
//    }
	public GameObject enemyGrenedierPrefab;
	public GameObject enemyMortarPrefab;
	public GameObject BloodParticle;
    public Transform mortarGeneralAimYPoint;
    public float zonalTargetingCurrentPlayerPreference = 2f;
    [SerializeField][HideInInspector]public List<PlayerDamageInputModifier> playerDamageInputMods = new List<PlayerDamageInputModifier>();

	//public bool 



	void Awake()
	{
		instance = this;
		globalNadeThrowCallIgnoreTime = Time.time;
	}
}

[System.Serializable]
public struct AITarget
{
	public Transform target;
	public float weight;
	public FighterRole fighterID;
}
[System.Serializable]
public class AIInitData
{
	public float damage = 9.5f;
	public float accuracy = 0.25f;
	public float intelligence = 0.5f;
}
//[System.Serializable]
//public class PlayerDamageInputModifier
//{
//	public bool usePerPersonDamageModifier = false;
//	public float n_ASS = 1;
//	public float n_SMG = 1;
//	public float n_SNP = 1;
//	public bool useCustomHideDamageModifier = false;
//	[Range(0f,1.0f)]
//	public float h_ASS = 0.3f;
//	[Range(0f,1.0f)]
//	public float h_SMG = 0.3f;
//	[Range(0f,1.0f)]
//	public float h_SNP = 0.3f;
//}
[System.Serializable]
public class PlayerDamageInputModifier
{
    public FighterRole fighterID;
    public float baseDamageMultiplier = 1.0f;
    [Range(0f,1.0f)]
    public float hideToBaseDamageRatio = 0.3f;
    public PlayerDamageInputModifier()
    {
        fighterID = (FighterRole)1;
        baseDamageMultiplier = 1.0f;
        hideToBaseDamageRatio = 0.3f;
    }
}