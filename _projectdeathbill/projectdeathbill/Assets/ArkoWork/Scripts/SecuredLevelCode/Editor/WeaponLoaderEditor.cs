using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Portbliss.LevelManagment;
using SimpleJSON;

[CustomEditor(typeof(WeaponLoader))]
public class WeaponLoaderEditor : Editor {

    private WeaponLoader wpLoader;


    void OnEnable()
    {
        wpLoader = (WeaponLoader)target;
    }

    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI();

        GUIStyle style = new GUIStyle();
        style = GUI.skin.box;
        style.wordWrap = true;
        style.alignment = TextAnchor.MiddleLeft;

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        if (UserGameData.instance != null)
        {
            EditorGUILayout.LabelField("Game Level Status Report",EditorStyles.boldLabel);
            EditorGUILayout.Space();
            for (int i = 0; i < LevelManager.GetLevelCount(); i++)
            {
                EditorGUILayout.LabelField(string.Concat("Level ",(i+1).ToString(),"\t\t",UserGameData.instance.GetLevelStatus(i+1).ToString()));
                EditorGUI.indentLevel++;
                for (int j = 0; j < LevelManager.GetLevel(i).GetCheckPointCount(); j++)
                {
                    EditorGUILayout.LabelField(string.Concat("Chk ",(j+1).ToString(),"\t\t",UserGameData.instance.IsCheckPointUnlocked(i,j).ToString()));
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(UserGameData.instance.GetRawData(),style);
        }
        else
        {
            EditorGUILayout.HelpBox("Data available only in run mode",MessageType.Info);
        }
        EditorGUILayout.EndVertical();

        //Editing data
        EditorGUILayout.Space();
        wpLoader.useModifiedGameData = EditorGUILayout.Toggle("Use Modified Game Data",wpLoader.useModifiedGameData,GUILayout.Width(200));

        if (wpLoader.useModifiedGameData)
        {
            DrawModifiedGameDataPanel();
        

            EditorGUILayout.Space();
            if (GUILayout.Button("Apply"))
            {
                string key = "";
                if (chkPointSelected > 0)
                {
                    key = GetLevelKey((int)selectedLvl, chkPointSelected-1);
                }
                else
                {
                    key = GetLevelKey((int)selectedLvl);
                }

                string value = "";
                if (selectedStatus == LevelStatus.NotReady)
                    value = UserGameData.KEY_ZERO;
                else if (selectedStatus == LevelStatus.Ready)
                    value = UserGameData.KEY_TWO;
                else
                    value = UserGameData.KEY_ONE;

                UpdateValue(key, value);

                //apply change on run mode
                if (UserGameData.instance != null)
                    UserGameData.instance.ManipulateData(wpLoader.modifiedGameData);
            }

            //show the data
            EditorGUILayout.LabelField(wpLoader.modifiedGameData,style);
        }
    }

    LevelID selectedLvl;
    int chkPointSelected =0;
    LevelStatus selectedStatus;
    void DrawModifiedGameDataPanel()
    {
        Dictionary<LevelID,Level> data = LevelManager.GetLevelData();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Level", GUILayout.Width(70));
        EditorGUILayout.LabelField("Checkpoint", GUILayout.Width(70));
        EditorGUILayout.LabelField("Status", GUILayout.Width(70));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        selectedLvl = (LevelID)EditorGUILayout.EnumPopup(selectedLvl,GUILayout.Width(70));
        chkPointSelected = EditorGUILayout.Popup(chkPointSelected,GetCheckpointList(data[selectedLvl]),GUILayout.Width(70));
        selectedStatus = (LevelStatus)EditorGUILayout.EnumPopup(selectedStatus,GUILayout.Width(70));
        EditorGUILayout.EndHorizontal();

    }

    private void UpdateValue(string key, string value)
    {
        JObject jData = JSON.Parse(wpLoader.modifiedGameData);
        //trying to add a new key could return a error for invalid json format
        try
        {
            jData.Add("k","v");
        }
        catch(System.Exception ex)
        {
            Debug.LogWarning(ex.Message);
            JSONClass jsClass = new JSONClass();
            jsClass.Add("k","v");
            wpLoader.modifiedGameData = jsClass.ToString();
        }
        jData.Add(key,value);

        Debug.Log(key + value);
        wpLoader.modifiedGameData = jData.ToString();
        Debug.Log(wpLoader.modifiedGameData);
    }

    private string[] GetCheckpointList(Level lvl)
    {
        int i = lvl.GetCheckPointCount();
        string[] stringAry = new string[i+1];

        stringAry[0] = "-None-";

        for (int j = 1; j <= i; j++)
        {
            stringAry[j] = "Check" + j.ToString();
        }

        return stringAry;
    }

    private string GetLevelKey(int lvl)
    {
        return string.Concat("lvl",(lvl+1).ToString());
    }

    private string GetLevelKey(int lvl, int chkp)
    {
        string chkKey = "";

        switch(chkp)
        {
            case 0:
                chkKey = "a";
                break;

            case 1: 
                chkKey = "b";
                break;

            case 2: 
                chkKey = "c";
                break;

            case 3: 
                chkKey = "d";
                break;
        }

        return string.Concat(GetLevelKey(lvl),chkKey);
    }
}
