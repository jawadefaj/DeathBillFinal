using UnityEngine;
using System.Collections;

public class InfiniteWorker : BaseWorker {

	public BaseWorker[] workerList;

	private bool canDoWork = true;

	protected override void OnStart ()
	{
		ExecuteNextWork ();
	}

	public void StopWorking()
	{
		canDoWork = false;
		WorkFinished ();
	}

	private void ExecuteNextWork()
	{
		if (!canDoWork)
			return;
		
		int index = GetNextWorkerIndex ();
		StartCoroutine(DelayedWorkStart(index));
	}

	IEnumerator DelayedWorkStart(int index)
	{
		yield return null;
		workerList [index].StartWork (ExecuteNextWork);
	}

	protected virtual int GetNextWorkerIndex()
	{
		return Random.Range (0, workerList.Length);
	}
}
