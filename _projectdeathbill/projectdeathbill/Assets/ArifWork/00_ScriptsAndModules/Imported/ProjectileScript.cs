#define TEST
using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {
    public ProjectileType projectileType;
    public HitSource hitSource;
    public GameObject particleBlast;
	// Use this for initialization
    public bool alive;


    float maxDistance;
    float maxDamage;
    float damage;
    float distance;
    public bool premature = false;
    ParticleSystem particleRef;
    Rigidbody rgbd;
    CapsuleCollider capsuleCol;
    //throw vector based

    #region Init Types
    public void InitBasic(float speed, Vector3 normalizedDirection, HitSource hitSource, ProjectileType projectileType, float maxDamage, float maxDistance)
    {
        this.hitSource = hitSource;
        this.projectileType = projectileType;
        this.maxDamage = maxDamage;
        this.maxDistance = maxDistance;

        rgbd = GetComponent<Rigidbody>();
        rgbd.velocity = speed * normalizedDirection;

        capsuleCol = this.GetComponent<CapsuleCollider>();
        capsuleCol.isTrigger = true;
        alive = false;
        premature = false;
        kaBoom = false;

        switch (projectileType)
        {
            case ProjectileType.NADE:
                rgbd.AddTorque(Random.Range(0.0f, 100.0f), Random.Range(0.0f, 100.0f), Random.Range(0.0f, 100.0f));
                if(hitSource == HitSource.ENEMY)
                    StartCoroutine(DetonateAfter(3f));
                else
                    StartCoroutine(DetonateAfter(2.1f));
                break;
            case ProjectileType.MININADE:
                //rgbd.AddTorque(Random.Range(0.0f, 5.0f), Random.Range(0.0f, 5.0f), Random.Range(0.0f, 5.0f));
                //float dTime = Handy.Deviate(1.5f, 0.5f);
                StartCoroutine(DetonateAfter(10));
                break;
        }   
    }
    public void InitElevationAngle(float speed, Vector3 normalizedDirection, float elevationAngle, HitSource hitSource, ProjectileType projectileType, float maxDamage, float maxDistance)
    {
        float y0 = normalizedDirection.y;
        float w0 = Mathf.Sqrt(normalizedDirection.x * normalizedDirection.x + normalizedDirection.z * normalizedDirection.z);
        float rad0 = Mathf.Atan2(y0, w0);
        float rad1 = elevationAngle * Mathf.Deg2Rad + rad0;
        float y1 = Mathf.Sin(rad1)*w0;
        Vector3 newDir = new Vector3(normalizedDirection.x,y1,normalizedDirection.z);
        InitBasic(speed, newDir.normalized, hitSource, projectileType, maxDamage, maxDistance);
    }
    //target and angle based
    public void InitP2P(float elevationAngle_Degrees, Vector3 fromPosition, Vector3 toPosition, HitSource hitSource, ProjectileType projectileType, float maxDamage, float maxDistance)
    {
        float Angle = Mathf.Deg2Rad * elevationAngle_Degrees;
        Vector3 dir = toPosition - fromPosition;
        dir.y = 0;
        dir = dir.normalized;
        dir.y = Mathf.Tan(Angle);
        dir = dir.normalized;

        float x = (new Vector2(toPosition.x, toPosition.z) - new Vector2(fromPosition.x, fromPosition.z)).magnitude;
        float y = toPosition.y - fromPosition.y;
        float speed = Mathf.Sqrt((4.9f * x * x) / ((x * Mathf.Tan(Angle) - y) * Mathf.Pow(Mathf.Cos(Angle), 2)));

        InitBasic(speed,dir,hitSource,projectileType,maxDamage,maxDistance);
    }
    #endregion
    #region Static Calls
    public static void ReleaseNadeP2P(GameObject nade, Vector3 fromPosition, Vector3 toPosition, float elevationAngle_Deg, HitSource hitSource, float maxDamage, float damageRadius, Quaternion defaultRotation)
    {
        GameObject newNade = Pool.Instantiate(nade,fromPosition,defaultRotation);
        ProjectileScript pScript = newNade.GetComponent<ProjectileScript>();
        pScript.InitP2P(elevationAngle_Deg,fromPosition,toPosition,hitSource,pScript.projectileType,maxDamage,damageRadius);
    }
    public static void ReleaseNadeBasic(GameObject nade, Vector3 fromPosition, Vector3 normalizedDirection, float speed, HitSource hitSource, float maxDamage, float damageRadius, Quaternion defaultRotation)
    {
        GameObject newNade = Pool.Instantiate(nade, fromPosition, defaultRotation);
        ProjectileScript pScript = newNade.GetComponent<ProjectileScript>();
        pScript.InitBasic(speed,normalizedDirection, hitSource, pScript.projectileType, maxDamage, damageRadius);
    }
    public static void ReleaseNadeAngleElevated(GameObject nade, Vector3 fromPosition, Vector3 normalizedDirection,float elevationAngle, float speed, HitSource hitSource, float maxDamage, float damageRadius, Quaternion defaultRotation)
    {
        GameObject newNade = Pool.Instantiate(nade, fromPosition, defaultRotation);
        ProjectileScript pScript = newNade.GetComponent<ProjectileScript>();
        pScript.InitElevationAngle(speed, normalizedDirection, elevationAngle, hitSource, pScript.projectileType, maxDamage, damageRadius);
    }

    public static void PreMatureDetonation(GameObject nade, Vector3 position, HitSource hitSource , float maxDamage, float damageRadius)
    {
        GameObject newNade = Pool.Instantiate(nade, position,Quaternion.identity);
        ProjectileScript newProjectileScript = newNade.GetComponent<ProjectileScript>();
        newProjectileScript.hitSource = hitSource;
        newProjectileScript.maxDamage = maxDamage;
        newProjectileScript.maxDistance = damageRadius;
        newProjectileScript.alive = false;
        newProjectileScript.GetComponent<CapsuleCollider>().isTrigger = false;
        newProjectileScript.premature = true;
        newProjectileScript.StartCoroutine(newProjectileScript.DetonateAfter(2.5f));
        
    }
    #endregion
    public IEnumerator DetonateAfter(float time)
    {
        yield return new WaitForSeconds(time);
        TriggerDetonate();
    }

    void FixedUpdate()
    {

        if (kaBoom)
        {
            Detonate();
            return;
        }
        if (rgbd == null)
            return;
        if (rgbd.useGravity)
            return;
        if (projectileType == ProjectileType.MININADE)
        {
            rgbd.AddForce(new Vector3(0,-4f,0) , ForceMode.Acceleration);
        }
    }
    void Update()
    {
        switch (projectileType)
        {
            case ProjectileType.SHELL:
            case ProjectileType.MININADE:
                //transform.rotation = Quaternion.Euler( Mathf.Rad2Deg*rgbd.velocity.normalized);
                transform.LookAt(transform.position + rgbd.velocity.normalized);
                //Debug.Log(rgbd.velocity.normalized);
                Debug.DrawRay(this.transform.position, rgbd.velocity * 10);
                break;
        }


        if (!alive)
        {
            if (premature) return;
            if (rgbd.velocity.y < 0 || projectileType == ProjectileType.MININADE)
            {
                capsuleCol.isTrigger = false;
                alive = true;
            }
        }
    }


    Vector3 blastDirection = new Vector3();
    float blastPower = 0;
    const float maxBlastPower =5;

    void OnCollisionEnter(Collision collision)
    {
        switch (projectileType)
        {
            case ProjectileType.SHELL:
            case ProjectileType.MININADE:
                if (alive)
                {
                    alive = false;
                    TriggerDetonate();    
                }
                break;
        }
    }


    bool kaBoom = false;
    public void TriggerDetonate()
    {
        kaBoom = true;
    }
    private void Detonate()
    {
        kaBoom = false;
        if (alive) alive = false;
        //NadeLauncher.instance.hPoint.Add(transform.position);
        RaycastHit[] srcHits = Physics.SphereCastAll(origin: transform.position, radius: maxDistance, direction: Vector3.forward, maxDistance: 0.001f);
		if (projectileType == ProjectileType.SHELL) {
			InGameSoundManagerScript.PlayOnPointFromID (this.transform.position, ClipID.shellBlast);
		} else {
			if (hitSource == HitSource.ENEMY)
				InGameSoundManagerScript.PlayOnPointFromID (this.transform.position, ClipID.nadeBlastAI);
			else 
			{
				if (projectileType == ProjectileType.MININADE) {
					InGameSoundManagerScript.PlayOnPointFromID (this.transform.position, ClipID.gunFire_nadeBlast);
				} 
				else {
					InGameSoundManagerScript.PlayOnPointFromID (this.transform.position, ClipID.nadeBlastPlayer);
				}
        
			}
		}
        /*if (premature)
        foreach (RaycastHit rc in srcHits)
        {
                Debug.Log(rc.transform.name);
        }*/
        int aH = 0;
        //Debug.Log("Blast hit count: " +srcHits.Length.ToString());
        for (int i = 0; i < srcHits.Length; i++)
        {
            //hitTrans = rcHits[0].collider.transform;
            iProjectileImpact nadeDmger = srcHits[i].collider.transform.GetComponent<iProjectileImpact>();
			if (nadeDmger == null)
				continue;

            aH++;
            if (premature) Debug.Log("damager found");
            blastDirection = (srcHits[i].transform.position - this.transform.position);
            distance = blastDirection.magnitude;                  //Vector3.Distance(this.transform.position, hitTrans.position); 
            blastDirection = blastDirection.normalized;
            bool shouldIgnoreHidingDamageReduction = false;
            if (maxDamage > 0)
            {
                damage = maxDamage * (1 - ((distance * distance) / (maxDistance * maxDistance)));
                damage = Mathf.Clamp(damage, 0, maxDamage);
                blastPower = maxBlastPower * damage / maxDamage;
            }
            else
            {
                hitSource = HitSource.GAYEBI;
                //damage = PlayerInputController.instance.current_player.healthPoint * 0.75f;
                blastPower = maxBlastPower;
                shouldIgnoreHidingDamageReduction = true;
            }

            //Arko Code
            nadeDmger.TakeImpact(damage, blastPower * blastDirection, hitSource, projectileType,shouldIgnoreHidingDamageReduction,premature);
            //nadeDmger.TakeImpact(damage, blastPower * new Vector3(0,1,0), hitSource, projectileType,shouldIgnoreHidingDamageReduction,premature);

        }
        //Debug.Log("Blast hit actual count: " +aH.ToString());
        switch (projectileType)
        {
            case ProjectileType.NADE:
                ImprovedCameraCntroller.instance.ShakeTheCam(3, 0.2f);
                break;
            case ProjectileType.SHELL:
                ImprovedCameraCntroller.instance.ShakeTheCam(4,2);
                break;
            case ProjectileType.MININADE:
                ImprovedCameraCntroller.instance.ShakeTheCam(1,0.2f);
                break;
        }
		if (particleBlast != null)
        {
            particleRef = Pool.Instantiate(particleBlast, this.transform.position, Quaternion.Euler(-90, 0, 0)).GetComponent<ParticleSystem>();
            //particleRef.Play();
			GeneralManager.instance.StartCoroutine(DestroyAfter(particleRef.duration, particleRef.gameObject));
        }
        else { Debug.Log("no blast particle"); }
        Pool.Destroy(this.gameObject);
    }


    IEnumerator DestroyAfter(float time, GameObject particleObject)
    {
        //Debug.Log("destroy was called");
        yield return new WaitForSeconds(time);
        Pool.Destroy(particleObject);
    }
}
