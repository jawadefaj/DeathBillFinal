using UnityEngine;
using System.Collections;

public class PlayerBulletImpactTaker : MonoBehaviour, iBulletImpact {

	private ThirdPersonController controller;
	// Use this for initialization
	void Start () {
		controller = this.transform.root.gameObject.GetComponent<ThirdPersonController> ();
	}

	public void TakeImapct (RaycastHit hit, float damageValue, HitSource hitSource)
	{
		controller.TakeImapct (hit, damageValue, hitSource);
	}


}
