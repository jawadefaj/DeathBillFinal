using UnityEngine;
using System.Collections;

public class Work_Level2Phase3Setup : BaseWorker {
	public HUDSettings hudSetting;
	protected override void OnStart ()
	{
		//do not kill this
		finishWorkManually = true;
		hudSetting.Apply ();
		WorkFinished();

		//Switch to nura
		PlayerInputController.instance.GM_SwitchToPlayer(FighterRole.Leader);

		//attach ai to korim
		PlayerInputController.instance.GetPlayerByRole(FighterRole.Heavy).AttachAI(true);

		//attach korim intro event

		//attach kopila intro event
		Work_Level2Phase3KorimSolo.startingEvent += ()=>{ Debug.Log("that equal sign didnt remove me!");};
		Work_Level2Phase3KorimSolo.startingEvent = OnKorimIntroduceCalled;
		AIModelManager.mortarFirstFireEvent = () => {
			HUDPlayerAvailabilityManager.instance.StepToNextHUDSettings();
		};
	}

	private void OnKopilaIntroduceCalled()
	{
        PlayerInputController.instance.GM_SwitchToPlayer(FighterRole.Sniper);

		FinishWorkManually();
	}

	private void OnKorimIntroduceCalled()
	{
		Work_Level2Phase3KorimSolo.startingEvent = null;
		StartCoroutine(Work());
	}

	IEnumerator Work()
	{
		//remove ai from korim
		PlayerInputController.instance.GetPlayerByRole(FighterRole.Heavy).RemoveAI();

		//switch to korim
		PlayerInputController.instance.GM_SwitchToPlayer(FighterRole.Heavy);

		yield return new WaitForSeconds(1.5f);

		//move korim to next station
		bool result = false;
		do
		{
			result = PlayerInputController.instance.GUI_MoveNextStation();
			yield return null;
		}while(result == false);

		Work_Level2Phase3KorimSolo.instance.FinishMyJob();
	}
}
