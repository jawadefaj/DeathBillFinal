using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Portbliss.WayFinder
{
	
	public class WayFinder : MonoBehaviour {

		public Transform target;
		public Transform player;

		private List<Vector3> pathPoints = new List<Vector3>();

		// Use this for initialization
		void Start () {
			FindWay();
		}
		
		// Update is called once per frame
		void Update () {

			if(pathPoints.Count>1)
			{
				for(int i=0;i<pathPoints.Count-1;i++)
					Debug.DrawLine(pathPoints[i],pathPoints[i+1],Color.red);
			}
		
		}

		void FindWay()
		{
			Vector3 fromPoint = player.position;
			Vector3 toPoint = target.position;

			WayFindData data;

			do{
				data = RayCaster(fromPoint,toPoint);
				//add the returned points
				foreach(Vector3 p in data.points)
				{
					pathPoints.Add(p);
				}

				fromPoint = pathPoints[pathPoints.Count-1];
				//Debug.Log("main way found procedure");

			}while(data.didFindAWay == false);
				
			//foreach(Vector3 p in pathPoints)
			//	Debug.Log(p);
		}

		float minRayCastDistance = 2f;
		private WayFindData RayCaster(Vector3 fromPoint, Vector3 toPoint)
		{
			RaycastHit hit;
			WayFindData data;


			data.points = new List<Vector3>();

			float distanceBetweenPoints = Vector3.Distance(fromPoint,toPoint);

			if(Physics.SphereCast(fromPoint,0.5f,(toPoint-fromPoint).normalized,out hit,distanceBetweenPoints))
			//if(Physics.Raycast(fromPoint,(toPoint-fromPoint
			{
				if( Mathf.Abs(hit.distance - distanceBetweenPoints) <0.1f)
				{
					data.didFindAWay = true;
					data.points.Add(fromPoint);
					data.points.Add(toPoint);

					//Debug.Log("Closely hit, so found a way");
					return data;
				}
				else
				{
					//way not found
					if(hit.distance > minRayCastDistance)
					{
						float targetDistance = hit.distance - minRayCastDistance;
						Vector3 intPoint = (hit.point-fromPoint).normalized*targetDistance + fromPoint;
						//find the last point of the node
						Vector3 lastPoint = IntermidiateRaycaster(intPoint,hit.point);

						data.didFindAWay = false;
						data.points.Add(fromPoint);
						data.points.Add(intPoint);
						data.points.Add(lastPoint);
						//Debug.Log("No way was found. hit dastance is too far");
						return data;
					}
					else
					{
						float targetDistance = hit.distance - minRayCastDistance;
						Vector3 intPoint = fromPoint;
						//find the last point of the node
						Vector3 lastPoint = IntermidiateRaycaster(intPoint,hit.point);

						data.didFindAWay = false;
						data.points.Add(fromPoint);
						data.points.Add(lastPoint);
						//Debug.Log("No way was found. hit dastance is too close");
						return data;
					}
				}
			}
			else
			{
				//no hit
				data.didFindAWay = true;
				data.points.Add(fromPoint);
				data.points.Add(toPoint);
				//Debug.Log("No hit, so found a way");
				return data;
			}
		}

		private Vector3 IntermidiateRaycaster(Vector3 fromPoint, Vector3 toPoint)
		{
			Vector3 dir = (toPoint-fromPoint).normalized;
			RaycastHit hit;
			bool didHit = false;


			do
			{
				//rotate the vector
				dir = Quaternion.Euler(0,30,0)*dir;

				didHit = Physics.SphereCast(fromPoint,0.5f,dir,out hit,minRayCastDistance);
			}while(didHit==true);

			return fromPoint+dir*minRayCastDistance;
		}
	
	}

	struct WayFindData
	{
		public bool didFindAWay;
		public List<Vector3> points;
	}

}
