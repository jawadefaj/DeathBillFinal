using UnityEngine;
using System.Collections;

public class NadeLauncher : Weapon {
    public float aiMaxWeaponHotValue;

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
                    //hot value calculation only for AI
                    ChangeWeaponHotValue(weaponHotRate * Time.fixedDeltaTime);

                    bool canDischargeBullet;
                    if (playerController.GetAI() == null)
                    {
                        canDischargeBullet = (weaponHotValue == maxWeaponHotValue) ? true : false;
                    }
                    else
                    {

                        canDischargeBullet = (weaponHotValue == aiMaxWeaponHotValue) ? true : false;
                    }

                    if (AIDataManager.alertAvailableEnemies != null)
                        AIDataManager.alertAvailableEnemies ();
                    else
                        Handy.LogOnce ("no one registed for alert available enemies event");

                    //No heating for normal fire
                    if (playerController.GetAI() == null)
                        canDischargeBullet = true;

                    if (canDischargeBullet)
                    {
                        muzzleFlash.Play();

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
  
                        if (!AIShooting)
                            LaunchGranade();
                        else
                        {
                            AI_ReleaseGranade(AITarget.position);
                        }
						InGameSoundManagerScript.PlayOnPointFromID (this.transform.position,ClipID.gunFire_nadeLaunch);
                    
                        RotateBarrel();
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
            isFiring = false;
        }
        else
        {
            ChangeWeaponHotValue(-weaponCoolRate * Time.fixedDeltaTime);
            recoilTime = Mathf.Clamp(recoilTime-recoilDownRate,0f,10f);
            recoilVector = Vector2.zero;
        }
    }

    [Space]
    [Header("Nade Launcher Parameters")]
    public float maxRange = 50f;
    public float minRange = 10f;
    public GameObject nadeShellPrefab;
    public Transform gunBarrel;

    private bool isInitialized = false;
    private Vector3 screenCenter;
    private Camera main_camera;
    private float timeToRotateBarrel = 0.4f;
    private float barrelRotationValue = 60f; //degree

    void Init()
    {
        if (!isInitialized)
        {
            main_camera = ImprovedCameraCntroller.instance.gameObject.GetComponent<Camera>();
            isInitialized = true;
        }
    }

    void LaunchGranade()
    {
        Init();
        screenCenter = new Vector3 (Screen.width/2f,Screen.height/2f,0f);

        //calculate to position
        Vector3 toPos = Vector3.zero;
        Vector3 fromPos = lineRenderer.transform.position;
        Vector3 toPoint = new Vector3(screenCenter.x,screenCenter.y,0f);
        RaycastHit hit;
        bool didHit = false;
        Ray screenRay = main_camera.ScreenPointToRay(toPoint);

        didHit = Physics.Raycast(screenRay, out hit);

        if (!didHit)
        {
            toPos = main_camera.transform.position + screenRay.direction*maxRange;
        }
        else
        {
            toPos = hit.point;
        }
        //check for min distance
        float distance = Vector3.Distance (fromPos, toPos);
//        if(distance<minRange)
//        {
//            toPos = fromPos + ((toPos-fromPos).normalized)*minRange;
//            toPos.y = fromPos.y;
//        }
        //release granade
        //ProjectileScript.ReleaseNadeP2P (granade, fromPos, toPos, 30f, HitSource.PLAYER, 100, 10, Quaternion.identity);
        Vector3 camFrontDir = (toPos-fromPos).normalized;
        //Debug.Log(dir);
        //ProjectileScript.ReleaseNadeAngleElevated(nadeShellPrefab, fromPos, (toPos-fromPos).normalized,18, 15f, AIShooting?HitSource.PLAYER_AI:HitSource.PLAYER  , 100, 10, Quaternion.identity);
        //instance = this;

        float extraElevation = 15;
//
//        Debug.Log(toPos);
//        Debug.Log(fromPos);
//        Debug.Log(camFrontDir);
        if(camFrontDir.y <0)
        {
            float maxy = 0.707f;
            float yval = Mathf.Clamp(camFrontDir.y, -maxy,0);
            yval = Mathf.Abs(yval/maxy);

            extraElevation = Mathf.Lerp(15,0,yval);
        }
        //Debug.Log(extraElevation);
        ProjectileScript.ReleaseNadeAngleElevated(
            nade: nadeShellPrefab,
            fromPosition: fromPos,
            normalizedDirection: (toPos - fromPos).normalized,
            elevationAngle: 5,
            speed:extraElevation,
            hitSource: HitSource.PLAYER,
            maxDamage: damagePower,
            damageRadius: referenceRange,
            defaultRotation: Quaternion.identity
        );


    }

    protected override void ChangeWeaponHotValue(float value)
    {
        weaponHotValue += value;
        if (playerController.GetAI() == null)
        {
            weaponHotValue = Mathf.Clamp(weaponHotValue, 0, maxWeaponHotValue);
        }
        else
        {
            weaponHotValue = Mathf.Clamp(weaponHotValue, 0, aiMaxWeaponHotValue);
        }
    }

    void AI_ReleaseGranade(Vector3 toPos)
    {
        if (!playerController.IsAiming())
            return;
        
        float maxDeflection = 4f;
        Vector3 fromPos = lineRenderer.transform.position;

        Vector3 randomRadialDeflection = (new Vector3(Random.Range(0.0f, 1.0f), 0, Random.Range(0.0f, 1.0f))).normalized * Random.Range(-maxDeflection, maxDeflection);
        toPos = toPos + randomRadialDeflection*(1-accuracy);

        ProjectileScript.ReleaseNadeAngleElevated(
            nade: nadeShellPrefab,
            fromPosition: fromPos,
            normalizedDirection: (toPos - fromPos).normalized,
            elevationAngle: 5,
            speed:15,
            hitSource: HitSource.PLAYER,
            maxDamage: damagePower,
            damageRadius: referenceRange,
            defaultRotation: Quaternion.identity
        );
            
    }

    void RotateBarrel()
    {
        StopCoroutine("IEBarrelRotate");

        StartCoroutine("IEBarrelRotate");
    }

    IEnumerator IEBarrelRotate()
    {
        float rotTime = Mathf.Min(timeToRotateBarrel,(1/fireRate));
        Quaternion baseRotation = gunBarrel.localRotation;
        //gunBarrel.localRotation = Quaternion.Euler( new Vector3(gunBarrel.localRotation.x,gunBarrel.localRotation.y,0));
        barrelRotationValue = 360 / 8;

        float angularSpeed = barrelRotationValue / rotTime;

        do
        {
            gunBarrel.transform.Rotate(new Vector3(0, 0, angularSpeed * Time.deltaTime), Space.Self);
            yield return null;
        } while(gunBarrel.localRotation.eulerAngles.z<(baseRotation.eulerAngles.z+barrelRotationValue));
        gunBarrel.localRotation = baseRotation;
        //gunBarrel.localRotation=Quaternion.Euler(new Vector3(gunBarrel.localRotation.x,gunBarrel.localRotation.y,barrelRotationValue));
    }


}
