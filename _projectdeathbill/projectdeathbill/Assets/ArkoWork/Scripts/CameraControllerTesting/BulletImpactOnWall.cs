using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletImpactOnWall : MonoBehaviour,iBulletImpact{

    List<Vector3> hitPoints;

    void Start()
    {
        hitPoints = new List<Vector3>();
    }

    public void TakeImapct(RaycastHit hit, float damageValue, HitSource hitSource)
    {
        hitPoints.Add(hit.point);
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < hitPoints.Count; i++)
            Gizmos.DrawSphere(hitPoints[i], 0.1f);
    }
}
