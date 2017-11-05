using UnityEngine;
using System.Collections;
using DG.Tweening;

public class UpdatedContentNotificationAnimator : MonoBehaviour {
	float delay = 2;
	float duration= 1f;
	float str =25;
	int vib = 15;
	float elasticity = 0.5f;
	public bool attractAttention = false;
	IEnumerator Start () {
		while(true){
			yield return new WaitForSeconds (delay);
			if(attractAttention)
				this.transform.DOPunchRotation ( new Vector3(0,0,str), duration, vib, elasticity);
		}
	}

}
