using UnityEngine;
using System.Collections;

public class IBI_EnemyLimbs : MonoBehaviour, iBulletImpact, iPreHitQuery, iEnemyRefKeeper
{
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
    const float damageMultiplier = 0.35f;
    const HitType hitType = HitType.LIMB;

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
		if (hitSource == HitSource.ENEMY)
			damageValue *= AIDataManager.enemyFriendlyFireDamageMultiplier;
        personnelScript.TakeDamage(damageValue * damageMultiplier, hitType, hitSource);
		if (particle == null) {
			Debug.Log ("BloodParticleMissing!!");
			return;
		}
		//Debug.Log ("createing Particle");
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
