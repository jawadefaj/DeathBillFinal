using UnityEngine;
using System.Collections;

public class DeliveryOption : MonoBehaviour {
    public GameObject moverPrefab;
    public RoadInOutPair roadDefinition;
    /// <summary>
    /// The destination.
    /// for zoneblock, use transform with zoneblock script 
    /// for helidrop use transform with drom mechanix on it
    /// </summary>
    public Transform destination; 
   


    public ChopperDropArea destDropArea
    {
        get
        {
            return destination.GetComponent<ChopperDropArea>();
        }
    }
    public bool isAirType
    {
        get
        {
            return (destDropArea != null);
        }
    }

    public ZoneBlock destZoneBlock
    {
        get
        {
            if (isAirType)
            {
                return destDropArea.attachedZoneBlock;
            }
            else
                return destination.GetComponent<ZoneBlock>();
        }
    }
}
