using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PlayerDataManager))]
public class PlayerDataManagerEditor : Editor {

    private SerializedObject s_object;
    private SerializedProperty theList;
    private FighterName fName;

    void OnEnable()
    {
        s_object = new SerializedObject(target);
        theList = s_object.FindProperty("dataList");
        if (theList == null)
            Debug.Log("hhhh");
    }

    public override void OnInspectorGUI()
    {
        s_object.Update();

        //the add area
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        fName = (FighterName)EditorGUILayout.EnumPopup("Select A Player",fName, GUILayout.Width(250));
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Add", GUILayout.Width(100)))
        {
            AddItem();
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

    void AddItem()
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
            Debug.LogWarning("The player already exists. Cant add more");
            return;
        }

        theList.InsertArrayElementAtIndex(0);
        SetDefaultValues(theList.GetArrayElementAtIndex(0));
    }

    void SetDefaultValues(SerializedProperty element)
    {
        SerializedProperty sp_name = element.FindPropertyRelative("fName");
        SerializedProperty sp_headCamera = element.FindPropertyRelative("headCameraPos");
        SerializedProperty sp_shoulderCamera = element.FindPropertyRelative("shoulderCameraPos");
        SerializedProperty sp_personalZoomCamera = element.FindPropertyRelative("personalZoomValue");

        sp_name.enumValueIndex = (int)fName;
        sp_headCamera.vector3Value = Vector3.zero;
        sp_shoulderCamera.vector3Value = Vector3.zero;
        sp_personalZoomCamera.floatValue = 60f;
    }

    void DrawItem(SerializedProperty element, int index)
    {
        SerializedProperty sp_name = element.FindPropertyRelative("fName");
        SerializedProperty sp_headCamera = element.FindPropertyRelative("headCameraPos");
        SerializedProperty sp_shoulderCamera = element.FindPropertyRelative("shoulderCameraPos");
        SerializedProperty sp_personalZoomCamera = element.FindPropertyRelative("personalZoomValue");
        SerializedProperty sp_headCameraRot = element.FindPropertyRelative("headCameraRot");
        SerializedProperty sp_shoulderCameraRot = element.FindPropertyRelative("shoulderCameraRot");


        EditorGUILayout.BeginHorizontal();
        string title = string.Concat((index+1).ToString(), ". ", sp_name.enumNames[sp_name.enumValueIndex]);
        EditorGUILayout.LabelField( title,EditorStyles.boldLabel);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical("Box");

        EditorGUILayout.LabelField("Camera Values",EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Head View Camera", new GUILayoutOption[]{});
        EditorGUILayout.Vector3Field("Position", sp_headCamera.vector3Value, new GUILayoutOption[]{ });
        EditorGUILayout.Vector3Field("Rotation", sp_headCameraRot.quaternionValue.eulerAngles, new GUILayoutOption[]{});
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Shoulder View Camera", new GUILayoutOption[]{});
        EditorGUILayout.Vector3Field("Position", sp_shoulderCamera.vector3Value, new GUILayoutOption[]{ });
        EditorGUILayout.Vector3Field("Rotation", sp_shoulderCameraRot.quaternionValue.eulerAngles, new GUILayoutOption[]{});


        if (GUILayout.Button("Pick Camera Values"))
        {
            ThirdPersonController tpc = FindTPC(element);
            sp_headCamera.vector3Value = tpc.headViewCamera.localPosition;
            sp_headCameraRot.quaternionValue = tpc.headViewCamera.localRotation;

            sp_shoulderCamera.vector3Value = tpc.shoulderViewCamera.localPosition;
            sp_shoulderCameraRot.quaternionValue = tpc.shoulderViewCamera.localRotation;
        }


        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Others",EditorStyles.boldLabel);
        DrawProperty(sp_personalZoomCamera);

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

   /* void DrawVector3(SerializedProperty sp)
    {
        SerializedProperty x = sp.FindPropertyRelative("x");
        SerializedProperty y = sp.FindPropertyRelative("y");
        SerializedProperty z = sp.FindPropertyRelative("z");
        string title;

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Position",GUILayout.Width(100) );
        title = string.Concat("X : ",x.floatValue);
        EditorGUILayout.LabelField(title,GUILayout.Width(100) );

        title = string.Concat("Y : ",y.floatValue);
        EditorGUILayout.LabelField(title,GUILayout.Width(100) );

        title = string.Concat("Z : ",z.floatValue);
        EditorGUILayout.LabelField(title,GUILayout.Width(100) );
        EditorGUILayout.EndHorizontal();
    }*/



    ThirdPersonController FindTPC(SerializedProperty sp)
    {
        FighterName _name = (FighterName)sp.FindPropertyRelative("fName").enumValueIndex;

        PlayerInputController pic = GameObject.FindObjectOfType<PlayerInputController>();
        ThirdPersonController tpc = pic.GetPlayerByID(_name);

        if (tpc == null)
        {
            Debug.LogError("No such player exist in player input controller list!!");
            return null;
        }

        if (tpc.headViewCamera == null || tpc.shoulderViewCamera == null)
        {
            Debug.LogError("No camera refenrece found for the player");
            return null;
        }

        return tpc;
    }
}
