using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneralManager : MonoBehaviour {
	public static GeneralManager instance;
	PseudoRandomArbitrator PRA;

    public static bool godMode = false;
	//public bool runningForPromo = false;
	public const bool soundsImplemented = true;

    public static int adShowIndex = 0;
    public const int adShowInterval = 3;

	public int level=-1;
	public int phase=-1;

	public bool leftRightIndicatorsDisabled= false;
	public bool healthKitDsiabled;
	public bool hideDisabled;
	private int healthKitsCount;
	public bool gameOver = false;

	public float levelProgress = 0;
	private int wavesCompleted_private = 0;
	public int wavesCompleted{
		set{ 
			wavesCompleted_private = value;
			levelProgress = ((float)wavesCompleted_private)/ ((float) totalWaves);
		}
		get{ 
			return wavesCompleted_private;
		}
	}
	public const int totalWaves = 10;

	public int availableHealthKits {
        get{ return healthKitsCount;}
		set
		{
			if (value > healthKitsCount) {
				HUDManager.instance.AnimateHKitButton ();
			}
			healthKitsCount = value;
		}
	}
	public bool coverFireDisabled;
    private int coverFireCount;
	public int availableCoverFires {
		get{ return coverFireCount;}
		set
		{
			if (value > coverFireCount) {
				HUDManager.instance.AnimateCoverFireButton ();
			}
			coverFireCount = value;
		}
	}

	public int killCount;
	public int headShotCount;
	private int killPoints;
	private int killPointBasedGiftingCount;
	private int killPointBasedGiftingInterval = 500;
	public int score
	{
		set{ killPoints = value;
            if ((level == 2 && phase == 3) || (level == 3 && phase == 1)) {
				if (value == 0) {
					killPointBasedGiftingCount = 0;
				}
				if (killPoints >= (killPointBasedGiftingCount + 1) * killPointBasedGiftingInterval) 
				{
					killPointBasedGiftingCount++;
					char c= PRA.Arbitrate ();
					if (c == 'H') 
					{
						AddHealthKit ();
					} 
					else if (c == 'C') 
					{
						AddCoverFire ();
					}
				}
			}
			HUDManager.Update_Score ();
		}
		get{ return killPoints; }
	}
					
	public Transform camTrans;



	void Awake()
	{
		instance = this;
		camTrans = Camera.main.transform;

	}
	public void AddHealthKit()
	{
		availableHealthKits++;
		if (UserSettings.GetSingleToolTipStatus (ToolTipType.HealthKit)) {
			Handy.DoAfter (this, () => {
				HUDManager.TriggerToolTip (ToolTipType.HealthKit);
			}, 0.5f, null);
		}
	}	
	public void AddCoverFire()
	{
		availableCoverFires++;
		if (UserSettings.GetSingleToolTipStatus (ToolTipType.CoverFire)) {
			Handy.DoAfter (this, () => {
				HUDManager.TriggerToolTip (ToolTipType.CoverFire);
			}, 0.5f, null);
		}
	}


	//public GoogleMobileAdsScript gmaInstance;
	public static void StartGame (int level, int phase)
	{
//        
//		#region GMAds init
//		GameObject go = new GameObject();
//		instance.gmaInstance = go.AddComponent<GoogleMobileAdsScript>();
//		#endregion
		instance.gameOver = false;
		instance.level = level;
		instance.phase = phase;

		instance.killCount = 0;
		instance.headShotCount = 0;
		instance.score = 0;

		instance.availableCoverFires = 0;
		instance.availableHealthKits = 0;

		instance.coverFireDisabled = (level == 1);
		instance.healthKitDsiabled = (level == 1);
		instance.hideDisabled = false;// ((level==1)&&(phase==3));
		instance.leftRightIndicatorsDisabled = 
			(level == 1 && phase == 4);
		switch(level)
		{
		case 1:
			switch (phase) 
			{
			case 1:
				instance.availableCoverFires = 1;
				break;
			case 2:
				instance.availableCoverFires = 1;
				instance.availableHealthKits = 1;
				break;
			case 3:
				break;
			case 4:
				break;
			}
			break;
		case 2:
			switch (phase) 
			{
			case 1:
				break;
			case 2:
				break;
			case 3:
				instance.availableCoverFires = 0;
				instance.availableHealthKits =0;
				if (instance.PRA == null) 
				{
					Dictionary<char,float> probabilityListing = new Dictionary<char,float>();
					probabilityListing.Add ('H', 0.5f);
					probabilityListing.Add ('C', 0.5f);
					instance.PRA = new PseudoRandomArbitrator(probabilityListing,0.5f);
				}
				break;
			}
			break;
        case 3:
            switch (phase) 
            {
                case 1:
                    instance.availableCoverFires = 1;
                    instance.availableHealthKits = 1;
                    if (instance.PRA == null) 
                    {
                        Dictionary<char,float> probabilityListing = new Dictionary<char,float>();
                        probabilityListing.Add ('H', 0.5f);
                        probabilityListing.Add ('C', 0.5f);
                        instance.PRA = new PseudoRandomArbitrator(probabilityListing,0.5f);
                    }
                    break;
            }
            break;
		}
	}
		
	public static int savedLevelScore = 0;

	public bool gameWon  = false;
	public static void EndGame(bool won, bool showAdd = true)
	{
		if (!instance.gameOver) 
		{
			instance.gameOver = true;
			instance.gameWon = won;
			//show add
			if(showAdd)
			{
                if (won)
                {
                    IronSourceManager.ShowSmartAd();
                    adShowIndex = 0;
                }
                else
                {
                    if (adShowIndex == 1)
                    {
                        IronSourceManager.ShowSmartAd();
                    }
                    adShowIndex++;
                    if (adShowIndex >= adShowInterval)
                        adShowIndex = 0;
                }
					
			}


			int prevphasebest = UserGameData.instance.GetScore (instance.level, instance.phase);
			savedLevelScore = ((instance.level == 1) ? UserGameData.instance.GetLevel1Score () : UserGameData.instance.GetLevel2Score ()) - prevphasebest + instance.score;



			UserGameData.instance.SetScore (instance.level, instance.phase, instance.score);
			SocialManagerScript.PostScoreToLeaderboard01(UserGameData.instance.GetTotalScore());
	
			if (!won) {
				InEndGameMenuManager.instance.InitEndGameMenu (won);
                HDKeep.lossCount.value++;
                if (HDKeep.lossCount.value >= ratePromptLossCount)
                {
                    RateUsFetcher.ConditionalFetch();
                }

			} else {
				if (((instance.level == 1) && (instance.phase == 4)) || ((instance.level == 2) && (instance.phase == 3))) {
					InEndGameMenuManager.instance.InitEndGameMenu (won);		
				} else {
					WeaponLoader.ClearWeapon (true);
				}

			}
		}
	}

    public const int ratePromptLossCount = 3;


	public void AddKP(HitType hitType)
	{
		switch (hitType) {
		case HitType.HEAD:
			headShotCount++;
			score += GameConstants.KPonHS;
			break;
		case HitType.BODY:
			score += GameConstants.KPonBS;
			break;
		case HitType.LIMB:
			score += GameConstants.KPonLS;
			break;
		case HitType.BLAST:
			score += GameConstants.KPonNade;
			break;
		case HitType.KNIFE:
			score += GameConstants.KPonKnife;
			break;
		}
		killCount++;
	}

}
