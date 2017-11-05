using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SWS;

namespace Portbliss.Station
{
	[RequireComponent(typeof(splineMove))]
	public class StationController : MonoBehaviour {

		[SerializeField]
		private PathManager path;
		private PathManager movingPath;
		private splineMove spMove;

		private List<Station> stations;
		private int currentPoint = 0;
		private int currentStation=0;
		private bool isMoving;
		private bool isMovingBetweenStations = false;
		private bool isAutoMoving = false;
		private float defaultMoveSpeed = 5f;
		//[SerializeField]
		private float maxAllowedSpeed = 8f;
		//[SerializeField]
		private float moveSpeedDownRatePerSec = 5f;
		private string stationTag = "Station";
		private string stationOutTag = "StationOut";
		private string wayOutTag = "WayOut";
		private string coverPointTag = "CoverPoint";
		private Vector3 _finalTarget;
		private LookAtManager.LookAtInfo _finalTargetInfo;
		private PlayerAI ai;
        private ThirdPersonController _pController;
		private ThirdPersonController playerController
        {
            get
            {
                if(_pController==null) _pController = this.GetComponent<ThirdPersonController>();
                return _pController;
            }
            set
            {
                _pController = value;
            }
        }

		public bool leftWayOpen = false;
		public bool rightWayOpen = false;

		public delegate void CheckPointReach();
		public event CheckPointReach OnStationReached;

		private const float ROTATING_SPEED = 50f;
		private const float MIN_DISTANCE_FROM_POINT = 50f;

		void Awake()
		{
			UpdatePathData();

			if(path==null) 
			{
				this.enabled = false;
				return;
			}
			spMove = this.GetComponent<splineMove>();
			movingPath = (PathManager) Instantiate(path);
			ai = this.GetComponent<PlayerAI>();
			playerController = this.GetComponent<ThirdPersonController>();

            //UpdateCurrentStationData();
			UpdateLeftRightCoverPointAvailability ();
		}

		/*void OnGUI()
		{
			if(GUI.Button(new Rect(10,10,100,100),"Next CheckPoint"))
				MoveToNextStation();

			if(GUI.Button(new Rect(110,10,100,100),"Move"))
				MoveToNextMovePoint();

			if(GUI.Button(new Rect(210,10,100,100),"Manual Move"))
				MoveToNextStation(true);
		}

		void Update()
		{
			if(Input.GetKeyDown(KeyCode.T))
				SpeedUp();
		}*/

		public void SetPath(PathManager pm)
		{
			path = pm;
			currentPoint =0;
			currentStation =0;
			UpdatePathData();
			UpdateCurrentStationData();
		}

		public void UpdateCurrentStationData()
		{
			//update current station point based on distacnce
			Transform t;
			float minDistance = float.MaxValue;
			int now_point = 0;
			int now_station = 0;
			Station s;

			if(stations==null) Debug.Log("null station");
			for(int i=0;i<stations.Count;i++)
			{
				s = stations[i];
				t = path.waypoints[s.GetStationWaypointIndex()].transform;

				float distance = Vector3.SqrMagnitude(t.position-this.transform.position);
				if(distance<minDistance)
				{
					minDistance = distance;
					now_point = s.GetStationWaypointIndex();
					now_station = i;
				}
			}

			currentPoint = now_point;
			currentStation = now_station;

			UpdateLeftRightCoverPointAvailability ();
		}

		public void UpdatePathData()
		{
			if(path==null)
			{
				Debug.LogError("Path not assigned");
				return;
			}

			stations = new List<Station>();
			List<int> coverPoints;
			Station s;

			for(int i=0;i<path.waypoints.Length;i++)
			{
				if(path.waypoints[i].CompareTag(stationTag))
				{
					//got a new station
					coverPoints = new List<int>();
					coverPoints.Add(i);

					//get the next moveable points
					for(int j=i+1;j<path.waypoints.Length;j++)
					{
						if(path.waypoints[j].CompareTag(coverPointTag))
							coverPoints.Add(j);
						if(path.waypoints[j].CompareTag(stationOutTag))
							break;
						if(path.waypoints[j].CompareTag(stationTag))
							break;
					}

					//generate a new station
					s= new Station(i,coverPoints.ToArray());
					stations.Add(s);
				}
			}
		}

        public bool MoveToNextStation(Transform finalTarget=null, bool useSpeedFalloff = false, Action<bool> callback=null)
		{
			if(isMoving) return false;

			if(playerController.IsDead()) return false;

			if(playerController.IsThrowingGranade()) return false;

			if(playerController!=null)
			{
				if(playerController.IsReloading() || playerController.IsShooting() || playerController.IsThrowingGranade()) return false;
			}
			else
			{
				return false;
			}

			//turn of shake
			//ShakeCamera.instance.StopCameraShake();

			GenerateStationMovePath(currentPoint);
            StartMove(true,callback);
			currentStation ++;
			currentPoint = stations[currentStation].GetStationWaypointIndex();

			if(finalTarget==null) 
			{
				_finalTargetInfo = LookAtManager.instance.GetLookAtPoint(path.waypoints[currentPoint].position);
				_finalTarget = _finalTargetInfo.point;
			}
			else
			{
				_finalTarget = finalTarget.position;
			}

			if(useSpeedFalloff)
			{
				StartCoroutine(IE_UseSpeedFallOff());
			}

			//start cinematics cover down
			if(ai==null)
			{
				CinematicCoverUp.instance.FlagDown();
				//reset camera field of view
                ImprovedCameraCntroller.instance.ResetFieldOfView();
			}

			isMovingBetweenStations = true;

			isAutoMoving = !useSpeedFalloff;

			return true;
		}

        public bool MoveToNextMovePoint(Transform finalTarget = null, Action<bool> callback = null)
		{
			if(isMoving) return false ;
			if(playerController.IsDead()) return false;
			if(playerController.IsReloading() || playerController.IsShooting() || playerController.IsThrowingGranade()) return false;

			//stop camrea transityion
			//Debug.Log("Yellow "+CameraController.instance.isCameraDragging.ToString());
			//if(CameraController.instance.IsCameraTransiting()) CameraController.instance.StopAllTransition();
			//Debug.Log("Yellow "+CameraController.instance.isCameraDragging.ToString());

			//turn of shake
			//ShakeCamera.instance.StopCameraShake();

			int toPoint = stations[currentStation].GetNextMoveablePoint(currentPoint);

			//cancel move if in the same point
			if(currentPoint==toPoint) return false;

			if(finalTarget==null) 
			{
				_finalTargetInfo = LookAtManager.instance.GetLookAtPoint(path.waypoints[toPoint].position);
				_finalTarget = path.waypoints[toPoint].position + this.transform.forward*5f;
			}
			else
			{
				_finalTarget = finalTarget.position;
			}

			//reset camera field of view
            ImprovedCameraCntroller.instance.ResetFieldOfView();

			GenerateCoverMovePath(currentPoint,toPoint);
			StartMove(false,callback);
			currentPoint = toPoint;
			isMovingBetweenStations = false;
			isAutoMoving = true;
			//Debug.Log("Yellow "+CameraController.instance.isCameraDragging.ToString());
			return true;
		}

        public bool MoveToPrevMovePoint(Transform finalTarget=null, Action<bool> callback=null)
		{
			if(isMoving) return false;
			if(playerController.IsDead()) return false;
			if(playerController.IsReloading() || playerController.IsShooting() || playerController.IsThrowingGranade()) return false;

			//turn off camera transition
			//if(CameraController.instance.IsCameraTransiting()) CameraController.instance.StopAllTransition();

			//turn of shake
			//ShakeCamera.instance.StopCameraShake();

			int toPoint = stations[currentStation].GetPrevMoveablePoint(currentPoint);

			//cancel move if in the same point
			if(currentPoint==toPoint) return false;

			if(finalTarget==null) 
			{
				_finalTargetInfo = LookAtManager.instance.GetLookAtPoint(path.waypoints[toPoint].position);
				_finalTarget = path.waypoints[toPoint].position + this.transform.forward*5f;
			}
			else
			{
				_finalTarget = finalTarget.position;
			}

			//reset camera field of view
            ImprovedCameraCntroller.instance.ResetFieldOfView();

			GenerateCoverMovePath(currentPoint,toPoint);
			StartMove(false,callback);
			currentPoint = toPoint;
			isMovingBetweenStations = false;
			isAutoMoving = true;
			return true;
		}

		public bool HasStationCoverPoints()
		{
			return stations[currentStation].HasCoverPoint();
		}

		public bool CanGoRight(Vector3 forward)
		{
			int nxtPoint = stations[currentStation].GetNextMoveablePoint(currentPoint);
			int prvPoint = stations[currentStation].GetPrevMoveablePoint(currentPoint);

			Vector3 toNextPoint = (path.waypoints[nxtPoint].position - path.waypoints[currentPoint].position).normalized;
			Vector3 toPrevPoint = (path.waypoints[prvPoint].position - path.waypoints[currentPoint].position).normalized;

			Vector3 up = Vector3.up;
			Vector3 right = -Vector3.Cross(forward.normalized,up.normalized);

			float angleWithFwd;
			float angleWithRight;

			if(nxtPoint != currentPoint)
			{
				angleWithRight = Vector3.Angle(right,toPrevPoint);

				if(angleWithRight<91f) return true;
			}

			if(prvPoint != currentPoint)
			{
				angleWithRight = Vector3.Angle(right,toNextPoint);

				if(angleWithRight<90f) return true;
			}

			return false;
		}

		public bool CanGoLeft(Vector3 forward)
		{
			int nxtPoint = stations[currentStation].GetNextMoveablePoint(currentPoint);
			int prvPoint = stations[currentStation].GetPrevMoveablePoint(currentPoint);

			Vector3 toNextPoint = (path.waypoints[nxtPoint].position - path.waypoints[currentPoint].position).normalized;
			Vector3 toPrevPoint = (path.waypoints[prvPoint].position - path.waypoints[currentPoint].position).normalized;

			Vector3 up = Vector3.up;
			Vector3 right = -Vector3.Cross(forward.normalized,up.normalized);

			float angleWithFwd;
			float angleWithRight;

			if(nxtPoint != currentPoint)
			{
				angleWithRight = Vector3.Angle(right,toPrevPoint);

				if(angleWithRight>90f) return true;
			}

			if(prvPoint != currentPoint)
			{
				angleWithRight = Vector3.Angle(right,toNextPoint);

				if(angleWithRight>90f) return true;
			}

			return false;
		}

		IEnumerator IE_UseSpeedFallOff()
		{
			do
			{
				float distance = (movingPath.waypoints[movingPath.waypoints.Length-1].position-this.transform.position).sqrMagnitude;
				if(distance>MIN_DISTANCE_FROM_POINT)
					spMove.ChangeSpeed(Mathf.Clamp(spMove.speed-moveSpeedDownRatePerSec*Time.deltaTime,0,maxAllowedSpeed));
				else
					spMove.ChangeSpeed(Mathf.Clamp(spMove.speed,defaultMoveSpeed,maxAllowedSpeed));

				yield return null;
			}while(isMoving == true);
		}

		public void AttachAI(PlayerAI _ai)
		{
			ai = _ai;
		}

		//[SerializeField]
		private float speedGain = 1.3f;
		public void SpeedUp()
		{
			spMove.ChangeSpeed(Mathf.Clamp(spMove.speed+speedGain,0,maxAllowedSpeed));
		}
			
		public bool IsMoving()
		{
			return isMoving;
		}

		public bool IsAutoMoving()
		{
			if (!isMoving) return false;
			else return isAutoMoving;
		}

		public void StopMoveByForce()
		{
			isMoving = false;
			isMovingBetweenStations = false;
			isAutoMoving = false;

            if(spMove!=null)
			    spMove.Stop();

		}

        private void StartMove(bool useCameraFollowMode = true, Action<bool> follow_callback=null)
		{
			isMoving = true;
			float defWait = 0;

			if(useCameraFollowMode)
			{
				if(ai!=null)
				{
                    if (!ai.IsAIOn())
                    {
                        defWait = ImprovedCameraCntroller.instance.GetFollowCameraInitTime();
                        ImprovedCameraCntroller.instance.RequestFollowCameraMove(MovementPriority.Normal, follow_callback, IsMoving, this.transform);
                    }
				}
				else
				{
                    defWait = ImprovedCameraCntroller.instance.GetFollowCameraInitTime();
                    ImprovedCameraCntroller.instance.RequestFollowCameraMove(MovementPriority.Normal, follow_callback, IsMoving, this.transform);
				}
			}
			else
			{
                ImprovedCameraCntroller.instance.RequestDragMove(MovementPriority.Normal, null, IsMoving, playerController.transform);
				defWait = 0;
			}
			
			StartCoroutine(TurnGradually(movingPath.waypoints[1].position,()=>{
				spMove.SetPath(movingPath);
				spMove.moveCompleted.AddListener(MoveComplt);
				spMove.ChangeSpeed(defaultMoveSpeed);
				spMove.StartMove();
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

		private GameObject tempTransform;

		private void MoveComplt()
		{
			spMove.moveCompleted.RemoveAllListeners();
			spMove.ChangeSpeed(defaultMoveSpeed);

			if(isMovingBetweenStations)
			{
				if(ai==null)
					CinematicCoverUp.instance.FlagUp();
			}

			if(isMovingBetweenStations)
			{
				if(tempTransform==null) tempTransform = new GameObject("Look At Fixer");
				tempTransform.transform.position = _finalTarget;

				tempTransform.transform.RotateAround(playerController.transform.position,Vector3.up,playerController.GetAngleWithAimCamera());
				_finalTarget = tempTransform.transform.position;

				playerController.ResetPlayerBaseCameraRot();
			}

			StartCoroutine(TurnGradually(_finalTarget, WorksAfterMoveComplete));

		}

		private void WorksAfterMoveComplete()
		{
			if(ai!=null)
			{
				if(!ai.IsAIOn())
				{
//					if(isMovingBetweenStations)
//						CameraController.instance.StopFollowing();
//					else
//						CameraController.instance.StopCameraDrag();
				}
			}
			else
			{
//				if(isMovingBetweenStations)
//					CameraController.instance.StopFollowing();
//				else
//					CameraController.instance.StopCameraDrag();
			}

			playerController.UpdateSavedDirections(_finalTargetInfo.vr, _finalTargetInfo.point);

			UpdateLeftRightCoverPointAvailability();

			if(OnStationReached!=null) OnStationReached();

			isMoving = false;
			isAutoMoving = false;
		}

		private void GenerateCoverMovePath(int fromPoint, int toPoint)
		{
			int totalBlock = Mathf.Abs(toPoint-fromPoint);
			bool isReverse = toPoint>fromPoint? false:true;
			totalBlock++;
			movingPath.waypoints = new Transform[totalBlock];

			if(isReverse)
			{
				for(int i=fromPoint,j=0;i>=toPoint;i--,j++)
				{
					movingPath.waypoints[j] = path.waypoints[i];
				}
			}
			else
			{
				for(int i=fromPoint,j=0;i<=toPoint;i++,j++)
				{
					movingPath.waypoints[j] = path.waypoints[i];
				}
			}
		}

		private void UpdateLeftRightCoverPointAvailability()
		{
			if(playerController !=null)
			{
				leftWayOpen = CanGoLeft(playerController.GetBaseForward());
				rightWayOpen = CanGoRight(playerController.GetBaseForward());
			}
			else
			{
				leftWayOpen = false;
				rightWayOpen = false;
			}
		}

		private void GenerateStationMovePath(int fromPoint)
		{
			List<Transform> points = new List<Transform>();

			points.Add(path.waypoints[fromPoint]);
			bool waypointFound = false;

			for(int i=fromPoint+1;i<path.waypoints.Length;i++)
			{
				if(path.waypoints[i].CompareTag(wayOutTag))
				{
					if(!waypointFound)
					{
						points.Add(path.waypoints[i]);
						waypointFound = true;
					}
				}
				else if (path.waypoints[i].CompareTag(coverPointTag))
				{
					continue;
					//if(!waypointFound)
						//points.Add(path.waypoints[i]);
				}
				else if (path.waypoints[i].CompareTag(stationOutTag))
				{
					//last stop to station
					points.Add(path.waypoints[i]);
				}
				else if (path.waypoints[i].CompareTag(stationTag))
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

	[System.Serializable]
	public class Station
	{
		private int _stationIndex;
		private int[] _moveablePointIndices;

		public Station(int stationIndex, int[] coverPoints)
		{
			_stationIndex = stationIndex;
			_moveablePointIndices = coverPoints;
		}

		public int GetStationWaypointIndex()
		{
			return _stationIndex;
		}

		public bool HasCoverPoint()
		{
			return _moveablePointIndices.Length>1?true:false;
		}
			
		public int GetNextMoveablePoint(int nowPoint)
		{
			int index = Array.IndexOf(_moveablePointIndices,nowPoint);
			index++;
			if(index>_moveablePointIndices.Length-1) index =0;

			return _moveablePointIndices[index];
		}

		public int GetPrevMoveablePoint(int nowPoint)
		{
			int index = Array.IndexOf(_moveablePointIndices,nowPoint);
			index--;
			if(index<0) index =_moveablePointIndices.Length-1;

			return _moveablePointIndices[index];
		}
	}
}
