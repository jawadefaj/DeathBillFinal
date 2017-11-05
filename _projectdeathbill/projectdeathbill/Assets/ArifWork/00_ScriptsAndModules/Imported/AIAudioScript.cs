using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AIAudioScript : MonoBehaviour {
    internal AudioSource primaryAudioSource;
    internal AudioSource secondaryAudioSource;


    const float primarybaseVolume = 1;
    const float primaryCursingVolume = 0.8f;
    const float primaryRunningVolume = 0.15f;
	const float primaryhurtingVolume = 1.0f;

    const float gruntChance = 0.3f;

	internal AIPersonnel personnelScript;
	public void Init(AIPersonnel ai)
	{
		personnelScript = ai;
	}
    void OnEnable()
    {
        primaryAudioSource = this.GetComponent<AudioSource>();
        primaryAudioSource.maxDistance = 100;
        primaryAudioSource.spatialBlend = 0.7f;
        if (primaryAudioSource == null) Debug.LogError("no audio source on ai!");
		primaryAudioSource.volume = primarybaseVolume;
        nextCurseTime = Time.time  + GetCurseFreqLerp() * (1 + Random.Range(-curseIntervalVariance, curseIntervalVariance));
        //Debug.Log(nextCurseTime);
        //Debug.Log(nextCurseTime);
    }
    #region fixed update curse block
    float nextCurseTime;
    float curseTimerStartingOFfset = 3;
	float curseIntervalBase3 =  4*   3;
	float curseIntervalBase15 =  2.5f*  18;
    float curseIntervalVariance = 0.7f;

    float GetCurseFreqLerp()
    {
       // Debug.Log(Mathf.Lerp(curseIntervalBase3, curseIntervalBase15, (AIDataManager.activeEnemyCount) / 18.0f));
		return Mathf.Lerp(curseIntervalBase3, curseIntervalBase15, (AIDataManager.activeEnemyCount) / 18.0f);
    }
    private AudioClip tempClip;
    void FixedUpdate()
    {

		if (UserSettings.SoundOn && !personnelScript.status.dead ) {
			if (!personnelScript.status.unAlert) {
				if ((!primaryAudioSource.isPlaying) && Time.time >= nextCurseTime) {
					
					tempClip = InGameSoundManagerScript.instance.AIcurseSounds [Random.Range (0, InGameSoundManagerScript.instance.AIcurseSounds.Count)];
					primaryAudioSource.clip = tempClip;
					primaryAudioSource.volume = primaryCursingVolume;
                    primaryAudioSource.Play();
					//InGameSoundManagerScript.PlayOnPoint (tempClip, this.transform.position, primaryCursingVolume);
					nextCurseTime = Time.time + GetCurseFreqLerp () * (1 + Random.Range (-curseIntervalVariance, curseIntervalVariance));

                    //Debug.Log("curse@ " + nextCurseTime.ToString());
				}
			}
		}
		else if(UserSettings.SoundOn && personnelScript.status.dead)
		{
			if (primaryAudioSource.isPlaying && personnelScript.enemyType == EnemyType.RAJAKAR)
				primaryAudioSource.Stop();
		}
		else
		{
			if (primaryAudioSource.isPlaying)
				primaryAudioSource.Stop();
		}
        
    }
    #endregion
	public void PlayNormalHitSound() { if(GeneralManager.soundsImplemented)if(Random.Range(0,1.0f)<gruntChance)PlayASoundFromThisList(InGameSoundManagerScript.instance.AIhitSoundsNormal); }
	public void PlayHeadShotHitSound() { if(GeneralManager.soundsImplemented)if (Random.Range(0, 1.0f) < gruntChance) PlayASoundFromThisList(InGameSoundManagerScript.instance.AIhitSoundsHS); }
	public void PlayNormalDeathSound() { if(GeneralManager.soundsImplemented)PlayASoundFromThisList(InGameSoundManagerScript.instance.AIdeathSoundsNormal); }
	public void PlayHeadShotDeathSound(){ if(GeneralManager.soundsImplemented)PlayASoundFromThisList(InGameSoundManagerScript.instance.AIdeathSoundsHS); }
	public void PlayDeathByBlastSound() {if(GeneralManager.soundsImplemented) PlayASoundFromThisList(InGameSoundManagerScript.instance.AIdeathSoundsHS); }

    void PlayASoundFromThisList(List<AudioClip> tempClipList)
    {
        if (UserSettings.SoundOn)
        {
			primaryAudioSource.Stop ();
            primaryAudioSource.volume = primaryhurtingVolume;
            primaryAudioSource.clip = tempClipList[Random.Range(0, tempClipList.Count)];
            primaryAudioSource.Play();
			//Debug.Log ( primaryAudioSource.clip);
			//StartCoroutine (cor(primaryAudioSource));
			//Debug.Break ();
        }
    }
	/*IEnumerator cor(AudioSource aud){
		for (int i = 0; i < 20; i++) {
			Debug.Log (aud.isPlaying.ToString () + " " + aud.clip.name + " " + aud.volume);
			yield return null;
		}
	}*/
    
	public void PlayRunningSound()
    {
		if (UserSettings.SoundOn &&GeneralManager.soundsImplemented) {
			if (!personnelScript.status.unAlert)
			{
				primaryAudioSource.clip = InGameSoundManagerScript.instance.AIrunning;
			
				primaryAudioSource.Play ();
				primaryAudioSource.volume = primaryRunningVolume;
			}
        }
    }
    public void StopRunningSound()
	{
		//Debug.Log("run off called");
        if (primaryAudioSource.clip == InGameSoundManagerScript.instance.AIrunning)
        {
            primaryAudioSource.Stop();
            primaryAudioSource.volume = primarybaseVolume;
        }
    }

}
