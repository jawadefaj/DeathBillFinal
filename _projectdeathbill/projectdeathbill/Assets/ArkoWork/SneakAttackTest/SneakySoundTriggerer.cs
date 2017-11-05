using UnityEngine;
using System.Collections;

public class SneakySoundTriggerer : MonoBehaviour {


	//===============================SOUND CALLBACKS=====================================

	public void OnAirKnifeStab()
	{
		ClipInfo ci = BaseAudioKeeper.GetRandomClipInfoWithID (ClipID.knifeAirSwish);
		InGameSoundManagerScript.PlayOnPoint(ci.clip, this.transform.position, ci.volume );
	}

	public void OnRajakarFleeing()
	{
		InGameSoundManagerScript.PlayOnTransformFromIDMutable (this.transform, ClipID.rajakarRunningAway);
	}


	///===========================finalized
	public void OnPlayerStep()
	{
		ClipInfo ci = BaseAudioKeeper.GetRandomClipInfoWithID (ClipID.walkSingleStep);
		InGameSoundManagerScript.PlayOnPoint(ci.clip, this.transform.position, ci.volume*Mathf.Lerp(0.2f,0.9f,SneakyPlayerManager.instance.GetNoiseLevel()/SneakyPlayerManager.MAX_ALLOWABLE_NOISE_LEVEL) );
	}
	public void OnRajakarBodyFall()
	{
		Debug.Log("OnRajakarBodyFall");
	}
	public void OnRajakarDying()
	{
		InGameSoundManagerScript.PlayOnTransformFromIDMutable (this.transform, ClipID.rajakarDying);
	}
	public void OnKnifeStab()
	{
		InGameSoundManagerScript.PlayOnTransformFromIDMutable (this.transform, ClipID.knifeSlitThroat);
	}

	//===============================SOUND CALLBACKS END=================================
}
