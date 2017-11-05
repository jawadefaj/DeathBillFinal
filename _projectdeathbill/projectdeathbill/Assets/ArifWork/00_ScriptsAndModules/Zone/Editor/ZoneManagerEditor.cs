using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

[CustomEditor(typeof(ZoneManager))]
public class ZoneManagerEditor : Editor {

    ZoneManager zm;

    void OnEnable()
    {
        zm = (ZoneManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //------
        DrawAdvanteList();

        if (GUILayout.Button("Apply"))
        {
            if (EditorUtility.DisplayDialog("Zone Changer", "Are you sure you want to apply these advantage settings to all zones!?", "Yup!", "Hell No!"))
            {
                ApplyChanges();
            }

        }
    }

    void ApplyChanges()
    {
        Zone[] zones = GameObject.FindObjectsOfType<Zone>();
        if (zm.sampleZone != null)
        {
            Zone z = zm.sampleZone.GetComponent<Zone>();
            if (!z.lockAdvantageValues)
            {
                z.AdvantageAgainstFighters = new List<float>();
                for (int i = 0; i < zm.advantageValues.Count; i++)
                    z.AdvantageAgainstFighters.Add(zm.advantageValues[i]);
            }
        }
        foreach (Zone z in zones)
        {
            if (!z.lockAdvantageValues)
            {
                z.AdvantageAgainstFighters = new List<float>();
                for (int i = 0; i < zm.advantageValues.Count; i++)
                    z.AdvantageAgainstFighters.Add(zm.advantageValues[i]);
            }
        }
    }
    void DrawAdvanteList()
    {
        int freeFighterLength = Enum.GetNames (typeof(FighterRole)).Length;
        string[] nameArr = Enum.GetNames (typeof(FighterRole));

        if (zm.advantageValues == null) 
        {
            zm.advantageValues= new List<float> ();
        }

        zm.AddRemoveAdvantages ();
     
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical("Box");

        for (int i = 1; i < zm.advantageValues.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            zm.advantageValues [i] = EditorGUILayout.Slider(nameArr[i],zm.advantageValues[i],0,1);
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }
}
