using UnityEngine;
using System.Collections;

public class BeeperSoundTriggerer : MonoBehaviour {

	public void PlayC4Beep()
	{
		InGameSoundManagerScript.PlayOnPointFromID (InEndGameMenuManager.instance.transform.position,ClipID.c4Beep);
	}
}
