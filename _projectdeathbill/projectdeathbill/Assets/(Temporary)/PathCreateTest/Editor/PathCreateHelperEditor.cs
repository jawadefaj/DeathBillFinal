using UnityEngine;
using UnityEditor;
using System.Collections;
using Portbliss.EditorTools;

[CustomEditor(typeof(PathCreateHelper))]
public class PathCreateHelperEditor : Editor {

    bool isPlacing = false;
    PathCreateHelper manager;
    GameObject pathCreateHelperGO;
    GameObject pathHolder;
    int pathIndex = 0;
    //int permetereIndex =0;

    void OnEnable()
    {
        manager = (PathCreateHelper)target;
        pathCreateHelperGO = manager.gameObject;
    }

    public void OnSceneGUI()
    {
        if (Event.current.type != EventType.keyDown || !isPlacing) return;


        if (Event.current.keyCode == KeyCode.P)
        {
            Event.current.Use();
            //cast a ray against mouse position
            Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(worldRay, out hitInfo))
            {
                GameObject go = new GameObject("HitPoint");
                go.transform.position = hitInfo.point;
                go.transform.SetParent(pathHolder.transform);
                pathHolder.GetComponent<Path>().AddPoint(go);
            }
            else
            {
                Debug.LogWarning("No Collider Found!");
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (!isPlacing)
        {
            if (GUILayout.Button("Start Positioning",GUILayout.Height(30)))
            {
                isPlacing = true;
                pathHolder = new GameObject("NewJalika " + pathIndex.ToString());
                pathHolder.AddComponent<Path>().Initiate();
                pathHolder.transform.SetParent(pathCreateHelperGO.transform);
                GetSceneView().Focus();
            }
        }
        else
        {
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("End Positioning",GUILayout.Height(30)))
            {
                isPlacing = false;
                pathHolder.GetComponent<Path>().drawMesh = true;
            }
            GUI.backgroundColor = Color.white;
        }
    }

    public static SceneView GetSceneView()
    {
        SceneView view = SceneView.lastActiveSceneView;
        if (view == null)
            view = EditorWindow.GetWindow<SceneView>();

        return view;
    }
}
