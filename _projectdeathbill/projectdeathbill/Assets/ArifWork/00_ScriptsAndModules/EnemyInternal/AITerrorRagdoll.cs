using UnityEngine;
using System.Collections;

public class AITerrorRagdoll : MonoBehaviour {

	public Animator thisAnimator;

	internal Vector3 force;
	internal float lifeTime;
	public Rigidbody refRigidBody;

	void OnEnable()
	{
		thisAnimator.enabled = false;
		refRigidBody.AddForce(force, ForceMode.Impulse);
		StartCoroutine (CleanAfter());
	}

	IEnumerator CleanAfter()
	{
		yield return new WaitForSeconds (lifeTime);
		Pool.Destroy (this.gameObject);
	}

	void OnDisable()
	{
		thisAnimator.enabled = true;
	}


}
