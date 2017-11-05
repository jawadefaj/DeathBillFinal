using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseAudioKeeper : MonoBehaviour {
	public List<ClipInfo> allAudioClips = new List<ClipInfo>();

	public static ClipInfo GetClipInfoWithID (ClipID id){
		if (instance == null) {
			Debug.LogError ("Audio Keeper instance not found!!");
			return null;
		}
		for (int i = 0; i < instance.allAudioClips.Count; i++) {
			if (instance.allAudioClips [i].id == id) {
				return instance.allAudioClips [i];
			}
		}
		Debug.LogError ("No audio clip with this id: "+id.ToString());
		return null;
	}
	public static AudioClip GetClipWithID (ClipID id){
		ClipInfo ci = GetClipInfoWithID (id);
		if (ci != null)
			return ci.clip;
		else
			return null;
	}
	public static List<ClipInfo> GetAllClipInfoWithID (ClipID id){
		List<ClipInfo> clipinfolist = new List<ClipInfo> ();
		if (instance == null) {
			Debug.LogError ("Audio Keeper instance not found!!");
			return clipinfolist;
		}
		for (int i = 0; i < instance.allAudioClips.Count; i++) {
			if (instance.allAudioClips [i].id == id) {
				clipinfolist.Add (instance.allAudioClips[i]);
			}
		}
		return clipinfolist;
	}
	public static List<AudioClip> GetAllClipWithID(ClipID id){
		List<AudioClip> cliplist = new List<AudioClip> ();
		if (instance == null) {
			Debug.LogError ("Audio Keeper instance not found!!");
			return cliplist;
		}
		for (int i = 0; i < instance.allAudioClips.Count; i++) {
			if (instance.allAudioClips [i].id == id) {
				cliplist.Add (instance.allAudioClips[i].clip);
			}
		}
		return cliplist;
	}
	public static ClipInfo GetRandomClipInfoWithID(ClipID id)
	{
		List<ClipInfo> cliplist = GetAllClipInfoWithID(id);
		if (cliplist.Count <= 0) {
			return null;
		}
		int index = Random.Range (0,cliplist.Count);
		return cliplist [index];
	}
	public static AudioClip GetRandomClipWithID(ClipID id)
	{
		ClipInfo ci = GetRandomClipInfoWithID (id);
		if (ci != null)
			return ci.clip;
		else
			return null;
	}



	public static BaseAudioKeeper instance;
	void Awake () {
		instance = this;
		foreach (Transform childTrans in this.transform) {
			ExtraAudioKeeper extraAud = childTrans.GetComponent<ExtraAudioKeeper> ();
			if (extraAud != null)
				allAudioClips.AddRange (extraAud.extraAudioClips);
		}
	}
	void OnDestroy()
	{
		instance = null;
	}
}
[System.Serializable]
public class ClipInfo
{
	public ClipID id;
	public AudioClip clip;
	[Range(0,1)]
	public float volume = 1;
}
public enum ClipID
{

	BG_Intro=0,
	BG_Music=1,
	BG_PauseMenu=2,
	BG_LOWHP_Male=3,
	BG_LOWHP_Female=4,


	gunFire_rifleM4=101,
    gunFire_rifleAR15 = 102,
	gunFire_smg_MP5= 103,
	gunFire_sniperRXM=104,
    gunFire_minigun = 105,
    gunFire_nadeLaunch = 106,
    gunFire_G3SG1 = 107,


	gunFire_sniperRXM_boltAct=204,
    gunFire_minigun_start = 205,
    gunFire_minigun_end = 206,
    gunFire_nadeBlast = 207,

    gunFire_AI_rifleAK47 =151,

	

    gunReload=10,
	bulletHitFlesh =11,
	mortarFire=12,
	shellBlast=13,
	nadeBlastAI=14,
	nadeBlastPlayer=15,
	engineTruck = 16,
	engineJeep = 17,
	engineChopper = 43,
	vehicleBreak = 18,
	joyBanglaShout= 19, 
	victoryClip = 20,
	defeatClip = 21,
	enemyHit = 22,
	enemyHitHS = 23,
	enemyDie = 24,
	enemyDieHS = 25,
	enemyDialogue = 26,
	enemyRunning = 27,


	personalDialoguesNura = 28,
	personalDialoguesKorim = 29,
	personalDialoguesJamal = 30,
	personalDialoguesKopila = 31,
	joyBanglaNura = 32,
	joyBanglaKorim = 33,
	joyBanglaJamal = 34,
	joyBanglaKopila = 35,


	walkSingleStep =36,
	knifeAirSwish = 37,
	knifeSlitThroat =38,
	rajakarDying =39,
	rajakarRunningAway =40,
	c4Beep = 41,
	fireSound =42,

	//used by value u wont find reference
	Bakers_RandomDialogue = 501, 
	Bakers_HitDialogue = 502,
	Bakers_WaveClearDialogue = 503,
	Bakers_InspirationShout = 504,

	JB_RandomDialogue = 511,
	JB_HitDialogue = 512,
	JB_WaveClearDialogue = 513,
	JB_InspirationShout = 514,

	Dom_RandomDialogue = 521,
	Dom_HitDialogue = 522,
	Dom_WaveClearDialogue = 523,
	Dom_InspirationShout = 524,

	Phillips_RandomDialogue = 531,
	Phillips_HitDialogue = 532,
	Phillips_WaveClearDialogue = 533,
	Phillips_InspirationShout = 534,

	Grump_RandomDialogue = 541,
	Grump_HitDialogue = 542,
	Grump_WaveClearDialogue = 543,
	Grump_InspirationShout = 544,

	Killary_RandomDialogue = 551,
	Killary_HitDialogue = 552,
	Killary_WaveClearDialogue = 553,
	Killary_InspirationShout = 554
	//=======================================
}