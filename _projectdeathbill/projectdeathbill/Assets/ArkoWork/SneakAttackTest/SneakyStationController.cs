using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SWS;

namespace Portbliss.SneakyStation
{
	[RequireComponent(typeof(splineMove))]
	public class SneakyStationController : MonoBehaviour {

		[SerializeField]
		private PathManager path;
		private PathManager movingPath;
		private splineMove spMove;
		private SneakyPlayer sp;
		private string stationTag = "Station";
		private List<Station> stations;
		private int currentStation = 0;
		private int currentPoint = 0;
		private bool isMoving = false;
		private bool isFixingRotation = false;
		private float defaultMoveSpeed = 6f;
		private float maxAllowedSpeed = 8f;
		private float moveSpeedDownRatePerSec = 5f;
		private Vector3 _finalTarget;
		private bool finishManualWalk = false;
		private MovementData currentMovementData;
        private Animator animator;

		private const float ROTATING_SPEED = 5f;
		private const float MIN_DISTANCE_FROM_POINT = 4f;

		public delegate void CheckPointReach();
		public event CheckPointReach OnStationReached;


		void Awake()
		{
			UpdatePathData();
		}

		void Start()
		{
            animator = this.GetComponent<Animator>();
			spMove = this.GetComponent<splineMove>();
			movingPath = (PathManager) Instantiate(path);
			sp = this.GetComponent<SneakyPlayer>();

			currentMovementData.hasData = false;
		}

		void Update()
		{
			if(isFixingRotation && !sp.isStabing)
			{
				//Debug.Log("is fixing rotation");
				Vector3 relativePos = path.waypoints[currentPoint].transform.position - transform.position;
				Quaternion rotation = Quaternion.LookRotation(relativePos);
				transform.rotation = Quaternion.Slerp(transform.rotation,rotation,1f*Time.deltaTime);
			}
		}

		public bool IsMoving()
		{
			return isMoving;
		}

		public bool IsAutoMoving()
		{
			if(!isMoving) return false;
			else
				return !currentMovementData.isUsingSpeedFalloff;
		}

		public void FinishManualWalkAutomatically()
		{
			if(isMoving)
				finishManualWalk = true;
		}

		public float GetMaxAllowedSpeed()
		{
			return maxAllowedSpeed;
		}

		public float GetCurrentSpeed()
		{
			if(isMoving)
				return spMove.speed;
			else
				return 0;
		}

		public float GetSpeedRatio()
		{
			if(isMoving)
				return spMove.speed/maxAllowedSpeed;
			else
				return 0;
		}

		public Transform GetStationTransform()
		{
			return path.waypoints[currentPoint];
		}

		public void ActivateCamera()
		{
			//move camera to station zero camera pos
            SneakyCamera.instance.SetPanRect(stations[currentStation].panRect);
			SneakyCamera.instance.TransitCamera(stations[currentStation].camT,1f,100f,null);

		}

		public void UpdatePathData()
		{
			if(path==null)
			{
				Debug.LogError("Path not assigned");
				return;
			}

			stations = new List<Station>();

			Station s;
			Transform cameraT;

			for(int i=0;i<path.waypoints.Length;i++)
			{
				if(path.waypoints[i].CompareTag(stationTag))
				{
					//got a new station
					cameraT = path.waypoints[i].FindChild("Camera");
					s = new Station(cameraT,i);
					stations.Add(s);

					//remove camera
					if(cameraT!=null)
					{
						Destroy(cameraT.GetComponent<GUILayer>());
						Destroy(cameraT.GetComponent<FlareLayer>());
						Destroy(cameraT.GetComponent<AudioListener>());
						Destroy(cameraT.GetComponent<Camera>());
					}
				}
			}

			currentPoint = 0;
			currentStation = 0;
		}

		public Transform GetNextStationPoint()
		{
			int p = stations[currentStation+1].GetStationWaypointIndex();
			return path.waypoints[p].transform;
		}

		public void UpdateStationPoint()
		{
			currentStation ++;
			currentPoint = stations[currentStation].GetStationWaypointIndex();

			//Move is complete call
			spMove.moveCompleted.RemoveAllListeners();
			spMove.ChangeSpeed(defaultMoveSpeed);
			isFixingRotation = false;
			finishManualWalk = false;
			//reposition camera if it has camera setup
			if(stations[currentStation].hasCamera)
			{
                SneakyCamera.instance.SetPanRect(stations[currentStation].panRect);
				SneakyCamera.instance.TransitCamera(stations[currentStation].camT,1f,1f,null);				
			}
		}
			

		public bool MoveToNextStation(Transform finalTarget=null, bool useSpeedFalloff = false)
		{
			if(isMoving) return false;

            animator.SetInteger("WalkType",0);
			GenerateStationMovePath(currentPoint);
			StartMove(useSpeedFalloff);
			currentStation ++;
			currentPoint = stations[currentStation].GetStationWaypointIndex();

			if(finalTarget==null) 
			{
				//TODO
				// Set direction properly here
				_finalTarget = path.waypoints[Mathf.Min(currentPoint+1, path.waypoints.Length-1)].transform.position + transform.forward*20f;
			}
			else
			{
				_finalTarget = finalTarget.position;
			}

			if(useSpeedFalloff)
			{
				StartCoroutine("IE_UseSpeedFallOff");
			}

			currentMovementData = new MovementData(useSpeedFalloff,finalTarget);

			return true;
		}

		IEnumerator IE_UseSpeedFallOff()
		{
			do
			{
				float distance = (movingPath.waypoints[movingPath.waypoints.Length-1].position-this.transform.position).sqrMagnitude;
				if(!finishManualWalk)
					spMove.ChangeSpeed(Mathf.Clamp(spMove.speed-moveSpeedDownRatePerSec*Time.deltaTime,0,maxAllowedSpeed));
				else
					spMove.ChangeSpeed(Mathf.Clamp(spMove.speed,defaultMoveSpeed,maxAllowedSpeed));

				yield return null;
			}while(isMoving == true);
		}

		private float speedGain = 1.3f;
		public void SpeedUp()
		{
			spMove.ChangeSpeed(Mathf.Clamp(spMove.speed+speedGain,0,maxAllowedSpeed));
		}

		public void GetBackToPreviousCover()
		{
			//check if walking
			if(!isMoving) return;
			//check for manual walk
			if(!currentMovementData.isUsingSpeedFalloff) return;
			//check cover distance
			int p = stations[currentStation-1].GetStationWaypointIndex();
			Vector3 coverPoint = path.waypoints[p].position;
			if(Vector3.SqrMagnitude((coverPoint-this.transform.position))<2f) return;
			//check if stabbing
			if(sp.isStabing) return;

			//we will go there externally
			StopCoroutine("IE_UseSpeedFallOff");
			isMoving = false;
			isFixingRotation = false;
			currentStation --;
			currentPoint = stations[currentStation].GetStationWaypointIndex();
			spMove.Stop();
			sp.GetMovementController().OnPrevPointReached+= ResumePreviousMovement;
			sp.GetMovementController().GetBackToPreviousCover(coverPoint);

		}

		private void ResumePreviousMovement()
		{
			//Debug.Log("previous move manual? "+ currentMovementData.isUsingSpeedFalloff);
			MoveToNextStation(currentMovementData.finalTarget,currentMovementData.isUsingSpeedFalloff);

			//release ourselves from external call back
			sp.GetMovementController().OnPrevPointReached+= ResumePreviousMovement;
		}

		private void StartMove(bool isManual = false)
		{
			isMoving = true;

			float defWait = 0;

			/*if(ai!=null)
			{
				if(!ai.IsAIOn())
					defWait = CameraController.instance.StartFollowing();
			}
			else
			{
				defWait = CameraController.instance.StartFollowing();
			}*/

			StartCoroutine(TurnGradually(movingPath.waypoints[1].position,()=>{
				spMove.SetPath(movingPath);
				spMove.moveCompleted.AddListener(MoveComplt);

				if(isManual)
					spMove.ChangeSpeed(1f);
				else
					spMove.ChangeSpeed(defaultMoveSpeed);
				
				spMove.StartMove();
				isFixingRotation = true;
			},defWait));
		}

		private IEnumerator TurnGradually(Vector3 target, Action callback, float defWait = 0f)
		{
			float elapsedTime =0;
			float tolerance = 5f;
			Vector3 dir = this.transform.forward;
			target.y = this.transform.position.y;
			Vector3 targetDir = (target-this.transform.position).normalized;
			float angle = Vector3.Angle(dir,targetDir);
			Quaternion lookRotation;

			//Debug.Log("lagging angle "+Vector3.Angle(dir,targetDir));

			do
			{
				lookRotation = Quaternion.LookRotation(targetDir);
				transform.rotation = Quaternion.Lerp(transform.rotation,lookRotation,Time.deltaTime*ROTATING_SPEED);

				dir = this.transform.forward;
				targetDir = (target-this.transform.position).normalized;
				angle = Vector3.Angle(dir,targetDir);
				elapsedTime+=Time.deltaTime;
				yield return null;
			}while(angle>tolerance);

			if(defWait!=0)
			{
				if((defWait-elapsedTime)>0)
					yield return new WaitForSeconds(defWait-elapsedTime);
			}
			if(callback!=null)
				callback();
		}

		private void MoveComplt()
		{
			spMove.moveCompleted.RemoveAllListeners();
			isFixingRotation = false;
			finishManualWalk = false;
			//reposition camera if it has camera setup
			if(stations[currentStation].hasCamera)
			{
                SneakyCamera.instance.SetPanRect(stations[currentStation].panRect);
				SneakyCamera.instance.TransitCamera(stations[currentStation].camT,1f,1f,null);				
			}

			StartCoroutine(TurnGradually(_finalTarget, ()=>{
				isMoving = false;
				spMove.ChangeSpeed(defaultMoveSpeed);

				currentMovementData.Clear();

				if(OnStationReached!=null) OnStationReached();

			}));

		}

		private void GenerateStationMovePath(int fromPoint)
		{
			List<Transform> points = new List<Transform>();

			points.Add(path.waypoints[fromPoint]);

			for(int i=fromPoint+1;i<path.waypoints.Length;i++)
			{

				if (path.waypoints[i].CompareTag(stationTag))
				{
					points.Add(path.waypoints[i]);
					break;
				}
				else
				{
					points.Add(path.waypoints[i]);
				}
			}

			//list generated
			movingPath.waypoints = points.ToArray();
		}

	}

	public class Station
	{
		public Transform camT;
		public bool hasCamera = false;
		public int _index;
		public PanRect panRect;

		public Station (Transform camera, int index)
		{
			if(camera!=null)
			{
				camT = camera;
				hasCamera = true;

				CameraPanRect cpr = camera.GetComponent<CameraPanRect>();
				if(cpr!=null)
				{
					panRect = cpr.panRect;
				}
				else
				{
					panRect = PanRect.GetSqRect(3f);
				}
			}
			else
			{
				hasCamera = false;
			}

			_index = index;
		}

		public int GetStationWaypointIndex()
		{
			return _index;
		}
	}

	public struct MovementData
	{
		public bool isUsingSpeedFalloff;
		public Transform finalTarget;
		public bool hasData ;

		public MovementData(bool isManual, Transform final_target)
		{
			hasData = true;
			isUsingSpeedFalloff = isManual;
			finalTarget = final_target;
		}

		public void Clear()
		{
			hasData = false;
		}
	}
}
