#define TEST
using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class AIModelManager : MonoBehaviour {
    [SerializeField][HideInInspector]public bool ikOn = false;
    //public Transform target;
    public AIPersonnel personnelScript;
    public Rigidbody ragdollForceObject;
    public LineRenderer lineRenderer;

    //gun hierarchy
    public Transform nadeReleasePoint;
    public Transform GunPositionRightHand;
    public Transform GunPositionLeftHand;
    public Transform Gun;

	public Transform shootGunPos;
	public Transform walkGunPos;
	public Transform chillGunPos;

	//for rajakar only
	public Renderer rajakarLungi;

	private AIStatus stat;

    //properties:===============================================
    private Animator _selfAnim;
    private Animator selfAnim
    {
        set{_selfAnim = value;} 
        get{
            if (_selfAnim == null)
                _selfAnim = this.GetComponent<Animator>();
            return _selfAnim;
        }
    }
    private AimIK _selfAimIK;
   
    private AimIK selfAimIK{ 
        set{_selfAimIK = value;}
        get{ 
            if (_selfAimIK == null)
                _selfAimIK = this.GetComponent<AimIK>();
            return _selfAimIK;
        }
    }
    //===========================================================


	void LateUpdate()
	{
		if (lateDischargeFlag) {
			DischargeBulletLate ();
		}
		if(personnelScript.enemyType!= EnemyType.RAJAKAR) return;

		if (stat == null)
			stat = this.personnelScript.status;
		if (!stat.dead) {
		
			if (stat.unAlert) {
				if(!stat.moving || stat.turning_Test)
				{
					SetChillGunPos ();
				}
				if (stat.moving && !stat.turning_Test) {
					SetWalkGunPos ();
				}
			} else {
				//Debug.LogError ("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
				SetShootGunPos ();
			}
		}

	}

    public void SetAimIKTarget(Transform target)
    {
        if (selfAimIK == null)
            selfAimIK = this.GetComponent<AimIK>();
        if (selfAimIK == null)
            return;
        selfAimIK.solver.target = target;
    }
	public void SetWalkGunPos()
	{
		Gun.SetParent (null);
		Gun.SetParent (walkGunPos);
		Gun.localPosition = Vector3.zero ;
		Gun.localRotation = Quaternion.identity;
	}
	public void SetChillGunPos()
	{
		Gun.SetParent (null);
		Gun.SetParent (chillGunPos);
		Gun.localPosition = Vector3.zero ;
		Gun.localRotation = Quaternion.identity;
//		Gun.SetParent (chillGunPos.parent);
//		Gun.localPosition = chillGunPos.localPosition;
//		Gun.localRotation = chillGunPos.localRotation;
	}
	public void SetShootGunPos()
	{
		Gun.SetParent (null);
		Gun.SetParent (shootGunPos);
		Gun.localPosition = Vector3.zero ;
		Gun.localRotation = Quaternion.identity;
//		Gun.SetParent (shootGunPos.parent);
//		Gun.localPosition = shootGunPos.localPosition;
//		Gun.localRotation = shootGunPos.localRotation;
	}
    //public Vector3 correction;
    private Vector3 basePoint;
    private Vector3 targetPoint;
    private Vector3 direction;
    private Vector3 modifiedPoint;

    private Vector3 up;
    private Vector3 right;


    private bool wasSet = false;
    Vector3 pos = Vector3.zero;
    Quaternion rot = Quaternion.identity;
    void Awake()
    {
        if (transform.parent.GetComponent<AIPersonnel>() == null) Debug.LogError("No ai personnel with ai  model script");
		if(personnelScript.enemyType == EnemyType.MORTAR)
		{
			if (wasSet)
			{
				transform.localPosition = pos;
				transform.localRotation = rot;
			}
			else
			{
				wasSet = true;
				pos = transform.localPosition;
				rot = transform.localRotation;
			}
		}
    }

    void OnEnable()
    {
        hasUnpinnedNade = false;
    }

	private AimIK aimik;
	private static Transform tempTargTrans;
	void FixedUpdate()
	{
		if ((personnelScript.enemyType == EnemyType.RAJAKAR) && (personnelScript.targetPlayer != null && selfAnim != null)) {
			if (aimik == null)
				aimik = this.GetComponent<AimIK> ();
			if (ikOn) { 
				aimik.solver.IKPositionWeight = 1f;
				Vector3 distVec3 = personnelScript.targetPlayer.position - this.transform.position;
				Vector2 distVec2 = new Vector2 (distVec3.x,distVec3.z);

				if (Vector2.SqrMagnitude (distVec2) > 4) {
					aimik.solver.target = personnelScript.targetPlayer;
				} else {
					if (tempTargTrans == null)
						tempTargTrans = (new GameObject ()).transform;
					distVec2 = distVec2 * 2 / distVec2.magnitude;
					distVec3 = new Vector3 (distVec2.x, distVec3.y, distVec2.y);
					tempTargTrans.position = this.transform.position + distVec3;
					aimik.solver.target = tempTargTrans;
				}
			} else {
			
				aimik.solver.IKPositionWeight = 0f;
			}
		}
	}

    [SerializeField][HideInInspector]public float xCorrection=0;
    [SerializeField][HideInInspector]public float yCorrection=0;
	void OnAnimatorIK () {

        if (selfAimIK != null)
        {
            if (ikOn)
                selfAimIK.solver.IKPositionWeight = 1.0f;
            else
                selfAimIK.solver.IKPositionWeight = 0.0f;
        }
        else
        {
            if (personnelScript.targetPlayer != null && selfAnim !=null && ikOn) {

                basePoint = this.transform.position;
                targetPoint = personnelScript.targetPlayer.position;
                direction = targetPoint - basePoint;
                up = Vector3.Cross(direction, Vector3.right);
                right = Vector3.Cross(direction, Vector3.up);
                if(personnelScript.enemyType == EnemyType.RAJAKAR)
                {
                    direction = Quaternion.AngleAxis(xCorrection, right) * direction;
                    direction = Quaternion.AngleAxis(yCorrection, up) * direction;
                }



                modifiedPoint = basePoint + direction;



                selfAnim.SetLookAtWeight(1,1,1,1,0);
                selfAnim.SetLookAtPosition(modifiedPoint);
            }
        }
	}
    Vector3 bulletstartpoint;
    Vector3 bulletendpoint;
    public const float bulletSpeed = 2.5f;
    public const float trailSpeed = 2f;


    private Recoil aiIKRecoil;
    public void FireUsingIKRecoil()
    {
        if (aiIKRecoil == null)
            aiIKRecoil = this.GetComponent<Recoil>();
        if (aiIKRecoil == null)
            return;
        aiIKRecoil.Fire(1.0f);
        DischargeBullet();
    }

	public void DischargeBullet()
	{
		lateDischargeFlag = true;

	}
	bool ldFlag;
	bool lateDischargeFlag{
		get{ 
			if (ldFlag) 
			{
				ldFlag = false;
				return true;
			}
			else 
			{
				ldFlag = false;
				return false;
			}
		}
		set{ldFlag = value; }
	}
	public void DischargeBulletLate()
	{
		InGameSoundManagerScript.PlayOnPointFromID (lineRenderer.transform.position, ClipID.gunFire_AI_rifleAK47);
		if (personnelScript.hitWhileShooting == null)
		{
			//Debug.Log("hit capturer cleared.");
			return;
		}
		else if (personnelScript.hitWhileShooting.wasHit)
		{
			//Debug.Log("canceled shot due to being hit");
			return;
		}
		if (lineRenderer == null) { Debug.LogError("linerenderer missing!!"); }
		else {
			personnelScript.muzzleFlashParticle.Play();
			direction = personnelScript.targetPlayer.position - lineRenderer.transform.position;
			up = Vector3.Cross(direction, Vector3.right);
			right = Vector3.Cross(direction, Vector3.up);
			float maxdeflection = 5*(1-personnelScript.profile.accuracy);

			direction = Quaternion.AngleAxis(Random.Range(-maxdeflection,maxdeflection), right) * direction;
			direction = Quaternion.AngleAxis(Random.Range(-maxdeflection,maxdeflection), up) * direction;
			RaycastHit hit;

			if (Physics.Raycast(lineRenderer.transform.position, direction, out hit, 100.0F))
			{
				StartCoroutine(BulletRenderer(hit.point, hit, true));

			}
			else
			{
				StartCoroutine(BulletRenderer(direction.normalized * 100 + lineRenderer.transform.position, hit, false));
			}

		}
	}
   
    Vector2 hitPoint2d = new Vector2();
    Vector2 shootPoint2d = new Vector2();
    Vector2 shotDir2d = new Vector2();
    Vector2 forward2d = new Vector2();
    Vector2 left2d = new Vector2();
    Vector2 right2d = new Vector2();
	public void ResetLineRenderer()
	{
		if (personnelScript.enemyType == EnemyType.MORTAR)
			return;
		lineRenderer.SetPosition(0, lineRenderer.transform.position);
		lineRenderer.SetPosition(1, lineRenderer.transform.position);
	}
    IEnumerator BulletRenderer(Vector3 endpoint, RaycastHit hit, bool didHit)
    {
        Vector3 startPoint = lineRenderer.transform.position;
        Vector3 bulletPoint = startPoint;
        Vector3 trailPoint = startPoint;
        Vector3 directionNorm = (endpoint - startPoint).normalized;
        float startTime = Time.time;
        while ((trailPoint - startPoint).sqrMagnitude < (endpoint-startPoint).sqrMagnitude)
        {
            trailPoint += directionNorm * trailSpeed;
            if(!((trailPoint - startPoint).sqrMagnitude < (endpoint - startPoint).sqrMagnitude))
                break;
            
            bulletPoint += directionNorm * bulletSpeed;
            if (!((bulletPoint - startPoint).sqrMagnitude < (endpoint - startPoint).sqrMagnitude))
            {
                bulletPoint = endpoint;
            }
            lineRenderer.SetPosition(0, trailPoint);
            lineRenderer.SetPosition(1, bulletPoint);
            yield return null;
        }

		ResetLineRenderer ();
        if (didHit)
        {
            iBulletImpact iBullIm = hit.transform.GetComponent<iBulletImpact>();
            if (iBullIm != null)
            {
				float damage = personnelScript.profile.baseDamage * Mathf.Exp(-AIPersonnel.baseEnemyCurveConstant * hit.distance);
				ThirdPersonController.aPlayerWasHitLRINDICATORVERSION = false;
				iBullIm.TakeImapct(hit, damage , HitSource.ENEMY);

				if (ThirdPersonController.aPlayerWasHitLRINDICATORVERSION && !personnelScript.iamWorthLess) {
					hitPoint2d.x = HUDManager.instance.camTrans.position.x;
					hitPoint2d.y = HUDManager.instance.camTrans.position.z;
					shootPoint2d.x = transform.position.x;
					shootPoint2d.y = transform.position.z;
					shotDir2d = shootPoint2d - hitPoint2d;
					forward2d.x = HUDManager.instance.camTrans.forward.x;
					forward2d.y = HUDManager.instance.camTrans.forward.z;
					right2d.x = HUDManager.instance.camTrans.right.x;
					right2d.y = HUDManager.instance.camTrans.right.z;
					left2d = -right2d;

					float angleWithFwd = Vector2.Angle (forward2d, shotDir2d);
					if ((angleWithFwd> 1.02f*30.0f*Screen.width/((float) Screen.height))  && !(angleWithFwd> 100.0f*Screen.width/((float) Screen.height)))
					{
						if (Vector2.Angle(shotDir2d, left2d) <= 90)
						{
							HUDManager.instance.LeftIndicatorShow();
						}
						else
						{
							HUDManager.instance.RightIndicatorShow();
						}
					}
				}
               
				//#endif
            }
        }
    }

    public void ReloadSuccessful()
    {
        personnelScript.ReloadCallback(success: true);
    }
    public void ThrowNadeSuccessful()
    {
        personnelScript.ThrowNadeCallBack();
        Gun.SetParent(GunPositionRightHand);
        Gun.localPosition = Vector3.zero;
        Gun.localRotation = Quaternion.identity;
    }

    //private Vector3 gunStartLeftHandLocalPos = new Vector3(-0.0725f, 0.0636f, 0.0956f);

    public void EnemyGrenedierTT()
    {
		HUDManager.TriggerToolTip (ToolTipType.Grenadier);
		if (UserSettings.GetSingleToolTipStatus (ToolTipType.PositionSwap)) {
			Handy.DoAfter (this, () => {
				HUDManager.TriggerToolTip (ToolTipType.PositionSwap);
			}, 0.5f, null);
		}
    }

	public static System.Action mortarFirstFireEvent;
    public void EnemyMortarTT()
    {

		if (mortarFirstFireEvent != null) {
			mortarFirstFireEvent ();
			mortarFirstFireEvent = null;

            if (UserSettings.GetSingleToolTipStatus (ToolTipType.Player3)) 
            {
                HUDManager.TriggerToolTip (ToolTipType.Player3);
                #if !TEST
                GameManagerScript.instance.FocusOnMortar(nadeReleasePoint.gameObject);
                #endif
            }
		}
		

    }

    public void ThrowNadeStart()
    {
        Gun.SetParent(GunPositionLeftHand);
        Gun.localPosition = Vector3.zero;
        Gun.localRotation = Quaternion.identity;
        if (personnelScript.enemyType != EnemyType.MORTAR)
        {
            //if (!GeneralManager.instance.runningForPromo)
                personnelScript.canvasController.TurnOnNadeAlert();
        }
    }
    internal bool hasUnpinnedNade;
    public void UnpinNade()
    {
        //Debug.Log("unpinned");
        hasUnpinnedNade = true;
    }



    public GameObject grenadePrefab;
    const float angleOfElevationForNadeRelease = 45;


    float projectileLandRadius = 0;
    float vel = 0;
    float Angle = 0;
    Vector3 dir = new Vector3();
    Vector3 toPosition = new Vector3();
    Vector3 randomRadialDeflection = new Vector3();
    Transform from;
    ProjectileScript projectileRef;
    GameObject projectileObj;
    ProjectileType projectileTypeTemp;
    float maxProjectileDamage;
    float maxProjectileDistance;

    public void ThrowProjectileRelease()
    {
        if (personnelScript.enemyType != EnemyType.MORTAR)
        {
            //if (!GeneralManager.instance.runningForPromo)
                personnelScript.canvasController.TurnOffNadeAlert();
        }
        else
        {
			personnelScript.targetPlayer = PlayerInputController.instance.current_player.GetTargetReference ();
			InGameSoundManagerScript.PlayOnPointFromID (nadeReleasePoint.position, ClipID.mortarFire);
        }
        //Debug.Log(grenadePrefab);
        from = nadeReleasePoint;
        projectileObj = Pool.Instantiate(grenadePrefab, from.position, Quaternion.identity);
        projectileRef = projectileObj.GetComponent<ProjectileScript>();
        projectileRef.hitSource = HitSource.ENEMY;

        //core choices
        if (personnelScript.enemyType == EnemyType.MORTAR)
        {
            Vector3 nrpFwd = nadeReleasePoint.forward;
            projectileLandRadius = AIPersonnel.shellAccuracyRadius; ////////
            randomRadialDeflection = (new Vector3(Random.Range(0.0f, 1.0f), 0, Random.Range(0.0f, 1.0f))).normalized * Random.Range(-projectileLandRadius, projectileLandRadius);
			toPosition = personnelScript.targetPlayer.position + randomRadialDeflection;
            Angle = Mathf.Atan2(nadeReleasePoint.forward.y, Mathf.Sqrt(nrpFwd.x * nrpFwd.x + nrpFwd.z * nrpFwd.z));
            projectileTypeTemp = ProjectileType.SHELL;
            maxProjectileDamage = AIPersonnel.shellMaxDamage;
            maxProjectileDistance = AIPersonnel.shellMaxDamageRadius;
            projectileObj.transform.LookAt(projectileObj.transform.position + nadeReleasePoint.forward);
        }
        else
        {
			float extraRadius = Mathf.Clamp(
				(Vector3.SqrMagnitude(from.position - personnelScript.targetPlayer.position)/(AIPersonnel.nadeOptimumDistance * AIPersonnel.nadeOptimumDistance)) -1
				,0, float.MaxValue)*AIPersonnel.nadeAccuracyRadius;
			projectileLandRadius = AIPersonnel.nadeAccuracyRadius + extraRadius; //////
            randomRadialDeflection = (new Vector3(Random.Range(0.0f, 1.0f), 0, Random.Range(0.0f, 1.0f))).normalized * Random.Range(-projectileLandRadius, projectileLandRadius);
			toPosition = personnelScript.targetPlayer.position + randomRadialDeflection;
            Angle = Mathf.Deg2Rad * angleOfElevationForNadeRelease;
            projectileTypeTemp = ProjectileType.NADE;
            maxProjectileDamage = AIPersonnel.nadeMaxDamage;
            maxProjectileDistance = AIPersonnel.nadeMaxDamageRadius;
        }

        //otherChoices
        if (personnelScript.enemyType == EnemyType.MORTAR)
        {
            personnelScript.muzzleFlashParticle.Play();
        }
        else
        {
            hasUnpinnedNade = false;
        }

        dir = toPosition - from.position;
        dir.y = 0;
        dir = dir.normalized;
        dir.y = Mathf.Tan(Angle);
        dir = dir.normalized;

        float x = (new Vector2(toPosition.x, toPosition.z) - new Vector2(from.position.x, from.position.z)).magnitude;
        float y = toPosition.y - from.position.y;
        vel = Mathf.Sqrt((4.9f * x * x) / ((x * Mathf.Tan(Angle) - y) * Mathf.Pow(Mathf.Cos(Angle), 2)));
        
        //if (EnemyType.MORTAR == personnelScript.enemyType) vel = vel / 5;
        projectileRef.InitBasic(vel,dir,HitSource.ENEMY, projectileTypeTemp, maxProjectileDamage, maxProjectileDistance);
    }
}
