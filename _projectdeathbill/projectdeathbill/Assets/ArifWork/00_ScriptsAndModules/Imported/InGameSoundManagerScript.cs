using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InGameSoundManagerScript : MonoBehaviour {
	//===============renewed
	public static InGameSoundManagerScript instance;
	internal GameObject pooledAudioSourcePrefab;
	internal AudioSource selfAudioSource;
	internal AudioSource secondaryAudioSource;
	public static float  primaryMaxVolume; //= 0.18f;
	public const float  secondaryMaxVolumeMale = 0.495f;
	public const float  secondaryMaxVolumeFeMale = 0.9f;
	public List<AudioSource> externalAudioSources;

	//===============old
	internal List<AudioClip> AIhitSoundsNormal = new List<AudioClip>();
	internal List<AudioClip> AIhitSoundsHS = new List<AudioClip>();
	internal List<AudioClip> AIdeathSoundsNormal = new List<AudioClip>();
	internal List<AudioClip> AIdeathSoundsHS = new List<AudioClip>();
	internal List<AudioClip> AIcurseSounds = new List<AudioClip>();

    internal AudioClip AIrunning;

    //public bool shouldPlaySounds = true;
    public bool paused = false;
	//internal Dictionary<FreedomFighter,List<AudioClip>> perPlayerAudios = new Dictionary<FreedomFighter, List<AudioClip>> ();
    
	public List<PerPersonDialogueProfile> perPlayerDialogueProfiles;
	ClipInfo singleLoopBGClipInfo;
	bool SingleLoop;
	void Awake()
    {
        instance = this;
		selfAudioSource = gameObject.GetComponent<AudioSource> ();
		if (selfAudioSource == null) {
			selfAudioSource = this.gameObject.AddComponent<AudioSource> ();
		}
		if (secondaryAudioSource == null) {
			GameObject g = new GameObject ();
			g.transform.SetParent (this.transform);
			g.transform.position = Vector3.zero;
			secondaryAudioSource = g.AddComponent <AudioSource>();
		}
		if (pooledAudioSourcePrefab == null) {
			GameObject g = new GameObject ();
			g.AddComponent <AudioSource>();
			pooledAudioSourcePrefab = g;
		}


        selfAudioSource = this.GetComponent<AudioSource>();
        if (selfAudioSource == null) Debug.LogError("No audio source found!");

		singleLoopBGClipInfo = BaseAudioKeeper.GetRandomClipInfoWithID(ClipID.BG_Intro);
		if(singleLoopBGClipInfo == null)
			singleLoopBGClipInfo = BaseAudioKeeper.GetRandomClipInfoWithID(ClipID.BG_Music);
		selfAudioSource.clip = singleLoopBGClipInfo.clip;

		selfAudioSource.volume = singleLoopBGClipInfo.volume;
        selfAudioSource.Play();  
		#region loading up sounds from audio keeper
		foreach(AudioClip clip in BaseAudioKeeper.GetAllClipWithID (ClipID.enemyHit))
		{
			AIhitSoundsNormal.Add (clip);
		}
		foreach(AudioClip clip in BaseAudioKeeper.GetAllClipWithID (ClipID.enemyHitHS))
		{
			AIhitSoundsHS.Add (clip);
		}		
		foreach(AudioClip clip in BaseAudioKeeper.GetAllClipWithID (ClipID.enemyDie))
		{
			AIdeathSoundsNormal.Add (clip);
		}
		foreach(AudioClip clip in BaseAudioKeeper.GetAllClipWithID (ClipID.enemyDieHS))
		{
			AIdeathSoundsHS.Add (clip);
		}
		foreach(AudioClip clip in BaseAudioKeeper.GetAllClipWithID (ClipID.enemyDialogue))
		{
			AIcurseSounds.Add (clip);
		}
		AIrunning = BaseAudioKeeper.GetClipWithID (ClipID.enemyRunning);
		#endregion


    }
	public void Start()
	{
		
	}
	public void ForAmericaShoutAll()
	{
		for (int i = 0; i < perPlayerDialogueProfiles.Count; i++) {
			perPlayerDialogueProfiles [i].UseUrgentDialogue(PerPersonDialogueTypes.InspirationalShout);
		}
	}
	public void ForAmericaShoutSingle(FighterName fName)
	{
		for (int i = 0; i < perPlayerDialogueProfiles.Count; i++) {
			if(perPlayerDialogueProfiles[i].tpc.fighterName == fName)
				perPlayerDialogueProfiles [i].UseUrgentDialogue(PerPersonDialogueTypes.InspirationalShout);
		}
	}
	public void PPDP_GotHitCry(FighterName fName)
	{
		for (int i = 0; i < perPlayerDialogueProfiles.Count; i++) {
			if(perPlayerDialogueProfiles[i].tpc.fighterName == fName)
				perPlayerDialogueProfiles [i].UseUrgentDialogue(PerPersonDialogueTypes.HitDialogue);
		}
	}
	public void PPDP_WaveClearCalls(FighterName fName)
	{
		for (int i = 0; i < perPlayerDialogueProfiles.Count; i++) {
			if(perPlayerDialogueProfiles[i].tpc.fighterName == fName)
				perPlayerDialogueProfiles [i].UseUrgentDialogue(PerPersonDialogueTypes.WaveCleared);
		}
	}

	bool perPersonDialogInitDone = false;
	void FixedUpdate () {
		if (!perPersonDialogInitDone) {
			//Debug.Log("A");
			if (PlayerInputController.instance != null) {
				//Debug.Log("B");
				perPersonDialogInitDone = true;
				perPlayerDialogueProfiles = new List<PerPersonDialogueProfile>();
				foreach (ThirdPersonController tpc in PlayerInputController.instance.players) {
					PerPersonDialogueProfile ppdp = new PerPersonDialogueProfile ();
					//Debug.Log (tpc.fighterID);
					if (ppdp.TryInit (tpc))
						perPlayerDialogueProfiles.Add (ppdp);
				}
			}
		} else {
			for (int i = 0; i < perPlayerDialogueProfiles.Count; i++) {
				perPlayerDialogueProfiles [i].RandomDialogueUpdater ();
			}
		}


        if (UserSettings.SoundOn)
        {
            if (!selfAudioSource.isPlaying)
            {
                if (!paused)
                {
					if(GeneralManager.instance!=null) SingleLoop = !(GeneralManager.instance.level == 2 && GeneralManager.instance.phase == 3);
					if (SingleLoop) {
						selfAudioSource.clip = singleLoopBGClipInfo.clip;
						selfAudioSource.volume = singleLoopBGClipInfo.volume;
					} else {
						singleLoopBGClipInfo = BaseAudioKeeper.GetRandomClipInfoWithID (ClipID.BG_Music);
						selfAudioSource.clip = singleLoopBGClipInfo.clip;
						selfAudioSource.volume = singleLoopBGClipInfo.volume;
					}
                    selfAudioSource.Play();
                }
                else
                {
					selfAudioSource.clip = BaseAudioKeeper.GetClipWithID (ClipID.BG_PauseMenu);
                    selfAudioSource.Play();
                }
            }
			if ((GeneralManager.instance.level == 2) && (GeneralManager.instance.phase == 1)) {
				
			} 
			else 
			{
				float hp = PlayerInputController.instance.current_player.healthPoint;
				if (hp <= beatingSoundStartHP) {
					switch (PlayerInputController.instance.current_player.fighterName) 
					{
					case FighterName.JB:
					case FighterName.Hillary:
						{
							if (secondaryAudioSource.clip != BaseAudioKeeper.GetClipWithID (ClipID.BG_LOWHP_Female)) {
								secondaryAudioSource.clip = BaseAudioKeeper.GetClipWithID (ClipID.BG_LOWHP_Female);
								secondaryAudioSource.volume = secondaryMaxVolumeFeMale;
							}
						}
						break;
					default:
						{
							if (secondaryAudioSource.clip != BaseAudioKeeper.GetClipWithID (ClipID.BG_LOWHP_Male)) {
								secondaryAudioSource.clip = BaseAudioKeeper.GetClipWithID (ClipID.BG_LOWHP_Male);
								secondaryAudioSource.volume = secondaryMaxVolumeMale;
							}
						}
						break;
					}
					if(!secondaryAudioSource.isPlaying)
					secondaryAudioSource.Play ();
				}
				float maxHP = 100;
				selfAudioSource.volume = Mathf.Lerp(selfAudioSource.volume, Mathf.Lerp(0, singleLoopBGClipInfo.volume,Mathf.Clamp((hp - musicStillOnAtMaxHP) / (maxHP - musicStillOnAtMaxHP), 0, 1) ), bgmusicshiftingspeed);
                if (PlayerInputController.instance.current_player.fighterRole == FighterRole.NotActive) 
					secondaryAudioSource.volume = Mathf.Lerp(secondaryAudioSource.volume,Mathf.Lerp(secondaryMaxVolumeFeMale, 0,Mathf.Clamp(hp / beatingSoundStartHP, 0, 1)), bgmusicshiftingspeed);
				else
					secondaryAudioSource.volume = Mathf.Lerp(secondaryAudioSource.volume,Mathf.Lerp(secondaryMaxVolumeMale, 0,Mathf.Clamp(hp / beatingSoundStartHP, 0, 1)), bgmusicshiftingspeed);
			}
        }
        else
        {
            if(selfAudioSource.isPlaying)
                selfAudioSource.Stop();
            selfAudioSource.volume = 0;
            secondaryAudioSource.volume = 0;
        }
        
    }

    public const float bgmusicshiftingspeed = 0.05f;
    public const float musicStillOnAtMaxHP = 25f;
    public const float beatingSoundStartHP = 60f;


    public void SetPausedState(bool isPaused)
    {
        //selfAudioSource.volume = 1.0f;
        //secondaryAudioSource.volume =0;
        paused = isPaused;
		if (paused) {
			KillExternalPossibleSources ();
			selfAudioSource.clip = BaseAudioKeeper.GetClipWithID (ClipID.BG_PauseMenu);
			selfAudioSource.volume = 1.0f;
			secondaryAudioSource.volume = 0;
		} else {
			selfAudioSource.clip = singleLoopBGClipInfo.clip;
			selfAudioSource.volume = singleLoopBGClipInfo.volume;
		}

        if(UserSettings.SoundOn)
            selfAudioSource.Play();
    }

    public static void KillAllPossibleSounds()
    {
        instance.selfAudioSource.Stop();
        instance.secondaryAudioSource.Stop();
        KillExternalPossibleSources();
    }
    public static void KillExternalPossibleSources()
    {
        for (int i = 0; i < instance.externalAudioSources.Count; i++)
        {
            instance.externalAudioSources[i].Stop();
        }
		for (int i = 0; i < AIDataManager.activeEnemyCount; i++)
        {
			AIDataManager.activeEnemyList[i].selfAudioManager.primaryAudioSource.Stop();
        }
    }
    /*public void SetSoundState(bool shouldPlay)
    {
        shouldPlaySounds = shouldPlay;
        if(!shouldPlay)
            selfAudioSource.Stop();

    }*/

    public static void PlayOnPoint(AudioClip clip, Vector3 pos, float vol)
    {
		//Debug.Log (clip.name);
		if (!UserSettings.SoundOn)
			return;
        AudioSource tempAudOb = Pool.Instantiate(instance.pooledAudioSourcePrefab,pos,Quaternion.identity).GetComponent<AudioSource>();
        tempAudOb.clip = clip;
        tempAudOb.volume = vol;
        tempAudOb.Play();
        instance.StartCoroutine(instance.DestroyTempAud(tempAudOb, clip.length)); 
    }
	public static AudioSource PlayOnPointFromID(Vector3 pos, ClipID cID)
	{
		if (!UserSettings.SoundOn)
			return null;
		ClipInfo ci = BaseAudioKeeper.GetClipInfoWithID (cID);
		if (ci == null) {
			Debug.LogError ("clip not found!");
			return null;
		}
		AudioSource tempAudOb = Pool.Instantiate(instance.pooledAudioSourcePrefab,pos,Quaternion.identity).GetComponent<AudioSource>();
		tempAudOb.clip = ci.clip;
		tempAudOb.volume = ci.volume;
		tempAudOb.Play();
		instance.StartCoroutine(instance.DestroyTempAud(tempAudOb, ci.clip.length)); 
		return tempAudOb;

	}
	public static AudioSource PlayOnPointMutable(Vector3 pos, ClipID cID)
	{
		AudioSource tempAudOb = PlayOnPointFromID (pos, cID);
		if (tempAudOb == null)
			return null;
		if (!instance.externalAudioSources.Contains (tempAudOb))
			instance.externalAudioSources.Add (tempAudOb);
		return tempAudOb;
	}

	public static AudioSource PlayOnTransformFromIDMutable(Transform trans, ClipID cID){
		AudioSource aud = PlayOnPointMutable (trans.position, cID);
		if (aud == null)
			return null;
		aud.transform.SetParent (trans);
		return aud;
	}

    IEnumerator DestroyTempAud(AudioSource audSource, float time)
    {
        yield return new WaitForSeconds(time);
        Pool.Destroy(audSource.gameObject);
    }

}
[System.Serializable]
public class PerPersonDialogueProfile
{
	public ThirdPersonController tpc;

	public ClipID cID_randomDialogue;
	public ClipID cID_gotHit;
	public ClipID cID_waveClear;
	public ClipID cID_inspirationalShout;


	private AudioSource audSource;

	public float hitDialogueCDuntil;
	public float hitDialogueCDduration = 8;
	public float hitDialogueCDvariation =  0.15f;
	public float nextDialogueTime;
	public float dialogueBaseInterval=20f;
	public float dialogueIntervalVariation = 0.5f;

	public int waveClearIntegerPeriod = 4;

	public bool TryInit(ThirdPersonController tpc)
	{
		this.tpc = tpc;
		FighterName ffID = tpc.fighterName;
		int Nrd = 1;
		int Nhd = 2;
		int Nwc = 3;
		int Nis = 4;
		int C = 0;
		switch (ffID) {
		case FighterName.Baker:
			C = 500;
			break;
		case FighterName.JB:
			C = 510;
			break;		
		case FighterName.Dom:
			C = 520;
			break;
		case FighterName.Philips:
			C = 530;
			break;
		case FighterName.Trump:
			C = 540;
			break;
		case FighterName.Hillary:
			C = 550;
			break;
		default:
			return false;
			break;
		}

		cID_randomDialogue = (ClipID) (C+Nrd);
		cID_gotHit = (ClipID) (C+Nhd);
		cID_waveClear = (ClipID) (C+Nwc);
		cID_inspirationalShout = 	(ClipID) (C+Nis);

		GameObject go = new GameObject ();
		go.name = "PlayerAudioSource_"+tpc.gameObject.name;
		audSource = go.AddComponent<AudioSource> ();
		audSource.playOnAwake = false;
        audSource.maxDistance = 100;
		go.transform.SetParent (tpc.transform);
		go.transform.position = Vector3.zero;
		hitDialogueCDuntil = 0;
		nextDialogueTime = Time.time + Handy.Deviate (dialogueBaseInterval, dialogueIntervalVariation);
		return true;
	}
	public void RandomDialogueUpdater()
	{
        if (tpc.IsInControll())
        {
            audSource.spatialBlend = Mathf.Lerp(audSource.spatialBlend, 0, 0.1f);
        }
        else
        {
            audSource.spatialBlend = Mathf.Lerp(audSource.spatialBlend, 0.6f, 0.1f);
        }
		if (UserSettings.SoundOn && !tpc.IsDead()) {
			if ((Time.time >= nextDialogueTime) && (tpc == PlayerInputController.instance.current_player)) {
				if (!audSource.isPlaying) {
					nextDialogueTime = Time.time + Handy.Deviate (dialogueBaseInterval, dialogueIntervalVariation);
					ClipInfo ci = BaseAudioKeeper.GetRandomClipInfoWithID (cID_randomDialogue);
					if (ci == null) {
						return;
					}
					audSource.clip = ci.clip;
					audSource.volume = ci.volume;
					audSource.Play ();
				}
			}
		} else {
			audSource.Stop ();
		}
	}
	public void UseUrgentDialogue(PerPersonDialogueTypes ppdt)
	{
		if (UserSettings.SoundOn && !tpc.IsDead()) {

			ClipInfo ci;
			switch (ppdt) {
			case PerPersonDialogueTypes.RandomDialogue:
				ci = BaseAudioKeeper.GetRandomClipInfoWithID (cID_randomDialogue);
				if (ci != null) {
					nextDialogueTime = Time.time + Handy.Deviate (dialogueBaseInterval, dialogueIntervalVariation);
				}
				break;
			case PerPersonDialogueTypes.HitDialogue:
				if (Time.time < hitDialogueCDuntil)
					return;
				ci = BaseAudioKeeper.GetRandomClipInfoWithID (cID_gotHit);
				if (ci != null) {
					nextDialogueTime = Time.time + 2*Handy.Deviate (dialogueBaseInterval, dialogueIntervalVariation);
					hitDialogueCDuntil = Time.time + Handy.Deviate (hitDialogueCDduration,hitDialogueCDvariation);
				}
				break;
			case PerPersonDialogueTypes.WaveCleared:
				int rollVal = Random.Range (0, waveClearIntegerPeriod);
				if (rollVal != 0)
					return;
				ci = BaseAudioKeeper.GetRandomClipInfoWithID (cID_waveClear);
				if (ci != null) {
					nextDialogueTime = Time.time + Handy.Deviate (dialogueBaseInterval, dialogueIntervalVariation);
				}
				break;
			case PerPersonDialogueTypes.InspirationalShout:
				ci = BaseAudioKeeper.GetRandomClipInfoWithID (cID_inspirationalShout);
				if (ci != null) {
					nextDialogueTime = Time.time + 2*Handy.Deviate (dialogueBaseInterval, dialogueIntervalVariation);
				}
				break;
			default:
				ci = null;
				return;
				break;
			}

			if (ci == null) {
				Debug.Log ("joy bangla clip not found");
				return;
			}
			audSource.clip = ci.clip;
			audSource.volume = ci.volume;
			audSource.Play ();
		}
	}
}
public enum PerPersonDialogueTypes
{
	RandomDialogue,
	HitDialogue,
	WaveCleared,
	InspirationalShout
}
