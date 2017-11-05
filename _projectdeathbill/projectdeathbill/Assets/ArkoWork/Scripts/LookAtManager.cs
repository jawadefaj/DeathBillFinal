using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LookAtManager : MonoBehaviour {

	public static LookAtManager instance;

	public bool debugMode = false;

	private const float MIN_POINT_DISTANCE_SQ = 25f;

	List<LookAtInfo> points = new List<LookAtInfo>();

	void Awake()
	{
		instance = this;

		PlayerViewRectContainer pvrc;

		foreach(Transform t in this.transform)
		{
			Vector3 p = t.position;
			ViewRect v = ViewRect.Zero();

			pvrc = t.gameObject.GetComponent<PlayerViewRectContainer>();
			if(pvrc!=null) v = pvrc.viewRect;

			points.Add(new LookAtInfo(p,v));
			t.gameObject.SetActive(false);
		}
		//Debug.Log(points.Count);
	}

	float radius = 0.2f;

	void OnDrawGizmos()
	{
		if(debugMode)
		{
			//radius = .5f*Mathf.Sin(Time.time*0.8f);
			radius = 0.5f;

			for(int i=0;i<points.Count;i++)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(points[i].point,radius);
			}
		}
	}

	public LookAtInfo GetLookAtPoint(Vector3 fromPos)
	{
		//Vector3 minPoint = Vector3.zero;
		LookAtInfo lai = new LookAtInfo();
		float minDistance= float.MaxValue;
		float distance;

		for(int i=0;i<points.Count;i++)
		{
			distance = (fromPos-points[i].point).sqrMagnitude;

			if(distance<minDistance)
			{
				minDistance = distance;
				lai = points[i];
			}
		}

		if (minDistance < MIN_POINT_DISTANCE_SQ)
			return lai;
		else
			return new LookAtInfo (Vector3.zero, ViewRect.Zero());
	}

	public struct LookAtInfo
	{
		public Vector3 point;
		public ViewRect vr;

		public LookAtInfo(Vector3 _point, ViewRect _vr)
		{
			point = _point;
			vr = _vr;
		}
	}
}


