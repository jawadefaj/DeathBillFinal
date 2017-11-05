using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ViewPortLerper : MonoBehaviour {
	public List<Transform> selectables = new List<Transform>();
	public ScrollRect scrollRect;
	int index;

	void Update () {
		float minSqrDist = float.MaxValue;
		for (int i = 0; i < selectables.Count; i++) {
			if(Vector3.SqrMagnitude(selectables[i].position - this.transform.position)<minSqrDist)
			{
				minSqrDist = Vector3.SqrMagnitude(selectables[i].position - this.transform.position);
				index = i;
			}
		}
		if(!Input.GetMouseButton(0))
		{
			scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition,((float)index)/(selectables.Count-1),0.1f) ;
		}
	}
}
