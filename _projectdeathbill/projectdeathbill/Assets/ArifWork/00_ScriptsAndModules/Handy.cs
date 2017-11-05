using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Handy {

    public static void SetLayer(this GameObject parent, string layerName, bool includeChildren = true)
    {
        int layer = LayerMask.NameToLayer(layerName);
        parent.layer = layer;
        if (includeChildren)
        {
            foreach (Transform trans in parent.transform.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = layer;
            }
        }
    }
    public static void SetLayer(this GameObject parent, int layer, bool includeChildren = true)
    {
        parent.layer = layer;
        if (includeChildren)
        {
            foreach (Transform trans in parent.transform.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = layer;
            }
        }
    }
    public static Transform Duplicate_WithLocalVals(this Transform sample, Transform nuparent, bool isEmpty)
    {
        GameObject go;
        if (!isEmpty)
        {
            go = MonoBehaviour.Instantiate(sample.gameObject);// new GameObject(target.name);
        }
        else
        {
            go = new GameObject(sample.name);
        }
        go.name = sample.name;
        go.transform.parent = nuparent;
        go.transform.localPosition = sample.localPosition;
        go.transform.localRotation = sample.localRotation;
        return go.transform;
    }
    public static void SetParent_WithSameLocals(this Transform target, Transform nuparent)
    {
        Vector3 locPos = target.localPosition;
        Quaternion locRot = target.localRotation;
        target.parent = nuparent;
        target.localPosition = locPos;
        target.localRotation = locRot;
    }
    public static void SetParentWithLocalsLikeAnother(this Transform target, Transform nuparent, Transform sample)
    {
        Vector3 locPos = sample.localPosition;
        Quaternion locRot = sample.localRotation;
        target.parent = nuparent;
        target.localPosition = locPos;
        target.localRotation = locRot;
    }


	static List<Component> trlist = new List<Component> ();
	public static List<Component> GetAllOfType(Transform root, System.Type type, bool getRoot = false)
	{
		if (trlist == null)
			trlist = new List<Component> ();
		else
			trlist.Clear ();
		
			
		if (getRoot) {
			Component c = root.GetComponent (type);
			if (c!= null) 	trlist.Add (c);
		}
		loadtransformList (root,type);

		return trlist;
	}
	private static void loadtransformList(Transform root, System.Type type)
	{
		foreach (Transform tr in root) 
		{
			Component c = tr.GetComponent (type);
			if (c!= null) 	trlist.Add (c);
			loadtransformList (tr,type);
		}
	}

	public static void DoAfter(MonoBehaviour mono, System.Action act, float delay, System.Func<bool> ShouldWait)
	{
		if(mono == null) 
		{
			Debug.Log("mono is null");
			return;
		}

		if(mono.gameObject.activeSelf == false)
		{
			Debug.Log("mono go is not active");
			return;
		}

		mono.StartCoroutine(Act(act,delay, ShouldWait));
	}
	private static IEnumerator Act(System.Action act, float delay,System.Func<bool> ShouldWait)
	{
		if (ShouldWait != null) {
			while (ShouldWait()) {
				yield return null;
			}
		}
		yield return new WaitForSeconds (delay);
        if(act!=null)act ();
	}

	public static float Deviate(float baseValue, float deviationFraction)
	{
		return baseValue*(1 + Random.Range (-deviationFraction,deviationFraction));
	}

	#if UNITY_EDITOR
	static List<string> onceLogged = new List<string> ();
	#endif
	public static void LogOnce(string message)
	{
		#if UNITY_EDITOR
		if(!onceLogged.Contains(message))
		{
			//Debug.Log(message);
			onceLogged.Add(message);
		}
		#endif
	}


    public static void DrawArcYaxis(Vector3 position, Vector3 direction, float radius, float angle,  int divisions, Color borderColor, Color fillColor)
    {
        divisions = Mathf.Clamp (divisions,1,100);
        Vector3 fwd = direction;
        float halfAngle = angle / 2;
        float sightRange = radius;

        float f;
        int N = divisions + 1;
        Mesh m = new Mesh();
        Vector3[] vertArr = new Vector3[N+1];
        int[] trisArr = new int[(N-1)*3];
        Vector3[] normArr = new Vector3[N+1];


        vertArr [0] = position;   
        normArr [0] = Vector3.up;


        for (int i = 0; i < N; i++) {
            f = -halfAngle + (2*halfAngle*i)/(N-1);
            Vector3 pos = position + (Quaternion.Euler (0, f, 0) * fwd)*sightRange;
            vertArr [i + 1] = pos;
            if (i < N-1) {
                trisArr [3 * i + 0] = 0;
                trisArr [3 * i + 1] = i+1;
                trisArr [3 * i + 2] = i+2;
            }
            normArr [i + 1] = Vector3.up;
        }
        m.vertices = vertArr;
        m.triangles = trisArr;
        m.normals = normArr;

        for (int i = 0; i < vertArr.Length -2 ; i++)
        {
            Gizmos.color = borderColor;

            if(i!=vertArr.Length-1)
                Gizmos.DrawLine(vertArr[i+1], vertArr[i+2]);
            //else if(i==vertArr.Length-2)
                
            else
                Gizmos.DrawLine(vertArr[1], vertArr[vertArr.Length-1]);
        }

        Gizmos.color = fillColor;
        Gizmos.DrawMesh(m);
    }
    public static void DrawDiscYaxis(Vector3 position, float radius, int divisions, Color borderColor, Color fillColor)
    {
        DrawArcYaxis(position,Vector3.forward,radius,360,divisions,borderColor,fillColor);
    }
	public static void  DrawSight(float angle, float range, int divisions, Transform thisTransform,Color color)
	{
        DrawArcYaxis(thisTransform.position,thisTransform.forward, range,angle,divisions, new Color(0,0,0,0) ,color);
//		divisions = Mathf.Clamp (divisions,1,100);
//		Vector3 fwd = thisTransform.forward;
//		float halfAngle = angle / 2;
//		float sightRange = range;
//
//		float f;
//		int N = divisions + 1;
//		Mesh m = new Mesh();
//		Vector3[] vertArr = new Vector3[N+1];
//		int[] trisArr = new int[(N-1)*3];
//		Vector3[] normArr = new Vector3[N+1];
//
//
//		vertArr [0] = thisTransform.position;	
//		normArr [0] = Vector3.up;
//
//
//		for (int i = 0; i < N; i++) {
//			f = -halfAngle + (2*halfAngle*i)/(N-1);
//			Vector3 pos = thisTransform.position + (Quaternion.Euler (0, f, 0) * fwd)*sightRange;
//			vertArr [i + 1] = pos;
//			if (i < N-1) {
//				trisArr [3 * i + 0] = 0;
//				trisArr [3 * i + 1] = i+1;
//				trisArr [3 * i + 2] = i+2;
//			}
//			normArr [i + 1] = Vector3.up;
//		}
//		m.vertices = vertArr;
//		m.triangles = trisArr;
//		m.normals = normArr;
//
//		Gizmos.color = color;
//		Gizmos.DrawMesh(m);
	}
}
public class ChancedList<T>
{
	Dictionary<T,float> inventory = new Dictionary<T, float>();
	public T dummy;
	public void Clear ()
	{
		inventory.Clear ();
	}

	public void Add(T item, float chance)
	{
		inventory.Add (item,chance);
	}
	public void ResetChanceFor(T item,float chance)
	{
		if (inventory.ContainsKey (item)) {
			inventory [item] = chance;
		} else {
			Debug.LogError ("Item not found");
		}
	}
	public float GetChanceFor(T item)
	{
		return inventory [item];
	}

	public T Roll()
	{
		float totalWeight = 0;
		foreach (KeyValuePair<T,float> item in inventory) {
			totalWeight += item.Value;
		}
		float rollValue = UnityEngine.Random.Range (0.0f, 1f);
		float cumulativeWeight = 0;
		foreach (KeyValuePair<T,float> item in inventory) {
			cumulativeWeight += item.Value;
			if (rollValue <= cumulativeWeight / totalWeight) {
				return item.Key;
			}
		}
		Debug.LogError ("Fallen Through!!");
		return dummy;
	}
}
[System.Serializable]
public class PseudoRandomArbitrator {
	public List<ProbabilityCase> data;
	public float totalOccurances;
	public float pseudoM;

	public PseudoRandomArbitrator(Dictionary<char,float> baseListing, float pseudoMultiplier)
	{
		totalOccurances = 0;
		pseudoM = pseudoMultiplier;
		data = new List<ProbabilityCase>();
		foreach (KeyValuePair<char,float> item in baseListing) {
			ProbabilityCase prC = new ProbabilityCase ();
			prC.id = item.Key;
			prC.baseProbability = item.Value;
			prC.occurances = 0;
			data.Add (prC);
		}
	}

	public char Arbitrate()
	{
		SetCurrentProbability ();
		float rollValue = UnityEngine.Random.Range (0.0f, 1f);
		float cumulativeWeight = 0;
		foreach (ProbabilityCase item in data) {
			cumulativeWeight += item.currentProbability;
			if (rollValue <= cumulativeWeight) {
				item.occurances += 1;
				totalOccurances += 1;
				return item.id;
			}
		}
		Debug.LogError ("We are in trouble here!!");
		return '?';
	}


	void SetCurrentProbability()
	{	
		if (totalOccurances != 0) {
			float addedCurrentProbability = 0;
			for (int i = 0; i < data.Count; i++) {
				float pDiff = (data [i].baseProbability - (data [i].occurances / totalOccurances)) * pseudoM;
				data [i].currentProbability = Mathf.Clamp (data [i].baseProbability + pDiff, 0, 1);		
				addedCurrentProbability += data [i].currentProbability;
				//Debug.Log (data [i].currentProbability);
			}
			for (int i = 0; i < data.Count; i++) {
				data [i].currentProbability /= addedCurrentProbability;
				//Debug.Log (data [i].currentProbability);
			}
		} else {
			for (int i = 0; i < data.Count; i++) {
				data [i].currentProbability = data[i].baseProbability;
			}
		}

	}

	public class ProbabilityCase
	{
		public char id;
		public float baseProbability;
		public float currentProbability;
		public float occurances;
	}
}
