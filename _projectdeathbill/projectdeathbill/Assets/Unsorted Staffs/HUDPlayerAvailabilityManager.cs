using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDPlayerAvailabilityManager : BaseWorker {
	public List<HUDSettings> hudSettingsList = new List<HUDSettings> ();
	//public HUDSettings currentHUDsettings = null;
	public static HUDPlayerAvailabilityManager instance;
	internal int index;

	protected override void OnSceneAwake ()
	{
		instance = this;
		index = 0;
	}
	protected override void OnStart ()
	{
		StepToNextHUDSettings ();
		WorkFinished ();
	}
	public void StepToNextHUDSettings()
	{
		hudSettingsList [index++].Apply();
	}


}
[System.Serializable]
public class FFListKeep
{
	public List<FighterRole> ffList = new List<FighterRole>();
}
