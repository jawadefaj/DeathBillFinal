using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class Weapon : MonoBehaviour {

    [Header("Weapon Properties")]
    public int maxBullet = 10;
	public float fireRate = 1f;
	public float recoilValue = 0.1f;

    [Space]
    public float damagePower = 10f;
    public float referenceRange = 10;
    [Range(0.1f,1f)]
    public float referencePercent = 0.6f;

    [Space]
    public bool hasScope = false;


    //public bool canZoom = false;
    public float personalZoomValue
    {
        get
        {
            return PlayerDataManager.GetPlayerDataStruct(playerController.fighterName).personalZoomValue;
        }
        set
        {
            PlayerDataManager.GetPlayerDataStruct(playerController.fighterName).SetPersonalZoomValue(value);
        }
    }

    //"AI Shooting Parameters"
    public float accuracy
    {
        get
        {
            return AIPlayerDataManager.GetDataStruct(playerController.fighterName).aiAccuracy;
        }
    }
    public int avgBulletFireInAI
    {
        get
        {
            return AIPlayerDataManager.GetDataStruct(playerController.fighterName).avgBulletFireInAI;
        }
    }

	public float AIBulletFireVariaton
    {
        get
        {
            return AIPlayerDataManager.GetDataStruct(playerController.fighterName).AIBulletFireVariaton;
        }
    }
	public float avgDelayTimeInAI
    {
        get
        {
            return AIPlayerDataManager.GetDataStruct(playerController.fighterName).avgDelayTimeInAI;
        }
    }
	
	public float AIDelayVariaton
    {
        get
        {
            return AIPlayerDataManager.GetDataStruct(playerController.fighterName).AIDelayVariaton;
        }
    }
	
	public float AIMaxShootingRangeSq
    {
        get
        {
            return AIPlayerDataManager.GetDataStruct(playerController.fighterName).AIMaxShootingRangeSq;
        }
    }

    [Header("Other Parameters")]
    public ParticleSystem muzzleFlash;

    private System.Collections.Generic.List<ParticleSystem> muzzleFlashList = new System.Collections.Generic.List<ParticleSystem>();
    private int flashCountN = -1;
    private int flashIndex = 0;
    private ParticleSystem muzzleFlashPlayer
    {
        get
        {
            if (flashCountN < 0)
            {
                flashCountN = Mathf.CeilToInt(fireRate / 10.0f);
            }
            if (muzzleFlashList.Count < flashCountN)
            {
                muzzleFlashList.Clear();
                for (int i = 0; i < flashCountN; i++)
                {
                    if (i == 0)
                    {
                        muzzleFlashList.Add(muzzleFlash);
                    }
                    else
                    {
                        GameObject go = Instantiate(muzzleFlash.gameObject, muzzleFlash.transform.position, muzzleFlash.transform.rotation)as GameObject;
                        go.transform.parent = muzzleFlash.transform.parent;
                        muzzleFlashList.Add(go.GetComponent<ParticleSystem>());
                    }
                }
            }
            if (flashIndex >= flashCountN)
            {
                flashIndex = 0;
            }
            return muzzleFlashList[flashIndex++];

        }
    }

    public LineRenderer lineRenderer;
	public bool shellExtractionOnEvent = false;

    [Header("Weapon Hot Values")]
    public float maxWeaponHotValue = 0f;
    public float weaponHotRate = 4f; //per second
    public float weaponCoolRate = 2.5f; //per second
    protected float weaponHotValue=0;

	public float GetCurrentHotValue(){
		return weaponHotValue;
	}

    private bool PF_isHeatingUp;
    public bool isHeatingUp
    {
        get
        {
            if(PF_isHeatingUp)
            {
                PF_isHeatingUp = false;
                return true;
            }
            else
            {
                return false;
            }
        }
        set
        {
            PF_isHeatingUp = value;
        }
    }

    protected ThirdPersonController playerController;
    protected Animator thisAnimator;
    protected bool isFiring = false;
    protected float nextFire = 0f;
    protected float fireInterval;
    protected float recoilTime;
    protected Vector2 recoilVector = Vector2.zero;
    protected float recoilUpRate = 0.2f;
    protected float recoilDownRate = 2.0f;

    protected int remainingBullet =0;
    protected bool AIShooting = false;
    protected Transform AITarget = null;
    protected BulletShellGenarator bullet_shell_generator;
    protected float a;
    protected int nextFireHalt = 10;
    protected float timeToDelayInAIFire = 1f;
    protected Recoil weaponRecoil;


	void Start()
	{
		remainingBullet = maxBullet;
        ResetAIBurstControll();
	}

	public void Initialize(ThirdPersonController third_person_controller)
	{
		playerController = third_person_controller;
		thisAnimator = playerController.GetAnimator();
		bullet_shell_generator = playerController.gameObject.GetComponent<BulletShellGenarator> ();
		fireInterval = 1f/fireRate;
	
		//calculae "a" value for dmage fallof
		a = (-Mathf.Log(referencePercent))/referenceRange;

		//get recoil component
		weaponRecoil = third_person_controller.gameObject.GetComponent<Recoil>();
	}

	public void SecondaryInitialize(ThirdPersonController third_person_controller)
	{
		playerController = third_person_controller;
		thisAnimator = playerController.GetAnimator();
		bullet_shell_generator = playerController.gameObject.GetComponent<BulletShellGenarator> ();
		fireInterval = 1f/fireRate;
		a = (-Mathf.Log(referencePercent))/referenceRange;
	}

	public float GetDamagedPoint(float distance)
	{
		return damagePower*Mathf.Exp(-a*distance);
	}

	public void Fire()
	{
		if(playerController.IsReloading() || playerController.IsThrowingGranade()) return;
		AIShooting = false;
		isFiring = true;
	}

    public virtual void AIFire(Transform target)
	{
		if(playerController.IsReloading() || playerController.IsThrowingGranade()) return;
		if(playerController.IsInControll()) return;

		//for burst controll
		if(nextFireHalt>0)
		{
            playerController.GetAnimator().SetBool("aim", true);
			AIShooting = true;
			AITarget = target;
			isFiring = true;
		}
		else
		{
            playerController.GetAnimator().SetBool("aim", false);

			if(timeToDelayInAIFire<0)
			{
				//prepare to shoot again
				ResetAIBurstControll();
			}
			else
			{
				timeToDelayInAIFire-=Time.deltaTime;
			}
		}
	}

    public bool canAIFire()
    {
        return nextFire < 0;
    }

	public void ResetAIBurstControll()
	{
		nextFireHalt = Mathf.RoundToInt((float)avgBulletFireInAI * Random.Range((1f-AIBulletFireVariaton),(1f+AIBulletFireVariaton)));
		timeToDelayInAIFire = avgDelayTimeInAI*Random.Range((1f-AIDelayVariaton),(1f+AIDelayVariaton));
	}

	public void ResetBulletCount()
	{
		remainingBullet = maxBullet;
	}

	public bool IsFiring()
	{
		return isFiring;
	}

	public bool CanReload()
	{
		if(remainingBullet == maxBullet) return false;
		else return true;
	}

	public Vector3 GetWeaponHeadPoint()
	{
		return lineRenderer.transform.position;
	}

	void FixedUpdate()
	{
       
		nextFire -= Time.deltaTime;

		if(isFiring)
		{
			if((thisAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash != Animator.StringToHash("Aim")) && (thisAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash != Animator.StringToHash("Fire")))
			{
				if(!AIShooting)
				{
					isFiring = false;
					return;
				}
			}

			if(SlowMotionBullet.instance.IsSlowMotionOn()) 
			{
				isFiring = false;
				return;
			}

			if(nextFire<0f)
			{
				if(remainingBullet>0)
				{
                    ChangeWeaponHotValue(weaponHotRate * Time.fixedDeltaTime);
                    bool canDischargeBullet = (weaponHotValue == maxWeaponHotValue) ? true : false;
                    isHeatingUp = true;
					if (AIDataManager.alertAvailableEnemies != null)
						AIDataManager.alertAvailableEnemies ();

					if(AIShooting)
                        playerController.Shoot(0,0,canDischargeBullet,true,AITarget);
					else
                        playerController.Shoot(recoilVector.x,recoilVector.y,canDischargeBullet);

                    if (canDischargeBullet)
                    {
                        muzzleFlashPlayer.Play();

                        //gun thrust effect
                        if (hasScope && playerController.isScopeOn)
                        {
                            switch (playerController.fighterName)
                            {
                                case FighterName.JB:
                                    ImprovedCameraCntroller.instance.GunThrustEffect(1.5f, 0.1f, 0.5f);
                                    break;
                                case FighterName.Philips:
                                    ImprovedCameraCntroller.instance.GunThrustEffect(0.2f, 0.05f, 0.25f);
                                    break;
                            }
                        }

                        if (weaponRecoil != null)
                            weaponRecoil.Fire(1f);

                        nextFire = fireInterval;
                        Recoil();

                        if (!AIShooting)
                        {
                            ImprovedCameraCntroller.instance.RecoilCursorEffect();
                        }
                        else
                            nextFireHalt--;

                        //shell extraction
                        if (shellExtractionOnEvent == false)
                            bullet_shell_generator.GenerateBulletShell();

                        remainingBullet--;
                    }

					if (remainingBullet <= 4) {
						if (PlayerInputController.instance.current_player.fighterRole == playerController.fighterRole) {
							HUDManager.TriggerToolTip (ToolTipType.Reload);
						}
					}
					if(remainingBullet==0) StartCoroutine(AssignAutoReload());

                }
				else
				{
					playerController.Reload();
				}
			}
			//Debug.Log ("firing false");
			isFiring = false;
		}
		else
		{
            ChangeWeaponHotValue(-weaponCoolRate * Time.fixedDeltaTime);
			recoilTime = Mathf.Clamp(recoilTime-recoilDownRate,0f,10f);
			recoilVector = Vector2.zero;
            //timeToDelayInAIFire-=Time.deltaTime;
		}
	}

    protected virtual void ChangeWeaponHotValue(float value)
    {
        weaponHotValue += value;
        weaponHotValue = Mathf.Clamp(weaponHotValue, 0, maxWeaponHotValue);
    }

    public float GetWeaponHotPercent()
    {
        return (weaponHotValue / maxWeaponHotValue);
    }
    
	protected IEnumerator AssignAutoReload()
	{
		yield return new WaitForSeconds(1f);
		do
		{
			playerController.Reload();
			yield return null;
		}while(remainingBullet==0);
	}
    public void RenderBulletTrail(Vector3 endPoint)
    {
        StopCoroutine("BulletRenderer");
        StartCoroutine(BulletRenderer(endPoint));
    }

    public float AmmoPercentage()
    {
        return  ((float)remainingBullet) / maxBullet;
    }
    public int RemainingAmmo()
    {
        return remainingBullet;
    }


    IEnumerator BulletRenderer(Vector3 endpoint)
    {
        lineRenderer.SetPosition(0,lineRenderer.transform.position);
        lineRenderer.SetPosition(1,endpoint);
        yield return new WaitForSeconds(0.05f);
        lineRenderer.SetPosition(0, lineRenderer.transform.position);
        lineRenderer.SetPosition(1, lineRenderer.transform.position);
    }

    protected void Recoil()
    {
        recoilTime = Mathf.Clamp(recoilTime + recoilUpRate, 0f, 10f);

        float rangeX = (recoilTime * recoilValue) * (Screen.width / 1024f);
        float rangeY = (recoilTime * recoilValue) * (Screen.height / 768f);
        if (rangeX != 0 && rangeY != 0)
            recoilVector = new Vector2(Random.Range(-rangeX, rangeX), Random.Range(-rangeY, rangeY));
    }

}
