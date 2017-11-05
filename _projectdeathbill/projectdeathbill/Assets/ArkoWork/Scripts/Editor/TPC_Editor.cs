using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ThirdPersonController))]
public class TPC_Editor : Editor {

    private ThirdPersonController tpc;
    private bool viewStructure = false;

    void OnEnable()
    {
        tpc = (ThirdPersonController)target;
    }

	public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        SerializedObject s_object = new SerializedObject(target);
        s_object.Update();
        SerializedProperty boneList = s_object.FindProperty("modelStructure.bones");
        EditorGUILayout.Space();
        viewStructure =  EditorGUILayout.Foldout(viewStructure, "Model Structure");
        if (viewStructure)
        {
            for (int i = 0; i < boneList.arraySize; i++)
            {
                SerializedProperty bone = boneList.GetArrayElementAtIndex(i);
                SerializedProperty boneTransform = bone.FindPropertyRelative("_boneRef");
                SerializedProperty name = bone.FindPropertyRelative("_name");
                EditorGUILayout.PropertyField(boneTransform, new GUIContent(name.stringValue));
            }
        }

        if (GUILayout.Button("Fill Model Structure"))
        {
            tpc.modelStructure.root = tpc.gameObject.transform;
            ModelStructure.FillModel(tpc.modelStructure);

            tpc.modelStructure.gun = tpc.assignedWeapon.transform;
        }

        s_object.ApplyModifiedProperties();
    }
}
