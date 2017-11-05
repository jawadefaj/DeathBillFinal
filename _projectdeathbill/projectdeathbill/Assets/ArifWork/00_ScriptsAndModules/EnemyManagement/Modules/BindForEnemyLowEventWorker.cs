using UnityEngine;
using System.Collections;

public class BindForEnemyLowEventWorker : BaseWorker {
	public int enemyLowCount = 2;

	protected override void OnStart()
	{
		AIDataManager.EnemyCountLowEnoughRefValue = enemyLowCount;
		AIDataManager.EnemyCountLowEnoughAction += PlayerInputController.instance.GetAiPlayer (FighterRole.Leader).MoveStation;
		WorkFinished ();
	}
}
