using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Portbliss.EditorTools;

[CustomEditor(typeof(ZoneBlock))]
public class ZoneBlockEditor : Editor {
    ZoneBlock thisZoneBlock;

    void OnEnable()
    {
        thisZoneBlock = (ZoneBlock)target;
    }


    void InitializeZoneBlock()
    {
        ZoneBlock zs = Selection.activeGameObject.GetComponent<ZoneBlock> ();
        if (zs.zoneManagerRef == null) 
        {
            ZoneManager zm = FindObjectOfType<ZoneManager> ();
            if (zm != null) {
                zs.zoneManagerRef = zm.gameObject;
            } 
            else 
            {
                zs.zoneManagerRef = (Instantiate (zs.zoneManagerPrefab) as GameObject);
                zs.zoneManagerRef.name = "ZoneManager";
            }
        }
    }
    void RefreshZoneBlock()
    {
        Transform sel = (Selection.activeObject as GameObject).transform;
        ZoneBlock zoneSet = sel.GetComponent<ZoneBlock> ();
        if (!zoneSet.isInitialized)
        {
            Debug.LogError("ZoneBlock is not initialized!");
            return;
        }
        zoneSet.spwnList.Clear();
        if (zoneSet.zParent != null)
        {
            foreach(Transform tr in zoneSet.zParent)
            {
                //Debug.Log (" asd");
                Zone z = tr.GetComponent<Zone> ();
                if(z !=null)
                {
                    z.zoneConnections.Clear ();
                    z.spwnOnly = true;
                    if (!zoneSet.spwnList.Contains (z))
                    {
                        zoneSet.spwnList.Add (z);
                    }
                }
            }

            if (zoneSet.cParent != null)
            {
                foreach(Transform tr in zoneSet.cParent)
                {
                    ZoneConnectionKeep zoneConKeeper = tr.GetComponent<ZoneConnectionKeep> ();
                    ZoneConnection zc = new ZoneConnection ();
                    zc.endZone = zoneConKeeper.endZone;
                    zc.rootZone = zoneConKeeper.startZone;
                    zc.isFromType = false;
                    zc.zoneRoad = new ZoneRoad ();
                    zc.zoneRoad.side1 = zoneConKeeper.zoneRoad.side1;
                    zc.zoneRoad.side2 = zoneConKeeper.zoneRoad.side2;
                    zoneConKeeper.startZone.zoneConnections.Add (zc);
                    zc.endZone.spwnOnly = false;
                    if (zoneSet.spwnList.Contains(zc.endZone))zoneSet.spwnList.Remove(zc.endZone);


                    if (zoneConKeeper.isBidirectional) 
                    {
                        zc = new ZoneConnection ();
                        zc.endZone = zoneConKeeper.startZone;
                        zc.rootZone = zoneConKeeper.endZone;
                        zc.isFromType = true;
                        zc.zoneRoad = new ZoneRoad ();
                        zc.zoneRoad.side1 = zoneConKeeper.zoneRoad.side1;
                        zc.zoneRoad.side2 = zoneConKeeper.zoneRoad.side2;
                        zoneConKeeper.endZone.zoneConnections.Add (zc);
                        zc.endZone.spwnOnly = false;
                        if (zoneSet.spwnList.Contains(zc.endZone))zoneSet.spwnList.Remove(zc.endZone);
                    }
                }
            }
        }

        if (zoneSet.zParent != null)
        {
            foreach (Transform zt in zoneSet.zParent)
            {
                Zone z = zt.GetComponent<Zone>();
                if (z != null)
                {
                    if (z.slots == null)
                        break;
                    if (z.slots.Count == 0)
                        break;
                    float xAvg = 0;
                    float zAvg = 0;
                    Transform prevParent = z.slots[0].parent;
                    for (int i = 0; i < z.slots.Count; i++)
                    {
                        z.slots[i].parent = null;
                        xAvg += z.slots[i].position.x;
                        zAvg += z.slots[i].position.z;
                    }
                    xAvg /= z.slots.Count;
                    zAvg /= z.slots.Count;
                    zt.position = new Vector3(xAvg, zt.position.y, zAvg);
                    prevParent.position = zt.position;
                    float rmsDist = 0;
                    float maxSqrDist = 0;
                    foreach (Transform zslot in z.slots)
                    {
                        zslot.parent = prevParent;
                        float sqrMag = zslot.localPosition.sqrMagnitude;
                        rmsDist += sqrMag;
                        if (maxSqrDist < sqrMag)
                        {
                            maxSqrDist = sqrMag;
                        }
                    }
                    rmsDist /= z.slots.Count;
                    rmsDist = Mathf.Sqrt(rmsDist);
//                Debug.Log(maxSqrDist);
//                Debug.Log(rmsDist);
                    z.zoneRadius = (rmsDist * 1.5f + Mathf.Sqrt(maxSqrDist)) / 2;
                }
            }
        }
        PlaceAllToGround(zoneSet.transform,100); 
    }

    void PlaceAllToGround(Transform tr, float scanHeight)
    {
        Zone z = tr.GetComponent<Zone>();
        if (z != null)
        {
            if (z.dontGroundOnRefresh)
            {
                return;
            }
        }

        PlaceToGround(tr,scanHeight);
        foreach (Transform item in tr)
        {
            PlaceAllToGround(item, scanHeight);
        }
    }

    bool PlaceToGround(Transform trans,float scanHeight)
    {
        if (trans == null)
        {
            Debug.LogError("Transform is null! Cannot place to ground!");
            return false;
        }

        Vector3 tempV = trans.position;
        tempV.y += scanHeight;
        RaycastHit rchit = new RaycastHit();
        if (Physics.Raycast(tempV, Vector3.down, out rchit, scanHeight * 2))
        {
            trans.position = rchit.point;
            return true;
        }
        else
        {
            return false;
        }

    }
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI ();

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

       
        if (GUILayout.Button ("Initialize", new GUILayoutOption[]{ GUILayout.Height(30)})) 
        {
            InitializeZoneBlock();
        } 
        if ( GUILayout.Button ("Refresh",  new GUILayoutOption[]{ GUILayout.Height(30)})) 
		{
            RefreshZoneBlock();
		}

        EditorGUILayout.EndHorizontal();
        //EditorGUILayout.Space();
        EditorGUILayout.Space();


        EditorGUILayout.LabelField("Create Zones", EditorStyles.boldLabel);
        if (!isPlacingZones)
        {
            if (GUILayout.Button("+Zones",GUILayout.Height(30)))
            {
                isPlacingZones = true;

                GetSceneView().Focus();
            }
        }
        else
        {
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Done!",GUILayout.Height(30)))
            {
                isPlacingZones = false;
            }
            GUI.backgroundColor = Color.white;
        }

        //EditorGUILayout.Space();
        EditorGUILayout.Space();
        //EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Connect Zones", EditorStyles.boldLabel);
        //EditorGUILayout.Space();


        fromZoneIndex = EditorGUILayout.Popup("Start Zone", fromZoneIndex, GetZoneNameList());
        toZoneIndex = EditorGUILayout.Popup("End Zone", toZoneIndex, GetZoneNameList());
        if (thisZoneBlock.zParent != null)
        {
            fromZone = thisZoneBlock.zParent.GetChild(fromZoneIndex).GetComponent<Zone>();
            toZone = thisZoneBlock.zParent.GetChild(toZoneIndex).GetComponent<Zone>();
        }
           
        oneDir = EditorGUILayout.Toggle("Is One Directional", oneDir);

        if (!isConnectingZones)
        {
            string msg="";
            if (!IsReadyToDrawPath(ref msg))
            {
                EditorGUILayout.HelpBox(msg, MessageType.Warning);
            }
            else
            {
                if (GUILayout.Button("+Path", GUILayout.Height(30)))
                {
                    isConnectingZones = true;

                    ZoneBlock zBlock = (ZoneBlock)target;
                    if (zBlock.cParent == null)
                    {
                        GameObject cPar = new GameObject("Connections");
                        zBlock.cParent = cPar.transform;
                        cPar.transform.parent = zBlock.transform;
                        cPar.transform.position = zBlock.transform.position;
                    }

                    Transform c = (new GameObject(fromZone.name + "-" + toZone.name)).transform;
                    c.position = (fromZone.transform.position + toZone.transform.position) / 2;
                    c.parent = zBlock.cParent.transform;
                    lastConenction = c.gameObject.AddComponent<ZoneConnectionKeep>();
                    lastConenction.drawOptions = new ZCKDrawOptions();
                    lastConenction.Initiate();
                    lastConenction.startZone = fromZone;
                    lastConenction.endZone = toZone;
                    lastConenction.isBidirectional = !oneDir;
                    GetSceneView().Focus();
                }
            }
        }
        else
        {
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Finish",GUILayout.Height(30)))
            {
                isConnectingZones = false;
                lastConenction.LoadZoneRoadsFromLists();
                if (lastConenction.zoneRoad.side1.waypoints.Length < 2 || lastConenction.zoneRoad.side1.waypoints.Length != lastConenction.zoneRoad.side2.waypoints.Length )
                {
                    Object.DestroyImmediate(lastConenction.gameObject);
                }
                lastConenction.MakeReadyToDraw();
                RefreshZoneBlock();

            }
            GUI.backgroundColor = Color.white;
        }

       // EditorGUILayout.EndVertical();
	}



    int fromZoneIndex=0;
    int toZoneIndex=0;
    Zone fromZone;
    Zone toZone;
    bool oneDir =true;

    public void OnSceneGUI()
    {

        if (isPlacingZones)
        {
            if (Event.current.type != EventType.keyDown) return;


            if (Event.current.keyCode == KeyCode.P)
            {
                Event.current.Use();
                //cast a ray against mouse position
                Ray worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit hitInfo;

                if (Physics.Raycast(worldRay, out hitInfo))
                {
                    ZoneBlock zBlock = (ZoneBlock)target;
                    if (zBlock.zParent == null)
                    {
                        GameObject zPar = new GameObject("Zones");
                        zBlock.zParent = zPar.transform;
                        zPar.transform.parent = zBlock.transform;
                        zPar.transform.position = zBlock.transform.position;
                    }

                    int currentZoneCount = zBlock.zParent.childCount;
                    string suggestedName="";
                    bool nameRejected;
                    for (int i = 0; i <= currentZoneCount; i++)
                    {
                        suggestedName = "Z(" + i.ToString() + ")";
                        nameRejected = false;
                        foreach(Transform ztran in zBlock.zParent)
                        {
                            if (ztran.name == suggestedName)
                            {
                                nameRejected = true;
                                break;
                            } 
                        }
                        if (!nameRejected)
                            break;
                    }
                    Transform z = (Instantiate(zBlock.sampleZone) as GameObject).transform;
                    z.name = suggestedName;
                    z.position = hitInfo.point;
                    z.parent = zBlock.zParent;




                }
                else
                {
                    Debug.LogWarning("No Collider Found!");
                }
            }
        }
        else if (isConnectingZones)
        {
            //display Zone label
            GUIStyle style = new GUIStyle();
            GUI.backgroundColor = new Color(1,1,1,.4f);
            style = GUI.skin.box;
            style.fontSize = 12;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.black;

            Handles.Label(fromZone.transform.position , "Start", style);
            Handles.Label(toZone.transform.position , "End", style);
            /*//begin 2D GUI block
            Handles.BeginGUI();
            //translate waypoint vector3 position in world space into a position on the screen
            var guiPoint = HandleUtility.WorldToGUIPoint(fromZone.transform.position);
            //create rectangle with that positions and do some offset
            var rect = new Rect(guiPoint.x - 50.0f, guiPoint.y - 40, 100, 20);
            //draw box at position with current waypoint name
            GUI.Box(rect, "Starte Position",style);
            Handles.EndGUI(); //end GUI block*/


            if (Event.current.type != EventType.keyDown) return;


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
                    go.transform.SetParent(lastConenction.transform);
                    lastConenction.AddPoint(go.transform);
//                    ZoneBlock zBlock = (ZoneBlock)target;
//                    if (zBlock.zParent == null)
//                    {
//                        GameObject zPar = new GameObject("Zones");
//                        zBlock.zParent = zPar.transform;
//                        zPar.transform.parent = zBlock.transform;
//                        zPar.transform.position = zBlock.transform.position;
//                    }
//
//
//                    Transform z = (Instantiate(zBlock.zonePrefab) as GameObject).transform;
//                    z.name = "Zone (" + zBlock.zParent.childCount.ToString() + ")";
//                    z.position = hitInfo.point;
//                    z.parent = zBlock.zParent;
                }
                else
                {
                    Debug.LogWarning("No Collider Found!");
                }
            }
        }
        else
        {
            //display Zone label
            GUIStyle style = new GUIStyle();
            style = GUI.skin.box;
            GUI.backgroundColor = new Color(1,1,1,.4f);
            style.normal.textColor = Color.black;
            style.fontSize = 10;
            style.fontStyle = FontStyle.Bold;
           
            if (thisZoneBlock.zParent != null)
            {
                for (int i = 0; i < thisZoneBlock.zParent.childCount; i++)
                {
                    Transform t = thisZoneBlock.zParent.GetChild(i);
                    Handles.Label(t.position, t.name, style);

                }
            }
                
        }


       
    }
    bool isPlacingZones = false;
    bool isConnectingZones = false;
    ZoneConnectionKeep lastConenction;

    public static SceneView GetSceneView()
    {
        SceneView view = SceneView.lastActiveSceneView;
        if (view == null)
            view = EditorWindow.GetWindow<SceneView>();

        return view;
    }

    public string[] GetZoneNameList()
    {
        ZoneBlock zBlock = (ZoneBlock)target;

        if (zBlock.zParent == null)
            return new string[]{ "No Zone Added" };
        else if (zBlock.zParent.childCount == 0)
            return new string[] { "No Zone Added"};
        else
        {
            string[] list = new string[zBlock.zParent.childCount];

            for(int i=0;i<list.Length;i++)
                list[i] = zBlock.zParent.GetChild(i).name;

            return list;
        }
    }

    public bool IsReadyToDrawPath(ref string message)
    {
        bool valid = true;

        if (fromZoneIndex == toZoneIndex)
        {
            message = "Both Zones are equal. Please select two different zones.";
            valid = false;
        }

        if (fromZone == null || toZone == null)
        {
            message = "Not Enough zones to create connection. Create some zones first.";
            valid = false;
            return valid;
        }

        for (int i = 0; i < fromZone.zoneConnections.Count; i++)
        {
            if (fromZone.zoneConnections[i].endZone == toZone)
            {
                message ="Zone Connection already exists. Please try a new combination.";
                valid = false;
            }
        }

        return valid;
    }


}
