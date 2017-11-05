using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SWS;

public class DeliveryManager : MonoBehaviour{
	public static DeliveryManager instance;
    public List<DeliveryOption> landDeliveryOptions;
    public List<DeliveryOption> airDeliveryOptions;
    public List<DeliveryOption> mortarDeliveryOptions;


//	public GameObject mainMoverPrefab;
//    public GameObject chopperMoverPrefab;
//	public GameObject mortarMoverPrefab;
//	public List<RoadInOutPair> roadPairList_Main;
//    public List<RoadInOutPair> roadPairList_Chopper;
//	public List<RoadInOutPair> roadPairList_Mortar;

//    public RoadInOutPair randomRoadPairMain
//    {
//        get
//        {
//            return roadPairList_Main [UnityEngine.Random.Range (0, roadPairList_Main.Count)];
//        }
//    }public RoadInOutPair randomRoadPairChopper
//    {
//        get
//        {
//            return roadPairList_Chopper [UnityEngine.Random.Range (0, roadPairList_Chopper.Count)];
//        }
//    }
//	public RoadInOutPair randomRoadPairMortar
//	{
//		get
//		{
//			return roadPairList_Mortar [UnityEngine.Random.Range (0, roadPairList_Mortar.Count)];
//		}
//	}

    public DeliveryOption landDeliveryOption_RNDM
    {
        get
        {
            return landDeliveryOptions [UnityEngine.Random.Range (0, landDeliveryOptions.Count)];
        }
    }
    public DeliveryOption airDeliveryOption_RNDM
    {
        get
        {
            return airDeliveryOptions [UnityEngine.Random.Range (0, airDeliveryOptions.Count)];
        }
    }
    public DeliveryOption mortarDeliveryOption_RNDM
    {
        get
        {
            return mortarDeliveryOptions [UnityEngine.Random.Range (0, mortarDeliveryOptions.Count)];
        }
    }


	void Start()
	{
		instance = this;
	}
	public IEnumerator StopStartSounds(Transform tr, AudioSource prevSource, float delay)
	{
		yield return new WaitForSeconds (delay);
		InGameSoundManagerScript.PlayOnTransformFromIDMutable (tr, ClipID.vehicleBreak);
		yield return new WaitForSeconds (0.5f);
		if(prevSource != null)prevSource.Stop();
	}





    //========================================================================
    public static void StartDelivery(DeliveryOption deliveryOption , Action<MoverPack> onDeliveryPointReached, ClipID cID)
	{
        
		MoverPack moverPack = new MoverPack ();
        moverPack.selectedRoadInOutPair = deliveryOption.roadDefinition;
        moverPack.isOfChopperType = deliveryOption.isAirType;
        moverPack.mover = Pool.Instantiate (deliveryOption.moverPrefab,moverPack.selectedRoadInOutPair.path_in.bPoints [0].wp.position,Quaternion.identity);
		if (!deliveryOption.isAirType) {
			AudioSource audSource = InGameSoundManagerScript.PlayOnTransformFromIDMutable (moverPack.mover.transform, cID);
			instance.StartCoroutine (instance.StopStartSounds (moverPack.mover.transform, audSource, deliveryOption.roadDefinition.inTime - BaseAudioKeeper.GetClipWithID (ClipID.vehicleBreak).length));
		}
		bezierMove moverBezier = moverPack.mover.GetComponent<bezierMove> ();
		if (moverBezier == null)
			Debug.LogError ("no bezier move component on created mover!");
		moverBezier.onStart = false;
		moverBezier.moveToPath = false;
		moverBezier.reverse = false;
		moverBezier.startPoint = 0;
		moverBezier.sizeToAdd = 0;
		moverBezier.easeType = DG.Tweening.Ease.InOutSine;
		moverBezier.loopType = bezierMove.LoopType.none;
		moverBezier.timeValue = bezierMove.TimeValue.time;
		moverBezier.pathType = DG.Tweening.PathType.CatmullRom;
		moverBezier.pathMode = DG.Tweening.PathMode.Full3D;
		moverBezier.lookAhead = 0;
		moverBezier.lockPosition = DG.Tweening.AxisConstraint.None;
		moverBezier.lockRotation = DG.Tweening.AxisConstraint.None;

		moverBezier.SetPath(moverPack.selectedRoadInOutPair.path_in);
		moverBezier.speed = moverPack.selectedRoadInOutPair.inTime;
		moverBezier.StartMove ();
		int n = moverBezier.pathContainer.bPoints.Count - 1;
		moverBezier.events [n].RemoveAllListeners ();
		moverBezier.events [n].AddListener(()=>{
			moverBezier.events [n].RemoveAllListeners ();
			if(onDeliveryPointReached!=null)onDeliveryPointReached(moverPack);
		});
	}
	public static void RetrieveFromDelivery(MoverPack moverPack, Action onRetrievalComplete = null)
	{
		bezierMove moverBezier = moverPack.mover.GetComponent<bezierMove> ();
		moverBezier.pathContainer = moverPack.selectedRoadInOutPair.path_out;
		moverBezier.speed = moverPack.selectedRoadInOutPair.outTime;
		moverBezier.StartMove();
		int n = moverBezier.pathContainer.bPoints.Count - 1;
		moverBezier.events[n].RemoveAllListeners();
		moverBezier.events[n].AddListener(() =>
			{
				moverBezier.events[n].RemoveAllListeners();
				if (onRetrievalComplete != null) onRetrievalComplete();
				Pool.Destroy(moverPack.mover);
			});
	}
}
[System.Serializable]
public class RoadInOutPair{
	public BezierPathManager path_in;
	public BezierPathManager path_out;
	public float inTime = 5;
	public float outTime = 10;
}
[System.Serializable]
public class MoverPack
{
	public GameObject mover;
    public bool isOfChopperType;
	public RoadInOutPair selectedRoadInOutPair;
}