using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AIPlayerDataManager))]
public class AIPlayerDataManagerEditor : Editor {

    private AIPlayerDataManager ai_dm;

    private SerializedObject s_object;
    private SerializedProperty theList;
    private FighterName fName;

    void OnEnable()
    {
        ai_dm = (AIPlayerDataManager)target;
        s_object = new SerializedObject(target);
        theList = s_object.FindProperty("dataList");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        ai_dm.scenePlayerAIDamageMultiplier =  EditorGUILayout.FloatField("AI Damage Multi",ai_dm.scenePlayerAIDamageMultiplier, new GUILayoutOption[]{});


        s_object.Update();

        //the add area
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        fName = (FighterName)EditorGUILayout.EnumPopup("Select A Player",fName, GUILayout.Width(250));
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Add", GUILayout.Width(100)))
        {
            AddRole();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();


        //draw the element
        for (int i = 0; i < theList.arraySize; i++)
        {
            DrawItem(theList.GetArrayElementAtIndex(i), i);
            EditorGUILayout.Space();
        }

        s_object.ApplyModifiedProperties();
    }

    void AddRole()
    {
        bool alreadyExists = false;

        SerializedProperty sp_role;
        for (int i = 0; i < theList.arraySize; i++)
        {
            sp_role = theList.GetArrayElementAtIndex(i).FindPropertyRelative("fName");

            if (sp_role.enumValueIndex == (int)fName)
                alreadyExists = true;
        }

        if (alreadyExists)
        {
            Debug.LogWarning("The role already exists. Cant add more");
            return;
        }

        theList.InsertArrayElementAtIndex(0);
        SetDefaultValues(theList.GetArrayElementAtIndex(0));
    }

    void SetDefaultValues(SerializedProperty element)
    {
        SerializedProperty sp_role = element.FindPropertyRelative("fName");
        SerializedProperty sp_accurecy = element.FindPropertyRelative("aiAccuracy");
        SerializedProperty sp_bullet = element.FindPropertyRelative("avgBulletFireInAI");
        SerializedProperty sp_bulletVariation = element.FindPropertyRelative("AIBulletFireVariaton");
        SerializedProperty sp_delay = element.FindPropertyRelative("avgDelayTimeInAI");
        SerializedProperty sp_delayVariation = element.FindPropertyRelative("AIDelayVariaton");
        SerializedProperty sp_maxRang = element.FindPropertyRelative("AIMaxShootingRangeSq");
        SerializedProperty sp_regenAgg = element.FindPropertyRelative("regenRate_Aggressive");
        SerializedProperty sp_regenNonAgg = element.FindPropertyRelative("regenRate_NonAggressive");
        SerializedProperty sp_regenOffCont = element.FindPropertyRelative("regenRate_OffControl");
        SerializedProperty sp_regenAI = element.FindPropertyRelative("regenRate_ai");

        sp_role.enumValueIndex = (int)fName;
        sp_accurecy.floatValue = 0.4f;
        sp_bullet.intValue = 10;
        sp_bulletVariation.floatValue = 0.1f;
        sp_delay.floatValue = 1;
        sp_delayVariation.floatValue = 0.1f;
        sp_maxRang.floatValue = 400;
        sp_regenAgg.floatValue = 1;
        sp_regenNonAgg.floatValue = 5;
        sp_regenOffCont.floatValue = 8;
        sp_regenAI.floatValue = 2.5f;
    }

    void DrawItem(SerializedProperty element, int index)
    {
        SerializedProperty sp_role = element.FindPropertyRelative("fName");
        SerializedProperty sp_accurecy = element.FindPropertyRelative("aiAccuracy");
        SerializedProperty sp_bullet = element.FindPropertyRelative("avgBulletFireInAI");
        SerializedProperty sp_bulletVariation = element.FindPropertyRelative("AIBulletFireVariaton");
        SerializedProperty sp_delay = element.FindPropertyRelative("avgDelayTimeInAI");
        SerializedProperty sp_delayVariation = element.FindPropertyRelative("AIDelayVariaton");
        SerializedProperty sp_maxRang = element.FindPropertyRelative("AIMaxShootingRangeSq");
        SerializedProperty sp_regenAgg = element.FindPropertyRelative("regenRate_Aggressive");
        SerializedProperty sp_regenNonAgg = element.FindPropertyRelative("regenRate_NonAggressive");
        SerializedProperty sp_regenOffCont = element.FindPropertyRelative("regenRate_OffControl");
        SerializedProperty sp_regenAI = element.FindPropertyRelative("regenRate_ai");

        EditorGUILayout.BeginHorizontal();
        string title = string.Concat((index+1).ToString(), ". ", sp_role.enumNames[sp_role.enumValueIndex]);
        EditorGUILayout.LabelField( title,EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical("Box");

        EditorGUILayout.LabelField("AI Shooting Parameters",EditorStyles.boldLabel);
        DrawSlideProperty(sp_accurecy,0,1);
        DrawProperty(sp_bullet);
        DrawSlideProperty(sp_bulletVariation,0,1);
        DrawProperty(sp_delay);
        DrawSlideProperty(sp_delayVariation,0,1);
        DrawProperty(sp_maxRang);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("AI Regenaration Parameters",EditorStyles.boldLabel);
        DrawProperty(sp_regenAgg);
        DrawProperty(sp_regenNonAgg);
        DrawProperty(sp_regenOffCont);
        DrawProperty(sp_regenAI);

        EditorGUILayout.EndVertical();
    }

    void DrawProperty(SerializedProperty sp)
    {
        EditorGUILayout.PropertyField(sp,new GUILayoutOption[]{});
    }

    void DrawSlideProperty(SerializedProperty sp, float leftValue, float rightValue)
    {
        EditorGUILayout.Slider(sp, leftValue, rightValue, new GUILayoutOption[]{ });
    }

   
}
