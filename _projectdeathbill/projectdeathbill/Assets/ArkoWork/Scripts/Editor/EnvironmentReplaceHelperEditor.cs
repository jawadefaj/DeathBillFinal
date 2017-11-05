using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(EnvironmentReplacementHelper))]
public class EnvironmentReplaceHelperEditor : Editor {

	public override void OnInspectorGUI ()
	{
		//base.OnInspectorGUI ();

		if (GUILayout.Button("Attach", GUILayout.Height(30f)))
		{
            foreach(GameObject obj in Selection.gameObjects)
                obj.transform.SetParent(((EnvironmentReplacementHelper)target).transform);
		}
	}
}
