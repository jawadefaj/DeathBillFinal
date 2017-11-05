using UnityEngine;
using System.Collections;

public class IBI_Inanimate : MonoBehaviour, iBulletImpact
{
    public GameObject particle;
    private ParticleSystem pSystem;
    public void TakeImapct(RaycastHit hit, float damageValue, HitSource hitSource)
    {
        pSystem = Pool.Instantiate(particle, hit.point, Quaternion.identity).GetComponent<ParticleSystem>();
        pSystem.transform.LookAt(hit.point + hit.normal);
        pSystem.Play();
        StartCoroutine(DestroyAfter(pSystem.duration,pSystem.gameObject));

    }
    IEnumerator DestroyAfter(float time, GameObject particleObject)
    {
        yield return new WaitForSeconds(time);
        Pool.Destroy(particleObject);
    }
}
