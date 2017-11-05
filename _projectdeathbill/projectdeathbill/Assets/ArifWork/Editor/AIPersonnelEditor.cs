using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AIPersonnel))]
public class AIPersonnelEditor : Editor {
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI ();
		if (GUILayout.Button ("Take30", GUILayout.Height (30))) {
			AIPersonnel ap = Selection.activeGameObject.GetComponent<AIPersonnel> ();
			ap.TakeDamage (30, HitType.HEAD,  HitSource.GAYEBI);
		}
		if (GUILayout.Button ("Kill", GUILayout.Height (30))) {
			AIPersonnel ap = Selection.activeGameObject.GetComponent<AIPersonnel> ();
			ap.TakeDamage (1000, HitType.HEAD, HitSource.GAYEBI);
		}

	}
	void OnEnable()
	{

	}

}
