using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public enum PrimaryMovementType
{
	Null=-1,
	LerpMove=0,
	SlerpMove=1,
	FollowMove=3,
}

public enum MovementPriority
{
	VeryLow =0,
	Low =1,
	Normal =2,
	High =3,
	VeryHigh =4,
}
	
[RequireComponent(typeof(Camera))]
public class ImprovedCameraCntroller : MonoBehaviour {

	public static ImprovedCameraCntroller instance;

    private Image _cursorImg;
    public Image cursor{
        get{ 
            if (_cursorImg == null)
            {
                _cursorImg = HUDManager.instance.shootGroup.crossHair.GetComponent<Image>();
            }
            return _cursorImg;
        }
    }

    private Vector3 cursorBaseScale;
    private float cursorScaleMultiplier = 2.0f;

	private Camera thisCamera;
	private GameObject thisCameraObject;
	private GameObject effectorLayer;
	private GameObject primaryMovementLayer;
    private GameObject secondaryMovementLayer;

	private Transform finalTarget;
	private Action<bool> moveCallback;

	private PrimaryMoveData currentPrimaryMove = new PrimaryMoveData(PrimaryMovementType.Null,MovementPriority.VeryLow);
	private bool isPrimaryMoveOn = false;
	private IEnumerator primaryMoveCoroutine;

    private Func<bool> canDoSecondaryMove = null;
    private Transform secondaryMoveTarget = null;

	//Mono Functions
	void Awake()
	{
		thisCamera = this.GetComponent<Camera>();

		if (thisCamera == null)
		{
			Debug.LogError("This script needs to be attached with a Camera object.");
			Destroy(this);
			return;
		}

		effectorLayer = new GameObject("Camera Effect Layer");
        primaryMovementLayer = new GameObject("Primary Movement Layer");
        secondaryMovementLayer = new GameObject("Secondary Movement Layer");
		finalTarget = new GameObject("Camera Target").transform;
		thisCameraObject = this.gameObject;

		effectorLayer.transform.position = thisCameraObject.transform.position;
		effectorLayer.transform.rotation = thisCameraObject.transform.rotation;
		primaryMovementLayer.transform.position = thisCameraObject.transform.position;
		primaryMovementLayer.transform.rotation = thisCameraObject.transform.rotation;
        secondaryMovementLayer.transform.position = thisCameraObject.transform.position;
        secondaryMovementLayer.transform.rotation = thisCameraObject.transform.rotation;

		thisCameraObject.transform.SetParent(null);
		thisCameraObject.transform.SetParent(effectorLayer.transform);
        effectorLayer.transform.SetParent(secondaryMovementLayer.transform);
        secondaryMovementLayer.transform.SetParent(primaryMovementLayer.transform);

        cursorBaseScale = cursor.transform.localScale;

		instance = this;
	}

    void Update()
    {
        if (!isPrimaryMoveOn)
        {
            if (canDoSecondaryMove != null && secondaryMoveTarget != null)
            {
                if (canDoSecondaryMove())
                {
                    primaryMovementLayer.transform.position = Vector3.Lerp(primaryMovementLayer.transform.position, secondaryMoveTarget.position, 0.5f);
                    secondaryMovementLayer.transform.rotation = Quaternion.Slerp(secondaryMovementLayer.transform.rotation, secondaryMoveTarget.rotation, 0.5f);
                }
            }
        }
    }

	//Public Methods
    public bool RequestDragMove(MovementPriority priority, Action<bool> callback, Func<bool> isTargetMoving, Transform target)
    {
        return RequestPrimaryMovement(PrimaryMovementType.LerpMove, priority, Vector3.zero, Quaternion.identity, callback, 0, 1, isTargetMoving, target);
    }

    public bool RequestCameraTransitMove(MovementPriority priority, Action<bool> callback, Vector3 finalPos, Quaternion finalRot, float timeLimit, float SlerpOffset=15f)
    {
        return RequestPrimaryMovement(PrimaryMovementType.SlerpMove, priority, finalPos, finalRot, callback, timeLimit, SlerpOffset, null,null);
    }

    public bool RequestFollowCameraMove(MovementPriority priority, Action<bool> callback, Func<bool> isTargetMoving, Transform target)
    {
        return RequestPrimaryMovement(PrimaryMovementType.FollowMove, priority, Vector3.zero, Quaternion.identity, callback, 0, 1, isTargetMoving, target);
    }

    public void RequestCameraAimMove(Func<bool> canMove, Transform target)
    {
        ReIntSecondaryMovement(canMove, target);
    }

    public void RecoilCursorEffect()
    {
        StopCoroutine("IECursorScaleDown");
        cursor.transform.localScale = cursorBaseScale * cursorScaleMultiplier;
        StartCoroutine("IECursorScaleDown");
    }

    public void SetZoomCameraValue(float value)
    {
        zoomCameraValue = value;
    }

    public void ShakeTheCam(float magnitude, float timeSpan)
    {
        StartCoroutine(IEShakeCam(magnitude,timeSpan));
    }

    public void GunThrustEffect(float effectAngle, float upTime, float downTime)
    {
        StartCoroutine(IEGunThrustEffect(effectAngle, upTime, downTime));
    }

    public void ResetFieldOfView()
    {
        thisCamera.fieldOfView = 60f;
        isCameraZoomedIn = false;
    }

    public float GetFollowCameraInitTime()
    {
        return followInitTime;
    }

    public bool IsPrimaryMovementOn()
    {
        //returns if any primary movement like following, draging or slerp movement is going on or not
        return isPrimaryMoveOn;
    }

    public bool IsShaking()
    {
        return isShaking;
    }


    //Private Methods
    #region Primary Movement
    private bool RequestPrimaryMovement(PrimaryMovementType type, MovementPriority priority, Vector3 finalDestination, Quaternion finalRotation, Action<bool> callback, float timeLimit, float slerpOffset, Func<bool> isFollowTargetMoving, Transform followTarget)
	{
       
		if (isPrimaryMoveOn) 
		{
			//we could accept this movement or queue or reject this movement
			int c = (int)currentPrimaryMove.priority;
			int r = (int)priority;

			if (r < c)
			{
				//reject this move
				return false;
			}
			else if (r >= c)
			{
				if (primaryMoveCoroutine != null)
				{
					StopCoroutine(primaryMoveCoroutine);
					OnPrimaryMovementComplete(false);
				}
				else
				{
					Debug.LogError("Condition says primary movement is on but no coroutine of primary movement found");
				}

                DoPrimaryMovement(type, priority, finalDestination, finalRotation, callback, timeLimit, slerpOffset,isFollowTargetMoving,followTarget);

				return true;
			}
                
		}

		//Do our movement work
        DoPrimaryMovement(type, priority, finalDestination, finalRotation, callback, timeLimit, slerpOffset,isFollowTargetMoving,followTarget);
		return true;
	}
        
    private void DoPrimaryMovement(PrimaryMovementType type, MovementPriority priority, Vector3 finalDestination, Quaternion finalRotation, Action<bool> callback, float timeLimit, float slerpOffset, Func<bool> isFollowTargetMoving, Transform followTarget)
	{
		finalTarget.position = finalDestination;
		finalTarget.rotation = finalRotation;

		moveCallback = callback;

        currentPrimaryMove = new PrimaryMoveData(type, priority);
        isPrimaryMoveOn = true;

        if (type == PrimaryMovementType.SlerpMove)
        {
            primaryMoveCoroutine = DoSlerpMove(timeLimit, slerpOffset);
            StartCoroutine(primaryMoveCoroutine);
        }
        else if (type == PrimaryMovementType.LerpMove)
        {
            primaryMoveCoroutine = DoLerpMove(isFollowTargetMoving, followTarget);
            StartCoroutine(primaryMoveCoroutine);
        }
        else if (type == PrimaryMovementType.FollowMove)
        {
            primaryMoveCoroutine = DoFollowMove(isFollowTargetMoving, followTarget);
            StartCoroutine(primaryMoveCoroutine);
        }
	}

    float followInitTime = 0.5f;
    private IEnumerator DoFollowMove(Func<bool> isFollowTargetMoving, Transform followTarget)
    {
        m_Target = followTarget;
        targetRigidbody = m_Target.GetComponent<Rigidbody>();

        followCameraRoot.transform.position = m_Target.position;
        followCameraRoot.transform.rotation = m_Target.rotation;

        //smoothly transit to that point
        yield return DoSlerpMove(followInitTime,15f,followCameraHolder);

        primaryMovementLayer.transform.SetParent(followCameraHolder);

        do
        {
            FollowTarget(Time.deltaTime);
            yield return null;
        } while(isFollowTargetMoving());

        primaryMovementLayer.transform.SetParent(null);

        OnPrimaryMovementComplete();
    }

    private IEnumerator DoLerpMove(Func<bool> isFollowTargetMoving, Transform lerpTarget)
    {
        /// We can not do the lerp method in traditional way. Cause its kind of following mode
        /// So we need to get a target and follow it

        float posLerpRate = 0.5f;

        Transform dragTraget = lerpTarget;
        Vector3 dragTargetLastPos = dragTraget.position;
        Vector3 dragCameraTargetPos = primaryMovementLayer.transform.position;

        do
        {
            Vector3 nowTargetPos = dragTraget.position;
            Vector3 deltaPos = nowTargetPos-dragTargetLastPos;

            if(deltaPos != Vector3.zero)
            {
                dragCameraTargetPos+= deltaPos;
            }

            //lerp camera to its target pos
            Vector3 toPos = Vector3.Lerp(primaryMovementLayer.transform.position,dragCameraTargetPos,posLerpRate);
            primaryMovementLayer.transform.position = toPos;

            //update last pos
            dragTargetLastPos = nowTargetPos;

            yield return null;

        } while(isFollowTargetMoving());

        OnPrimaryMovementComplete();
    }

	private IEnumerator DoSlerpMove(float time, float upOffSet, Transform personalTarget = null)
	{
		float timer = 0.0f;  
        Transform targetTransform = personalTarget == null ? finalTarget : personalTarget;

		Vector3 fromPos = primaryMovementLayer.transform.position;
		Quaternion fromRot = secondaryMovementLayer.transform.rotation;


		//calculate center for slep
        Vector3 center = (fromPos+targetTransform.position)*0.5f;
		center -= new Vector3(0,upOffSet,0);

		//edit center value to a perpendicular direction
        Vector3 a = targetTransform.position-fromPos;
		Vector3 b = center - fromPos;
		Vector3 bi_norm = Vector3.Cross(a,b);
		Vector3 norm = Vector3.Cross(a,bi_norm);
		norm.Normalize();
        center = (fromPos+targetTransform.position)*0.5f;
		center -= norm*upOffSet;

		//calculate slerp vector
		Vector3 riseRelCenter = fromPos-center;
        Vector3 setRelCenter = targetTransform.position-center;


		while (timer <= time) {
			float t = 1.0f + Mathf.Pow((timer / time - 1.0f), 3.0f);

			primaryMovementLayer.transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, t);
			primaryMovementLayer.transform.position += center;
            secondaryMovementLayer.transform.rotation = Quaternion.Slerp(fromRot,targetTransform.rotation,t);
			timer += Time.deltaTime;

			yield return null;
		}
        primaryMovementLayer.transform.position = targetTransform.position;
        secondaryMovementLayer.transform.rotation = targetTransform.rotation;

        if (personalTarget == null)
		    OnPrimaryMovementComplete();
	}

	private void OnPrimaryMovementComplete(bool success = true)
	{
		if (isPrimaryMoveOn)
		{
			isPrimaryMoveOn = false;
            primaryMoveCoroutine = null;

			if (moveCallback != null)
				moveCallback(success);
		}
	}

    //Object Follow mechanism code
    //the following code is adopted from the Internet
    public Transform followCameraHolder;
    public Transform followCameraRoot;
    private Transform m_Target;
    private Rigidbody targetRigidbody;
    [SerializeField] private float m_MoveSpeed = 3; // How fast the rig will move to keep up with target's position
    [SerializeField] private float m_TurnSpeed = 10; // How fast the rig will turn to keep up with target's rotation
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

    #endregion

    #region Secondary Movement

    private void ReIntSecondaryMovement(Func<bool> canMoveCamera, Transform trackTarget)
    {
        canDoSecondaryMove = canMoveCamera;
        secondaryMoveTarget = trackTarget;
    }

    #endregion

    #region CAMERA EFFECTS

    private bool isCameraZoomedIn = false;
    private float zoomTime = 1f;
    private float zoomCameraValue = 40f;

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

    private bool isThrustEffectOn = false;
    private IEnumerator IEGunThrustEffect(float effectAngle, float upTime, float downTime)
    {
        if (isThrustEffectOn)
            yield break;

        Quaternion initRot = effectorLayer.transform.localRotation;    //Should be a Zero Vector
        float elapsedTime = 0;
        Vector3 temp;
        Vector3 baseRot;
        isThrustEffectOn = true;

        float upAngularSpeed = effectAngle / upTime;
        float downAngulaSpped = effectAngle / downTime;

        //camera goes up
        do
        {
            temp = Vector3.left*upAngularSpeed*Time.deltaTime;
            baseRot = effectorLayer.transform.localRotation.eulerAngles;
            baseRot += temp;
            effectorLayer.transform.localRotation = Quaternion.Euler(baseRot);

            elapsedTime+=Time.deltaTime;
            yield return null;
        }while(elapsedTime<upTime);

        //camera goes down
        elapsedTime =0;
        do
        {
            temp = Vector3.left*downAngulaSpped*Time.deltaTime;
            baseRot = effectorLayer.transform.localRotation.eulerAngles;
            baseRot -= temp;
            effectorLayer.transform.localRotation = Quaternion.Euler(baseRot);

            elapsedTime+=Time.deltaTime;
            yield return null;
        }while(elapsedTime<downTime);

        effectorLayer.transform.localRotation = initRot;

        isThrustEffectOn = false;
    }

    private bool isShaking = false;
    private IEnumerator IEShakeCam(float magnitude, float duration)
    {
        //no two shake in one time
        if(isShaking) yield break;
        if (isThrustEffectOn) yield break;

        Quaternion initRot = effectorLayer.transform.localRotation;    //Should be a Zero Vector

        float mag;
        float elapsedTime = 0;
        Vector3 temp;
        Vector3 baseRot;
        isShaking = true;

        do
        {
            mag = Mathf.Lerp(magnitude, 0, elapsedTime/ duration);
            temp = new Vector3(UnityEngine.Random.Range(-0.1f * mag, 0.1f * mag), UnityEngine.Random.Range(-0.1f * mag, 0.1f * mag), UnityEngine.Random.Range(-0.1f * mag, 0.1f * mag));
            baseRot = effectorLayer.transform.localRotation.eulerAngles;
            baseRot += temp;
            effectorLayer.transform.localRotation = Quaternion.Euler(baseRot);

            elapsedTime+=Time.deltaTime;
            yield return null;
        }while(elapsedTime<duration);


        //done shaking. get back to our real poisiton
        float timer = 0.0f;  
        float timeToGetBack = 0.8f;

        Quaternion fromRot = effectorLayer.transform.localRotation;

        while (timer <= timeToGetBack) {
            float t = 1.0f + Mathf.Pow((timer / timeToGetBack - 1.0f), 3.0f);
            effectorLayer.transform.localRotation = Quaternion.Slerp(fromRot,initRot,t);
            timer += Time.deltaTime;

            yield return null;
        }

        effectorLayer.transform.localRotation = initRot;
        isShaking = false;
    }

    private IEnumerator IECursorScaleDown()
    {
        //if(Camera.main == null) yield break;

        float time = 1.5f;
        float timer = 0.0f;  

        Vector3 fromScale = cursor.transform.localScale;
        Vector3 toScale = cursorBaseScale;
        Vector3 resultScale = Vector3.zero;

        //add some halt
        yield return new WaitForSeconds(0.0f);

        while (timer <= time) {
            float t = 1.0f + Mathf.Pow((timer / time - 1.0f), 3.0f);
            resultScale = Vector3.Lerp(fromScale,toScale,t);
            cursor.transform.localScale = resultScale;
            timer += Time.deltaTime;
            yield return null;
        }
        cursor.transform.localScale = cursorBaseScale;
    }

    #endregion

    #region Static Methods
    public static float GetCameraDragFactor()
    {
        float fov1 = 60;
        float fov2 = 10;

        float f1 = 1;
        float f2 = (0.16666f);

        float rate = (fov1-fov2)/(f1-f2);

        float fov3 = ImprovedCameraCntroller.instance.thisCamera.fieldOfView;
        float diff_fov = fov1 - fov3;
        float diff_f = diff_fov / rate;

        return f1 - diff_f;
    }
    #endregion
	//struct for data storage
	private struct PrimaryMoveData
	{
		public PrimaryMovementType type;
		public MovementPriority priority;

		public PrimaryMoveData(PrimaryMovementType type, MovementPriority priority)
		{
			this.type = type;
			this.priority = priority;
		}
	}
        
}


