using UnityEngine;
using System.Collections;
using System;

public class PlayerNadeImpactTaker : MonoBehaviour, iProjectileImpact {

	private ThirdPersonController controller;
	// Use this for initialization
	void Start () {
		controller = this.transform.root.gameObject.GetComponent<ThirdPersonController> ();
	}

    void iProjectileImpact.TakeImpact(float damageValue, Vector3 blastForce, HitSource hitSource, ProjectileType projectileType, bool shouldIgnoreHidingDamgeReduction, bool failNade)
    {
        controller.TakeNadeImpact(damageValue, hitSource, shouldIgnoreHidingDamgeReduction);
    }
}
