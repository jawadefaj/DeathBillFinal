using UnityEngine;
using System.Collections;
using UnityEditor;

public class GetGlobalPos : Editor {

	[MenuItem("Portbliss/GetGlobalPostion")]
	public static void GetGlobalPostion()
	{
		Debug.Log (Selection.activeTransform.name + " : "+ Selection.activeTransform.position);
	}
}
