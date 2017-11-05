using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZoneConnectionKeep : MonoBehaviour {

	public Zone startZone;
	public Zone endZone;
	public ZoneRoad zoneRoad;
	public bool isBidirectional;

    public ZCKDrawOptions drawOptions;

	[SerializeField] [HideInInspector] List<Transform> leftBorderPoints;
	[SerializeField] [HideInInspector] List<Transform> rightBorderPoints;
    public void LoadZoneRoadsFromLists()
    {
        zoneRoad = new ZoneRoad();

        GameObject side1 = new GameObject("Side1");
        side1.transform.position = this.transform.position;
        side1.transform.parent = this.transform;
        zoneRoad.side1 = side1.AddComponent<SWS.PathManager>();
        zoneRoad.side1.waypoints = new Transform[leftBorderPoints.Count];
        for (int i = 0; i < leftBorderPoints.Count; i++)
        {
            zoneRoad.side1.waypoints[i] = leftBorderPoints[i];
            leftBorderPoints[i].parent = side1.transform;
        }

        GameObject side2 = new GameObject("Side2");
        side2.transform.position = this.transform.position;
        side2.transform.parent = this.transform;
        zoneRoad.side2 = side2.AddComponent<SWS.PathManager>();
        zoneRoad.side2.waypoints = new Transform[rightBorderPoints.Count];
        for (int i = 0; i < rightBorderPoints.Count; i++)
        {
            zoneRoad.side2.waypoints[i] = rightBorderPoints[i];
            rightBorderPoints[i].parent = side2.transform;
        }
        side1.SetActive(false);
        side2.SetActive(false);
    }

    public void Initiate()
    {
        leftBorderPoints = new List<Transform>();
        rightBorderPoints = new List<Transform>();
    }

    public void AddPoint(Transform point)
    {
        if (leftBorderPoints.Count == rightBorderPoints.Count)
        {
            leftBorderPoints.Add(point);
        }
        else
        {
            rightBorderPoints.Add(point);
        }
    }

    void OnDrawGizmos()
    {
        //if (CustomGizmoSwitch.CanDrawOnScene == false)
        //    return;
        
        if (leftBorderPoints == null || rightBorderPoints == null)
            return;
        if (leftBorderPoints.Count == 0 || rightBorderPoints.Count == 0)
            return;
        #if UNITY_EDITOR
        if (UnityEditor.Selection.activeGameObject == this.gameObject)
        {
            GUIStyle style = new GUIStyle();
            GUI.backgroundColor = new Color(1,1,1,.4f);
            style = GUI.skin.box;
            style.fontSize = 12;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.black;


            UnityEditor.Handles.Label(startZone.transform.position , "Start", style);
            UnityEditor.Handles.Label(endZone.transform.position , "End", style);
        }

        #endif
        //draw two border point
        if (!drawOptions.useCustomColor)
        {
            Gizmos.color = ZCKDrawOptions.defaultPathBorderColor;
        }
        else
        {
            Gizmos.color = drawOptions.pathBorderColor;
        }

        for (int i = 0; i < rightBorderPoints.Count; i++)
        {
            Gizmos.DrawLine(leftBorderPoints[i].position,rightBorderPoints[i].position);
        }

        //draw left border
        for (int i = 0; i < leftBorderPoints.Count-1; i++)
        {
            Gizmos.DrawLine(leftBorderPoints[i].transform.position, leftBorderPoints[i+1].transform.position);
        }

        //draw right border
        for (int i = 0; i < rightBorderPoints.Count-1; i++)
        {
            Gizmos.DrawLine(rightBorderPoints[i].transform.position, rightBorderPoints[i+1].transform.position);
        }

        if (readyToDraw && !drawOptions.dontDraw)
        {
            DrawMesh(false);
            DrawMesh(true);
        }        
    }

    public void MakeReadyToDraw()
    {
        readyToDraw = true;
    }
    public bool readyToDraw = false;
    public void DrawMesh(bool reverted)
    {
        pointingToLeft = true;
        leftTris = true;
        leftIndex = 0;
        rightIndex = 0;

        int N = leftBorderPoints.Count + rightBorderPoints.Count;
        Vector3[] vertArr = new Vector3[N];
        Vector3[] normArr = new Vector3[N];
        int[] trisArr =  new int[(N-2)*3];
        for (int i = 0; i < N/2; i++)
        {
            vertArr[i] = leftBorderPoints[i].transform.position;
            normArr[i] = Vector3.up;
        }
        for (int i = 0; i < N/2; i++)
        {
            vertArr[(N/2) + i] = rightBorderPoints[i].transform.position;
            normArr[(N/2) + i] = Vector3.up;
        }

        List<int> listTrisMini = new List<int>();
        for (int i = 0; i < N-2; i++)
        {
            List<int> retlist = RecruitTriangle(listTrisMini,N);
            trisArr[3 * i + 0] = retlist[0];
            if (!reverted)
            {
                trisArr[3 * i + 1] = retlist[1];
                trisArr[3 * i + 2] = retlist[2];
            }
            else
            {
                trisArr[3 * i + 1] = retlist[2];
                trisArr[3 * i + 2] = retlist[1];
            }

        }

        Mesh m = new Mesh();
        m.vertices = vertArr;
        m.triangles = trisArr;
        m.normals = normArr;
        if (!drawOptions.useCustomColor)
        {
            Gizmos.color =  ZCKDrawOptions.defaultPathFillColor;
        }
        else
        {
            Gizmos.color = drawOptions.pathFillColor;
        }
        Gizmos.color = drawOptions.pathFillColor;
        Gizmos.DrawMesh(m);

    }

    bool pointingToLeft;
    bool leftTris;
    int leftIndex;
    int rightIndex;
    List<int> RecruitTriangle(List<int> trisListMini, int N)
    {
        if (trisListMini.Count == 3)
            trisListMini.RemoveAt(0);
        while (trisListMini.Count < 3)
        {
            if (pointingToLeft)
            {
                trisListMini.Add(leftIndex);
                leftIndex++;
            }
            else
            {
                trisListMini.Add(rightIndex+ (N/2));
                rightIndex++;
            }

            pointingToLeft = !pointingToLeft;
        }
        List<int> retList = new List<int>();
        for (int i = 0; i < trisListMini.Count; i++)
        {
            retList.Add(trisListMini[i]);
        }
        if (leftTris)
        {
            int val = retList[2];
            retList[2] = retList[1];
            retList[1] = val;
        }
        leftTris = !leftTris;
        return retList;
    }

}
[System.Serializable]
public class ZCKDrawOptions
{
    public bool useCustomColor = false;
    public Color pathBorderColor = new Color(0, 0, 0, 1);
    public Color pathFillColor = new Color(1,1,1,0.5f);
    public bool dontDraw = false;

    public static Color defaultPathBorderColor = new Color(0, 1, 0, 1);
    public static Color defaultPathFillColor = new Color(0.25f,0.53f,1,0.52f);
}