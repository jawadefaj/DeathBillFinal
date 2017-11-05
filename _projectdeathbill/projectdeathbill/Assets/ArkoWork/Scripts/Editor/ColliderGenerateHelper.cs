using UnityEngine;
using UnityEditor;
using System.Collections;

public class ColliderGenerateHelper : EditorWindow {

    private Transform colliderCollector;

    [MenuItem("Portbliss/Collider Generate Helper")]
    public static void CG_Helper()
    {
        EditorWindow.GetWindow(typeof(ColliderGenerateHelper));
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Collider Generator Helper", EditorStyles.boldLabel);
        colliderCollector = (Transform)EditorGUILayout.ObjectField("Collider Collector", colliderCollector, typeof(Transform), new GUILayoutOption[]{ });
        EditorGUILayout.Space();

        if (GUILayout.Button("Collect Collider"))
        {
            CollectCollider();
        }
    }

    void CollectCollider()
    {
        if (colliderCollector == null)
        {
            Debug.LogError("No collider collector");
            return;
        }


        if (Selection.activeGameObject == null)
        {
            Debug.LogError("No selection");
            return;
        }

        GameObject go = Selection.activeGameObject;
        if (go.GetComponent<Collider>() == null)
        {
            Debug.LogError("No collider attached");
            return;
        }

        //create empty
        GameObject c = new GameObject("Collider");
        c.transform.SetParent(go.transform);
        c.transform.localPosition = Vector3.zero;
        c.transform.localRotation = Quaternion.identity;
        c.transform.localScale = Vector3.one;

        CopyCollider(c,go);

        c.transform.SetParent(colliderCollector);

        c.SetLayer("Obstackle", true);
    }


    void CopyCollider(GameObject target, GameObject sample)
    {
        if (sample.GetComponent<BoxCollider>() != null)
        {
            CopyComponent(target, sample, typeof(BoxCollider));
            DestroyImmediate(sample.GetComponent<BoxCollider>());
            return;
        }

        if (sample.GetComponent<SphereCollider>() != null)
        {
            CopyComponent(target, sample, typeof(SphereCollider));
            DestroyImmediate(sample.GetComponent<SphereCollider>());
            return;
        }

        if (sample.GetComponent<CapsuleCollider>() != null)
        {
            CopyComponent(target, sample, typeof(CapsuleCollider));
            DestroyImmediate(sample.GetComponent<CapsuleCollider>());
            return;
        }

        Debug.Log("Specific collider not found");
    }

    void CopyComponent(GameObject target, GameObject sample, System.Type type)
    {
        Component comp = target.GetComponent(type); 
        if (comp == null) comp = target.AddComponent(type);
        EditorUtility.CopySerialized(sample.GetComponent(type),comp);
    }
}
