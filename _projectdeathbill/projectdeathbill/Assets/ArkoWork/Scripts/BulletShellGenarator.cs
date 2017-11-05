using UnityEngine;
using System.Collections;

public class BulletShellGenarator : MonoBehaviour {

	public Transform shellInstantiatePoint;
	public float shellExitVelocity = 10f;
	public float shellSize = 0.3f;

	private GameObject bulletShell;

	public void GenerateBulletShell()
	{
		//Debug.Log ("BulletShell");
		bulletShell =BulletShellProvider.instance.PoolFromList();

		bulletShell.transform.position = shellInstantiatePoint.position;
		bulletShell.transform.rotation = shellInstantiatePoint.rotation;
		bulletShell.transform.localScale = new Vector3 (shellSize, shellSize, shellSize);

		bulletShell.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		bulletShell.GetComponent<Rigidbody> ().AddForce (Vector3.up * shellExitVelocity);
		bulletShell.GetComponent<Rigidbody> ().AddForce (transform.right * shellExitVelocity*Random.Range(0.2f,1.0f));
		BulletShellProvider.instance.DestroyGameObject(bulletShell, 3f);
	}
}
