using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SneakyTestGUI : MonoBehaviour {

	public GameObject walkBtn;

		

	public void Stab()
	{
		SneakyPlayerManager.instance.Stab();
	}

	public void Walk()
	{
		SneakyPlayerManager.instance.Walk();
	}
		
}
