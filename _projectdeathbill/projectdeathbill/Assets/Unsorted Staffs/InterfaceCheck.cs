using UnityEngine;
using System.Collections;

public class InterfaceCheck : MonoBehaviour {

	// Use this for initialization
	void Start () {

		SampleClass sClass = new SampleClass();
		TestInterface ti = sClass;

		SampleClass nClass = (SampleClass) ti;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public interface TestInterface
{
	void MethodA();
}

public class SampleClass : TestInterface 
{
	public void MethodA ()
	{
		throw new System.NotImplementedException ();
	}
}

public class NewClass : TestInterface 
{
	public void MethodA ()
	{
		throw new System.NotImplementedException ();
	}
}
