using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallelDependantWorkers : BaseWorker {
	public List<BaseWorker> branches = new List<BaseWorker>();
	int N;
	int i;
	protected override void OnStart ()
	{
		N = branches.Count;
		i = 0;
		foreach (BaseWorker bw in branches) {
			bw.StartWork(OnAnyCompleted);
		}
	}
	private void OnAnyCompleted()
	{
		i++;
		if (i == N) {
			WorkFinished ();
		}
	}
	
}
