using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Work_AIFireSwitch : BaseWorker {

    public List<FighterRole> fighterList = new List<FighterRole>() ;
	public bool FireSwitchOn = true;

	protected override void OnStart ()
	{

        foreach (FighterRole item in fighterList)
        {
            PlayerAI ai = PlayerInputController.instance.GetAiPlayer(item);

            if(ai!=null)
            {
                ai.SwitchFire(FireSwitchOn);
            }
            else
            {
                Debug.LogError("No such AI player found");
            }
        }


		WorkFinished();
	}
}
