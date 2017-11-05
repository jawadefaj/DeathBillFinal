using UnityEngine;
using System.Collections;

public class MovementControler : MonoBehaviour {

	public Transform enemyTarget;
	public LayerMask rayCastingLayer;

	public float walkSpeed = 3f;   //min valu 3 and max value is 5
	public float speedSmoothing =10.0f;


	internal float x_input = 0.0f;
	internal float y_input = 0.0f;
	internal float vertical_input = 0.0f;
//	internal float velocity = 0.0f;

	private float moveSpeed = 0.0f;
	private float rotateSpeed = 500.0f;

//	private Vector3 lastPosition;
	private Vector3 moveDirection;
	//private Vector3 targetDirection;
	private Vector3 originPos = Vector3.zero;
	private Vector3 prevPos = Vector3.zero;

	private Animator animator;
	private SneakyPlayer sp;

	//Action List
	private bool isMoving = false;
	private bool isManualWalk = false;
	private bool isRunningTowardsPreviousStation = false;

	//events
	public delegate void CheckPointReached();
	public event CheckPointReached OnPointReached;
	public event CheckPointReached OnPrevPointReached;

	//for new logic variables
	private float tapPerSecond = 0f;

//	public void ResetMoveDirection()
//	{
//		moveDirection = this.transform.forward;
//	}

	void Awake () {
		moveDirection = this.transform.forward;
		animator = this.GetComponent<Animator>();
		sp = this.GetComponent<SneakyPlayer>();
	}


	// Update is called once per frame
	void Update () {

		//ModifiedUpdate();
		//return;

		animator.SetBool("IsMoving",isMoving);
		if(!isMoving) 
		{
			if(sp.GetSneakyStationController().IsMoving() == false)
				animator.SetFloat("Speed",0f);
			return;
		}

		//x_input and y_input is set from outside which denotes where we need to go
		SetFakeMoveInput();
		//Debug.Log(string.Format("x: {0}, y: {1}",x_input,y_input));

		//calculate player velocity
//		velocity = Vector3.Distance (this.transform.position,lastPosition)/Time.deltaTime;
//		lastPosition = this.transform.position;

		//RISKY CODE
		if(walkSpeed==0) 
		{
			moveSpeed = 0;
			if(sp.GetSneakyStationController().IsMoving() == false)
				animator.SetFloat("Speed",0f);
			return;
		}

//		if(velocity<1)
//		{
//			rotateSpeed = 500f;
//		}
//		else
//			rotateSpeed = 500f;



		Vector3 targetDirection;

		targetDirection = new Vector3(x_input,vertical_input,y_input);

		if(targetDirection!=Vector3.zero)
		{
			// If we are really slow, just snap to the target direction
			if (moveSpeed < walkSpeed * 0.9 )
			{
				if(Vector3.Angle (moveDirection,targetDirection)<5f)
					moveDirection = targetDirection;		//*****
				else
				{
					moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 0);		
					//moveDirection = moveDirection.normalized;		//*******
				}

			}
			// Otherwise smoothly turn towards it
			else
			{
				//moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
				moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 0);

				//moveDirection = moveDirection.normalized;		//*****
			}
		}
		else
		{
			//if no movement input, then we will rotate and look at our enemy

			/*if(animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(Animator.StringToHash("SlowWalk")))
			{

			}
			else*/
			{
				Vector3 lookDir = (enemyTarget.position-this.transform.position).normalized;
				lookDir.y =0;
				moveDirection = Vector3.RotateTowards(moveDirection, lookDir, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 0);
			}


		}

		// Smooth the speed based on the current target direction
		//float curSmooth = speedSmoothing * Time.deltaTime;
		
		// Choose target speed
		//* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
		//targetDirection.Normalize ();	//****
		float targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);

		// Pick speed modifier Here

		targetSpeed *= walkSpeed;

		//End of speed midifier

		moveSpeed = targetSpeed;



		float modifiedSpeedValue = moveSpeed/maxAllowedSpeed;

		if(modifiedSpeedValue<0.1f)
		{
			moveSpeed =0;
			modifiedSpeedValue = 0;
		}

		animator.SetFloat ("Speed",modifiedSpeedValue);	//Could Use MoveSpeed

		// Calculate actual motion
		Vector3 movement = moveDirection * moveSpeed *Time.deltaTime ;

		if(targetDirection.magnitude <0.8f && targetDirection !=Vector3.zero) 
		{
			movement = targetDirection*moveSpeed *Time.deltaTime ;
			if(Mathf.Abs(movement.magnitude - targetDirection.magnitude) < 0.1f)
				movement = targetDirection;

			//animator.SetFloat ("Speed",(movement.magnitude/maxAllowedSpeed));

			//moveDirection = targetDirection;
		}


		//rotate our transform to the move direction
		float angle = Vector3.Angle(this.transform.forward,movement);
		this.transform.Rotate(Vector3.up,angle);


		//apply movement
		transform.Translate(movement,Space.World);		//*****



		if(!sp.isStabing)
		{
			moveDirection.y =0;
			transform.rotation = Quaternion.LookRotation(moveDirection);
		}

		//reset values
		x_input =0;
		y_input =0;
		vertical_input = 0;

	}

	private bool canUseSpeedFallOff = true;

	public void CanRelease()
	{
		canUseSpeedFallOff = true;
	}

	public void CanNotRelease()
	{
		canUseSpeedFallOff = false;
	}
		
	private float tapCheckInterval = .3f;
	private int tapCount =0;
	private float nextTapCheck =.3f;
    /*
	void ModifiedUpdate()
	{
		animator.SetBool("IsMoving",isMoving);
		if(!isMoving) return;

		//x_input and y_input is set from outside which denotes where we need to go
		SetFakeMoveInput();

		float m_speed=0;

		//calculate tap per second
		if(nextTapCheck<0)
		{
			tapPerSecond = tapCount/tapCheckInterval;
			nextTapCheck = tapCheckInterval;
			tapCount =0;
		}
		nextTapCheck -= Time.deltaTime;

		//conostant speed midifer
		if(tapPerSecond<0.4f)
		{
			m_speed = 0;
		}
		else if (tapPerSecond<0.7f)
		{
			m_speed = 1;
		}
		else if (tapPerSecond >0.7f)
		{
			m_speed = 2;
		}


		Vector3 targetDirection;
		targetDirection = new Vector3(x_input,vertical_input,y_input);

		Vector3 movement = targetDirection * m_speed *Time.deltaTime ;

		transform.Translate(movement,Space.World);		

		if(targetDirection==Vector3.zero)
		{
			//no input
		}
		else
		{
			//has input
		}

		//reset values
		x_input =0;
		y_input =0;
		vertical_input = 0;
	}
	*/	

	public float Vector3Distance(Vector3 a, Vector3 b)
	{
		a.y = b.y;
		return Vector3.Distance(a,b);
	}

	float minStoppingDistance = 0.2f; //1

	private void SetFakeMoveInput()
	{
		Vector3 targetPosition;

		if(isRunningTowardsPreviousStation)
		{
			targetPosition = prevPos;
		}
		else
		{
			targetPosition = enemyTarget.transform.position;
		}


		//#######################################################################
		//Filterrring target
		//#######################################################################

		//cheking any obstackles infront and re routing out path
		//***** This can override any set up target
		RaycastHit hit;
		Vector3 rayOriginPos = transform.position;
		rayOriginPos.y +=1;
		Vector3 dir = targetPosition-transform.position;
		dir.y = 0;
		dir.Normalize ();

		Vector3 rayTarget = targetPosition;
		rayTarget.y = rayOriginPos.y;
		Debug.DrawLine(rayOriginPos,rayTarget,Color.white);

		//check if we are too close to our target
		if (Vector3Distance (transform.position, targetPosition) < minStoppingDistance) {
			targetPosition = transform.position;

			if(isRunningTowardsPreviousStation)
			{
				if(OnPrevPointReached!=null)
					OnPrevPointReached();
			}
			else
			{
				if (OnPointReached != null)
					OnPointReached ();
			}
		} 
		else 
		{
			if (Physics.SphereCast (rayOriginPos, 0.5f, dir, out hit, Vector3.Magnitude(this.transform.position-targetPosition)-minStoppingDistance, rayCastingLayer)) 
			{
				

				//re route ourselves
				Vector3 dirRevNorm = -hit.normal;
				Vector3 dirR90 =  Quaternion.Euler(0,90f,0)*dirRevNorm;
				Vector3 dirL90 =  Quaternion.Euler(0,-90f,0)*dirRevNorm;
				Vector3 closestPoint = hit.collider.ClosestPointOnBounds (rayOriginPos);

				Debug.DrawLine (rayOriginPos, hit.point, Color.black);
				Debug.DrawLine (rayOriginPos, closestPoint, Color.cyan);
				Debug.DrawRay (hit.point, dirR90, Color.red);
				Debug.DrawRay (hit.point, dirL90, Color.green);
				Debug.DrawRay (hit.point, hit.normal, Color.blue);




				float c = 1.5f;
				float d = Mathf.Clamp(Vector3.Distance(closestPoint,rayOriginPos),c,float.MaxValue);



				Vector3 dirR = Quaternion.Euler(0,45f,0)*dir;
				Vector3 dirL = Quaternion.Euler(0,-45f,0)*dir;

				Vector3 dirToObj = (hit.transform.position-rayOriginPos).normalized;
				float angleR = Vector3.Angle(dirToObj,dirR);
				float angleL = Vector3.Angle(dirToObj,dirL);

				bool right = angleR>angleL;

				if(right)
				{
					dir = Vector3.Lerp(dir,dirR90,c/d);
				}
				else
				{
					dir = Vector3.Lerp(dir,dirL90,c/d);
				}

				//dir.Normalize();
				Debug.DrawRay (rayOriginPos, dir, Color.yellow);
				targetPosition = this.transform.position + dir;
			}
		}
		//#######################################################################
		//firterring target for obstackle end
		//#######################################################################

		//set input
		Vector3 direction = (targetPosition-this.transform.position);

		if(direction.magnitude>1f) 
			direction = direction.normalized;

		x_input = direction.x;
		y_input = direction.z;
		vertical_input = direction.y;

	}

	public bool IsMoving()
	{
		return isMoving;
	}

	public bool IsAutoMoving()
	{
		if(!isMoving) return false;
		else return !isManualWalk;
	}
		
	public void GetBackToPreviousCover()
	{
		//check if walking
		if(!isMoving) return;
		//check for manual walk
		if(!isManualWalk) return;
		//check origin distance
		if(Vector3.SqrMagnitude((originPos-this.transform.position))<2f) return;
		//check if stabbing
		if(sp.isStabing) return;

		StartCoroutine(StopAndStoreCurrentMovement());
	}

	public void GetBackToPreviousCover(Vector3 externalTarget)
	{
		isRunningTowardsPreviousStation = true;
		prevPos = externalTarget;
		OnPrevPointReached += OnExternalWalkOver;
		StartAutoWalk();
	}

	private void OnExternalWalkOver()
	{
		StopWalk();
		OnPrevPointReached -= OnExternalWalkOver;
		isRunningTowardsPreviousStation = false;
	}

	private IEnumerator StopAndStoreCurrentMovement()
	{
		isMoving = false;
		isRunningTowardsPreviousStation = true;
		yield return null;
		prevPos = originPos;
		OnPrevPointReached += ResumePreviousMovement;
		StartAutoWalk();
	}
		
	private void ResumePreviousMovement()
	{
		StopWalk();
		isRunningTowardsPreviousStation = false;
		OnPrevPointReached -= ResumePreviousMovement;
		StartManualWalk();
	}

	public void StartManualWalk()
	{
		if(isMoving) return;

        animator.SetInteger("WalkType",0);
		originPos = this.transform.position;
		StopCoroutine("SpeedFallOff");
		walkSpeed = 0f;
		isMoving = true;
		isManualWalk = true;
		StartCoroutine("SpeedFallOff");
	}

	public void StartAutoWalk()
	{
		if(isMoving) 
		{
			//Debug.Log("i am already moving so returning");
			return;
		}
        animator.SetInteger("WalkType",1);
		originPos = this.transform.position;
		walkSpeed = autoRunSpeed;
		isMoving = true;
		isManualWalk = false;
	}

	public void StopWalk()
	{
		isMoving = false;
		isManualWalk = false;
	}

	public void SetTarget(Transform target)
	{
		enemyTarget = target;
	}

	//int tapCounter=0;
	public void SpeedUp()
	{
		if(walkSpeed <=0)
		{
			walkSpeed = 0.9f;
		}
		else
		{
			walkSpeed = (Mathf.Clamp(walkSpeed+speedGain,0,maxAllowedSpeed));
		}

	}

	public float GetSpeedRatio()
	{
		if(isMoving)
			return moveSpeed/maxAllowedSpeed;
		else
			return 0;
	}

	private float defaultMoveSpeed = 0.8f;	//5
	private float maxAllowedSpeed = 3f;		//8
	private float moveSpeedDownRatePerSec = 8f;	//5
	private float speedGain =0.9f;	//1.3
	private float autoRunSpeed = 3f;

	private IEnumerator SpeedFallOff()
	{
		do
		{
			if(isManualWalk)
			{
				/*if(animator.GetCurrentAnimatorStateInfo(0).shortNameHash.Equals(Animator.StringToHash("SlowWalk")) && x_input==0 && y_input==0)
				{
					walkSpeed = (Mathf.Clamp(walkSpeed-0.01f*Time.deltaTime,0,maxAllowedSpeed));
					//no change in move speed
					Debug.Log("No input but still moving");
				}
				else*/
				if(canUseSpeedFallOff)
					walkSpeed = (Mathf.Clamp(walkSpeed-moveSpeedDownRatePerSec*Time.deltaTime,0,maxAllowedSpeed));
			}
			else
			{
				walkSpeed = autoRunSpeed;
				//Debug.Log("No manual walk");
			}

			yield return null;
		}while(isMoving == true);
	}
}
