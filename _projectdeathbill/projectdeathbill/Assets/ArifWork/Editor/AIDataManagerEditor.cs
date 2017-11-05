using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AIDataManager))]
public class AIDataManagerEditor : Editor {

    AIDataManager adm;
    bool showPlayerDmgMods = true;

    void OnEnable()
    {
        adm = (AIDataManager)target;
    }

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI ();
		if (GUILayout.Button ("Alert Everyone Available", GUILayout.Height (30))) {
			AIDataManager.SetTempTargetOnAllActivePersonnel (Selection.transforms[0]);
			AIDataManager.alertAvailableEnemies ();
		}

        //=============================================
        DrawCustomPlayerDamageModifier();

	}

    void DrawCustomPlayerDamageModifier()
    {
        showPlayerDmgMods = EditorGUILayout.Foldout(showPlayerDmgMods,"Player Damage Input Modifier");

        if (showPlayerDmgMods)
        {
            for (int i = 0; i < adm.playerDamageInputMods.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical("Box");
               
                adm.playerDamageInputMods[i].fighterID = (FighterRole)EditorGUILayout.EnumPopup("FighterID",adm.playerDamageInputMods[i].fighterID);
                adm.playerDamageInputMods[i].baseDamageMultiplier = EditorGUILayout.FloatField("Base Damage", adm.playerDamageInputMods[i].baseDamageMultiplier);
                adm.playerDamageInputMods[i].hideToBaseDamageRatio = EditorGUILayout.Slider("HBD Ratio", adm.playerDamageInputMods[i].hideToBaseDamageRatio, 0, 1);
                EditorGUILayout.EndVertical();

                if (GUILayout.Button("-", new GUILayoutOption[]{ GUILayout.Width(30), GUILayout.Height(60) }))
                {
                    adm.playerDamageInputMods.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }

            if (GUILayout.Button("Add More", new GUILayoutOption[]{ GUILayout.Height(30) }))
            {
                PlayerDamageInputModifier pdim = new PlayerDamageInputModifier();
                adm.playerDamageInputMods.Add(pdim);
            }
        }
    }

}
