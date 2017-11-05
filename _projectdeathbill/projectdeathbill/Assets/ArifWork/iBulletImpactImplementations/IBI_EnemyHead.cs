using UnityEngine;
using System.Collections;

public class IBI_EnemyHead : MonoBehaviour, iBulletImpact ,iPreHitQuery, iEnemyRefKeeper, iProjectileImpact
{

    void iProjectileImpact.TakeImpact(float damageValue, Vector3 blastForce, HitSource hitSource, ProjectileType projectileType, bool shouldIgnoreHidingDamageReduction, bool failNade)
    {

        switch (projectileType)
        {
            case ProjectileType.MININADE:
                {
                    if (personnelScript.profile.HP <= damageValue)
                    {
                        personnelScript.DieOfGrenade(blastForce, hitSource, failNade);
                    }
                    else
                    {
                        //Debug.Log(transform.name+" DMG:"+damageValue.ToString());
                        personnelScript.TakeDamage(damageValue, HitType.BLAST,hitSource);
                    }
                }
                break;
            default:
                {
                    if (damageValue > 50 || failNade)
                    {
                        personnelScript.TakeDamage(damageValue, HitType.BLAST,hitSource);
                    }
                    else
                    {
                        //Debug.Log("skipped dealing damage, failnade:" + failNade.ToString());
                    }
                }
                break;
        }
    }

    public AIPersonnel personnelScriptProperty
    {
        get
        {
            return personnelScript;
        }
        set
        {
            personnelScript = value;
        }
    }
    const float damageMultiplier = 1.00f;
    const HitType hitType = HitType.HEAD;

    internal GameObject particle;
    public AIPersonnel personnelScript;
    private ParticleSystem pSystem;
    void Start()
    {
        if (personnelScript == null) Debug.LogError("No personnel script detected");
		particle = AIDataManager.instance.BloodParticle;
    }
    public void TakeImapct(RaycastHit hit, float damageValue, HitSource hitSource)
    {
		if (hitSource == HitSource.ENEMY) damageValue *= AIDataManager.enemyFriendlyFireDamageMultiplier;
        personnelScript.TakeDamage(damageValue * damageMultiplier, hitType,hitSource);
		if (pSystem == null) {
			//Debug.Log ("BloodParticleMissing!!");
			return;
		}
		Debug.Log ("createing Particle");
        pSystem = Pool.Instantiate(particle, hit.point, Quaternion.identity).GetComponent<ParticleSystem>();
        pSystem.transform.SetParent(this.transform);
        pSystem.transform.LookAt(hit.point + hit.normal);
        pSystem.Play();
        StartCoroutine(DestroyAfter(pSystem.duration, pSystem.gameObject));

    }
    IEnumerator DestroyAfter(float time, GameObject particleObject)
    {
        yield return new WaitForSeconds(time);
        Pool.Destroy(particleObject);
    }

    HitResult hitRes = new HitResult();
    public HitResult GetHitResults(float damage)
    {
        hitRes.damageOutput = personnelScript.DamageFunction(damage * damageMultiplier);
        hitRes.HPafterDamage = personnelScript.profile.HP - hitRes.damageOutput;
        hitRes.willDie = (hitRes.HPafterDamage <= 0);
        if (personnelScript.status.dead) hitRes.willDie = false;
        hitRes.aipersonnelsReference = personnelScript;
        return hitRes;
    }
}
