using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Work_Delay : BaseWorker {

	public float timeToWait = 1f;
	protected override void OnStart ()
	{
		StartCoroutine(Wait());
	}

	private IEnumerator Wait()
	{
		yield return new WaitForSeconds(timeToWait);
		WorkFinished();
	}
}
