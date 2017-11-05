using UnityEngine;
using System.Collections;

public class KnifeBloodSpillScript : MonoBehaviour {
	public ParticleSystem bloodSpillParticle;
	public void BloodSpillEvent()
	{
		//Debug.Log ("BloodSpillFunc");
		bloodSpillParticle.Play ();
	}
}
