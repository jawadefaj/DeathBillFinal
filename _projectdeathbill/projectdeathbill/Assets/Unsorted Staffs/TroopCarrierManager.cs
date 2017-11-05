using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SWS;

//public class TroopCarrierManager : MonoBehaviour {
//	public static TroopCarrierManager instance;
//	public bezierMove troopTruckBezier;
//    public bezierMove mortarTruckBezier;
//    public TruckRoadPair truckRoadPair;
//    public List<TruckRoadPair> truckRoadPairsForMortar;
//
//    private Transform spwnPoint;
//    //private bezierMove thisBezier;
//    //private TruckRoadPair currentPair;
//    private int chosenMortarRoadID;
//     
//
//	void Awake()
//    {
//        troopTruckBezier.GetComponent<AudioSource>().volume = InGameSoundManagerScript.truckEngineVolume;
//        mortarTruckBezier.GetComponent<AudioSource>().volume = InGameSoundManagerScript.jeepEngineVolume;
//        instance = this;
//	}
//    public void StartDeliveringTroops(Action OnDestination){
//        if(UserSettings.SoundOn)troopTruckBezier.GetComponent<AudioSource>().Play();
//        //Debug.Log("delivering: troops");
//        bezierMove thisBezier;
//        TruckRoadPair currentPair;
//        
//
//        thisBezier = troopTruckBezier;
//        currentPair = truckRoadPair;
//        
//		thisBezier.SetPath(currentPair.path_in);
//		thisBezier.speed = currentPair.inTime;
//		thisBezier.StartMove ();
//		int n = thisBezier.pathContainer.bPoints.Count - 1;
//		thisBezier.events [n].RemoveAllListeners ();
//		thisBezier.events [n].AddListener(()=>{
//			thisBezier.events [n].RemoveAllListeners ();
//			if(OnDestination!=null)OnDestination();
//		});
//	}
//
//    public void ReturnFromDeliveringTroops(Action OnReturnHome = null)
//    {
//        if (UserSettings.SoundOn) troopTruckBezier.GetComponent<AudioSource>().Play();
//        //Debug.Log("returning after delivering: troops");
//
//        bezierMove thisBezier;
//        TruckRoadPair currentPair;
//        thisBezier = troopTruckBezier;
//        currentPair = truckRoadPair;
//
//        thisBezier.pathContainer = currentPair.path_out;
//		thisBezier.speed = currentPair.outTime;
//		thisBezier.StartMove ();
//        int n = thisBezier.pathContainer.bPoints.Count - 1;
//        thisBezier.events[n].RemoveAllListeners();
//        thisBezier.events[n].AddListener(() => {
//            thisBezier.events[n].RemoveAllListeners();
//            if (OnReturnHome != null) OnReturnHome();
//            
//        });
//    }
//    public void StartDeliveringMortar( Action OnDestination)
//    {
//        if (UserSettings.SoundOn) mortarTruckBezier.GetComponent<AudioSource>().Play();
//        //Debug.Log("delivering: mortar");
//        bezierMove thisBezier;
//        TruckRoadPair currentPair;
//
//        thisBezier = mortarTruckBezier;
//        chosenMortarRoadID = UnityEngine.Random.Range(0, truckRoadPairsForMortar.Count);
//        currentPair = truckRoadPairsForMortar[chosenMortarRoadID];
//
//        thisBezier.SetPath(currentPair.path_in);
//        thisBezier.speed = currentPair.inTime;
//        thisBezier.StartMove();
//        int n = thisBezier.pathContainer.bPoints.Count - 1;
//        thisBezier.events[n].RemoveAllListeners();
//        thisBezier.events[n].AddListener(() => {
//            thisBezier.events[n].RemoveAllListeners();
//            if (OnDestination != null) OnDestination();
//        });
//    }
//
//    public void ReturnFromDeliveringMortar(Action OnReturnHome = null)
//    {
//        if (UserSettings.SoundOn) mortarTruckBezier.GetComponent<AudioSource>().Play();
//        //Debug.Log("returning after delivering: troops");
//
//        bezierMove thisBezier;
//        TruckRoadPair currentPair;
//        thisBezier = mortarTruckBezier;
//        currentPair = truckRoadPairsForMortar[chosenMortarRoadID];
//
//        thisBezier.pathContainer = currentPair.path_out;
//        thisBezier.speed = currentPair.outTime;
//        thisBezier.StartMove();
//        int n = thisBezier.pathContainer.bPoints.Count - 1;
//        thisBezier.events[n].RemoveAllListeners();
//        thisBezier.events[n].AddListener(() =>
//        {
//            thisBezier.events[n].RemoveAllListeners();
//            if (OnReturnHome != null) OnReturnHome();
//
//        });
//    }
//    [System.Serializable]
//	public class TruckRoadPair{
//		public BezierPathManager path_in;
//		public BezierPathManager path_out;
//		public float inTime = 5;
//		public float outTime = 10;
//	}
//}
