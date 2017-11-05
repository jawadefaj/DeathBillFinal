using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(PlayerDMGModifierUpdate))]
public class PlayerDMGModifierUpdateEditor : Editor
{
    PlayerDMGModifierUpdate adm;
    bool showPlayerDmgMods = true;

    void OnEnable()
    {
        adm = (PlayerDMGModifierUpdate)target;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DrawCustomPlayerDamageModifier();

    }
    void DrawCustomPlayerDamageModifier()
    {
        showPlayerDmgMods = EditorGUILayout.Foldout(showPlayerDmgMods,"Player Damage Input Modifier");

        if (showPlayerDmgMods)
        {
            for (int i = 0; i < adm.playerDMGInMod.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical("Box");

                adm.playerDMGInMod[i].fighterID = (FighterRole)EditorGUILayout.EnumPopup("FighterID",adm.playerDMGInMod[i].fighterID);
                adm.playerDMGInMod[i].baseDamageMultiplier = EditorGUILayout.FloatField("Base Damage", adm.playerDMGInMod[i].baseDamageMultiplier);
                adm.playerDMGInMod[i].hideToBaseDamageRatio = EditorGUILayout.Slider("HBD Ratio", adm.playerDMGInMod[i].hideToBaseDamageRatio, 0, 1);
                EditorGUILayout.EndVertical();

                if (GUILayout.Button("-", new GUILayoutOption[]{ GUILayout.Width(30), GUILayout.Height(60) }))
                {
                    adm.playerDMGInMod.RemoveAt(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }

            if (GUILayout.Button("Add More", new GUILayoutOption[]{ GUILayout.Height(30) }))
            {
                PlayerDamageInputModifier pdim = new PlayerDamageInputModifier();
                adm.playerDMGInMod.Add(pdim);
            }
        }
    }

}
