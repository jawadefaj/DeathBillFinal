using System;
using UnityEngine;
using System.Collections;
using Portbliss.SneakyStation;
using UnityEngine.Serialization;

public class SneakyPlayer : MonoBehaviour {

	internal bool isStabing = false;
    public FighterName fighterName;
    [FormerlySerializedAs("fighterID")]
	public FighterRole fighterRole;
	public Transform target;
	private SneakyStationController ssc;
	private MovementControler mc;
	private Animator animator;
	private const float ROTATING_SPEED = 5f;

	// Use this for initialization
	void Start () {
		ssc = this.GetComponent<SneakyStationController>();
		animator = this.GetComponent<Animator>();
		mc = this.GetComponent<MovementControler>();
	}

	public SneakyStationController GetSneakyStationController()
	{
		if(ssc==null) ssc = this.GetComponent<SneakyStationController>();
		return ssc;
	}

	public MovementControler GetMovementController()
	{
		if(mc==null) mc = this.GetComponent<MovementControler>();
		return mc;
	}

	public void OnStabAnimationPlayDone()
	{
		isStabing = false;
	}
	public void OnStabKillingDone(bool deactivateModel)
	{
		//activate this player
		this.gameObject.SetActive(deactivateModel);
		SneakyPlayerManager.instance.ClearStabTarget();
		//if we have pending manual walk then finish it
		ssc.FinishManualWalkAutomatically();
		isStabing = false;
		mc.StopWalk();
	}

	public float GetSpeed()
	{
		return ssc.GetCurrentSpeed();
	}

	public bool IsPlayerMoving()
	{
		return (GetSneakyStationController().IsMoving() || GetMovementController().IsMoving());
	}

	public bool IsPlayerAutoMoving()
	{
		return (GetSneakyStationController().IsAutoMoving() || GetMovementController().IsAutoMoving());
	}

	public void ActivatePlayer()
	{
		if(ssc == null) ssc = this.GetComponent<SneakyStationController>();
		ssc.ActivateCamera();
	}

	public void StabEnemy(Transform target, Action callback)
	{
		if(isStabing) return;
		if(target==null) return;

		isStabing = true;
		//TODO GetEnemy Target Here
		StartCoroutine(TurnGradually(target.position, () => 
			{

				//we make a distance check
				if(Vector3.SqrMagnitude(this.transform.position-SneakyPlayerManager.instance.GetStabTarget().position)<SneakyPlayerManager.STAB_RANGE_SQ)
				{
					//we can actually kill him

					//deactivate this player
					this.gameObject.SetActive(false);

					//kill the original enemy enemy instantly
                    SneakyPlayerManager.instance.nextTargetPersonel.TakeDamage (1000, HitType.KNIFE,HitSource.SNEAKY_PLAYER);

					//Play cinematics
					KnifeAnimationManager.instance.PlayKnifeKillingAnimation(this.transform);
				}
				else
				{
					//we can not kill him. just play a single animation
					animator.SetTrigger("Stab");
				}


				if(callback!=null)
				{
					StartCoroutine(AfterStabDone(callback));
				}
			},0));
	}

	private IEnumerator AfterStabDone(Action callback)
	{
		yield return new WaitForSeconds(2f);
		callback();
	}

	private IEnumerator TurnGradually(Vector3 target, Action callback, float defWait = 0f)
	{
		float elapsedTime =0;
		float tolerance = 5f;
		Vector3 dir = this.transform.forward;
		target.y = this.transform.position.y;
		Vector3 targetDir = (target-this.transform.position).normalized;
		float angle = Vector3.Angle(dir,targetDir);
		Quaternion lookRotation;

		//Debug.Log("lagging angle "+Vector3.Angle(dir,targetDir));

		do
		{
			lookRotation = Quaternion.LookRotation(targetDir);
			transform.rotation = Quaternion.Lerp(transform.rotation,lookRotation,Time.deltaTime*ROTATING_SPEED);

			dir = this.transform.forward;
			targetDir = (target-this.transform.position).normalized;
			angle = Vector3.Angle(dir,targetDir);
			elapsedTime+=Time.deltaTime;
			yield return null;
			//Debug.Log(angle);
		}while(angle>tolerance);

		if(defWait!=0)
		{
			if((defWait-elapsedTime)>0)
				yield return new WaitForSeconds(defWait-elapsedTime);
		}
		if(callback!=null)
			callback();
	}
}
