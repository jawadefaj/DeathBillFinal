using UnityEngine;
using System.Collections;

public class FireSoundPlayer : MonoBehaviour {

	AudioSource aud;
	void Start()
	{
		aud = this.GetComponent<AudioSource> ();
		if (aud == null)
			aud = this.gameObject.AddComponent<AudioSource> ();

		//InGameSoundManagerScript.instance.externalAudioSources.Add (aud);
	}
	bool pausedState=false;
	void Update() {
		if (InGameSoundManagerScript.instance != null) {
			pausedState = InGameSoundManagerScript.instance.paused;
		}
		if (UserSettings.SoundOn && !pausedState) {
			if (aud != null) {
				if (!aud.isPlaying) {
					ClipInfo ci = BaseAudioKeeper.GetClipInfoWithID (ClipID.fireSound);
					aud.clip = ci.clip;
					aud.volume = ci.volume;
					aud.Play ();
				}
			}
		} 
		else {
			if (aud != null) {
				if (aud.isPlaying) {
					aud.Stop ();
				}
			}
		}
	}
}
