using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Zone : MonoBehaviour {
	public bool spwnOnly = false;
    public bool dontGroundOnRefresh = false;
    public bool lockAdvantageValues = false;
	[Range(0,2)]
	public float zoneWeight = 1;
	[Range(0,15)]
	public int maxCapacity = 1;
	[SerializeField][HideInInspector]
	public List<float> AdvantageAgainstFighters = new List<float>();

    [SerializeField][HideInInspector]
    public float zoneRadius = 2.2f;
    //public ZoneParams zoneParameters;
    //public List<Transform> area;
    [SerializeField][HideInInspector] public List<Transform> slots;
	public Vector3 averageSlotPosition
	{
		get
		{
			if (slots.Count == 0)
				return transform.position;
			else 
			{
				Vector3 pos = Vector3.zero;
				foreach (Transform s in slots) 
				{
					pos += s.position;
				}
				return pos/slots.Count;
			}
		}
	}
    internal List<int> filledSlotIndexes = new List<int> ();
    internal int bookedSlotCount;


    [SerializeField][HideInInspector] public List<ZoneConnection> zoneConnections;



    void Awake () {
        AssignmentWarnings();
        filledSlotIndexes.Clear ();
		AddRemoveAdvantages ();
	}
    #region advantage
	public void AddRemoveAdvantages()
	{
		int enLength = System.Enum.GetNames (typeof(FighterRole)).Length;
		while (AdvantageAgainstFighters.Count < enLength) 
		{
			AdvantageAgainstFighters.Add (0);
		}
		while (AdvantageAgainstFighters.Count > enLength) 
		{
			AdvantageAgainstFighters.RemoveAt (AdvantageAgainstFighters.Count-1);
		}
		if(AdvantageAgainstFighters.Count!=0)
			AdvantageAgainstFighters [0] = 0;
	}

    public float GetWeightedAdvantageValue()
    {
        float advantageValue = 0;
        foreach (AITarget t in AIDataManager.EnemyTargets_readonly) {
            float addedAmount = t.weight*AdvantageAgainstFighters[(int)t.fighterID];
            //Debug.Log("added amount: " + t.weight.ToString()+"...adv: "+ AdvantageAgainstFighters[(int)t.fighterID]);
            advantageValue += addedAmount;
        }

        return advantageValue;
    }
    public bool CantSeeFighter(FighterRole ff)
    {
        return (AdvantageAgainstFighters[(int)ff] < AIPersonnel.ignoreAdvantageAgainstThreshold);
    }
    #endregion
    void AssignmentWarnings(){
		if(maxCapacity !=0){
            if(slots == null){
                Debug.LogError(transform.name + " zone has no slots assigned!");
            }
            else if(slots.Count == 0){
                Debug.LogError(transform.name + " zone has no slots assigned!");
            }
        }
        else{
            Debug.LogWarning(transform.name + " zone has zero max capacity!");
        }
		if(slots.Count<maxCapacity)
        {
            Debug.LogError(transform.parent.parent.parent.name+ "/"+transform.parent.parent.name+ "/"+transform.parent.name  + "/"+ transform.name + "zone has lesser slots than capacity!");
        }

    }

    public Transform GetEmptySlot()
    {
        int index = GetEmptySlotIndex();
        if (index < 0) {
            return null;
        } else {
            return slots [index];
        }
    }
    public int GetEmptySlotIndex()
    {
		if (filledSlotIndexes.Count >= maxCapacity || filledSlotIndexes.Count == slots.Count)
            return -1;
        int index;
        do {
            index = Random.Range (0, slots.Count);
        } while (filledSlotIndexes.Contains (index));
        return index;
    }
    #region Gizmo and auto fill area/slots
    void OnDrawGizmos() 
    {
        if (!spwnOnly)
        {
            Handy.DrawDiscYaxis(this.transform.position,zoneRadius,50,Color.green,new Color(0.3f,0.3f,0.3f,0.5f));
        }
        else
        {
            Handy.DrawDiscYaxis(this.transform.position,zoneRadius,50,Color.green, new Color(1,0,0,0.4f));
        }

        Transform slots = transform.FindChild("Slots");
        if(slots != null)
        {   
            List<Transform> slotList = new List<Transform>();
            Color baseColor = new Color(1,1f,0f,1);
            foreach (Transform item in slots) 
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
            this.slots = slotList;
        }
    }
    #endregion
}
