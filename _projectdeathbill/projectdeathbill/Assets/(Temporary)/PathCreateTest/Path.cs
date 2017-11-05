using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Portbliss.EditorTools
{
    public class Path : MonoBehaviour {

        List<GameObject> leftBorderPoints;
        List<GameObject> rightBorderPoints;

        public void Initiate()
        {
            leftBorderPoints = new List<GameObject>();
            rightBorderPoints = new List<GameObject>();
        }

        public void AddPoint(GameObject point)
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
            if (leftBorderPoints.Count == 0 || rightBorderPoints.Count == 0)
                return;

            //draw two border point
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(leftBorderPoints[0].transform.position, rightBorderPoints[0].transform.position);
            Gizmos.DrawLine(leftBorderPoints[leftBorderPoints.Count - 1].transform.position, rightBorderPoints[rightBorderPoints.Count - 1].transform.transform.position);

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

            if (drawMesh)
                DrawMesh();
        }

        public bool drawMesh = false;
        public void DrawMesh()
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
                trisArr[3 * i + 1] = retlist[1];
                trisArr[3 * i + 2] = retlist[2];

            }

            Mesh m = new Mesh();
            m.vertices = vertArr;
            m.triangles = trisArr;
            m.normals = normArr;
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
}
