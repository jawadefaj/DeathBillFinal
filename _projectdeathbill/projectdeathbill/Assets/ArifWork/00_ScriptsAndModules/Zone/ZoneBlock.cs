using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ZoneBlock : MonoBehaviour {
    private GameObject _sampleZone;
    public GameObject sampleZone
    {
        get
        {
            if (_sampleZone == null)
            {
                if (!isInitialized)
                    Debug.Log("ZoneBlock not intialized properly!");
                else
                {
                    _sampleZone = zoneManagerRef.GetComponent<ZoneManager>().sampleZone;
                }
            }
            return _sampleZone;
        }
        set
        {
            _sampleZone = value;
        }
    }
    [SerializeField]//[HideInInspector]   
    public List<Zone> spwnList = new List<Zone>(); // used in editor script
    [SerializeField][HideInInspector]   public Transform zParent;
    [SerializeField][HideInInspector]   public Transform cParent;


    [SerializeField][HideInInspector] public GameObject zoneManagerRef;
    public GameObject zoneManagerPrefab;

    public System.Action<GameObject,Zone> onWalkerAdded;
    public System.Action onAllEnemiesDeployed;



    public bool isInitialized{ get { 
            if (zoneManagerPrefab == null || zoneManagerRef == null)
            {
                return false;
            }
            else
            {
                ZoneManager zm = zoneManagerRef.GetComponent<ZoneManager>();
                if (zm == null)
                {
                    Debug.Log("Zonemanager reference is invalid!");
                    return false;
                }
                else
                {
                    if (zm.sampleZone == null)
                    {
                        Debug.Log("Zonemanager does not contain sample zone!");
                        return false;
                    }
                }
            }
            return true;
        } 
    }


    void Awake()
    {
        if(!isInitialized)
        {
			Debug.LogError("ZoneBlock:"+gameObject.name+" is not properly initialized!");
        }
    }

    public void Clean()
    {
        onWalkerAdded = null;
        onAllEnemiesDeployed = null;
        //maintainThisZone = false;
        //targetWalkerConcentration = 0;
        //remainingWalkerSupply = 0;
    }
    void  FixedUpdate()
    {
        if (maintainThisZone) 
        {
            if (isMainZoneBlock) {
                if (AIDataManager.activeEnemyCount < targetWalkerConcentration) 
                {
                    if (Time.time > nextGenerationAllowedTime) 
                    {
                        AddWalker (intel);
                    }
                }
            } else {
                if (rootSpwner.selfSpwnCount < targetWalkerConcentration) 
                {
                    if (Time.time > nextGenerationAllowedTime) 
                    {
                        AddWalker (intel);
                    }
                }
            }

        }

    }

    //public int activeWalkerConcentration;
    internal int targetWalkerConcentration=5;

    internal System.Action allEnemiesClear;

    internal bool maintainThisZone = false;
    internal int remainingWalkerSupply = 0;
    float generationGap=1;
    float generationGapDeviation = 0.5f;
    float nextGenerationAllowedTime = 0;
    float intel = 1.0f;

    //List<GameObject> walkerPrefabList = new List<GameObject> ();
    ChancedList<GameObject> walkerPrefabList = new ChancedList<GameObject> ();
    //GameObject walkerPrefab;
    bool isMainZoneBlock = true;
    AISpawner rootSpwner;
    public void ActivateZoneBlock(GameObject primaryPrefab, GameObject secondaryPrefab, float secondaryChance, int totalSupply, int targetConcentration, float generationGap, float generationGapDeviation, float intelligence, AISpawner spwner, bool isMain)
    {
        rootSpwner = spwner;
        isMainZoneBlock = isMain;
        remainingWalkerSupply = totalSupply;
        targetWalkerConcentration = targetConcentration;
        this.intel = intelligence;
        walkerPrefabList.Clear ();
        walkerPrefabList.Add (primaryPrefab, 1-secondaryChance);
        walkerPrefabList.Add (secondaryPrefab, secondaryChance);
        this.generationGap = generationGap;
        this.generationGapDeviation = generationGapDeviation;

        nextGenerationAllowedTime = Time.time + Handy.Deviate (generationGap,generationGapDeviation);
        maintainThisZone = true;
    }

    public void ActivateAsDropZoneBlock(GameObject primaryPrefab, GameObject secondaryPrefab, float secondaryChance, int totalSupply, float generationGap, float intelligence, AISpawner spwner)
    {
        rootSpwner = spwner;
        isMainZoneBlock = true;
        remainingWalkerSupply = totalSupply;
        targetWalkerConcentration = totalSupply;
        this.intel = intelligence;
        walkerPrefabList.Clear ();
        walkerPrefabList.Add (primaryPrefab, 1-secondaryChance);
        walkerPrefabList.Add (secondaryPrefab, secondaryChance);
        this.generationGap = generationGap;
        this.generationGapDeviation = 0;
        nextGenerationAllowedTime = Time.time + generationGap;
        maintainThisZone = true;
    }

    public const float ignoreAdvantageAgainstThreshold = 0.05f;
    List<ZoneConnection> spwnZCList = new List<ZoneConnection>();
    List<ZoneConnection> spwnzcShortlist = new List<ZoneConnection>();
    public ZoneConnection SelectSpwnZoneConnection(float intelligence)
    {
        ZoneConnection chosenZoneConnection = null;
        spwnZCList.Clear ();
        for (int i = 0; i <spwnList.Count ; i++) {
            for (int j = 0; j < spwnList[i].zoneConnections.Count; j++) {
                spwnZCList.Add (spwnList[i].zoneConnections[j]);
            }
        }


        spwnzcShortlist.Clear ();
        //Debug.Log (spwnZCList.Count);
        #region eliminate by capacity and zero exposure
        for (int i = 0; i < spwnZCList.Count; i++)
        {

			if (spwnZCList[i].GetWeightedAdvantageValue() <= ignoreAdvantageAgainstThreshold) { continue; }

            Zone zoneRef = spwnZCList[i].endZone;
            if( zoneRef.filledSlotIndexes == null)
            {
                zoneRef.filledSlotIndexes = new List<int>();
            }
			if (zoneRef.maxCapacity > zoneRef.filledSlotIndexes.Count)
            {
                spwnzcShortlist.Add(spwnZCList[i]);
            }
        }
        #endregion
        float prevAcceptanceFactor = 0;
        intelligence = Mathf.Clamp(intelligence, 0.0f, 1.0f);
        float weightFactor = 0;
        float advantageFactor = 0;
        float intelFactor = 0;
        float deterministicFactor = 0;
        float randomFactor = 0;
        float populationDensityFactor = 0;
        float thisAcceptanceFactor = 0;
        //if(spwnzcShortlist.Count==0)Debug.Log (spwnzcShortlist.Count);
        for (int i = 0; i < spwnzcShortlist.Count; i++)
        {
			weightFactor = spwnzcShortlist[i].endZone.zoneWeight;
			advantageFactor = spwnzcShortlist[i].GetWeightedAdvantageValue();
            intelFactor = intelligence;
            deterministicFactor = intelFactor * advantageFactor + (1 - intelFactor) * weightFactor;
            randomFactor = Random.Range(0.0f, 1.01f);
			populationDensityFactor = spwnzcShortlist[i].endZone.bookedSlotCount / spwnzcShortlist[i].endZone.maxCapacity;
            thisAcceptanceFactor = deterministicFactor + (1 - populationDensityFactor) * randomFactor * randomFactor;
            //          Debug.Log ("population =" + populationDensityFactor.ToString());
            //          Debug.Log ("random =" + randomFactor.ToString());
            //          Debug.Log ("total =" + thisAcceptanceFactor.ToString());
            if (thisAcceptanceFactor > prevAcceptanceFactor)
            {
                chosenZoneConnection = spwnzcShortlist[i];
                prevAcceptanceFactor = thisAcceptanceFactor;
                //Debug.Log(zclist[i].ToString() + "was accepted when value" + thisAcceptanceFactor.ToString() );
            }
        }
        //Debug.Break ();
        if(chosenZoneConnection == null)
            Debug.Log ("zc is null");
        return chosenZoneConnection;
    }
		

    void AddWalker(float intelligence)
    {
        if (remainingWalkerSupply > 0) 
        {
            remainingWalkerSupply--;
            if (remainingWalkerSupply == 0 && onAllEnemiesDeployed != null)
                onAllEnemiesDeployed ();

            ZoneConnection zc = SelectSpwnZoneConnection (intelligence);
            Zone selectedZone = zc.rootZone;
            //if(zc == null)
            int selSlot = Random.Range (0,selectedZone.slots.Count);

            GameObject go = Pool.Instantiate (walkerPrefabList.Roll(),selectedZone.slots[selSlot].position,Quaternion.identity) as GameObject;
            nextGenerationAllowedTime = Time.time + Handy.Deviate (generationGap,generationGapDeviation);
            if(onWalkerAdded!=null)onWalkerAdded (go,selectedZone);
        }


    }

    public void ForceStopZoneBlock()
    {
        remainingWalkerSupply = 0;
        maintainThisZone = false;
        if(allEnemiesClear!=null) allEnemiesClear ();
        allEnemiesClear = null;
    }

}
