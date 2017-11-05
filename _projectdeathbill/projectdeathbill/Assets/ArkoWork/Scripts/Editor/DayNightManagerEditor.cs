using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DayNightManager))]
public class DayNightManagerEditor : Editor {

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI();
		DayNightManager obj = (DayNightManager) target;

		GUILayout.Space(10);
		if (GUILayout.Button("Day", GUILayout.Height(30f)))
		{
			obj.ChangeToDay();
		}
		GUILayout.Space(10);
		if (GUILayout.Button("Night", GUILayout.Height(30f)))
		{
			obj.ChangeToNight();
		}
	}
}
