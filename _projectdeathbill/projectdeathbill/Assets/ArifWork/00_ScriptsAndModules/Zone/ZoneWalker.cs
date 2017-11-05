using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using SWS;
[RequireComponent(typeof (splineMove))]
public class ZoneWalker : MonoBehaviour {
	
    //public static GameObjectPool splinePathPool;

	public Zone currentZone;
	public int slotIndex = -1;

	public Action startedMove;
	public Action finishedMove;
	public Action abortedMove;
	private splineMove spline;

    PathManager pathA;
	PathManager pathB;
	PathManager pathN;
	Transform pathN_transform;

    static Vector3 tempVec3 = new Vector3();
    static Vector2 wpVec = new Vector2();
    static Vector2 pwVec = new Vector2();
    static int wpMaxCount;
    static float dotDistance;
	/*
	public void GoToZoneUsingPath(PathManager path, Zone endZone)
	{
		if (spline == null)
			spline = GetComponent<splineMove> ();
		if (endZone.zoneParameters.max_cap <= endZone.bookedSlotCount)
			Debug.LogError ("no space in end zone");

		pathN = path;
		pathN_transform = path.transform;
		//pathN_transform.SetParent(pathA.transform.parent);

		List<Transform> availableSlots = new List<Transform> ();
		for (int i = 0; i < endZone.slots.Count; i++) {
			if (!endZone.filledSlotIndexes.Contains (i)) {
				availableSlots.Add (endZone.slots [i]);
			}
		}
		if (availableSlots.Count <= 0)
			Debug.LogError ("no space in end zone");
		float distance = float.MaxValue;
		Vector3 targetPosition = new Vector3 ();
		int targetIndex = -1;
		for (int i = 0; i < availableSlots.Count; i++) {
			wpMaxCount = pathN.waypoints.Length;
			tempVec3 = pathN.waypoints [wpMaxCount - 1].position - pathN.waypoints [wpMaxCount - 2].position;
			wpVec.x = tempVec3.x;
			wpVec.y = tempVec3.z;
			tempVec3 = availableSlots [i].position - pathN.waypoints [wpMaxCount - 1].position;
			pwVec.x = tempVec3.x;
			pwVec.y = tempVec3.z;

			dotDistance = Math.Abs (pwVec.x * wpVec.y - pwVec.y * wpVec.x);
			if (dotDistance < distance) {
				distance = dotDistance;
				targetPosition = availableSlots [i].position;
				targetIndex = endZone.slots.IndexOf (availableSlots [i]);
			}
		}
		endZone.filledSlotIndexes.Add (targetIndex);
		pathN.waypoints [pathN.waypoints.Length - 1].position = targetPosition;
		//self zone data update
		currentZone = endZone;
		slotIndex = targetIndex;

		spline.moveToPath = true;
		spline.SetPath (pathN);
		int endPointIndex = pathN.waypoints.Length - 1;

		spline.events [endPointIndex].RemoveAllListeners ();
		spline.events [endPointIndex].AddListener (() => {
			spline.events [endPointIndex].RemoveAllListeners ();
			Pool.Destroy (pathN.gameObject);
			if (finishedMove != null)
				finishedMove ();
		});
		if (startedMove != null) startedMove ();
	}
*/

    public void GoToZone(ZoneConnection zc)//assumes zone is available
	{
        if (spline == null) spline = GetComponent<splineMove>();
        bool willGo = false;
        if (zc != null)
        {
            willGo = true;
            if (currentZone.filledSlotIndexes != null)
                if (currentZone.filledSlotIndexes.Contains(slotIndex))
                    currentZone.filledSlotIndexes.Remove(slotIndex);
            if (zc.endZone.filledSlotIndexes == null) zc.endZone.filledSlotIndexes = new List<int>();


            #region pathmaking
            pathA = zc.zoneRoad.side1;
            pathB = zc.zoneRoad.side2;
            if (!ZoneManager.instance.pathPrefabList.Contains(pathA.gameObject)) 
			{ 
				ZoneManager.instance.pathPrefabList.Add(pathA.gameObject); 
			}
            pathN_transform = Pool.Instantiate(pathA.gameObject, Vector3.zero, Quaternion.identity).transform;
            pathN = pathN_transform.GetComponent<PathManager>();
            pathN_transform.SetParent(pathA.transform.parent);

            float lerpRatio = CalculateLerpRatio(zc.isFromType);
            for (int i = 0; i < pathN.waypoints.Length; i++)
            {
                int k;
                if (!zc.isFromType) k = i;
                else k = pathA.waypoints.Length - 1 - i;
                pathN.waypoints[i].position = Vector3.Lerp(pathA.waypoints[k].position, pathB.waypoints[k].position, lerpRatio);
            }

            List<Transform> availableSlots = new List<Transform>();
            for (int i = 0; i < zc.endZone.slots.Count; i++)
            {
                if (!zc.endZone.filledSlotIndexes.Contains(i))
                {
                    availableSlots.Add(zc.endZone.slots[i]);
                }
            }
            float distance = float.MaxValue;
            Vector3 targetPosition = new Vector3();
            int targetIndex = -1;
            for (int i = 0; i < availableSlots.Count; i++)
            {
                wpMaxCount = pathN.waypoints.Length;
                tempVec3 = pathN.waypoints[wpMaxCount - 1].position - pathN.waypoints[wpMaxCount - 2].position;
                wpVec.x = tempVec3.x;
                wpVec.y = tempVec3.z;
                tempVec3 = availableSlots[i].position - pathN.waypoints[wpMaxCount - 1].position;
                pwVec.x = tempVec3.x;
                pwVec.y = tempVec3.z;

                dotDistance = Math.Abs(pwVec.x * wpVec.y - pwVec.y * wpVec.x);
                if (dotDistance < distance)
                {
                    distance = dotDistance;
                    targetPosition = availableSlots[i].position;
                    targetIndex = zc.endZone.slots.IndexOf(availableSlots[i]);
                }
            }

            zc.endZone.filledSlotIndexes.Add(targetIndex);
            pathN.waypoints[pathN.waypoints.Length - 1].position = targetPosition;
            #endregion
            //self zone data update
            currentZone = zc.endZone;
            slotIndex = targetIndex;

            spline.moveToPath = true;
            spline.SetPath(pathN);
            int endPointIndex = pathN.waypoints.Length - 1;
            //for (int i = 0; i < spline.waypoints.Length; i++)
            // {
            //    spline.events[i].RemoveAllListeners();
            // }
            spline.events[endPointIndex].RemoveAllListeners();
            spline.events[endPointIndex].AddListener(() => {
                spline.events[endPointIndex].RemoveAllListeners();
                Pool.Destroy(pathN.gameObject);
				if(finishedMove!=null) finishedMove();
            });
        }
        else
        {
            List<Transform> availableSlots = new List<Transform>();
            for (int i = 0; i < currentZone.slots.Count; i++)
            {
				//Debug.Log (currentZone.filledSlotIndexes);
                if (!currentZone.filledSlotIndexes.Contains(i))
                {
                    availableSlots.Add(currentZone.slots[i]);
                }
            }
			if (availableSlots.Count != 0) {
				willGo = true;
				Transform chosenSlot = availableSlots [UnityEngine.Random.Range (0, availableSlots.Count - 1)];
				pathN = Pool.Instantiate (ZoneManager.instance.intraZoneSamplePath.gameObject, Vector3.zero, Quaternion.identity).GetComponent<PathManager> ();
				pathN.waypoints [0].position = transform.position;
				pathN.waypoints [1].position = chosenSlot.position;
				pathN.transform.SetParent (ZoneManager.instance.intraZoneSamplePath.transform.parent);
				int newIndex = currentZone.slots.IndexOf (chosenSlot);
				currentZone.filledSlotIndexes.Add (newIndex);
				currentZone.filledSlotIndexes.Remove (slotIndex);

//				Debug.Log ("R2");
				slotIndex = newIndex;
				//spline.moveToPath = true;
				spline.SetPath (pathN);
				int endPointIndex = pathN.waypoints.Length - 1;
				spline.events [endPointIndex].RemoveAllListeners ();
				spline.events [endPointIndex].AddListener (() => {
					spline.events [endPointIndex].RemoveAllListeners ();
					Pool.Destroy (pathN.gameObject);
					if (finishedMove != null)
						finishedMove ();
				});
			} 
        }

        if (!willGo)
        {
			if (abortedMove != null) abortedMove ();
        }
        else
        {
			if (startedMove != null) startedMove ();
        }      
	}

	public void ClearInfoFromZone()
	{
		//Debug.Log ("A");
		if (currentZone != null) {
			//Debug.Log ("B");
			if (currentZone.filledSlotIndexes.Contains (slotIndex)) {
				currentZone.filledSlotIndexes.Remove(slotIndex);
				currentZone.bookedSlotCount--;
				currentZone = null;
			}
		}
	}

	#region Calculate Lerp Ratio
	float CalculateLerpRatio(bool isFromTypeRoad)		//from type road true means needs reversal of road
	{	

		int i0;
		int i1;
		if(!isFromTypeRoad)
		{
			i0 = 0;
			i1 = 1;
		}
		else{
			i0 = pathA.waypoints.Length - 1;
			i1 = i0 -1;
		}

		Vector2 wD =(new Vector2(	pathA.waypoints[i1].position.x - pathA.waypoints[i0].position.x,
		                            pathA.waypoints[i1].position.z - pathA.waypoints[i0].position.z) 
		            +new Vector2(	pathB.waypoints[i1].position.x - pathB.waypoints[i0].position.x,
		             				pathB.waypoints[i1].position.z - pathB.waypoints[i0].position.z) 
		             )/2;
		Vector2 aD = new Vector2( 	pathB.waypoints[i0].position.x - pathA.waypoints[i0].position.x,
		                         	pathB.waypoints[i0].position.z - pathA.waypoints[i0].position.z);
		Vector2 fP = new Vector2(transform.position.x,transform.position.z);
		Vector2 rP = new Vector2(pathA.waypoints[i0].position.x, pathA.waypoints[i0].position.z);
		
		float q = (wD.x*(fP.y-rP.y) + wD.y*(rP.x-fP.x))/(aD.y*wD.x - aD.x*wD.y);
		
		Vector3 intersectPoint = new Vector3(rP.x + q*aD.x, (pathA.waypoints[i0].position.y + pathB.waypoints[i0].position.y)/2, rP.y + q*aD.y);
	
		return Vector3.Magnitude( pathA.waypoints[i0].position - intersectPoint)/Vector3.Magnitude(pathA.waypoints[i0].position - pathB.waypoints[i0].position);
	}
	#endregion
}
