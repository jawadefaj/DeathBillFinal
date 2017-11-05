using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraController1 : MonoBehaviour {

	public static CameraController1 instance;

	//public Sprite crossHair;
	//public Sprite sinppingCrossHair;
	public Image crossHairHolder;
	public float maxLarge = 2f;
	public float snippingFactor = 1.5f;
	public float zoomCameraValue = 10f;
	public Transform followCameraHolder;
	public Transform followCameraRoot;

	private Camera thisCamera;

	private Vector3 baseScale;
    private bool isTransiting = false;
	private float baseZoomCameraValue;
	
	void Awake()
	{
		instance = this;
		thisCamera = this.GetComponent<Camera>();
	}

	void Start()
	{
		
		baseScale = crossHairHolder.transform.localScale;
	}

    public bool IsCameraTransiting()
    {
		return (isTransiting || isFollowing || isCameraDragging);
    }
		
	public Vector3 GetBaseScale()
	{
		return crossHairHolder.transform.localScale;
	}

	public float GetMaxRecoilFactor()
	{
		return maxLarge;
	}

	public void SetCrossHairScale(Vector3 scale)
	{
		crossHairHolder.transform.localScale = scale;
	}

	public void ResetFieldOfView()
	{
		thisCamera.fieldOfView = 60f;
		isCameraZoomedIn = false;
	}

	public void ResetCrossHairScale()
	{
		crossHairHolder.transform.localScale = baseScale;
	}


	private bool isCameraZoomedIn = false;
	private float zoomTime = 1f;

	public void ZoomCamera(bool zoomIn, float allowTime)
	{
		StopCoroutine("Zoom");
		isCameraZoomedIn = zoomIn;
		zoomTime = allowTime;
		StartCoroutine("Zoom");
	}

	private IEnumerator Zoom()
	{
		float nowValue = thisCamera.fieldOfView;

		if(isCameraZoomedIn)
		{
			float timer = 0.0f; 

			while (timer <= zoomTime) {
				float t = 1.0f + Mathf.Pow((timer / zoomTime - 1.0f), 3.0f);
				thisCamera.fieldOfView = Mathf.Lerp(nowValue,zoomCameraValue,t);
				timer += Time.deltaTime;
				yield return null;
			}

			thisCamera.fieldOfView = zoomCameraValue;
		}
		else
		{
			float timer = 0.0f; 
			
			while (timer <= zoomTime) {
				float t = 1.0f + Mathf.Pow((timer / zoomTime - 1.0f), 3.0f);
				thisCamera.fieldOfView = Mathf.Lerp(nowValue,60,t);
				timer += Time.deltaTime;
				yield return null;
			}
			
			thisCamera.fieldOfView = 60f;
		}
	}

	private IEnumerator moveTo;
	public void TransitCamera(Transform from_camera, Transform to_camera, float _time, float sLerpOffset = 15f, Action callback=null)
	{
		//Debug.Log("Transit camera called");
		if(moveTo!=null)
			StopCoroutine(moveTo);
		
        isTransiting = true;
		from = from_camera;
		to = to_camera;
		time = _time;
		upOffSet = sLerpOffset;
		moveTo = MoveTo(callback);

		StartCoroutine(moveTo);
	}

	public void StopAllTransition()
	{
		if(moveTo!=null)
			StopCoroutine(moveTo);
		
        isTransiting = false;
		isCameraDragging = false;
		isFollowing = false;
	}
	
	private Transform from;
	private Transform to;
	private float time;
	private float upOffSet;

	IEnumerator MoveTo(Action callback) {

		float timer = 0.0f;  

		Vector3 fromPos = from.position;
		Quaternion fromRot = from.rotation;

		//calculate center for slep
		Vector3 center = (fromPos+to.position)*0.5f;
		center -= new Vector3(0,upOffSet,0);

		//edit center value to a perpendicular direction
		Vector3 a = to.position-fromPos;
		Vector3 b = center - fromPos;
		Vector3 bi_norm = Vector3.Cross(a,b);
		Vector3 norm = Vector3.Cross(a,bi_norm);
		norm.Normalize();
		center = (fromPos+to.position)*0.5f;
		center -= norm*upOffSet;

		//calculate slerp vector
		Vector3 riseRelCenter = fromPos-center;
		Vector3 setRelCenter = to.position-center;


		while (timer <= time) {
			float t = 1.0f + Mathf.Pow((timer / time - 1.0f), 3.0f);

			from.transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, t);
			from.transform.position += center;
			from.transform.rotation = Quaternion.Slerp(fromRot,to.rotation,t);
			timer += Time.deltaTime;

			yield return null;
		}
		from.transform.position = to.position;
		from.transform.rotation = to.rotation;
        

		if(callback!=null) callback();
		isTransiting = false;
	}

	//====================================Camera Follower===============================

	private bool isFollowing = false;
	public bool isCameraDragging = false;
	private Transform dragTraget;
	private Vector3 dragTargetLastPos;
	private Vector3 dragCameraTargetPos;
	private Transform m_Target;
	private Rigidbody targetRigidbody;
	private float transitTime = 0.5f;

	void Update()
	{
		if(isFollowing)
		{
			FollowTarget(Time.deltaTime);
		}
		else if (isCameraDragging)
		{
			DragCamera();
		}
	}

	private void DragCamera()
	{
		Vector3 nowTargetPos = dragTraget.position;
		Vector3 deltaPos = nowTargetPos-dragTargetLastPos;

		if(deltaPos != Vector3.zero)
		{
			dragCameraTargetPos+= deltaPos;
		}

		//lerp camera to its target pos
		Vector3 toPos = Vector3.Lerp(thisCamera.transform.position,dragCameraTargetPos,0.5f);
		thisCamera.transform.position = toPos;

		//update last pos
		dragTargetLastPos = nowTargetPos;
	}

	public void StartCameraDrag()
	{
		dragTraget = thisCamera.transform.parent;
		dragTargetLastPos = dragTraget.position;
		dragCameraTargetPos = thisCamera.transform.position;

		thisCamera.transform.SetParent(null);
		isCameraDragging = true;

	}

	public void StopCameraDrag()
	{
		if(!isCameraDragging) return;

		isCameraDragging = false;
		thisCamera.transform.parent = PlayerInputController.instance.current_player.transform;
	}

	public float StartFollowing()
	{
		if(isFollowing) return 0;

		m_Target = this.transform.parent;
		targetRigidbody = m_Target.GetComponent<Rigidbody>();

		followCameraRoot.transform.position = m_Target.position;
		followCameraRoot.transform.rotation = m_Target.rotation;

		thisCamera.transform.SetParent(followCameraHolder);
		//smoothly transit to that point
		TransitCamera(this.transform,followCameraHolder,transitTime,0,()=>{isFollowing = true;});

		return transitTime;
	}

	public void StopFollowing()
	{
		if(!isFollowing) return;

		isFollowing = false;
		thisCamera.transform.parent = PlayerInputController.instance.current_player.transform;
		PlayerInputController.instance.current_player.SwitchToHeadCameraView();
	}


	[SerializeField] private float m_MoveSpeed = 3; // How fast the rig will move to keep up with target's position
	[SerializeField] private float m_TurnSpeed = 1; // How fast the rig will turn to keep up with target's rotation
	[SerializeField] private float m_RollSpeed = 0.2f;// How fast the rig will roll (around Z axis) to match target's roll.
	[SerializeField] private bool m_FollowVelocity = false;// Whether the rig will rotate in the direction of the target's velocity.
	[SerializeField] private bool m_FollowTilt = true; // Whether the rig will tilt (around X axis) with the target.
	[SerializeField] private float m_SpinTurnLimit = 90;// The threshold beyond which the camera stops following the target's rotation. (used in situations where a car spins out, for example)
	[SerializeField] private float m_TargetVelocityLowerLimit = 4f;// the minimum velocity above which the camera turns towards the object's velocity. Below this we use the object's forward direction.
	[SerializeField] private float m_SmoothTurnTime = 0.2f; // the smoothing for the camera's rotation

	private float m_LastFlatAngle; // The relative angle of the target and the rig from the previous frame.
	private float m_CurrentTurnAmount; // How much to turn the camera
	private float m_TurnSpeedVelocityChange; // The change in the turn speed velocity
	private Vector3 m_RollUp = Vector3.up;// The roll of the camera around the z axis ( generally this will always just be up )

	private void FollowTarget(float deltaTime)
	{
		// if no target, or no time passed then we quit early, as there is nothing to do
		if (!(deltaTime > 0) || m_Target == null)
		{
			return;
		}

		// initialise some vars, we'll be modifying these in a moment
		var targetForward = m_Target.forward;
		var targetUp = m_Target.up;

		if (m_FollowVelocity && Application.isPlaying)
		{
			// in follow velocity mode, the camera's rotation is aligned towards the object's velocity direction
			// but only if the object is traveling faster than a given threshold.

			if (targetRigidbody.velocity.magnitude > m_TargetVelocityLowerLimit)
			{
				// velocity is high enough, so we'll use the target's velocty
				targetForward = targetRigidbody.velocity.normalized;
				targetUp = Vector3.up;
			}
			else
			{
				targetUp = Vector3.up;
			}
			m_CurrentTurnAmount = Mathf.SmoothDamp(m_CurrentTurnAmount, 1, ref m_TurnSpeedVelocityChange, m_SmoothTurnTime);
		}
		else
		{
			// we're in 'follow rotation' mode, where the camera rig's rotation follows the object's rotation.

			// This section allows the camera to stop following the target's rotation when the target is spinning too fast.
			// eg when a car has been knocked into a spin. The camera will resume following the rotation
			// of the target when the target's angular velocity slows below the threshold.
			var currentFlatAngle = Mathf.Atan2(targetForward.x, targetForward.z)*Mathf.Rad2Deg;
			if (m_SpinTurnLimit > 0)
			{
				var targetSpinSpeed = Mathf.Abs(Mathf.DeltaAngle(m_LastFlatAngle, currentFlatAngle))/deltaTime;
				var desiredTurnAmount = Mathf.InverseLerp(m_SpinTurnLimit, m_SpinTurnLimit*0.75f, targetSpinSpeed);
				var turnReactSpeed = (m_CurrentTurnAmount > desiredTurnAmount ? .1f : 1f);
				if (Application.isPlaying)
				{
					m_CurrentTurnAmount = Mathf.SmoothDamp(m_CurrentTurnAmount, desiredTurnAmount,
						ref m_TurnSpeedVelocityChange, turnReactSpeed);
				}
				else
				{
					// for editor mode, smoothdamp won't work because it uses deltaTime internally
					m_CurrentTurnAmount = desiredTurnAmount;
				}
			}
			else
			{
				m_CurrentTurnAmount = 1;
			}
			m_LastFlatAngle = currentFlatAngle;
		}

		// camera position moves towards target position:
		followCameraRoot.transform.position = Vector3.Lerp(followCameraRoot.transform.position, m_Target.position, deltaTime*m_MoveSpeed);

		// camera's rotation is split into two parts, which can have independend speed settings:
		// rotating towards the target's forward direction (which encompasses its 'yaw' and 'pitch')
		if (!m_FollowTilt)
		{
			targetForward.y = 0;
			if (targetForward.sqrMagnitude < float.Epsilon)
			{
				targetForward = followCameraRoot.transform.forward;
			}
		}
		var rollRotation = Quaternion.LookRotation(targetForward, m_RollUp);

		// and aligning with the target object's up direction (i.e. its 'roll')
		m_RollUp = m_RollSpeed > 0 ? Vector3.Slerp(m_RollUp, targetUp, m_RollSpeed*deltaTime) : Vector3.up;
		followCameraRoot.transform.rotation = Quaternion.Lerp(followCameraRoot.transform.rotation, rollRotation, m_TurnSpeed*m_CurrentTurnAmount*deltaTime);
	}

}
