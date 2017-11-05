using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerDMGModifierUpdate : BaseWorker {
    [HideInInspector][SerializeField]public List<PlayerDamageInputModifier> playerDMGInMod = new List<PlayerDamageInputModifier>() ;

	protected override void OnStart ()
	{
		AIDataManager.instance.playerDamageInputMods = playerDMGInMod;
		WorkFinished ();
	}
}
