using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(GameManagerMaster))]
public class GameManagerMasterEditor : Editor {

	private GameManagerMaster gmManager;

	public override void OnInspectorGUI ()
	{
		gmManager = (GameManagerMaster) target;

		if(gmManager.workList==null) gmManager.workList = new List<WorkInfo>();

		if(gmManager.workList.Count ==0)
		{
			gmManager.workList.Add(new WorkInfo());
		}

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Work List", EditorStyles.boldLabel);
		EditorGUILayout.Space();

		for(int i=0;i<gmManager.workList.Count;i++)
		{
			GUILayout.BeginHorizontal();
			WorkInfo wi = gmManager.workList[i];
			//is active?
			wi.SetActivity(GUILayout.Toggle(gmManager.workList[i].isActive,string.Format("{0:D2}",i+1),GUILayout.Width(35)));

			//create an object field for every waypoint
			wi.SetWork((BaseWorker) EditorGUILayout.ObjectField(gmManager.workList[i].work, typeof(BaseWorker), true));

			//set the item
			gmManager.workList[i] = wi;

			//display an "Add Waypoint" button for every array row except the last one
			if (GUILayout.Button("+", GUILayout.Width(30f)))
			{
				AddWorkAtIndex(i);
				break;
			}

			//display an "Remove Waypoint" button for every array row except the first and last one
			if (GUILayout.Button("-", GUILayout.Width(30f)))
			{
				RemoveWorkAtIndex(i);
				break;
			}

			GUILayout.EndHorizontal();
		}
	}

	void AddWorkAtIndex(int i)
	{
		WorkInfo wi;
		wi.work = new BaseWorker();
		wi.isActive = true;
		gmManager.workList.Insert(i+1,wi);
	}

	void RemoveWorkAtIndex(int i)
	{
		if(i==0 && gmManager.workList.Count==1) return ;
		gmManager.workList.RemoveAt(i);
	}
}
