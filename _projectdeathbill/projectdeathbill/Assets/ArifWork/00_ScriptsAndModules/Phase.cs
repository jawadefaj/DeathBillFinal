using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using SWS;
using DG.Tweening;
using Portbliss;
#region Phase Base Definition
public abstract class Phase 
{
	#region Branching
	public static Phase StartPhase;
	public static Dictionary<string,List<Phase>> phaseListDictionary;
	public static string mainBranch = "main";
	public static string branchInUse = mainBranch;
	public static int globalPhaseCount;
	#endregion
	#region static mandatory initialization 
	//data
	public static MonoBehaviour mono;
	#endregion
	#region Other Vars
	public Action End;

	public int id=0;
	public Vector3 vec3temp = new Vector3();
	public Vector2 vec2temp = new Vector2();
	#endregion
	#region functions
	public virtual void Init(int callID){
		id =callID;
		List<Phase> phaseList = phaseListDictionary[branchInUse];
		phaseList[phaseList.Count-1].End += ()=>this.Go();
		phaseList.Add(this);
	}

	public virtual void Go(){
		Debug.Log("did: "+ id.ToString());
		mono.StartCoroutine(WaitAndEnd(id));
	}

	public virtual IEnumerator WaitAndEnd(int callID, float waitAmount = 0)
	{
		yield return new WaitForSeconds(waitAmount);
		if(End!=null) End();
		else{Debug.Log("Reached the end of a branch...");}
	}
	public void ChooseMainBranch(){
		branchInUse = mainBranch;
	}
	public void ChooseBranch(string branchName){
		branchInUse = branchName;
	}
	public void AddBranchOnThis(string branchName){
		List<Phase> phaseList = new List<Phase>();
		phaseList.Add(this);
		phaseListDictionary.Add(branchName, phaseList);
		branchInUse = branchName;
	}
	#endregion
}
#endregion
#region Phase : 100) Init
public class P_Init : Phase
{
	public P_Init(){
		StartPhase = this;
		phaseListDictionary = new Dictionary<string,List<Phase>>();
		phaseListDictionary.Add(mainBranch, new List<Phase>());
		phaseListDictionary[mainBranch].Add(this);
		branchInUse = mainBranch;

		mono = GeneralManager.instance as MonoBehaviour;
	}
	override public void Go(){
		mono.StartCoroutine(WaitAndEnd(id));
	}
}
#endregion
#region Phase : 999) DeInit 
public class P_DeInit : Phase
{
	Action act;
	public P_DeInit(Action act = null){
		Init(999);
		this.act = act;
	}
	override public void Go(){
		if(act!=null) {act();}
		else {//GameStepController.StepComplete();
		}
	}
}
#endregion
#region Phase : 0) Dummy
public class P_Dummy : Phase
{
	public P_Dummy(int callID=0){
		Init(callID);
	}
	override public void Go(){
		//

		//
		mono.StartCoroutine(WaitAndEnd(id,0.0f));
	}
}
#endregion