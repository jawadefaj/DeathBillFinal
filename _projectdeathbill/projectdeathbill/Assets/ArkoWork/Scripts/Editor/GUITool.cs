using UnityEditor;
using UnityEngine;

public class GUITools{

	[MenuItem("Portbliss/Anchors to Corners %q")]

	static void AnchorsToCorners(){
		RectTransform r = Selection.activeTransform as RectTransform;
		RectTransform rp = Selection.activeTransform.parent as RectTransform;
		
		if(r == null || rp == null) return;
		
		Vector2 newAnchorsMin = new Vector2(r.anchorMin.x + r.offsetMin.x / rp.rect.width, r.anchorMin.y + r.offsetMin.y / rp.rect.height);
		Vector2 newAnchorsMax = new Vector2(r.anchorMax.x + r.offsetMax.x / rp.rect.width, r.anchorMax.y + r.offsetMax.y / rp.rect.height);
		
		r.anchorMin = newAnchorsMin;
		r.anchorMax = newAnchorsMax;
		r.offsetMin = r.offsetMax = new Vector2(0, 0);
	}

}