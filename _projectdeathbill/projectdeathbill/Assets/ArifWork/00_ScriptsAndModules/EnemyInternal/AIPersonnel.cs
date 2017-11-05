using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SWS;
using RootMotion.FinalIK;

public class AIPersonnel : MonoBehaviour {
    //public string lastDeathCause;

	//public static ShooterPlayerID spID = ShooterPlayerID.SMG;
	public bool enablePersonalLog = false;
    public AIModelManager selfModel;
	public GameObject ragdollModel;
    private Animator _selfAnimator;
    public Animator selfAnimator{
        get
        {
            if(_selfAnimator==null) 
            {
                if (selfModel == null)
                    Debug.LogError("No model reference");
                else
                    _selfAnimator = selfModel.gameObject.GetComponent<Animator>();
            }
            return _selfAnimator;
        }
    }
    public AIAudioScript selfAudioManager;
    public AIPersonnelCanvasController canvasController;
    public ParticleSystem muzzleFlashParticle;
    public AIProfile profile;
    public AIStatus status;
	//public bool isPatrolAI;
	public AISpawner spwnerRef;

	public AIPersonnelBehaviourTypes personalityType;
	public AIMotionStates motionState;
	public AITargetUpdateState targetUpdateState = AITargetUpdateState.DYNAMIC;


	public bool hasRandomOffsetOnLoops;
	//public bool interZoneMovementDisabled;
    //public Transform bellyReference;
    public Transform headReference;
    public EnemyType enemyType;
	internal bool iamWorthLess = false;
    //public Transform personalTarget;

	public PathManager fixedStartPath;
	public Zone fixedStartZone;

    private Transform _targetPlayer;
    public Transform targetPlayer
    {
        set{
            _targetPlayer = value;
            selfModel.SetAimIKTarget(
                _targetPlayer);
        } 
        get{
            return _targetPlayer;
        }
    }

	internal ZoneWalker zwalker;
	splineMove spline;

    //Nade vars
    float nextNadeTime;                     //must init
	const float autoNadeTime = 12;
	const float autoNadeTimeVariancePercentage = 0.45f;
	private const float minimalGlobalNadeTimeClearanceBase = 6.0f;
	private const float minimalGlobalNadeTimeClearanceMaxFall = 6.0f;
	private const float minimalGlobalNadeTimeClearanceFallPerPlayer = 1/3.0f ;
	public float minimalGlobalNadeTimeClearance{
		get{
			float extraNadePpl = -2;
			foreach (AIPersonnel ai in AIDataManager.activeEnemyList) {
				if (ai.enemyType == EnemyType.GRENADIER)
					extraNadePpl++;
			}
			if (extraNadePpl > 0) {
				float mf = minimalGlobalNadeTimeClearanceMaxFall;
				float cf = 0;
				float factor = minimalGlobalNadeTimeClearanceFallPerPlayer;
				for (int i = 0; i < extraNadePpl; i++) {
					cf = (mf - cf) * factor + cf;
					//Debug.Log ("value of fall progression: "+ cf.ToString());
				}
				return minimalGlobalNadeTimeClearanceBase - cf;
			} else {
				return minimalGlobalNadeTimeClearanceBase;
			}
		}
  	}
	const float nadeMinSqrDistance = 10*10;
	public const float nadeOptimumDistance = 18;
    const float nadeMaxSqrDistance = 25*25;
    public const float nadeAccuracyRadius = 4.5f;
    public const float nadeMaxDamage = 50;
    public const float nadeMaxDamageRadius = nadeAccuracyRadius*1.2f;
    //Shell vars
    public const float shellAccuracyRadius = 10f;
    public const float shellMaxDamage = 85;
    public const float shellMaxDamageRadius = shellAccuracyRadius * 1.2f;
    //Shoot vars
    float nextShootTime;                    //must init
    const float autoShootTime = 2.5f;
    const float autoShootTimeVariancePercentage = 0.75f;
    const int roundPerAttemptBase = 4;
    const int roundPerAttemptVariance = 1;
    //repos vars
    float lastDamageBasedRepositionHP;
    float lastReposTime;          
    const float minimalAnyRposInterval = 0.1f;
	const float damageBasedReposInterval_DMG = 70;
	const float damageBasedReposChance = 0.4f;
	const float baseAlertAutoRposTime = 8;
	const float autoRposTimeVariancePercentage = 0.75f;
	float autoRposTime;
    //collision avoidance
	float speedMax;
	float speedStep;
    //damage parameter
    //public const float baseEnemyDamage = 9.5f;
    public const float baseEnemyCurveConstant = 0.026f;
    private const float interFireInterval = 0.2f; //600 rounds per minutes

	//speeds
	private const float runSpeed = 5;
	private const float walkSpeed = 1.5f;

    public WasHitCheck hitWhileShooting;

    Vector3 selfModelLocalPosition = new Vector3();
    Quaternion selfModelLocalRotation = Quaternion.identity;

	void OnDrawGizmos()
	{
		if(status.hasVision)
			Handy.DrawSight (profile.visionAngle,profile.sightRange,20,this.transform, new Color(0.31f,0.30f,1,0.5f));
	}

	public bool isInitialized;
	public void SetToWorthLess()
	{
		if (isInitialized)
			iamWorthLess = true;
		else
			Debug.LogWarning ("not set to worthless, ai not initialized");
	}

	public void Init(Zone startZone, Transform target,float damage, float accuracy, float intel, AIPersonnelBehaviourTypes personality, AIMotionStates motionState, AITargetUpdateState targetUpState, bool hasRandomOffsetOnLoops = true)
	{
		isInitialized = true;
		iamWorthLess = false;
		selfAudioManager.Init (this);
		targetPlayer = target;
		personalityType = personality;
		this.motionState = motionState;
		this.hasRandomOffsetOnLoops = hasRandomOffsetOnLoops;
		this.targetUpdateState = targetUpState;
		alertRepositionPending = false;
		rajakarAlertReason = AlertReason.NULL;
		status.dead = true;
		if (enemyType == EnemyType.RAJAKAR) {
			canvasController.targetIcon.SetActive (false);
		}
        if (canvasController == null) Debug.LogError("Canvas controller script reference missing on AIPersonnel: " + this.transform.name);
        if (selfModel == null)
        {
            Debug.LogError("Model not assigned!");
            Debug.Break();
        }
        selfModel.transform.localPosition = selfModelLocalPosition;
        selfModel.transform.localRotation = selfModelLocalRotation; 

        if (enemyType != EnemyType.MORTAR)
        {
            zwalker = GetComponent<ZoneWalker>();
            if (zwalker == null) Debug.LogError("ZoneWalker script missing on non stationary AIPersonnel: " + this.transform.name);
            zwalker.currentZone = startZone;

			switch (personality) {
			case AIPersonnelBehaviourTypes.REINFORCEMENT:
			case AIPersonnelBehaviourTypes.DROP:
				zwalker.slotIndex = -1;
				break;
			}
            spline = this.GetComponent<splineMove>();
			if (personality != AIPersonnelBehaviourTypes.PATROL) {
				spline.speed = runSpeed;
			} else {
				spline.speed = walkSpeed;
				spline.moveToPath = false;
			}
            speedMax = spline.speed;
            speedStep = spline.speed / 4;
        }
        selfAnimator.SetBool("ISDEAD", false);
        selfModel.ikOn = true;
        FullBodyBipedIK fbbik = selfModel.gameObject.GetComponent<FullBodyBipedIK>();
        if (fbbik != null)
            fbbik.enabled = true;
        AimIK aimik = selfModel.gameObject.GetComponent<AimIK>();
        if (aimik != null)
            aimik.enabled = true;
		if (enemyType != EnemyType.MORTAR) {
			if (personality == AIPersonnelBehaviourTypes.PATROL) 
			{
				selfAnimator.SetTrigger ("CHILL");
				status.SetSpwningChillingWithVision();
			} 
			else 
			{
				status.Reset();
				Reposition(RepositionShowCause.SPWN);
			}
		} else {
			selfAnimator.SetTrigger("MORTARPREP");
			status.Reset();
		}
            

        //profile setup
        //if (!GeneralManager.instance.runningForPromo) 
        canvasController.UpdateHP(1);
        profile.HP = 100;
        profile.armor = 0.1f;
		profile.baseDamage = damage;
        profile.accuracy = accuracy;
		profile.intelligence = intel;
        profile.magazineSize = 20;
        profile.ammo = profile.magazineSize;
           


        //===========================
        hitWhileShooting = null;
        lastDamageBasedRepositionHP = profile.HP;

        //timing
        nextNadeTime = 0;
        if (motionState == AIMotionStates.STATIONARY)
            nextShootTime = Time.time;
        else
        nextShootTime = Time.time + autoShootTime*(1 + Random.Range(-autoShootTimeVariancePercentage, autoShootTimeVariancePercentage));
		if (status.unAlert) {
			lastReposTime = Time.time;
			ZonePatrolParameters zpp = zwalker.currentZone.GetComponent<ZonePatrolParameters> ();
			if (zpp != null) {
				autoRposTime = zpp.zoneWaitTime;
			} else {
				autoRposTime = Random.Range (0, profile.baseUnalertAutoRposTime);
			}
		} else {

			lastReposTime = Time.time;
			autoRposTime = Handy.Deviate (baseAlertAutoRposTime, autoRposTimeVariancePercentage);
		}
    }

	public bool alertRepositionPending;
	private AlertReason rajAlReas;
	public AlertReason rajakarAlertReason {
		set {
			rajAlReas = value;
			if (rajAlReas != AlertReason.NULL)
			{
				//Arko
				if(enemyType == EnemyType.RAJAKAR)
					SneakyPlayerManager.instance.OnGameOver();
				else
					GeneralManager.EndGame (false);
			}
		}
		get
		{
			return rajAlReas;
		}
	}
	public enum AlertReason {NULL,SEEN,HEARD,TOUCHED}
	public void Alert(bool panic, AlertReason reason)
	{
		if (status.dead) {
			//Debug.Log("Dead man cant be alert!");
			return;
		}
		
		if (status.unAlert) {
			if(enemyType==EnemyType.RAJAKAR)
				rajakarAlertReason = reason;
			if (panic) {
				nextShootTime = Time.time;
				status.panicking = true;
			}
			if (status.moving) {
				for (int i = 0; i < spline.waypoints.Length; i++)
				{
					spline.events[i].RemoveAllListeners();
				}
				spline.Stop();
				Pool.Destroy(spline.pathContainer.gameObject);
				status.Reset ();
			}

			status.unAlert = false;
			alertRepositionPending = true;
			spline.speed = runSpeed;
			if (status.moving) {
				spline.Stop ();
				selfAnimator.SetTrigger("ENDRUN");
				status.moving = false;
			}
			if (targetPlayer == null)
				Debug.LogError ("No target player set on alert event!");
			TurnToTarget(targetPlayer,()=>{
				Reposition(RepositionShowCause.ALERT);
			});
		}

	}

    void FixedUpdate()
    {

		if (enemyType != EnemyType.MORTAR)
		{
			if (Time.time - lastReposTime > autoRposTime )
				Reposition (RepositionShowCause.BOREDOM);
			if (Time.time > nextShootTime )
				Shoot (roundPerAttemptBase + Random.Range (-roundPerAttemptVariance, roundPerAttemptVariance + 1));
			
		}
		if (enemyType == EnemyType.GRENADIER) {
			if (Time.time > nextNadeTime && Time.time>=AIDataManager.globalNadeThrowCallIgnoreTime)
				ThrowNade ();
		}
		if (!status.unAlert && alertRepositionPending) 
		{
			Reposition (RepositionShowCause.ALERT);
		}
		if (status.hasVision) 
		{
			Stare ();
		}

        /*if (Physics.Raycast(transform.position, transform.forward, 0.5f, LayerMask.GetMask("RaycastAI"))) 
		{
			speed = spline.speed - speedStep;
			speed = Mathf.Clamp(speed,0,speedMax);
			spline.ChangeSpeed(speed);
			Debug.Log(spline.speed);
		}
		else
		{
			speed = spline.speed + speedStep;
			speed = Mathf.Clamp(speed,0,speedMax);
			spline.ChangeSpeed(speed);
		}
        */
    }
	#region Vision

	void Stare()
	{
		Transform target = targetPlayer;
		if (target == null) {
			//Debug.Log ("null target");
			return;
		}
		if( Vector3.SqrMagnitude(target.position - headReference.transform.position)< profile.sightRange*profile.sightRange)
		{
			RaycastHit rchit;
			Debug.DrawRay(headReference.position, (target.position- headReference.position).normalized*profile.sightRange  ,Color.blue);
			if(Physics.Raycast (headReference.position, target.position- headReference.position,out rchit))
			{
				Debug.DrawLine (headReference.position, rchit.point,Color.magenta);
				if (rchit.collider.CompareTag ("SNEAKY")) 
				{
					Vector3 fwd = this.transform.forward;
					fwd.y = 0;
					Vector3 dir = target.position - this.transform.position;
					dir.y = 0;
					if (Vector3.Angle (fwd, dir) < profile.visionAngle / 2) 
					{
						Debug.LogWarning ("The enemy saw you");
						Alert (panic: true, reason: AlertReason.SEEN);
						SneakyPlayerManager.instance.OnGameOver();
					}
				}
			}

			//RaycastHit rchit = Physics.Raycast (headReference.position,);
		}
	}
	#endregion

    #region Shoot and Reload
    public void Shoot(int rounds)
    {
		
        if (status.CantAttack()) { return; }
		//Debug.Log ("Shoot time: "+ Time.time.ToString ());
		status.shooting =true;
        hitWhileShooting = new WasHitCheck();
        StartCoroutine( FireRound(rounds));
    }

    IEnumerator FireRound(int rounds)
    {
        for (int i = 0; i < rounds; i++)
        {
            if (hitWhileShooting.wasHit)
            {
                hitWhileShooting.wasHit = false;
                break;
            }
            if (profile.ammo <= 0)
            {
                Reload();
                break;
            }
            selfAnimator.SetTrigger("SHOOT");
            selfModel.FireUsingIKRecoil();
            nextShootTime = Time.time + autoShootTime * (1 + Random.Range(-autoShootTimeVariancePercentage, autoShootTimeVariancePercentage));
            profile.ammo--;
            if (profile.ammo == 0)
            {
                Reload();
                break;
            }
            yield return new WaitForSeconds(interFireInterval);
        }
        DoneShooting();
    }
    void DoneShooting()
    {
        status.shooting =false;
        hitWhileShooting = null;
    }
    void Reload()
    {
        if (status.dead) { Debug.Log("reload failed"); return; }
        status.reloading = true;
        selfAnimator.SetTrigger("RELOAD");
    }
    public void ReloadCallback(bool success)
    {
        if(success)
            profile.ammo = profile.magazineSize;
        status.reloading = false;

    }
    //internal Vector3 tempNadeTargetPosition;
    void ThrowNade()
    {
		//Debug.Log ("A");
		if (targetPlayer == null)
			return;
		//Debug.Log ("B");
        float sqrDistance = (this.transform.position - targetPlayer.position).sqrMagnitude;
        if (sqrDistance < nadeMinSqrDistance || sqrDistance > nadeMaxSqrDistance) return;

        if (status.CantAttack()) return;
		AIDataManager.globalNadeThrowCallIgnoreTime = Time.time + minimalGlobalNadeTimeClearance;
        status.throwingNade = true;
        nextNadeTime = Time.time + autoNadeTime * (1 + Random.Range(-autoNadeTimeVariancePercentage, autoNadeTimeVariancePercentage));
        selfAnimator.SetTrigger("NADE");
    }
    public void ThrowNadeCallBack()
    {
        status.throwingNade = false;
    }
    #endregion
    #region Reposition
    public void Reposition(RepositionShowCause RShowCause)
	{
		if(status.CantMove()) return;
        if (Time.time - lastReposTime <= minimalAnyRposInterval && RShowCause == RepositionShowCause.BOREDOM) return;
		if(zwalker==null) zwalker = GetComponent<ZoneWalker>();



		ZoneConnection zc =null;
//		if (!status.unAlert) {
//			zc = SelectRepositionZone (zwalker.currentZone, profile.intelligence, spID, true);
//		}
		bool usingZoneBlocks = false;
		if(!status.unAlert && personalityType != AIPersonnelBehaviourTypes.ROOFIE) usingZoneBlocks = true;

		if (enablePersonalLog)
			Debug.Log ("reposcalled with motion state: " + motionState.ToString());
		switch (motionState) {
		case AIMotionStates.NORESTRICTION:
			zc = SelectRepositionZone (zwalker.currentZone, this, usingZoneBlocks: usingZoneBlocks);
			break;
		case AIMotionStates.SINGLEZONE:
			zc = null;
			break;
		case AIMotionStates.INTERZONEONLY:
			zc = SelectRepositionZone (zwalker.currentZone, this, usingZoneBlocks: usingZoneBlocks);
			if (zc == null)
				return;
			break;
		case AIMotionStates.STATIONARY:
			return;
			break;
		}
		if (enablePersonalLog)
			Debug.Log (zc);
		status.moving = true;
		selfAudioManager.PlayRunningSound();

		//if()
		if (alertRepositionPending) {
			if (zc == null) {
				RepositionAborted ();
				return;
			} else {
				alertRepositionPending = false;
                nextShootTime = Time.time;
			}
		} 
		zwalker.abortedMove += ()=>{RepositionAborted(); zwalker.abortedMove = null;};
		zwalker.finishedMove += ()=>{
			if(zc == null)
				RepositionFinished(zwalker.currentZone);
			else
				RepositionFinished(zc.endZone);
			zwalker.finishedMove = null;
		};
		zwalker.startedMove += ()=>{RepositionStarted(); zwalker.startedMove = null;};

		zwalker.GoToZone(zc);
		if (enablePersonalLog)
			Debug.Log ("repos func ended");
	}
	/*public void Reposition(PathManager splinePath, Zone engagingZone)
	{
		Debug.Log ("R2");
		if (status.CantMove()) return;
		Handy.LogOnce ("consider using has made spwn move");
		status.moving = true;
		selfAudioManager.PlayRunningSound();
		autoRposTime = baseAutoRposTime * (1 + Random.Range(-autoRposTimeVariancePercentage, autoRposTimeVariancePercentage));
		if(zwalker==null) zwalker = GetComponent<ZoneWalker>();
		zwalker.abortedMove += ()=>{RepositionAborted(); zwalker.abortedMove = null;};
		zwalker.finishedMove += ()=>{RepositionFinished(); zwalker.finishedMove = null;};
		zwalker.startedMove += ()=>{RepositionStarted(); zwalker.startedMove = null;};

		zwalker.GoToZoneUsingPath(splinePath, engagingZone);
	}
	*/
	void RepositionStarted()
	{
		//isPatrolAI = false;
		if(!status.unAlert)
			selfAnimator.SetTrigger("RUN");
		else
			selfAnimator.SetTrigger("WALK");
	}
	void RepositionFinished(Zone reachedZone)
	{
		if (status.dead)
		{
			status.moving = false;
		}
		else
		{
			//Debug.Log (selfAudioManager);
			if(targetUpdateState == AITargetUpdateState.ZONAL)
				targetPlayer = AIDataManager.GetZonalAITarget(reachedZone);
			selfAudioManager.StopRunningSound();
			selfAnimator.SetTrigger("ENDRUN");

			if (status.unAlert) {
				ZonePatrolParameters zpp = zwalker.currentZone.GetComponent<ZonePatrolParameters> (); 
				if (zpp != null) {
					TurnToRotation (zwalker.currentZone.slots [0].rotation, () => {
						autoRposTime = zpp.zoneWaitTime;
						lastReposTime = Time.time;
						status.moving = false;	
					});
				}
				else
				{
					autoRposTime = profile.baseUnalertAutoRposTime;
					lastReposTime = Time.time;
					status.moving = false;	
				}
			} 
			else {
				TurnToTarget (targetPlayer, () => {
					autoRposTime = Handy.Deviate (baseAlertAutoRposTime, autoRposTimeVariancePercentage);
					lastReposTime = Time.time;
					status.moving = false;
				});
			}
		}
	}
	void RepositionAborted()
	{
		status.moving = false;
	}

	void TurnToTarget(Transform lookTarget,System.Action callback){
		Vector3 targetRotVec3 = this.transform.rotation.eulerAngles;
		targetRotVec3.y = Mathf.Rad2Deg * Mathf.Atan2((lookTarget.position.x - transform.position.x), (lookTarget.position.z - transform.position.z));
		Quaternion targetRotation = Quaternion.Euler(targetRotVec3);
		TurnToRotation (targetRotation,callback);
	}
	void TurnToRotation(Quaternion targetRotation,System.Action callback){
		StartCoroutine (TurnGraduallyToRotation (targetRotation,callback));
	}

	IEnumerator TurnGraduallyToRotation(Quaternion targetRotation,System.Action callback)
	{
		status.turning_Test = true;
		float savedTime = Time.time;
		while (Time.time - savedTime < 0.4f)
		{
			this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, 0.1f);
			yield return null;
		}
		this.transform.rotation = targetRotation;
		status.turning_Test = false;
		if (callback != null)
			callback ();
	}


	/*IEnumerator TurnToPlayerY(Transform lookTarget,System.Action callback)
    {
		if (!status.unAlert) {
			//status.turning_Test = true;
			Vector3 targetRotVec3 = this.transform.rotation.eulerAngles;
			targetRotVec3.y = Mathf.Rad2Deg * Mathf.Atan2((lookTarget.position.x - transform.position.x), (lookTarget.position.z - transform.position.z));
			Quaternion targetRotation = Quaternion.Euler(targetRotVec3);
			float savedTime = Time.time;
			while (Time.time - savedTime < 0.4f)
			{
				this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, 0.1f);
				yield return null;
			}
			this.transform.rotation = targetRotation;
			//status.turning_Test = false;
		}
		if (callback != null)
			callback ();
    }*/
    #region ZoneSelection function
    public const float ignoreAdvantageAgainstThreshold = 0.05f;
    public const float sameZoneMovementPercentage = 50;
	public static ZoneConnection SelectRepositionZone(Zone currentZone, AIPersonnel ai, bool usingZoneBlocks)
    {
		if (usingZoneBlocks) {
			//try{
				AIManagerModule aiman =  (ai.spwnerRef as AIManagerModule);
				if ((Random.Range(0.0f, 100.0f) <= sameZoneMovementPercentage) && !currentZone.spwnOnly) 
				{ 
					return null; 
				}
//			}
//			catch {
//				Debug.Log ("no active zone");
//				return null;
//			}

		}

        ZoneConnection chosenZoneConnection = null;

        List<ZoneConnection> zclist = new List<ZoneConnection>();
        #region eliminate by capacity and zero exposure
        for (int i = 0; i < currentZone.zoneConnections.Count; i++)
        {
			if (currentZone.zoneConnections[i].GetWeightedAdvantageValue() <= ignoreAdvantageAgainstThreshold) { continue; }

            Zone zoneRef = currentZone.zoneConnections[i].endZone;
			if (zoneRef.maxCapacity > zoneRef.filledSlotIndexes.Count)
            {

                zclist.Add(currentZone.zoneConnections[i]);
            }
        }
        #endregion

        float prevAcceptanceFactor = 0;
		float intelligence = Mathf.Clamp(ai.profile.intelligence, 0.0f, 1.0f);
        float weightFactor = 0;
        float advantageFactor = 0;
        float intelFactor = 0;
        float deterministicFactor = 0;
        float randomFactor = 0;
        float populationDensityFactor = 0;
        float thisAcceptanceFactor = 0;
        for (int i = 0; i < zclist.Count; i++)
        {
			weightFactor = zclist[i].endZone.zoneWeight;
			advantageFactor = zclist[i].GetWeightedAdvantageValue();
            intelFactor = intelligence;
            deterministicFactor = intelFactor * advantageFactor + (1 - intelFactor) * weightFactor;
            randomFactor = Random.Range(0.0f, 1.01f);
			populationDensityFactor = zclist[i].endZone.bookedSlotCount / zclist[i].endZone.maxCapacity;
            thisAcceptanceFactor = deterministicFactor + (1 - populationDensityFactor) * randomFactor * randomFactor;
            if (thisAcceptanceFactor > prevAcceptanceFactor)
            {
                chosenZoneConnection = zclist[i];
                prevAcceptanceFactor = thisAcceptanceFactor;
                //Debug.Log(zclist[i].ToString() + "was accepted when value" + thisAcceptanceFactor.ToString() );
            }
        }
        if (chosenZoneConnection != null)
        {
            chosenZoneConnection.endZone.bookedSlotCount++;
            currentZone.bookedSlotCount--;
        }
        return chosenZoneConnection;
    }


    #endregion
    #endregion
    #region Damage And Death


    public void TakeDamage(float dmg, HitType hitType, HitSource hitSource)
    {
        if (hitType != HitType.KNIFE)
            InGameSoundManagerScript.PlayOnPointFromID(transform.position, ClipID.bulletHitFlesh);
        if (status.CantTakeDamage()) return;

		Alert (panic: false, reason: AlertReason.NULL);
        LowerHPDueToDamage(dmg);
        
        if (status.shooting) hitWhileShooting.wasHit = true;
        if (status.reloading) ReloadCallback(false);
        if (status.throwingNade)
        {
            ThrowNadeCallBack();
            canvasController.TurnOffNadeAlert();
            if (selfModel.hasUnpinnedNade)
            {
                Debug.Log("with nade unppined");
                selfModel.hasUnpinnedNade = false;
                ProjectileScript.PreMatureDetonation(selfModel.grenadePrefab, selfModel.nadeReleasePoint.position, HitSource.ENEMY, nadeMaxDamage,nadeMaxDamageRadius);
            }
        }
		if (hitSource == HitSource.SNEAKY_PLAYER ) {
			if(!iamWorthLess) GeneralManager.instance.AddKP (HitType.KNIFE);
			Obliterate ();
		}
		else if (hitSource == HitSource.GAYEBI ) {
			Obliterate ();
		}

        if (profile.HP <= 0)
        {
            Die(hitType, hitSource);
        }
        else
        {
            Suffer(hitType);
            //Debug.Log("got hit");
        }


    }

    public float DamageFunction(float damage)
    {
        return (damage * (1 - (profile.armor / 2)));
    }

    public void LowerHPDueToDamage(float damage)
    {
        profile.HP -= DamageFunction(damage);
        //if (!GeneralManager.instance.runningForPromo) 
            canvasController.UpdateHP(profile.HP / 100);
    }
    public void Suffer(HitType hitType)
    {
        if (status.CantSuffer() || enemyType == EnemyType.MORTAR) return;
        if (lastDamageBasedRepositionHP - profile.HP > damageBasedReposInterval_DMG)
        {
            if (Random.Range(0, 1.0f) > damageBasedReposChance)
            {
                lastDamageBasedRepositionHP = profile.HP;
                Reposition(RepositionShowCause.PAIN);
            }

        }
        else
        {
            status.suffering = true;
            switch (hitType)
			{ 
			case HitType.HEAD:
				selfAnimator.SetTrigger("TAKEHIT");
				selfAudioManager.PlayHeadShotHitSound();
				break;
			default:
				selfAnimator.SetTrigger("TAKEHIT");
				selfAudioManager.PlayNormalHitSound();
				break;
               
            }
        }
    }
	public void Obliterate()
	{
		if (!status.dead) {
			Funeral ();
			StartCoroutine (RemoveGoreTimed(0));
		}
	}
    public void Die(HitType hitType, HitSource hitSource)
    {
        if (!status.dead)
        {
			if (hitSource == HitSource.PLAYER)
			{
				if(!iamWorthLess)
					GeneralManager.instance.AddKP(hitType);
			}

            Funeral();
            switch (hitType)
            {
			case HitType.HEAD:
				selfAudioManager.PlayHeadShotDeathSound();
				selfAnimator.SetTrigger("DEATHST1");
				break;
			case HitType.KNIFE:
				selfAnimator.SetTrigger("DEATHST2");
				break;
			default:
				selfAudioManager.PlayNormalDeathSound();
				selfAnimator.SetTrigger("DEATHST2");
				break;
            }

        }

    }
//	public void DieOfKnife()
//	{
//		if (!status.dead) {
//			LowerHPDueToDamage (9999999);
//			selfAnimator.enabled = false;
//			StartCoroutine(DestroyAfterRagdollDeath(5));
//			Funeral();
//		}
//	}
    public void DieOfGrenade(Vector3 blastForce, HitSource hitSource, bool failNade)
    {
        if (!status.dead)
        {
            selfModel.ikOn = false;
            //Arko Code
            FullBodyBipedIK fbbik = selfModel.gameObject.GetComponent<FullBodyBipedIK>();
            if (fbbik != null)
                fbbik.enabled = false;
            AimIK aimik = selfModel.gameObject.GetComponent<AimIK>();
            if (aimik != null)
                aimik.enabled = false;
            
            LowerHPDueToDamage(9999999);
            if(hitSource == HitSource.PLAYER)
			if(!iamWorthLess)GeneralManager.instance.AddKP(HitType.BLAST);
            else if(hitSource == HitSource.ENEMY && failNade)
			if(!iamWorthLess) GeneralManager.instance.AddKP(HitType.BLAST);
//            selfAnimator.enabled = false;
//            if (selfModel.ragdollForceObject != null)
//                selfModel.ragdollForceObject.AddForce(blastForce, ForceMode.Impulse);
//            else
//                Debug.Log("Rgidbody not found!");
//			GeneralManager.instance.StartCoroutine(RemoveGoreTimed(5));
			AITerrorRagdoll aiTR =  Pool.Instantiate(ragdollModel,this.transform.position,this.transform.rotation).GetComponent<AITerrorRagdoll>();
			aiTR.lifeTime = 5;
			aiTR.force = blastForce;
			//Debug.Log("Blasted with force: " + blastForce.ToString());
            Funeral();
			//==========================================
			if (enemyType != EnemyType.MORTAR)
			{
				Pool.Destroy(this.gameObject);
			}

			else
				Destroy(this.gameObject);
        }

    }
    public void Funeral()
    {
        //if (status.turning_Test) Debug.Log("died while turning");
        status.dead = true;
		isInitialized = false;
		iamWorthLess = false;
		StopAllCoroutines ();
		selfModel.StopAllCoroutines ();
		if (enemyType == EnemyType.RAJAKAR) {
			canvasController.targetIcon.SetActive (false);
		}
		else
			selfModel.ResetLineRenderer ();
        if (status.moving)
        {
            for (int i = 0; i < spline.waypoints.Length; i++)
            {
                spline.events[i].RemoveAllListeners();
            }
            spline.Stop();
            Pool.Destroy(spline.pathContainer.gameObject);
        }
        if (enemyType != EnemyType.MORTAR)
        {
			zwalker.ClearInfoFromZone ();
        }
		if(enablePersonalLog) Debug.Log ("an enemy is dying");

		switch (personalityType) {
		case AIPersonnelBehaviourTypes.REINFORCEMENT:
			(spwnerRef as AIManagerModule).RemoveReinforcementInfo(this);
			break;
		case AIPersonnelBehaviourTypes.PATROL:
			(spwnerRef as AIPatrolModule).RemovePatrolInfo (this);
			break;
		case AIPersonnelBehaviourTypes.ROOFIE:
			(spwnerRef as AIRoofieModule).RemoveRoofieInfo (this);
			break;
		case AIPersonnelBehaviourTypes.DROP:
			(spwnerRef as AIGeneratorModule).RemoveDropInfo (this);
			break;
		}
	}
    IEnumerator RemoveGoreTimed(float time)
    {
		//Debug.Log ("B");
        yield return new WaitForSeconds(time);
		//Debug.Log ("C");
        selfAnimator.enabled = true;
        if (enemyType != EnemyType.MORTAR)
        {
            Pool.Destroy(this.gameObject);
        }
            
        else
            Destroy(this.gameObject);
    }

    #endregion



}
#region supporting classes and enums
[System.Serializable]
public class AIProfile
{
	[Range(0,100.0f)]
	public float HP;
	[Range(0,1.0f)]
	public float armor;
	[Range(0,1.0f)]
	public float baseDamage;
    [Range(0,1.0f)]
    public float accuracy;
    [Range(0,1.0f)]
    public float intelligence;
    [Range(0,100)]
    public int ammo;
    [Range(1,100)]
    public int magazineSize;

	[Range(0.2f,12f)]
	public float sightRange;
	[Range(0f,120f)]
	public float visionAngle;
	public float baseUnalertAutoRposTime = 2;
}
[System.Serializable]
public class AIStatus
{
	public bool unAlert;
	public bool panicking;
	public bool hasVision;

    public bool invincible;
    public bool dead;
    public bool moving;
    public bool suffering;
    public bool shooting;
    public bool reloading;
    public bool throwingNade;
    public bool turning_Test;

    public void Reset()
    {
		turning_Test = false;
		panicking = false;
		hasVision = false;
		unAlert = false;
        invincible = false;
        dead = false;
        moving = false;
        suffering = false;
        shooting = false;
        reloading = false;
        throwingNade = false;
    }
    public void SetSpwningInvincible() { Reset(); invincible = true; }
	public void SetSpwningChilling() { Reset(); unAlert = true; }
	public void SetSpwningChillingWithVision() { Reset(); unAlert = true; hasVision = true; }
    //check permissions
	public bool CantAttack() { return (invincible || dead || moving || suffering || shooting || reloading || throwingNade || unAlert); }
	public bool CantMove() { return (invincible || dead || moving || suffering || shooting || reloading || throwingNade || panicking); }
    public bool CantTakeDamage() { return (invincible || dead); }
    public bool CantSuffer() { return (invincible || dead || moving || suffering); }
	//public bool CantAdjustRotation() { return (dead || unAlert || moving || suffering || reloading || shooting || throwingNade);}
}

public enum AIPersonnelBehaviourTypes
{
	PATROL,
	REINFORCEMENT,
	ROOFIE,
	DROP
}
public enum AIMotionStates
{
	STATIONARY,
	SINGLEZONE,
	INTERZONEONLY,
	NORESTRICTION
}
public enum AITargetUpdateState
{
	STATIC,
	DYNAMIC,
	ZONAL,
	ASNEEDED
}
public interface AISpawner 
{
	int selfSpwnCount { get;}
}
//public enum AISpwnRotationStates
//{
//	RANDOM,
//	ZONESLOT,
//	TARGET
//}
//public enum AIRepositionEndRotationStates
//{
//	ZONESLOT,
//	TARGET
//}

#endregion

