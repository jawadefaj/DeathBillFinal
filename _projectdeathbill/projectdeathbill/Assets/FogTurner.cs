using UnityEngine;
using System.Collections;

public class FogTurner : MonoBehaviour {

    public float turnRateDegrees;
//    [Range(0,1)]
//    public float deviation;
//    public float avgFixedTime;
//    [Range(0,1)]
//    public float fixedTimeVariance;
//    public float ChangeLerpRate = 0.05f;
//
//
//    float PF_NCT;
//    float nextChangeTime{
//        get{ 
//            float f = PF_NCT;
//            PF_NCT = Time.time + Handy.Deviate(avgFixedTime,fixedTimeVariance);
//            return f;
//        }
//    }
//
//    float maxRate;
//    float minRate;
//    float currentTurnRate;
//
//    void Start()
//    {
//        maxRate = turnRateDegrees * (1 + deviation);
//        minRate = turnRateDegrees * (1 - deviation);
//        nextChangeTime;
//    }

	void Update () 
    {
        
        this.gameObject.transform.Rotate (this.transform.up*Time.deltaTime*turnRateDegrees);
	}
}
