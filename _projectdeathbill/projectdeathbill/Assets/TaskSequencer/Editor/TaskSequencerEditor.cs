using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using Portbliss.TaskSequencer;

[CustomEditor(typeof(TaskSequencer))]
public class TaskSequencerEditor : Editor {

	SerializedObject s_Object;
	SerializedProperty workerList;
	SerializedProperty isMaster;
	private List<bool> elementVisibility = new List<bool>();
	private List<bool> elementAdvanceOptionVisible = new List<bool>();
	private List<int> elementVariableSelectedIndex = new List<int>();

    private bool workerExpanded = false;
    private bool allExpanded = false;

	void OnEnable()
	{
        if (target == null)
            return;
		s_Object = new SerializedObject(target);
		workerList = s_Object.FindProperty ("workList");
		isMaster = s_Object.FindProperty ("isMaster");
		
		if(elementVisibility==null || elementVisibility.Count<1)
		{
			elementVisibility = new List<bool>();
			elementAdvanceOptionVisible = new List<bool>();
			elementVariableSelectedIndex = new List<int>();
			
			for(int i=0;i<s_Object.FindProperty("workList").arraySize;i++)
			{	
				elementVisibility.Add(false);
				elementAdvanceOptionVisible.Add(false);
				elementVariableSelectedIndex.Add(0);
			}
		}
	}

	public override void OnInspectorGUI ()
	{
		s_Object.Update();

		//is it a master game manager
		GUILayout.Space (10);
		EditorGUILayout.PropertyField (isMaster);
        GUILayout.BeginHorizontal();
        if (workerExpanded)
        {
            if (GUILayout.Button("Hide Workers",new GUILayoutOption[]{GUILayout.Height(20),GUILayout.Width(120)}))
            {
                workerExpanded = !workerExpanded;
                ExpandAllWorkers(workerExpanded);
            }
        }
        else
        {
            if (GUILayout.Button("Expand Workers",new GUILayoutOption[]{GUILayout.Height(20),GUILayout.Width(120)}))
            {
                workerExpanded = !workerExpanded;
                ExpandAllWorkers(workerExpanded);
            }
        }
        GUILayout.FlexibleSpace();
        if (allExpanded)
        {
            if (GUILayout.Button("Hide All",new GUILayoutOption[]{GUILayout.Height(20),GUILayout.Width(120)}))
            {
                allExpanded = !allExpanded;
                workerExpanded = allExpanded;
                ExpandAll(allExpanded);
            }
        }
        else
        {
            if (GUILayout.Button("Expand All",new GUILayoutOption[]{GUILayout.Height(20),GUILayout.Width(120)}))
            {
                allExpanded = !allExpanded;
                workerExpanded = allExpanded;
                ExpandAll(allExpanded);
            }
        }

        GUILayout.EndHorizontal();
		GUILayout.Space (10);

		//if no array item then display a add button
		if(workerList.arraySize ==0)
		{
			if (GUILayout.Button("Add New Worker"))
			{
				workerList.InsertArrayElementAtIndex(0);
				elementVisibility.Insert(0,true);
				elementAdvanceOptionVisible.Insert(0,false);
				elementVariableSelectedIndex.Insert(0,0);
			}
		}
		else
		{
			ListIterator("workList");
		}

		s_Object.ApplyModifiedProperties();
	}

	public void ListIterator(string propertyPath)
	{
		SerializedProperty listProperty = s_Object.FindProperty(propertyPath);
		

		EditorGUI.indentLevel++;
		
		for(int i=0;i<listProperty.arraySize;i++)
		{
			bool b = elementVisibility[i];
			bool c = elementAdvanceOptionVisible[i];
			bool r = ShowElementItem(listProperty,i, ref b, ref c);
			if(!r) break;
			elementVisibility[i] =b;
			elementAdvanceOptionVisible[i] =c;
		}
		
		EditorGUI.indentLevel--;

	}

	public bool ShowElementItem(SerializedProperty listProperty, int i, ref bool visible, ref bool adv_optn_visible)
	{
		SerializedProperty elementProperty = listProperty.GetArrayElementAtIndex(i);
		SerializedProperty worker = elementProperty.FindPropertyRelative ("work");
		SerializedProperty isActive = elementProperty.FindPropertyRelative ("isActive");
		SerializedProperty title = elementProperty.FindPropertyRelative("title");
		//SerializedProperty workOption = elementProperty.FindPropertyRelative("workOptions");
		SerializedProperty options_keepalive = elementProperty.FindPropertyRelative("workOptions.keepAlive");
		SerializedProperty options_delayStart = elementProperty.FindPropertyRelative("workOptions.delayStart");
		SerializedProperty options_delayStartTime = elementProperty.FindPropertyRelative("workOptions.delayStartTime");
		SerializedProperty options_handleReleaseOptn = elementProperty.FindPropertyRelative("workOptions.handleReleaseOption");
		SerializedProperty options_delayHandleReleaseTime = elementProperty.FindPropertyRelative("workOptions.delayHandleReleaseTime");
		SerializedProperty options_variableFields = elementProperty.FindPropertyRelative("workOptions.variableFields");


		GUILayout.BeginHorizontal();
		EditorGUILayout.PropertyField(isActive,GUIContent.none,GUILayout.Width(30f));

		GUILayout.BeginVertical(EditorStyles.helpBox);
		
		GUIStyle modifiedFoldOut = new GUIStyle(EditorStyles.foldout);
		modifiedFoldOut.fontSize = 12;
		modifiedFoldOut.fontStyle = FontStyle.Bold;
		

		string workerName = "New Worker";
		if(!string.IsNullOrEmpty(title.stringValue)) workerName = title.stringValue;
		else if (worker.objectReferenceValue!=null) workerName = worker.objectReferenceValue.name;
		else workerName = "New Worker";

		visible = EditorGUILayout.Foldout(visible,workerName,modifiedFoldOut);
		
		if(visible)
		{
			EditorGUILayout.PropertyField(title);
			EditorGUILayout.PropertyField(worker);

			adv_optn_visible = EditorGUILayout.Foldout(adv_optn_visible,"Advance Options");

			if(adv_optn_visible)
			{

				EditorGUILayout.PropertyField(options_keepalive);
				EditorGUILayout.PropertyField(options_delayStart);
				if(options_delayStart.boolValue == true) EditorGUILayout.PropertyField(options_delayStartTime);
				EditorGUILayout.PropertyField(options_handleReleaseOptn);
				if(options_handleReleaseOptn.enumValueIndex == 3) EditorGUILayout.PropertyField(options_delayHandleReleaseTime);

				//for code reflection options

				if(worker.objectReferenceValue !=null)
				{
					EditorGUILayout.LabelField("Variable Options :");
					FieldInfo[] f_infos =  worker.objectReferenceValue.GetType().GetFields();
					string[] names = new string[f_infos.Length];

					for(int j=0;j<f_infos.Length;j++)
						names[j] = f_infos[j].Name;

					EditorGUILayout.BeginHorizontal();
					elementVariableSelectedIndex[i] = EditorGUILayout.Popup(elementVariableSelectedIndex[i],names);
					if (GUILayout.Button("+", GUILayout.Width(30f)))
					{
						if(options_variableFields == null) 
						{
							Debug.Log("is null");
							//GameManager gm = (GameManager)target;
							//gm.workList[i].workOptions.Init();
						}
						else
						{
							//attact the selected type
							FieldInfo fi = f_infos[elementVariableSelectedIndex[i]];
							int iType=0;
							System.Type t = fi.FieldType;

                            if (t == typeof(int))
                                iType = 0;
                            else if (t == typeof(float))
                                iType = 1;
                            else if (t == typeof(string))
                                iType = 2;
                            else if (t == typeof(GameObject))
                                iType = 3;
                            else if (t == (typeof(Texture)))
                                iType = 4;
                            else if (t == (typeof(Sprite)))
                                iType = 5;

							if(IsDuplicate(options_variableFields,fi.Name))
							{
								Debug.Log("Already contains the value");
								EditorGUILayout.EndHorizontal();
								return false;
							}

							options_variableFields.InsertArrayElementAtIndex(options_variableFields.arraySize);

							options_variableFields.GetArrayElementAtIndex(options_variableFields.arraySize-1).FindPropertyRelative("stype").intValue = iType;
							options_variableFields.GetArrayElementAtIndex(options_variableFields.arraySize-1).FindPropertyRelative("fieldName").stringValue = fi.Name;

						}
					}
					EditorGUILayout.EndHorizontal();

					//display the fields
					if(options_variableFields != null)
					{
						for(int k=0;k<options_variableFields.arraySize;k++)
						{
							//EditorGUILayout.PropertyField(options_variableFields.GetArrayElementAtIndex(k));
							DrawVariableEditor(options_variableFields,k);
						}
					}
				}

				//end of code reflection option

			}
			
			EditorGUILayout.BeginHorizontal();
			
			GUILayout.Space(10);
			
			if (GUILayout.Button("Add", GUILayout.Width(60f)))
			{
				listProperty.InsertArrayElementAtIndex(i);
				elementVisibility.Insert(i,true);
				elementAdvanceOptionVisible.Insert(i,false);
				elementVariableSelectedIndex.Insert(i,0);
				return false;
			}
			
			GUILayout.FlexibleSpace();

            if (GUILayout.Button("▲", GUILayout.Width(60f)))
            {
                //move item up
                if (i != 0)
                {
                    listProperty.MoveArrayElement(i, i - 1);
                }
                return false;
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("▼", GUILayout.Width(60f)))
            {
                //move item down
                if (i != listProperty.arraySize-1)
                {
                    listProperty.MoveArrayElement(i, i + 1);
                }
                return false;
            }

            GUILayout.FlexibleSpace();
			
			if (GUILayout.Button("Remove", GUILayout.Width(60f)))
			{
				listProperty.DeleteArrayElementAtIndex(i);
				elementVisibility.RemoveAt(i);
				elementAdvanceOptionVisible.RemoveAt(i);
				elementVariableSelectedIndex.RemoveAt(i);
				return false;
			}
			EditorGUILayout.EndHorizontal();
			
			GUILayout.Space(10);
			
		}


		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		
		return true;
	}

	public bool IsDuplicate(SerializedProperty list, string f_name)
	{
		for(int i=0;i<list.arraySize;i++)
		{
			SerializedProperty sp = list.GetArrayElementAtIndex(i);
			string fn = sp.FindPropertyRelative("fieldName").stringValue;

			if(fn==f_name) return true;
		}
		return false;
	}

	public void DrawVariableEditor( SerializedProperty list, int index)
	{
		SerializedProperty sp = list.GetArrayElementAtIndex(index);
		SerializedProperty v_name = sp.FindPropertyRelative("fieldName");
		SerializedProperty v_value = sp.FindPropertyRelative("value");
		SerializedProperty v_type = sp.FindPropertyRelative("stype");

        SerializedProperty v_goValue = sp.FindPropertyRelative("go_value");
        SerializedProperty v_txtValue = sp.FindPropertyRelative("txt_value");
        SerializedProperty v_spriteValue = sp.FindPropertyRelative("sprite_value");

		string vName = v_name.stringValue;

		EditorGUILayout.BeginHorizontal();

        if (v_type.intValue == 0)
        {
            int iVal = 0;
            int.TryParse(v_value.stringValue, out iVal);
            v_value.stringValue = EditorGUILayout.IntField(vName, iVal).ToString();
        }
        else if (v_type.intValue == 1)
        {
            v_value.stringValue = EditorGUILayout.FloatField(vName, float.Parse(v_value.stringValue)).ToString();
        }
        else if (v_type.intValue == 2)
        {
            v_value.stringValue = EditorGUILayout.TextField(vName, v_value.stringValue);
        }
        else if (v_type.intValue == 3)
        {
            EditorGUILayout.PropertyField(v_goValue, new GUIContent(vName));
        }
        else if (v_type.intValue == 4)
        {
            EditorGUILayout.PropertyField(v_txtValue, new GUIContent(vName));
        }
        else if (v_type.intValue == 5)
        {
            EditorGUILayout.PropertyField(v_spriteValue,new GUIContent(vName));
        }

		if (GUILayout.Button("-", GUILayout.Width(30f)))
		{
			list.DeleteArrayElementAtIndex(index);
		}

		EditorGUILayout.EndHorizontal();
	}

    private void ExpandAllWorkers(bool visible)
    {
        if(elementVisibility!=null)
            for (int i = 0; i < elementVisibility.Count; i++)
            {
                elementVisibility[i] = visible;
            }
    }

    private void ExpandAll(bool visible)
    {
        if(elementVisibility!=null)
            for (int i = 0; i < elementVisibility.Count; i++)
            {
                elementVisibility[i] = visible;
            }

        if(elementAdvanceOptionVisible!=null)
            for (int i = 0; i < elementAdvanceOptionVisible.Count; i++)
            {
                elementAdvanceOptionVisible[i] = visible;
            }
    }

}
