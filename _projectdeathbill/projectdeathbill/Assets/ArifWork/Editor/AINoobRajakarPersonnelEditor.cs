using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AINoobRajakarPersonnel))]
public class AINoobRajakarPersonnelEditor : Editor {

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI ();
		if (GUILayout.Button ("Alert", GUILayout.Height (30))) {
			AINoobRajakarPersonnel ap = Selection.activeGameObject.GetComponent<AINoobRajakarPersonnel> ();
			ap.AlertNoobRajakar ();
		}
		if (GUILayout.Button ("Kill", GUILayout.Height (30))) {
			AINoobRajakarPersonnel ap = Selection.activeGameObject.GetComponent<AINoobRajakarPersonnel> ();
			ap.KillNoobRajakar ();
		}
	}
}
