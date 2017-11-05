using UnityEngine;
using System.Collections;

public class LungiManager : MonoBehaviour {

	public static LungiManager instance;

	public Texture[] lungiCollection;

	private Texture usedLungi;

	void Awake()
	{
		instance = this;
	}

	public Texture GetANewLungi()
	{
		int i = Random.Range(0,lungiCollection.Length);

		usedLungi = lungiCollection[i];

		Debug.Log("Selected lungi "+i);

		return usedLungi;
	}

	public Texture GetUsedLungi()
	{
		if(usedLungi==null) Debug.LogError("No lungi was used.");
		return usedLungi;
	}
}
