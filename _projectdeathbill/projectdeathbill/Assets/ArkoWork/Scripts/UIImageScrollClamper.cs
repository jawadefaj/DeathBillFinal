using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIImageScrollClamper : MonoBehaviour {

	public ScrollRect sr;

	public void OnValueChanged(Vector2 value)
	{
		float h = value.x;

		if (h<0.33f)
			sr.horizontalNormalizedPosition = 0;
		else if(h<0.67f)
			sr.horizontalNormalizedPosition = 0.5f;
		else
			sr.horizontalNormalizedPosition =1f;
	}
}
