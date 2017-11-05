using UnityEngine;
using UnityEditor;
using System.Collections;

public class MeshReplacer : EditorWindow {

    private MeshFilter original;
    private MeshFilter replaceWith;

    [MenuItem("Portbliss/Low Poly Mesh Replacer")]
    public static void LPM_Replacer()
    {
        EditorWindow.GetWindow(typeof(MeshReplacer));
    }

    void OnGUI()
    {
        
        original = (MeshFilter)EditorGUILayout.ObjectField("Find Mesh : ", original, typeof(MeshFilter), new GUILayoutOption[]{ });
        replaceWith = (MeshFilter)EditorGUILayout.ObjectField("Replace With : ", replaceWith, typeof(MeshFilter), new GUILayoutOption[]{ });

        if (GUILayout.Button("Find And Replace"))
        {
            FindAndReplace();
        }
    }

    void FindAndReplace()
    {
        Vector3 oScale = original.transform.localScale;
        Vector3 rScale = replaceWith.transform.localScale;

        Vector3 scaleFactor = new Vector3(rScale.x/oScale.x,rScale.y/oScale.y,rScale.z/oScale.z);

        MeshFilter[] meshes = GameObject.FindObjectsOfType<MeshFilter>();

        int c = 0;
        for(int i=0;i<meshes.Length;i++)            
        {
            MeshFilter mf = meshes[i];

            if (mf.sharedMesh == original.sharedMesh)
            {
                if (mf.gameObject.tag.Equals("Skip"))
                    continue;
                if (mf.gameObject == original.gameObject)
                    continue;
                //found a matching mesh
                GameObject newObj = (GameObject)Instantiate(replaceWith.gameObject,mf.transform.parent);
                newObj.name = "PPAP";
                newObj.transform.localPosition = mf.transform.localPosition;
                newObj.transform.localRotation = mf.transform.localRotation;
                newObj.transform.localScale = Vector3.Scale(mf.transform.localScale, scaleFactor);

                DestroyImmediate(mf.gameObject);
                c++;
            }
        }

        //change the source also
        {
            MeshFilter mf = original;
            GameObject newObj = (GameObject)Instantiate(replaceWith.gameObject, mf.transform.parent);
            newObj.name = "PPAP";
            newObj.transform.localPosition = mf.transform.localPosition;
            newObj.transform.localRotation = mf.transform.localRotation;
            newObj.transform.localScale = Vector3.Scale(mf.transform.localScale, scaleFactor);

            DestroyImmediate(mf.gameObject);
            c++;
        }

        Debug.Log("Total removed "+c.ToString());
    }
}
