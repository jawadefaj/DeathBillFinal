using UnityEngine;
using System.Collections;

public class Work_AnimationValueReseter : BaseWorker {

	public int value=0;
	protected override void OnStart ()
	{
		KnifeAnimationManager.instance.SetAnimationIndex(value);
		WorkFinished();
	}
}
