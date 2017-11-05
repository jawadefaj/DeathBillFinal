using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(Zone))]
public class ZoneEditor : Editor {

	public override void OnInspectorGUI ()
	{
        base.OnInspectorGUI ();

        Zone z = (Zone)target;


		int freeFighterLength = Enum.GetNames (typeof(FighterRole)).Length;
		string[] nameArr = Enum.GetNames (typeof(FighterRole));

		if (z.AdvantageAgainstFighters == null) 
        {
			z.AdvantageAgainstFighters = new List<float> ();
		}

//		if (z.AdvantageAgainstFighters.Count == 0) {
//			z.AdvantageAgainstFighters.Clear();
//
			z.AddRemoveAdvantages ();
//		}

        EditorGUILayout.Space();
		EditorGUILayout.BeginVertical("Box");

		for (int i = 1; i < z.AdvantageAgainstFighters.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			z.AdvantageAgainstFighters [i] = EditorGUILayout.Slider(nameArr[i],z.AdvantageAgainstFighters[i],0,1);
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.EndVertical();


//        EditorGUILayout.Space();
//        EditorGUILayout.LabelField("Hide/Unhide Sprites", EditorStyles.boldLabel);
//        EditorGUILayout.BeginVertical ("Box");
//        hm.unhideSprite = (Sprite)EditorGUILayout.ObjectField("Unhide Sprites", hm.unhideSprite, typeof(Sprite));
//        hm.hideSprite = (Sprite)EditorGUILayout.ObjectField("Hide Sprites", hm.hideSprite, typeof(Sprite));
//        EditorGUILayout.EndVertical ();
	}

}
