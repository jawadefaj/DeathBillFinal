using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIKillAndCleanModule : BaseWorker {
	[SerializeField] bool workerRemainsAlive = false;
	[SerializeField] float actualExecutionDelay = 0f;
	// Use this for initialization
	protected override void OnStart () {
		finishWorkManually = workerRemainsAlive;
		if (!workerRemainsAlive) {	
			Init ();
		} 
		else {
			Handy.DoAfter (this, ()=>{
				Init();
				FinishWorkManually();
			},actualExecutionDelay, ()=>{
				return PlayerInputController.instance.current_player.GetStationController().IsMoving();
			});
		}
		WorkFinished ();
	}
	void Init(){
		//Debug.Log ("inside KAC");
		List<AIPersonnel> aitemplist = new List<AIPersonnel> ();

		if (AIPatrolModule.activeInstance != null) {
			aitemplist.AddRange (AIPatrolModule.activeInstance.patrolList);
			foreach (AIPersonnel ai in aitemplist) {

				if(ai!=null)
					ai.TakeDamage (1000, HitType.HEAD, HitSource.GAYEBI);
			}
		}
		foreach (AIManagerModule aimm in AIManagerModule.nonMainAIMM_List) {
			aitemplist.Clear ();
			aitemplist.AddRange (aimm.reinforcementList);
			foreach (AIPersonnel ai in aitemplist) {

				if(ai!=null)
					ai.TakeDamage (1000, HitType.HEAD, HitSource.GAYEBI);
			}
            Debug.Log("Force stopped called for: "+aimm.transform.name);
			aimm.zoneSet.ForceStopZoneBlock ();
		}
		AIManagerModule.nonMainAIMM_List.Clear ();
		if (AIPatrolModule.activeInstance != null)
			Debug.LogError ("Patrol Clearance Went wrong");
		if (AIManagerModule.activeInstance != null)
			Debug.LogError ("reinforcement Clearance Went wrong");
	}
}
