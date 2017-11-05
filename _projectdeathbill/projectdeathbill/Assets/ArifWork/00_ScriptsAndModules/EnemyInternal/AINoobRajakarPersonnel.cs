using UnityEngine;
using System.Collections;
using SWS;

public class AINoobRajakarPersonnel : MonoBehaviour {
	splineMove spline;
	public Animator anim;
	// Use this for initialization
	void Start () {
		spline = this.GetComponent<splineMove> ();
		//anim = this.GetComponent<Animator> ();
	}

	public void AlertNoobRajakar()
	{
		anim.SetTrigger ("ALERT");
	}
	public void StartRunning()
	{
		spline.StartMove ();
	}
	public void KillNoobRajakar()
	{
		anim.SetTrigger ("KILL");
		spline.Stop ();
	}
}
