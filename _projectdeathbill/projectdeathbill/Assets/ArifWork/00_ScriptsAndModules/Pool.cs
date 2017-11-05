using UnityEngine;
using System.Collections.Generic;

public class GameObjectPool
{

    private List<GameObject> pool = new List<GameObject>();
    private GameObject originalReference;
    
    public int population { get { return pool.Count; } }

    public GameObjectPool(GameObject original)
    {
        originalReference = original;
    }

    public GameObject Instantiate(Vector3 position, Quaternion rotation)
    {
        if (pool.Count == 0)
        {
            return MonoBehaviour.Instantiate(originalReference, position, rotation) as GameObject;
        }
        else
        {
            GameObject retObject = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
            Transform tr = retObject.transform;
            tr.position = position;
            tr.rotation = rotation;
            retObject.SetActive(true);
            return retObject;
        }
    }
    public void Destroy(string poolID, GameObject gameObject)
    {
        gameObject.SetActive(false);
        pool.Add(gameObject);
    }
}

public static class Pool
{

    private static Dictionary<GameObject, List<GameObject>> poolDictionary = new Dictionary<GameObject, List<GameObject>>();
    private static Dictionary<GameObject, GameObject> prefabMap = new Dictionary<GameObject, GameObject>();
    public static Vector3 defaultPosition = Vector3.zero;
    public static Quaternion defaultRotation = Quaternion.identity;

    public static int poolCount=0;

	public static Transform UnmanagedPooledObjectParent;

	public static GameObject Instantiate(GameObject original)
	{
		return Instantiate (original, Vector3.zero, Quaternion.identity);
	}

    public static GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(original))
        {
            poolDictionary.Add(original, new List<GameObject>());
            poolCount++;
        }

		if (UnmanagedPooledObjectParent == null)
			UnmanagedPooledObjectParent =( new GameObject ("UnmanagedPooledObjects")).transform;
        List<GameObject> currentPool = poolDictionary[original];
        if (currentPool.Count == 0)
        {
            GameObject go = MonoBehaviour.Instantiate(original, position, rotation) as GameObject;
			go.transform.parent = UnmanagedPooledObjectParent;
            go.AddComponent<PooledItem>();
            go.GetComponent<PooledItem>().original = original;
            go.GetComponent<PooledItem>().alive = true;
            prefabMap.Add(go, original);
            return go;
        }
        else
        {
            GameObject retObject = currentPool[currentPool.Count - 1];
            currentPool.RemoveAt(currentPool.Count - 1);
            Transform tr = retObject.transform;
			tr.parent = UnmanagedPooledObjectParent;
            retObject.GetComponent<PooledItem>().alive = true;
            tr.position = position;
            tr.rotation = rotation;
            retObject.SetActive(true);
            return retObject;
        } 
    }
    public static void Destroy(GameObject gameObject)
    {
        if (!prefabMap.ContainsKey(gameObject))
        {
            Debug.Log("Pool.Destroy was called with an unpooled object...");
            return;
        }
        GameObject original = prefabMap[gameObject];
        if (!poolDictionary.ContainsKey(original))
        {
            poolDictionary.Add(original, new List<GameObject>());
            poolCount++;
        }
        List<GameObject> currentPool = poolDictionary[original];
        gameObject.GetComponent<PooledItem>().alive = false;
        gameObject.SetActive(false);
        currentPool.Add(gameObject);
        
    }
    public static void ReleasePool(GameObject original)
    {
        if (poolDictionary.ContainsKey(original))
        {
            List<GameObject> currentPool = poolDictionary[original];
            for (int i = 0; i < currentPool.Count; i++)
            {
                prefabMap.Remove(currentPool[i]);
                MonoBehaviour.Destroy(currentPool[i]);
            }
            poolDictionary.Remove(original);
            poolCount--;
        }
    }
}
public class PooledItem : MonoBehaviour
{
    public bool alive;
    internal GameObject original;
    void OnDestroy() { Pool.ReleasePool(original);}
}
