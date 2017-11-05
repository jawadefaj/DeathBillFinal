using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(ExtraAudioKeeper))]
public class ExtraAudioKeeperEditor : Editor {

	SerializedObject s_Object;

	private bool listVisibility = true;
	private List<bool> elementVisibility = new List<bool>();

	void OnEnable()
	{
		s_Object = new SerializedObject(target);

		//initializing visibility list for list items
		if(elementVisibility==null || elementVisibility.Count<1)
		{
			elementVisibility = new List<bool>();

			for(int i=0;i<s_Object.FindProperty("extraAudioClips").arraySize;i++)
				elementVisibility.Add(false);
		}
	}

	public override void OnInspectorGUI ()
	{
		s_Object.Update();

		ListIterator("extraAudioClips",ref listVisibility);
		s_Object.ApplyModifiedProperties();
	}

	public void ListIterator(string propertyPath, ref bool visible)
	{
		SerializedProperty listProperty = s_Object.FindProperty(propertyPath);

		visible = EditorGUILayout.Foldout(visible,listProperty.name);

		if(visible)
		{
			EditorGUI.indentLevel++;

			for(int i=0;i<listProperty.arraySize;i++)
			{
				bool b = elementVisibility[i];
				bool r = ShowElementItem(listProperty,i, ref b);
				if(!r) break;
				elementVisibility[i] =b;
			}

			EditorGUI.indentLevel--;
		}
	}

	public bool ShowElementItem(SerializedProperty listProperty, int i, ref bool visible)
	{
		SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(i);
		SerializedProperty vol = elementProperty.FindPropertyRelative("volume");
		SerializedProperty id = elementProperty.FindPropertyRelative("id");
		SerializedProperty clip = elementProperty.FindPropertyRelative("clip");


		GUILayout.BeginVertical(EditorStyles.helpBox);

		GUIStyle modifiedFoldOut = new GUIStyle(EditorStyles.foldout);
		modifiedFoldOut.fontSize = 12;
		modifiedFoldOut.fontStyle = FontStyle.Bold;


		visible = EditorGUILayout.Foldout(visible,id.enumNames[id.enumValueIndex],modifiedFoldOut);

		if(visible)
		{
			EditorGUILayout.PropertyField(id,GUIContent.none);
			EditorGUILayout.PropertyField(clip,GUIContent.none);
			EditorGUILayout.PropertyField(vol,GUIContent.none);

			EditorGUILayout.BeginHorizontal();

			GUILayout.Space(10);

			if (GUILayout.Button("Add", GUILayout.Width(100f)))
			{
				listProperty.InsertArrayElementAtIndex(i);
				elementVisibility.Insert(i,true);
				return false;
			}

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Remove", GUILayout.Width(100f)))
			{
				listProperty.DeleteArrayElementAtIndex(i);
				elementVisibility.RemoveAt(i);
				return false;
			}
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(10);

		}

		GUILayout.EndVertical();

		return true;
	}
}
