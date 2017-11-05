using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour {

	private List<GameObject> poolList = new List<GameObject>();
	public GameObject prefab;
	private GameObject holder;
	
	public GameObject PoolFromList()
	{
		if(poolList.Count <1)
		{
			return Instantiate(prefab);
		}
		else
		{
			GameObject g = poolList[poolList.Count-1];
			g.transform.parent = null;
			g.SetActive(true);
			poolList.RemoveAt(poolList.Count-1);
			return g;
		}
	}

	public void DestroyGameObject(GameObject g, float destroyTime)
	{
		StartCoroutine(GetBackToPoolList(g,destroyTime));
	}

	private IEnumerator GetBackToPoolList(GameObject g, float destroyTime)
	{
		yield return new WaitForSeconds(destroyTime);

		if(holder == null)
		{
			holder = new GameObject();
			holder.name = string.Format("{0}:{1}", this.gameObject.name, "Pool");
		}

		g.transform.parent = holder.transform;
		g.SetActive(false);
		poolList.Add (g);
	}

}
