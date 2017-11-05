using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ModelStructureKeeper))]
public class ModelStructureKeeperEditor : Editor {

    private bool viewStructure = true;

    public override void OnInspectorGUI()
    {
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

        ModelStructureKeeper msk = (ModelStructureKeeper)target;
        if (GUILayout.Button("Fill Model Structure"))
        {
            msk.modelStructure.root = msk.gameObject.transform;
            ModelStructure.FillModel(msk.modelStructure);

        }

        if (msk.modelStructure.gun == null)
        {
            EditorGUILayout.HelpBox("Assign Gun Transform!!!", MessageType.Error);
        }

        s_object.ApplyModifiedProperties();
    }
}
