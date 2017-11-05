using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CameraPanRect))]
public class CameraPanRectEditor : Editor {

	SerializedObject sObject;
	SerializedProperty sPropLeft;
	SerializedProperty sPropRight;
	SerializedProperty sPropTop;
	SerializedProperty sPropBottom;

	public void OnEnable()
	{
		sObject = new SerializedObject(target);
		sPropLeft = sObject.FindProperty("panRect.left");
		sPropRight = sObject.FindProperty("panRect.right");
		sPropTop = sObject.FindProperty("panRect.top");
		sPropBottom = sObject.FindProperty("panRect.bottom");
	}

	public override void OnInspectorGUI ()
	{
		sObject.Update();
		GUILayout.Space(10);

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.PropertyField(sPropTop,GUIContent.none,GUILayout.Width(30));
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space(10);

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PropertyField(sPropLeft,GUIContent.none,GUILayout.Width(30));
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField("Pan Rectangle",GUILayout.Width(90));
		GUILayout.FlexibleSpace();
		EditorGUILayout.PropertyField(sPropRight,GUIContent.none,GUILayout.Width(30));
		EditorGUILayout.EndHorizontal();

		GUILayout.Space(10);

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.PropertyField(sPropBottom,GUIContent.none,GUILayout.Width(30));
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		sObject.ApplyModifiedProperties();
	}
}
