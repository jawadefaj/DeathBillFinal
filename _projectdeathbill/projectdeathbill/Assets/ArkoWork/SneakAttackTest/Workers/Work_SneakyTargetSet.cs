using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Work_SneakyTargetSet : BaseWorker {

	protected override void OnStart ()
	{
		//set target for player
		SneakyPlayerManager.instance.SetStabTarget(AIDataManager.activeEnemyList[0]);

		//update lungi texture
		//AIDataManager.activeEnemyList[0].selfAnimator.GetComponent<AIModelManager>().rajakarLungi.material.mainTexture = LungiManager.instance.GetANewLungi();

		//IndicatorManager.instance.ShowIndicator(IndicatorType.BoxIndicator, AIDataManager.activeEnemyList[0].transform);
		AIDataManager.activeEnemyList[0].canvasController.targetIcon.SetActive(true);
		//set target for AI
		List<AITarget> at = new List<AITarget>();
		AITarget ai_target;
		ai_target.target = SneakyPlayerManager.instance.GetCurrentPlayer().target;
		ai_target.weight =1f;
		ai_target.fighterID = SneakyPlayerManager.instance.GetCurrentPlayer ().fighterRole;
		at.Add(ai_target);

		AIDataManager.SetAITargets(at,1f);

		WorkFinished ();
	}
}
