using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(HUDManager))]
public class HudManagerEditor : Editor {

    public static bool showSelection = false;
    public static bool showPortraits = false;

	public override void OnInspectorGUI ()
	{
        base.OnInspectorGUI ();

        HUDManager hm = (HUDManager)target;

        int freeFighterLength = Enum.GetNames (typeof(FighterName)).Length;
        string[] nameArr = Enum.GetNames (typeof(FighterName));

        //for selection images
        if (hm.playerSelectionButtonSprites == null) 
        {
            hm.playerSelectionButtonSprites = new List<Sprite> ();
		}
        if (hm.playerPortraitSprites == null)
        {
            hm.playerPortraitSprites = new List<Sprite>();
        }

        if (hm.playerSelectionButtonSprites.Count == 0 || hm.playerPortraitSprites.Count ==0) {

            hm.playerSelectionButtonSprites.Clear();
            hm.playerPortraitSprites.Clear();

			for (int j = 0; j < freeFighterLength; j++) {
                hm.playerSelectionButtonSprites.Add (null);
                hm.playerPortraitSprites.Add(null);
			}
		}

        //Defining foldout style
        GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
        myFoldoutStyle.fontStyle = FontStyle.Bold;
        myFoldoutStyle.fontSize = 12;
  
        EditorGUILayout.Space();
        showSelection = EditorGUILayout.Foldout(showSelection, "Player Selection Button Sprites",myFoldoutStyle);
        if (showSelection)
        {
            EditorGUILayout.BeginVertical("Box");

            for (int i = 0; i < hm.playerSelectionButtonSprites.Count; i++)
            {

                if (string.Equals(nameArr[i], "None"))
                    continue;
                EditorGUILayout.BeginHorizontal();
                hm.playerSelectionButtonSprites[i] = (Sprite)EditorGUILayout.ObjectField(nameArr[i], hm.playerSelectionButtonSprites[i], typeof(Sprite), false);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

//        EditorGUILayout.Space();
//        showPortraits = EditorGUILayout.Foldout(showPortraits, "Player Portrait Sprites",myFoldoutStyle);
//        if (showPortraits)
//        {
//            EditorGUILayout.BeginVertical("Box");
//
//            for (int i = 0; i < hm.playerPortraitSprites.Count; i++)
//            {
//
//                if (string.Equals(nameArr[i], "None"))
//                    continue;
//                EditorGUILayout.BeginHorizontal();
//                hm.playerPortraitSprites[i] = (Sprite)EditorGUILayout.ObjectField(nameArr[i], hm.playerPortraitSprites[i], typeof(Sprite), false);
//                EditorGUILayout.EndHorizontal();
//            }
//            EditorGUILayout.EndVertical();
//        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Other Toggle Sprites", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical ("Box");
        hm.hideSprite = (Sprite)EditorGUILayout.ObjectField("Hide Sprites", hm.hideSprite, typeof(Sprite));
        hm.unhideSprite = (Sprite)EditorGUILayout.ObjectField("Unhide Sprites", hm.unhideSprite, typeof(Sprite));
        hm.scopeSprite = (Sprite)EditorGUILayout.ObjectField("Scope Sprites", hm.scopeSprite, typeof(Sprite));
        hm.unscopeSprite = (Sprite)EditorGUILayout.ObjectField("Unscope Sprites", hm.unscopeSprite, typeof(Sprite));
        hm.hkitSprite = (Sprite)EditorGUILayout.ObjectField("HealthKit Sprites", hm.hkitSprite, typeof(Sprite));
        EditorGUILayout.EndVertical ();
	}
}
