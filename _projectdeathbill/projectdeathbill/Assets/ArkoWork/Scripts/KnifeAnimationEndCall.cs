using UnityEngine;
using System.Collections;

public class KnifeAnimationEndCall : MonoBehaviour {

	void OnDead()
	{
		//this.transform.parent.GetComponent<KnifeAnimation>().OnPlayDone();
		KnifeAnimationManager.instance.OnAnimationPlayDone();
	}
}
