using UnityEngine;
using System.Collections;

public class BulletGenerator : MonoBehaviour {
	public LineRenderer lineRenderer;
	public Transform targetPlayer;


	float nextShootTime = 0;                    //must init
	const float autoShootTime = 2.5f;
	const float autoShootTimeVariancePercentage = 0.75f;
	const int roundPerAttemptBase = 4;
	const int roundPerAttemptVariance = 1;
	private const float interFireInterval = 0.2f; //600 rounds per minutes


	void OnEnable()
	{
		StartCoroutine (FireRound (roundPerAttemptBase + Random.Range (-roundPerAttemptVariance, roundPerAttemptVariance + 1)));
	}

	IEnumerator FireRound(int rounds)
	{
		for (int i = 0; i < rounds; i++)
		{
			nextShootTime = Time.time + autoShootTime * (1 + Random.Range(-autoShootTimeVariancePercentage, autoShootTimeVariancePercentage));
			yield return new WaitForSeconds(interFireInterval/2);
			DischargeBulletLate ();
			yield return new WaitForSeconds(interFireInterval/2);
		}
	}
	private Vector3 basePoint;
	private Vector3 targetPoint;
	private Vector3 direction;
	private Vector3 modifiedPoint;

	private Vector3 up;
	private Vector3 right;
	public void DischargeBulletLate()
	{
		InGameSoundManagerScript.PlayOnPointFromID (lineRenderer.transform.position, ClipID.gunFire_AI_rifleAK47);
		if (lineRenderer == null) { Debug.LogError("linerenderer missing!!"); }
		else {
			direction = targetPlayer.position - lineRenderer.transform.position;
			up = Vector3.Cross(direction, Vector3.right);
			right = Vector3.Cross(direction, Vector3.up);
			float accuracy = 0;
			float maxdeflection = 2*(1-accuracy);

			direction = Quaternion.AngleAxis(Random.Range(-maxdeflection,maxdeflection), right) * direction;
			direction = Quaternion.AngleAxis(Random.Range(-maxdeflection,maxdeflection), up) * direction;
			RaycastHit hit;

			if (Physics.Raycast(lineRenderer.transform.position, direction, out hit, 100.0F))
			{
				StartCoroutine(BulletRenderer(hit.point, hit, true));

			}
			else
			{
				StartCoroutine(BulletRenderer(direction.normalized * 100 + lineRenderer.transform.position, hit, false));
			}

		}
	}

	public const float bulletSpeed = 2.5f;
	public const float trailSpeed = 2f;
	IEnumerator BulletRenderer(Vector3 endpoint, RaycastHit hit, bool didHit)
	{
		Vector3 startPoint = lineRenderer.transform.position;
		Vector3 bulletPoint = startPoint;
		Vector3 trailPoint = startPoint;
		Vector3 directionNorm = (endpoint - startPoint).normalized;
		float startTime = Time.time;
		while ((trailPoint - startPoint).sqrMagnitude < (endpoint-startPoint).sqrMagnitude)
		{
			trailPoint += directionNorm * trailSpeed;
			if(!((trailPoint - startPoint).sqrMagnitude < (endpoint - startPoint).sqrMagnitude))
				break;

			bulletPoint += directionNorm * bulletSpeed;
			if (!((bulletPoint - startPoint).sqrMagnitude < (endpoint - startPoint).sqrMagnitude))
			{
				bulletPoint = endpoint;
			}
			lineRenderer.SetPosition(0, trailPoint);
			lineRenderer.SetPosition(1, bulletPoint);
			yield return null;
		}

		lineRenderer.SetPosition(0, startPoint);
		lineRenderer.SetPosition(1, startPoint);
	}


}
