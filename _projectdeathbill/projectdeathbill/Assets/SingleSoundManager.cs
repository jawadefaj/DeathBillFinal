using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SingleSoundManager : MonoBehaviour {
	public ClipID clipID;
	public bool isLooping;

	ClipInfo clipInfo;
	AudioSource selfAudio;

	void Start () {
		selfAudio = GetComponent<AudioSource> ();
		clipInfo = BaseAudioKeeper.GetClipInfoWithID(clipID);
		if (clipInfo != null) 
		{
			selfAudio.clip = clipInfo.clip;
			selfAudio.volume = clipInfo.volume;
			selfAudio.loop = isLooping;
			selfAudio.playOnAwake = false;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (clipInfo != null) 
		{
			if (UserSettings.SoundOn) {
				if (!selfAudio.isPlaying) 
				{
					selfAudio.Play ();
				}
				
			} 
			else 
			{
				if (selfAudio.isPlaying) 
				{
					selfAudio.Stop ();
				}
					
			}
		}
	}
}
