using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Work_TargetSetForAI : BaseWorker {

	[Range(0f,1f)]
	public float urgency = 1f;
	public List<TargetSelection> targetSelection = new List<TargetSelection>();

	private List<Transform> targets;

	protected override void OnStart ()
	{
		List<AITarget> at = new List<AITarget>();
		for(int i=0;i<targetSelection.Count;i++)
		{
			at.Add(targetSelection[i].GetAITarget());
		}

		//Do set values here
		AIDataManager.SetAITargets(at,urgency);

		WorkFinished();
	}
}
[System.Serializable]
public struct TargetSelection
{
	public FighterRole fighterID;
	public float weight;
	public Transform targetTransform
	{
		get
		{
			return  PlayerInputController.instance.GetPlayerTargetPoint (fighterID);
		}
	}

	public AITarget GetAITarget()
	{
		AITarget ait;
		ait.target = PlayerInputController.instance.GetPlayerTargetPoint(fighterID);
		ait.weight = this.weight;
		ait.fighterID = fighterID;
		return ait;
	}

}