using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SWS;

public class ZoneManager : MonoBehaviour {
	public static ZoneManager instance;
    public GameObject sampleZone;
    public PathManager intraZoneSamplePath;

    [SerializeField][HideInInspector] public List<float> advantageValues = new List<float>();
    [SerializeField] [HideInInspector] public List<GameObject> pathPrefabList = new List<GameObject>();

	void Awake(){
		instance = this;
        if (intraZoneSamplePath == null) Debug.LogError("No sample path for intra-zone movement!");
        else { pathPrefabList.Add(intraZoneSamplePath.gameObject); }
        //if (SpwnZone == null) Debug.LogError("No spwn zone selected!");

    }
    void OnDestroy()
    {
        for (int i = 0; i < pathPrefabList.Count; i++)
        {
            Pool.ReleasePool(pathPrefabList[i]);
        }        
    }
    public void AddRemoveAdvantages()
    {
        int enLength = System.Enum.GetNames (typeof(FighterRole)).Length;
        while (advantageValues.Count < enLength) 
        {
            advantageValues.Add (0);
        }
        while (advantageValues.Count > enLength) 
        {
            advantageValues.RemoveAt (advantageValues.Count-1);
        }
        if(advantageValues.Count!=0)
            advantageValues [0] = 0;
    }
}

//[System.Serializable]
//public class ZoneParams
//{
//	[Range(0,2)]
//	public float weight = 1;
//	[Range(0,10)]
//	public int max_cap = 1;
//	[Range(0,1)]
//	public float advAgainst_SNP = 1;
//	[Range(0,1)]
//	public float advAgainst_ASS = 1;
//	[Range(0,1)]
//	public float advAgainst_SMG = 1;
//}
[System.Serializable]
public class ZoneConnection {
	public Zone rootZone;
	public Zone endZone;
	public ZoneRoad zoneRoad;
	public bool isFromType;

	public float GetWeightedAdvantageValue()
	{
		if (endZone == null) 
		{
			Debug.LogError ("end zone not set up");
			return 0;
		}
        return endZone.GetWeightedAdvantageValue();
	}
}

[System.Serializable]
public class ZoneRoad
{
	public PathManager side1;
	public PathManager side2;
}