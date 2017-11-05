using UnityEngine;
using System.Collections;
using Portbliss.Station;
using RootMotion.FinalIK;
using UnityEngine.Serialization;

public class ThirdPersonController : MonoBehaviour {

	public bool canPlaySlowMotionBullet = true;
	public bool canSwitchBetweenCameras = true;

    public FighterName fighterName;
    [FormerlySerializedAs("fighterID")]
    public FighterRole fighterRole;

	public Transform headViewCamera;
	public Transform shoulderViewCamera;
    public Transform targetReference;
    public Weapon assignedWeapon;

    public AIPersonnelCanvasController personalCanvas;
    private float PF_healthPoint= 100.0f;
    public float healthPoint
    {
        set
        {
            PF_healthPoint = value;
            if (personalCanvas != null)
            {
                personalCanvas.UpdateHP(PF_healthPoint/100.0f);
            }
        }
        get
        {
            return PF_healthPoint;
        }
    }


	#region arif
	public float lastHitTime;
	private bool washitrecentlyprivateHUDFLASHVERSION;
	public bool wasHitRecentlyHUDFLASHVERSION{
		set
		{ 
			washitrecentlyprivateHUDFLASHVERSION = value;
		}
		get
		{ 
			if (washitrecentlyprivateHUDFLASHVERSION) 
			{
				washitrecentlyprivateHUDFLASHVERSION = false;
				return true;
			} 
			else 
			{
				washitrecentlyprivateHUDFLASHVERSION = false;
				return false;
			}
		}
	}
	private static bool aPlayerWasHitPrivateLRINDICATORVERSION;
	public static bool aPlayerWasHitLRINDICATORVERSION{
		set
		{ 
			aPlayerWasHitPrivateLRINDICATORVERSION = value;
		}
		get
		{ 
			if (aPlayerWasHitPrivateLRINDICATORVERSION) 
			{
				aPlayerWasHitPrivateLRINDICATORVERSION = false;
				return true;
			} 
			else 
			{
				aPlayerWasHitPrivateLRINDICATORVERSION = false;
				return false;
			}
		}
	}
	#endregion


	public float regenRate_Aggressive
    {
        get
        {
            return AIPlayerDataManager.GetDataStruct(fighterName).regenRate_Aggressive;
        }
    }
	public float regenRate_NonAggressive
    {
        get
        {
            return AIPlayerDataManager.GetDataStruct(fighterName).regenRate_NonAggressive;
        }
    }
	public float regenRate_OffControl
    {
        get
        {
            return AIPlayerDataManager.GetDataStruct(fighterName).regenRate_OffControl;
        }
    }
	public float regenRate_ai
    {
        get
        {
            return AIPlayerDataManager.GetDataStruct(fighterName).regenRate_ai;
        }
    }

	public float cameraMaxVertical = 0f;
	public float cameraMinVertical = 0f;
	public float cameraMaxHorizontal = 0f;
	public float cameraMinHorizontal = 0f;


	public GameObject granade;
	public float minGranadeRange = 2f;
	public float maxGranadeRange = 30f;
	public float maxLookAtDistance = 50f;
	public float minLookAtDistance = 8f;
    [SerializeField][HideInInspector]public ModelStructure modelStructure;


    public bool isScopeOn
    {
        get
        {
            return _isScopeOn;
        }
        set
        {
            _isScopeOn = value;
            if (_isScopeOn)
            {
                if (assignedWeapon.hasScope)
                {
                    ZoomInCameraView();
                    SwitchMeshRender(false);
                }
            }
            else
            {
                if (assignedWeapon.hasScope)
                {
                    ZoomOutCameraView();
                    SwitchMeshRender(true);
                }
            }
        }
    }

	private StationController sc_controller;
	private Animator thisAnimator;
	private Camera main_camera;
	private Transform playerModel;
	private Transform rightThumb;

	private Vector3 savedForward;
	private Vector3 savedRight;
	private Vector3 cameraSavedForward;
	private Vector3 cameraSavedUp;
	private Vector3 screenCenter;
	private bool isCameraZoomedIn = false;
	private bool isThrowingGranade = false;
	private bool isReloading = false;
    private bool isInControll = false;
	private bool isDead = false;
    private bool isInvincible = false;
    private bool _isScopeOn = false;
	private PlayerAI ai;
	private ViewRect baseViewRect;
	private ViewRect viewRect;
	private bool wasAimingBeforeReloading = false;
    private bool wasScopingBeforeReloading = false;
	private AimIK aimik;
    private Renderer[] renderableMesh;

	private Quaternion aimCamRot;
	private Quaternion hideCamRot;

	void Start () {

		main_camera = Camera.main;
		thisAnimator = this.GetComponent<Animator>();
		playerModel = this.transform;

		screenCenter = new Vector3 (Screen.width/2f,Screen.height/2f,0f);
		assignedWeapon.Initialize(this);
        rightThumb = modelStructure.rightHandThumb1;
		sc_controller = this.GetComponent<StationController>();
		ai = this.GetComponent<PlayerAI>();
		aimik = this.GetComponent<AimIK>();

		//set view rect
		viewRect = new ViewRect(cameraMaxHorizontal, cameraMinHorizontal, cameraMaxVertical, cameraMinVertical);

		ViewRect nearest = LookAtManager.instance.GetLookAtPoint (this.transform.position).vr;
		if (nearest != ViewRect.Zero ())
			viewRect = nearest;

		baseViewRect = viewRect;

		aimCamRot = shoulderViewCamera.transform.localRotation;
		hideCamRot = headViewCamera.transform.localRotation;

		UpdateSavedDirections (viewRect, LookAtManager.instance.GetLookAtPoint(this.transform.position).point);

        //get mesh object
        PopulateMeshArray();

        //black out all camera
		Destroy(shoulderViewCamera.GetComponent<GUILayer>());
		Destroy(shoulderViewCamera.GetComponent<FlareLayer>());
		Destroy(shoulderViewCamera.GetComponent<Camera>());
		Destroy(headViewCamera.GetComponent<GUILayer>());
		Destroy(headViewCamera.GetComponent<FlareLayer>());
		Destroy(headViewCamera.GetComponent<Camera>());

		//headViewCamera.GetComponent<Camera>().enabled = false;
		//shoulderViewCamera.GetComponent<Camera>().enabled = false;

        //Setting up the camers
        PlayerDataStruct pds = PlayerDataManager.GetPlayerDataStruct(fighterName);

        if (pds.headCameraPos == Vector3.zero)
        {
            Debug.LogError("NO valid data structer found for the player "+ fighterName);
        }
        else
        {
            headViewCamera.transform.localPosition = pds.headCameraPos;
            headViewCamera.transform.localRotation = pds.headCameraRot;

            shoulderViewCamera.transform.localPosition = pds.shoulderCameraPos;
            shoulderViewCamera.transform.localRotation = pds.shoulderCameraRot;
        }

        ai_ik_target = new GameObject("Aim_IK_Target").transform;
	}
	
	private GameObject tempTarget;

    void LateUpdate()
    {
        AimIkSetup();
    }

	void Update () {

		

		//draw rays
		Debug.DrawRay(playerModel.transform.position,savedForward*3f,Color.red);
		Debug.DrawRay(playerModel.transform.position,savedRight*3f,Color.red);
		Debug.DrawRay(headViewCamera.transform.position,cameraSavedForward*3f,Color.green);
		Debug.DrawRay(headViewCamera.transform.position,cameraSavedUp*3f,Color.green);
		Debug.DrawRay(playerModel.transform.position,playerModel.transform.forward*3f,Color.white);

		if(isDead) return;

		//check for dead status
		if(healthPoint<0) 
			Dead();

		//increase health point when not in controll
		bool isMoving = false;
		if(sc_controller!=null)
			isMoving = sc_controller.IsMoving();
			
		if(IsInControll())
		{
			
			if (IsShooting () || IsThrowingGranade () || IsAiming ()) 
			{
				healthPoint = Mathf.Clamp(healthPoint + regenRate_Aggressive*Time.deltaTime,-1f,100f);
			} 
			if( IsReloading() || IsHiding() || isMoving )
			{
				healthPoint = Mathf.Clamp(healthPoint + regenRate_NonAggressive*Time.deltaTime,-1f,100f);
			}

		}
		else if (ai!=null)
		{
			if(ai.enabled == true)
				healthPoint = Mathf.Clamp(healthPoint + regenRate_ai*Time.deltaTime,-1f,100f);
			else
				healthPoint = Mathf.Clamp(healthPoint + regenRate_OffControl * Time.deltaTime, -1f, 100f);
		}
        else
        {
            healthPoint = Mathf.Clamp(healthPoint + regenRate_OffControl * Time.deltaTime, -1f, 100f);
        }

			
	}


    Vector3 direction = new Vector3();
    Vector3 up = new Vector3();
    Vector3 right = new Vector3();
    private Transform ai_ik_target;

	void AimIkSetup()
	{
		if(aimik==null) return;

        if (isDead)
        {
            aimik.solver.IKPositionWeight = 0f;
            return;
        }

		if(tempTarget==null) 
		{
			tempTarget = new GameObject();
			tempTarget.transform.position = Vector3.zero;
		}


		if(IsSlowMotionBulletPlaying())
		{
			aimik.solver.IKPositionWeight = 0f;
			return;
		}

		if(isInControll)
		{
			if (IsReloading()) 
			{
				aimik.solver.IKPositionWeight = 0f;
				return;
			}

			if (sc_controller.IsMoving()) 
			{
				aimik.solver.IKPositionWeight = 0f;
				return;
			}

            if (IsThrowingGranade())
            {
                aimik.solver.IKPositionWeight = 0f;
                return;
            }

			if (IsShooting() || IsAiming())
			{
				//lets setup some ik
				aimik.solver.IKPositionWeight = 1f;

				Ray screenRay = main_camera.ScreenPointToRay(screenCenter);
				RaycastHit hit;

				Vector3 lookAtPoint = main_camera.transform.position + screenRay.direction * maxLookAtDistance;

				if(Physics.Raycast(screenRay,out hit,maxLookAtDistance))
				{
					//check for distance
					if(Vector3.Distance(main_camera.transform.position,hit.point)<minLookAtDistance)
					{
						lookAtPoint = main_camera.transform.position + screenRay.direction * minLookAtDistance;
					}
					else
					{
						lookAtPoint = hit.point;
					}
				}

				//lerp the position
				if(tempTarget.transform.position !=Vector3.zero)
				{
					tempTarget.transform.position = Vector3.Lerp(tempTarget.transform.position,lookAtPoint,10f*Time.deltaTime);
				}
				else
				{
					tempTarget.transform.position = lookAtPoint;
				}


				aimik.solver.target = tempTarget.transform;
			}
			else
			{
				aimik.solver.IKPositionWeight = 0f;
				return;
			}
		}
		else
		{
            if (ai != null)
            {
                if (IsReloading())
                {
                    aimik.solver.IKPositionWeight = 0f;
                    return;
                }
                if (sc_controller.IsMoving())
                {
                    aimik.solver.IKPositionWeight = 0f;
                    return;
                }

                if (ai.GetAICurrentTarget() != null)
                {
                    if (IsAiming() || IsShooting())
                    {
                        //if (Vector3.Distance(ai_ik_target.transform.position, ai.GetAICurrentTarget().position) > 5f)
                        //    ai_ik_target.transform.position = ai.GetAICurrentTarget().position;
                        
                        ai_ik_target.position = Vector3.Lerp(ai_ik_target.position, ai.GetAICurrentTarget().position, 5f * Time.deltaTime);
                        aimik.solver.IKPositionWeight = 1f;
                        aimik.solver.target = ai_ik_target;
                        return;
                    }
                    else
                    {
                        aimik.solver.IKPositionWeight = 1f;
                        return;
                    }
                }
                else
                {
                    if (IsAiming() || IsShooting())
                    {
                        aimik.solver.IKPositionWeight = 1f;
                        return;
                    }                   
                    else
                    {
                        aimik.solver.IKPositionWeight = 1f;
                        return;
                    }
                }
            }

			aimik.solver.IKPositionWeight = 0f;
			return;
		}

	}
   

    private void PopulateMeshArray()
    {
        renderableMesh = this.gameObject.GetComponentsInChildren<Renderer>();
    }

    private void SwitchMeshRender(bool isOn)
    {
        for (int i = 0; i < renderableMesh.Length; i++)
            renderableMesh[i].enabled = isOn;
    }

    private bool CheckScopeFilter()
    {
        if (IsDead())
        {
            Debug.LogWarning("The target player is dead and can not turn on the scope");
            isScopeOn = false;
            return false;
        }
        if (!IsInControll())
        {
            Debug.LogWarning("The player is not in controll to turn on the scope");
            return false;
        }
        if (IsReloading())
        {
            Debug.LogWarning("The player is reloading. Can not switch to scope");
            return false;
        }
        if (IsThrowingGranade())
        {
            Debug.LogWarning("The player is throwing granade. Can not switch to scope");
            return false;
        }

        //good to go
        return true;
    }

	private void Dead()
	{
		if(GeneralManager.godMode) return;

		isDead = true;
        isScopeOn = false;
        PlayerInputController.instance.current_player.isScopeOn = false;
        PlayerInputController.instance.current_player.SetControll(false);

		if(sc_controller!=null) sc_controller.StopMoveByForce();

		thisAnimator.SetTrigger("dead");

		StartCoroutine(AfterGameWork());

    }

	private IEnumerator AfterGameWork()
	{
		//turn off all input control
		HUDManager.instance.forceDisableRun_ShootGroup = true;

		//turn off all camera transition
		//CameraController.instance.StopAllTransition();

		//wait for a sec
		yield return new WaitForSeconds(0.5f);

		//halt all camera transition

		//switch camera to this player
		//main_camera.transform.SetParent(null);
		//main_camera.transform.SetParent(this.transform);
		//SwitchToHeadCameraView(true);
        SwitchToDeathCameraView();

		yield return new WaitForSeconds(2.5f);

		GeneralManager.EndGame(false);
	}

	private IEnumerator AfterDeathAnimation(int indexOfDead)
	{
		yield return null;
		/*SwitchToHeadCameraView();
		yield return new WaitForSeconds(1.5f);
        GameManagerScript.instance.CharacterDeath(indexOfDead,true);*/
    }
		
    [HideInInspector]public GameObject dummyPlayer;
    [HideInInspector]public GameObject dummyCamera;

    public void SetPlayerScope(bool isOn)
    {
        if (isOn == true)
        {
            if (assignedWeapon.hasScope)
            {
                bool hasPermission = CheckScopeFilter();

                if (hasPermission == false)
                    return;

                if (IsAiming())
                {
                    isScopeOn = true;
                    return;
                }
                else
                {
                    SwitchToShoulderCameraView();
                    isScopeOn = true;
                }
            }
            else
            {
                Debug.LogWarning("The weapon does not have scope property turned on but you are still trying to set it");
                isScopeOn = false;
            }
        }
        else
        {
            isScopeOn = false;
        }
    }

	public void ResetPlayerBaseCameraRot()
	{
		headViewCamera.transform.localRotation = hideCamRot;
		shoulderViewCamera.transform.localRotation = aimCamRot;
	}

	public void UpdateSavedDirections (ViewRect vr, Vector3 toTarget)
	{
		toTarget.y = playerModel.transform.position.y;

		//create a false player a camera
		if(dummyPlayer == null) dummyPlayer = new GameObject("DummyArifVai");
		if(dummyCamera == null) dummyCamera = new GameObject ("DummyCamera");

		dummyCamera.transform.SetParent(dummyPlayer.transform);
		dummyCamera.transform.localPosition = headViewCamera.transform.localPosition;
		dummyCamera.transform.localRotation = headViewCamera.transform.localRotation;

		dummyPlayer.transform.position = playerModel.transform.position;
		dummyPlayer.transform.LookAt(toTarget);

		savedForward = dummyPlayer.transform.TransformDirection (Vector3.forward);
		savedRight = dummyPlayer.transform.TransformDirection (Vector3.right);
		cameraSavedForward = dummyCamera.transform.TransformDirection (Vector3.forward);
		cameraSavedUp = dummyCamera.transform.TransformDirection (Vector3.up);

		if(vr == ViewRect.Zero())
		{
			viewRect = baseViewRect;
		}
		else
		{
			viewRect = vr;
		}
	}

	public Vector3 GetBaseForward()
	{
        if (savedForward == Vector3.zero)
            return this.transform.forward;
        else
		    return savedForward;
	}


	public void AttachAI(bool useViewAngle)
	{
		ai = this.gameObject.GetComponent<PlayerAI>();
		if(ai==null) ai = this.gameObject.AddComponent<PlayerAI>();
		ai.enabled = true;
		ai.useViewAngle = useViewAngle;

		if(sc_controller!= null) sc_controller.AttachAI(ai);
		else {Debug.LogError("No Station controller");}

		if(!PlayerInputController.instance.aiPlayers.Contains(this))
			PlayerInputController.instance.aiPlayers.Add(this);
	}

	public void RemoveAI()
	{
		if(ai!=null)
		{
			this.gameObject.GetComponent<PlayerAI>().enabled = false;
			ai= null;
			sc_controller.AttachAI(null);

			if(PlayerInputController.instance.aiPlayers.Contains(this))
				PlayerInputController.instance.aiPlayers.Remove(this);
		}
	}

	public PlayerAI GetAI()
	{
		return ai;
	}

    public void AILookAt(Vector3 lookAtPoint)
    {
        if(assignedWeapon.canAIFire())
        {
            lookAtPoint.y = this.transform.position.y;
            this.transform.LookAt(lookAtPoint);
        }
    }

	public void TakeImapct(RaycastHit hit, float damageValue, HitSource hitSource)
	{
        if (hitSource == HitSource.PLAYER) return;
        //if (fighterID == FreedomFighter.Nura) damage = damage * 1.8f;
		if (AIDataManager.instance != null) {
            foreach (PlayerDamageInputModifier pdim in AIDataManager.instance.playerDamageInputMods)
            {
                if (pdim.fighterID == this.fighterRole)
                {
                    damageValue = damageValue * pdim.baseDamageMultiplier;
                    break;
                }
            }
		}
		if (IsHiding() || IsReloading()) {
			if (AIDataManager.instance != null) {
                foreach (PlayerDamageInputModifier pdim in AIDataManager.instance.playerDamageInputMods)
                {
                    if (pdim.fighterID == this.fighterRole)
                    {
                        damageValue = damageValue * pdim.hideToBaseDamageRatio*3.33f; //3.33 is premultiplied to mitigate effects of 0.3 mandatory multiplication
                        break;
                    }
                }
			}
			damageValue = damageValue * 0.3f; //this line is mandatory for some reason
		}

        LowerHP(damageValue);

    }

	public void TakeNadeImpact(float damageValue, HitSource hitSource, bool shouldIgnoreHiding = false)
	{
		if (hitSource == HitSource.PLAYER) return;

        if(hitSource == HitSource.GAYEBI)
        {
            damageValue = healthPoint * .75f;
            LowerHP(damageValue, shouldIgnoreHiding);
            StartCoroutine(MakeInvincible());
            return;
        }
		//Debug.Log (damageValue);
		LowerHP( damageValue, shouldIgnoreHiding);

	}

    private IEnumerator MakeInvincible()
    {
        isInvincible = true;
        yield return new WaitForSeconds(3.25f);
        isInvincible = false;
    }

    private void LowerHP(float damageValue, bool shouldIgnoreHiding = false)
    {
		if (!isInControll) {
			InGameSoundManagerScript.instance.PPDP_GotHitCry (this.fighterName);
		}
		wasHitRecentlyHUDFLASHVERSION = true;
		aPlayerWasHitLRINDICATORVERSION = true;
		lastHitTime = Time.time;
		if (PlayerInputController.instance.current_player.fighterRole == this.fighterRole) {
			HUDManager.TriggerToolTip (ToolTipType.Hide);
		}
        if (isInvincible)
        {
			return;
        }
		if(GeneralManager.godMode) return;


        healthPoint -= damageValue;
        healthPoint = Mathf.Clamp(healthPoint, -1f, 100f);
//		if (healthPoint <= 0)
//			Debug.Break ();
//		if(ai!=null) Debug.Log("I am a ai and taking damage. " + healthPoint);
   }

	public bool IsSlowMotionBulletPlaying()
	{
		return false;
		//return SlowMotionBullet.instance.IsSlowMotionOn();
	}

	public Transform GetTargetReference()
	{
		if(targetReference!=null)
			return targetReference;
		else
		{
			Debug.LogError(string.Format("No target reference set up at {0}",playerModel.name));
			return this.transform;
		}
	}

	public bool CanThrowGranade()
	{
		if(isDead || isThrowingGranade || isReloading || GetStationController().IsMoving()) return false;
		return true;
	}

	public void ThrowGranade()
	{
		if(isDead) return;
		if(isThrowingGranade || isReloading) return;
		if(GetStationController().IsMoving()) return;

		//stop camera shake
		//if(ShakeCamera.instance.IsShaking()) Debug.Log("hhhhhhhhhh");
		//ShakeCamera.instance.StopCameraShake();
		//if(ShakeCamera.instance.IsShaking()) Debug.Log("jjjjjjjjj");
		ReleaseCamera();

		isThrowingGranade = true;

		thisAnimator.SetTrigger("granade");
	}

	public void Reload()
	{
		if(isDead) return;
		if(isReloading || isThrowingGranade) return;
		if(!assignedWeapon.CanReload()) return;
		if(sc_controller!=null)
		{
			if(sc_controller.IsMoving()) return ;
		}

		//save my state
		wasAimingBeforeReloading = PlayerInputController.instance.isAiming;
        wasScopingBeforeReloading = isScopeOn;

		if(wasAimingBeforeReloading)
			SwitchToHeadCameraView(true);

        if (wasScopingBeforeReloading)
            isScopeOn = false;

        thisAnimator.SetBool("reloading",true);
		thisAnimator.SetTrigger ("reload");
		isReloading = true;
    }

	public StationController GetStationController()
	{
		if(sc_controller == null) sc_controller = this.gameObject.GetComponent<StationController>();

		return sc_controller;
	}

	public float GetReloadPercentage()
	{
		if(isReloading)
		{
			if(thisAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("Reload"))
			{
				return thisAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
			}
			else
			{
				return 0;
			}
		}
		else
		{
			return 1f;
		}
	}

	public void ThrowGranadeEvent()
	{
		if(isDead) return;

		//calculate to position
		Vector3 toPos = Vector3.zero;
		Vector3 fromPos = rightThumb.position;
		Vector3 toPoint = new Vector3(screenCenter.x,screenCenter.y,0f);
		RaycastHit hit;
		bool didHit = false;
		Ray screenRay = main_camera.ScreenPointToRay(toPoint);
		
		didHit = Physics.Raycast(screenRay, out hit);
		
		if (!didHit)
		{
			toPos = main_camera.transform.position + screenRay.direction*maxGranadeRange;
		}
		else
		{
			toPos = hit.point;
		}
		//check for min distance
		float distance = Vector3.Distance (fromPos, toPos);
		if(distance<minGranadeRange)
		{
			toPos = fromPos + ((toPos-fromPos).normalized)*minGranadeRange;
			toPos.y = fromPos.y;
		}
		//release granade
		//ProjectileScript.ReleaseNadeP2P (granade, fromPos, toPos, 30f, HitSource.PLAYER, 100, 10, Quaternion.identity);
        ProjectileScript.ReleaseNadeAngleElevated(granade, fromPos, (toPos-fromPos).normalized,18, 15f, HitSource.PLAYER, 100, 10, Quaternion.identity);

    }
    public void GranadeThrowDone()
	{
		
        if(PlayerInputController.instance.index == GetMyIndex())
		    ReparentCamera();

		isThrowingGranade = false;
	}

	public void CancelReload()
	{
		//we dont ant to reload

		//this function should only be called if we want to move to next cover point
		isReloading = false;
		thisAnimator.SetBool("reloading", false);

        if (wasScopingBeforeReloading && isInControll)
        {
            SetPlayerScope(true);
        }
        else
        {
            if (wasAimingBeforeReloading && isInControll)
                SwitchToShoulderCameraView();
        }
	}

	public void ReloadDone()
	{
		assignedWeapon.ResetBulletCount();
		isReloading = false;
        thisAnimator.SetBool("reloading", false);

        if (wasScopingBeforeReloading && isInControll)
        {
            SetPlayerScope(true);
        }
        else
        {
            if (wasAimingBeforeReloading && isInControll)
                SwitchToShoulderCameraView();
        }
    }

	public bool IsReloading()
	{
		return isReloading;
	}

	public bool IsThrowingGranade()
	{
		return isThrowingGranade;
	}

	public bool IsAiming()
	{
		return thisAnimator.GetBool("aim");
	}

	public bool IsShooting()
	{
        return thisAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("Fire");
	}


    public void SetControll(bool inControll)
    {
        isScopeOn = false;
        isInControll = inControll;

		if(inControll==false)
		{
            //hide player
            thisAnimator.SetBool("aim", false);
        }
    }

	public bool IsInControll()
	{
		return isInControll;
	}

	public bool IsDead()
	{
		return isDead;
	}

	public bool IsHiding()
	{
		return !thisAnimator.GetBool("aim");
	}

    public void PlayReloadSound()
    {
		InGameSoundManagerScript.PlayOnPointFromID (this.transform.position, ClipID.gunReload);
    }
    public void MiniLoadSound_Jamal()
    {
		InGameSoundManagerScript.PlayOnPointFromID (this.transform.position, ClipID.gunFire_sniperRXM_boltAct);
    }

	public Animator GetAnimator()
	{
		if(thisAnimator== null) thisAnimator =  this.GetComponent<Animator>();
		return thisAnimator;
	}
    public void Shoot(float recoilX, float recoilY, bool canActuallyDischarge, bool AIShoot = false, Transform AIShootTarget = null)
	{
		//Debug.Log ("A");
		if(isDead) return;
        switch (fighterName) {
            case FighterName.Baker:
                InGameSoundManagerScript.PlayOnPointFromID (this.transform.position,ClipID.gunFire_rifleM4);
                break;
            case FighterName.Dom:
                InGameSoundManagerScript.PlayOnPointFromID (this.transform.position, ClipID.gunFire_rifleAR15);
                break;
            case FighterName.Philips:
                switch (fighterRole)
                {
                    case FighterRole.Support:
                        InGameSoundManagerScript.PlayOnPointFromID (this.transform.position,ClipID.gunFire_smg_MP5);
                        break;
                    case FighterRole.Sniper:
                        InGameSoundManagerScript.PlayOnPointFromID (this.transform.position,ClipID.gunFire_G3SG1);
                        break;
                }
                break;
            case FighterName.JB:
                InGameSoundManagerScript.PlayOnPointFromID (this.transform.position,ClipID.gunFire_sniperRXM);
                break;

        }

		GetAnimator().SetTrigger("shoot");
        //assignedWeapon.muzzleFlash.Play();

        //if just for show off
        if(canActuallyDischarge==false) return;

		//for Ai Shooting
		if(AIShoot)
		{
			AILookAt(AIShootTarget.position);
			direction = AIShootTarget.position - assignedWeapon.lineRenderer.transform.position;
			up = Vector3.Cross(direction, Vector3.right);
			right = Vector3.Cross(direction, Vector3.up);
			float maxdeflection = 2*(1-assignedWeapon.accuracy);

			direction = Quaternion.AngleAxis(Random.Range(-maxdeflection,maxdeflection), right) * direction;
			direction = Quaternion.AngleAxis(Random.Range(-maxdeflection,maxdeflection), up) * direction;
			RaycastHit ai_hit;

			//render bullet trail
			assignedWeapon.RenderBulletTrail(assignedWeapon.lineRenderer.transform.position+ direction*20f);

			if (Physics.Raycast(assignedWeapon.lineRenderer.transform.position, direction, out ai_hit, 100.0F))
			{
				iBulletImpact ibt = ai_hit.collider.gameObject.GetComponent<iBulletImpact>();
				if (ibt != null)
				{
                    ibt.TakeImapct(new RaycastHit(), assignedWeapon.GetDamagedPoint(Vector3.Distance(this.transform.position,AIShootTarget.transform.position))*AIPlayerDataManager.instance.scenePlayerAIDamageMultiplier, HitSource.PLAYER_AI);
					//Debug.Log(fighterID + ": "+ "AI is taking damage "+assignedWeapon.GetDamagedPoint(Vector3.Distance(this.transform.position,AIShootTarget.transform.position))*.1f);
				}
				return;


			}
			else
			{
				return;
			}

		}

		Vector3 toPoint = new Vector3(screenCenter.x+recoilX,screenCenter.y+recoilY,0f);
		RaycastHit hit;
        bool didHit = false;
        Ray screenRay = main_camera.ScreenPointToRay(toPoint);

        didHit = Physics.Raycast(screenRay, out hit);
		//didHit = Physics.SphereCast(screenRay,0.1f, out hit);

        if (!didHit)
        {
            assignedWeapon.RenderBulletTrail(main_camera.transform.position + screenRay.direction*50f);
            return;
        }

		//draw debug ray
		//StartCoroutine(DrawBulletRay(screenRay.origin,hit.point));

        //Did hit
        //Check for slow motion bullet criteria


		if(canPlaySlowMotionBullet)
		{

            iPreHitQuery preHit = hit.transform.gameObject.GetComponent<iPreHitQuery>();
            if (preHit != null)
            {

                HitResult hitResult = preHit.GetHitResults(assignedWeapon.GetDamagedPoint(hit.distance));
                if (hitResult.willDie)
                {
                    if(AIDataManager.activeEnemyList.Count == 1 || hitResult.aipersonnelsReference.enemyType == EnemyType.MORTAR)
                    {
                        SlowMotionBullet.instance.PlaySlowMotionBullet(assignedWeapon.GetWeaponHeadPoint(), assignedWeapon.GetDamagedPoint(hit.distance), hitResult.aipersonnelsReference.transform, hit);
                        return;
                    }
                }
            }
		}
        


        //call line render
        assignedWeapon.RenderBulletTrail(hit.point);

        //tell them that are hit
        iBulletImpact it = hit.transform.gameObject.GetComponent<iBulletImpact>();
		if(it != null)
			it.TakeImapct(hit, assignedWeapon.GetDamagedPoint(hit.distance), HitSource.PLAYER);
	}

	private IEnumerator DrawBulletRay(Vector3 from, Vector3 to)
	{
		float rendertiem = 5f;
		do{
			Debug.DrawLine(from,to,Color.blue);
			rendertiem-=Time.deltaTime;
			yield return null;
		}while(rendertiem>0);
	}

    public bool CanRotateCamera()
    {
        if (GeneralManager.instance.gameOver)
            return false;
        if (isDead)
            return false;
        if(isThrowingGranade) 
            return false;
        if (ImprovedCameraCntroller.instance.IsPrimaryMovementOn())
            return false;
        if(ai!=null) {if(ai.IsAIOn()) return false;}

        return true;
    }

	public void RotateCameraHorizontal(float amount)
	{
		if(isDead) return;
		if(isThrowingGranade) return;
        if (ImprovedCameraCntroller.instance.IsPrimaryMovementOn()) return;
		if(ai!=null) {if(ai.IsAIOn()) return;}

        //set drag speed
        amount *=  ImprovedCameraCntroller.GetCameraDragFactor();

		float angleWithForward = Vector3.Angle(savedForward, playerModel.TransformDirection(Vector3.forward));
		float angleWithRight = Vector3.Angle(savedRight, playerModel.TransformDirection(Vector3.forward));

		if(angleWithRight> 90)
			angleWithForward = -angleWithForward;

		//clamp amount value
		if(amount<0 && angleWithForward > viewRect.minHorizontal)
		{
			amount = Mathf.Clamp(amount,viewRect.minHorizontal-angleWithForward,0);
		}
		else if(amount>0 && angleWithForward <viewRect.maxHorizontal)
		{
			amount = Mathf.Clamp(amount,0,viewRect.maxHorizontal-angleWithForward);
		}

		if( (amount<0 && angleWithForward > viewRect.minHorizontal) || (amount>0 && angleWithForward < viewRect.maxHorizontal))
		{
			playerModel.transform.Rotate(Vector3.up*amount,Space.Self);
		}

        if(IsInControll())
            ImprovedCameraCntroller.instance.RequestCameraAimMove(CanRotateCamera,IsAiming()?shoulderViewCamera:headViewCamera);
	}

	public void RotateCameraVertical(float amount)
	{
		if(isDead) return;
		if(isThrowingGranade) return;
        if (ImprovedCameraCntroller.instance.IsPrimaryMovementOn()) return;
		if(ai!=null) {if(ai.IsAIOn()) return;}

        //set drag speed
        amount *= ImprovedCameraCntroller.GetCameraDragFactor();

		Vector3 a = headViewCamera.TransformDirection(Vector3.forward);
		Vector3 b = new Vector3(a.x,cameraSavedForward.y,a.z);

		float verticalAngle = Vector3.Angle(a,b);
		float angleWithUp = Vector3.Angle(a,cameraSavedUp);

		if(angleWithUp>90)
			verticalAngle = -verticalAngle;

	
		//clamp values
		if(amount>0 && verticalAngle <viewRect.maxVertical)
		{
			amount = Mathf.Clamp(amount,0,viewRect.maxVertical-verticalAngle);
		}
		else if (amount<0 && verticalAngle > viewRect.minvertical)
		{
			amount = Mathf.Clamp(amount,viewRect.minvertical-verticalAngle,0);
		}

		if((amount>0 && verticalAngle <viewRect.maxVertical) || (amount<0 && verticalAngle > viewRect.minvertical))
		{
            headViewCamera.transform.Rotate(Vector3.right*(-amount), Space.Self);
            shoulderViewCamera.transform.Rotate(Vector3.right*(-amount), Space.Self);
            if(IsInControll())
                ImprovedCameraCntroller.instance.RequestCameraAimMove(CanRotateCamera,IsAiming()?shoulderViewCamera:headViewCamera);
		}
	}




	public void ZoomInCameraView()
	{
		if(isDead) return;
		//if(assignedWeapon.canZoom == false) return;
        if(ai!=null) if(ai.IsAIOn()) return;

		isCameraZoomedIn = true;
        ImprovedCameraCntroller.instance.ZoomCamera(true,.35f);
	}

	public void ZoomOutCameraView()
	{
		if(isDead) return;
		//if(assignedWeapon.canZoom == false) return;
        if(ai!=null) if(ai!=null) {if(ai.IsAIOn()) return;}

		isCameraZoomedIn = false;
        ImprovedCameraCntroller.instance.ZoomCamera(false,1f);
	}

    public void SwitchToDeathCameraView()
    {
        Debug.Log("i get called");
        Transform temp = new GameObject("DeathCamPos").transform;
        Vector3 newCamPos = (headViewCamera.position - this.transform.position).normalized;
        newCamPos *= 4f;
        temp.position = this.transform.position + newCamPos;
        temp.LookAt(this.transform.position);

        ImprovedCameraCntroller.instance.RequestCameraAimMove(null, null);
        ImprovedCameraCntroller.instance.RequestCameraTransitMove(MovementPriority.High, null, temp.position, temp.rotation, 2f);
    }



	public void SwitchToHeadCameraView(bool ignoreCameraSwitchControl=false)
	{
		//Debug.Log("requesting to hide");
		if(ai!=null) {if(ai.IsAIOn() && !isDead) return;}
		if(isThrowingGranade) return;
		if(!canSwitchBetweenCameras && !ignoreCameraSwitchControl) 
		{
			//can not switch between cameras but still call came?
			Debug.LogError("Why thu fuck did you call that?");
			return;
		}

        /*if(assignedWeapon.canZoom && !assignedWeapon.hasScope) 
		{
			isCameraZoomedIn = false;
            ImprovedCameraCntroller.instance.ZoomCamera(false,1f);
		}*/
        if (!assignedWeapon.hasScope)
            ZoomOutCameraView();


        ImprovedCameraCntroller.instance.RequestCameraTransitMove(MovementPriority.Normal, null, headViewCamera.position, headViewCamera.rotation, 1f);
        ImprovedCameraCntroller.instance.RequestCameraAimMove(CanRotateCamera, headViewCamera);

		thisAnimator.SetBool("aim",false);
	}

    public void SwitchToShoulderCameraView(System.Action<bool> callback=null)
	{
		if(isDead) return;
		if(isThrowingGranade) return;
		if(ai!=null) {if(ai.IsAIOn()) return;}

        /*if(assignedWeapon.canZoom && !assignedWeapon.hasScope) 
		{
			isCameraZoomedIn = true;
			ImprovedCameraCntroller.instance.ZoomCamera(true,.35f);
		}*/
        if (!assignedWeapon.hasScope)
            ZoomInCameraView();


        ImprovedCameraCntroller.instance.RequestCameraTransitMove(MovementPriority.Normal, callback, shoulderViewCamera.position, shoulderViewCamera.rotation, 0.35f);
        ImprovedCameraCntroller.instance.RequestCameraAimMove(CanRotateCamera, shoulderViewCamera);

		thisAnimator.SetBool("aim",true);
	}

	public int GetMyIndex()
	{
		return System.Array.IndexOf(PlayerInputController.instance.players,this);
	}
	    
	public float GetAngleWithAimCamera()
	{
		Vector3 playerFwd = playerModel.transform.forward;
		Vector3 camFwd = shoulderViewCamera.transform.forward;

		camFwd.y = playerFwd.y;
		camFwd.Normalize();

		return Vector3.Angle(camFwd,playerFwd);
	}

	public float GetAngleWithHeadViewcamera()
	{
		Vector3 playerFwd = playerModel.transform.forward;
		Vector3 camFwd = headViewCamera.transform.forward;

		playerFwd.y = camFwd.y;
		playerFwd.Normalize();

		return Vector3.Angle(camFwd,playerFwd);
	}

	private void ReleaseCamera()
	{
		//main_camera.transform.parent = null;
	}

	private void ReparentCamera()
	{
		//main_camera.transform.parent = this.transform;
		SwitchToHeadCameraView();
	}
	                        
}

[System.Serializable]
public struct ViewRect
{
	public float maxHorizontal;
	public float minHorizontal;
	public float maxVertical;
	public float minvertical;

	public ViewRect(float maxH, float minH, float maxV, float minV)
	{
		maxHorizontal = maxH;
		minHorizontal = minH;
		maxVertical = maxV;
		minvertical = minV;
	}

	public static ViewRect Zero()
	{
		return new ViewRect(0,0,0,0);
	}

	public bool IsEqual(ViewRect other)
	{
		if((this.maxHorizontal == other.maxHorizontal) && (this.minHorizontal == other.minHorizontal)
			&& (this.maxVertical == other.maxVertical) && (this.minvertical == other.minvertical))
			return true;
		else
			return false;
	}

	public static bool operator == (ViewRect a, ViewRect b)
	{
		return a.IsEqual(b);
	}

	public static bool operator != (ViewRect a, ViewRect b)
	{
		return !a.IsEqual(b);
	}
}
