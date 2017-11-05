#define TEMP_SKIP

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class HUDManager : MonoBehaviour {
	public static HUDSettings hudSettings;
	public static void UpdateHUDSettings( HUDSettings settings)
	{
		hudSettings = settings;
	}

	public static HUDManager instance;
    [HideInInspector][SerializeField] public bool player2IntroTTPending = true;
    [HideInInspector]
    [SerializeField] public List<Sprite> playerSelectionButtonSprites = new List<Sprite>();
    [HideInInspector]
    [SerializeField] public List<Sprite> playerPortraitSprites = new List<Sprite>();
    [HideInInspector][SerializeField] public Sprite hideSprite;
    [HideInInspector][SerializeField] public Sprite unhideSprite;
    [HideInInspector][SerializeField] public Sprite scopeSprite;
    [HideInInspector][SerializeField] public Sprite unscopeSprite;
    [HideInInspector][SerializeField] public Sprite hkitSprite;
    public Image bloodSplashBase;
    public Image blackCoverImage;
	public GameObject nonTTPanel;
	public CurrentPlayerGroupClass currentPlayerGroup;
	public SelectablePlayersGroupClass selectablePlayersGroup;
	public SupportingPlayersGroupClass supportingPlayersGroup;
	public bool forceDisableRun_ShootGroup = false;
	public ShootGroupClass shootGroup;
	public RunGroupClass runGroup;

    internal Transform camTrans;
    #region rects for focus


    //public RectTransform[] playerSelectRects;
    public RectTransform playerSelectionPanelRect;

    public RectTransform killPointPanel;
    //public RectTransform killPointClaimButon;
    public RectTransform grenadeClaimButtonRect;
    public RectTransform backupClaimButtonRect;

    public RectTransform GrenedierFlashingIconSampleRect;

    //public PerPlayerHUDRefs[] perPlayerRefs;
    #endregion

    public Button nadeClaimButton;
    public Button backupClaimButton;


    public Image backupClaimImage;
    public Image nadeClaimImage;
    public Image waveLeftDial;
    public Text WaveNumberText;

    public Button nadeClaimableButton;

    public Text scoreDisplayText;

    internal bool rightIndicatorOn;
    internal bool leftIndicatorOn;
    internal float lastRightIndicationStartTime;
    internal float lastLeftIndicationStartTime;
    public const float indicationTime = 1.1f;

    public RectTransform scopeFilter;
    public bool canUseScope{
        get
        {
            return PlayerInputController.instance.CanCurrentPlayerScope();
        }
    }
    public bool isUsingScope
    {
        get
        {
            return PlayerInputController.instance.IsCurrentPlayerScoping();
        }
    }
	#region initializations
    void Awake()
    {
        blackCoverImage.gameObject.SetActive(true);
        instance = this;
		camTrans = Camera.main.transform;
		rightIndicatorOn = false;
		leftIndicatorOn = false;

		InitRectRefs();
		InitToolTips ();
    }
    void InitRectRefs()
    {
		Transform tempTrans;
        //allPlayerHpImages = new Image[playerSelectRects.Length];
		for (int i = 0; i < selectablePlayersGroup.perPlayerRefs.Length; i++)
        {

            tempTrans = selectablePlayersGroup.perPlayerRefs[i].highlightRect.FindChild("Dial");
            selectablePlayersGroup.perPlayerRefs[i].slicedHPImage = tempTrans.GetComponent<Image>();

            tempTrans = selectablePlayersGroup.perPlayerRefs[i].highlightRect.FindChild("SelectButton");
            selectablePlayersGroup.perPlayerRefs[i].switchToButton = tempTrans.GetComponent<Button>();
            selectablePlayersGroup.perPlayerRefs[i].portraitImage = tempTrans.GetComponent<Image>();
            int j = i;
			selectablePlayersGroup.perPlayerRefs [i].switchToButton.onClick.RemoveAllListeners ();
            selectablePlayersGroup.perPlayerRefs[i].switchToButton.onClick.AddListener(() => { SwitchPlayer(j); });
			selectablePlayersGroup.perPlayerRefs [i].imageAnimator = selectablePlayersGroup.perPlayerRefs [i].portraitImage.GetComponent<Animator> ();

        }
		for (int i = 0; i < supportingPlayersGroup.perPlayerRefs.Length; i++)
		{

			tempTrans = supportingPlayersGroup.perPlayerRefs[i].highlightRect.FindChild("HPBarParent").FindChild("HPBar");
			supportingPlayersGroup.perPlayerRefs[i].slicedHPImage = tempTrans.GetComponent<Image>();

			tempTrans = supportingPlayersGroup.perPlayerRefs[i].highlightRect.FindChild("Portrait");
			supportingPlayersGroup.perPlayerRefs[i].portraitImage = tempTrans.GetComponent<Image>();
		}
    }
	void InitToolTips()
	{		
		waitingForTap = false;
		waitingInitialized = false;
		blinkRate = 1.0f;
		SizeMultiplier = 1.1f;
		minimumTTScreenDisableTime = 1.0f;
	}
    void Start()
    {
        StartCoroutine(ToolTipStartingSequence());
		InitialUpdateCall();
        //UserSettings.TutorialOn = true;
    }
	#endregion
    #region HUD Update
	void Update()
	{
		if (PlayerInputController.instance == null && SneakyPlayerManager.instance==null)
			return;
		if (ToolTipUpdating()) return;
		CheckAndShoot();
		if (Input.GetKeyDown(KeyCode.Escape))
		{
            if (!GeneralManager.instance.gameOver)
            {
                if (!InEndGameMenuManager.instance.PauseMenu.activeSelf)
                {
                    Pause();
                }
                else
                {
                    InEndGameMenuManager.instance.Resume();
                }
            }
            else
            {
                InEndGameMenuManager.instance.MainMenu();
            }

		}
	}
	void FixedUpdate()
	{
		UpdateHUDItems ();
	}

    public void UpdateHUDItems()
    {
		if (hudSettings == null) {
			Debug.Log ("No hud settings found!");
			return;
		}

		currentFFID = FighterRole.NotActive;
		if (PlayerInputController.instance != null)
		{
			if (PlayerInputController.instance.current_player != null)
				currentFFID = PlayerInputController.instance.current_player.fighterRole;
		}
		else if (SneakyPlayerManager.instance!=null)
		{
			if (SneakyPlayerManager.instance.GetCurrentPlayer () != null)
				currentFFID = SneakyPlayerManager.instance.GetCurrentPlayer ().fighterRole;
		}

		U_GroupSwitching ();
		U_CurrentPlayerHPandPortrait ();
		U_OtherPlayersHPandPortrait();

		switch (hudSettings.baseType) {
		case HUDBaseType.SHOOT:
			ShootUpdate ();
			break;
		case HUDBaseType.RUN:
			RunUpdate ();
			break;
		case HUDBaseType.KNIFE:
			KnifeUpdate ();
			break;
		}
    }
	void ShootUpdate()
	{
		U_AMMO ();
        U_EnemyIndicators ();
        U_A();
        U_B();
        U_C();
        U_D();
		U_SideMovementButtons ();
		U_BloodBasedOnHP ();
		U_C4Stuff ();
        U_ScopeSpecific();
	}
	void RunUpdate()
	{ 
		U_BloodBasedOnHP ();
	}
	void KnifeUpdate()
	{
		U_StabButtonAnimation ();
		U_NoiseLevel ();
	}



	#region Irregular HUD updates
	public static void Update_Score()
	{
		if(instance==null || GeneralManager.instance == null) return; 
		instance.scoreDisplayText.text = GeneralManager.instance.score.ToString();
	}
	public static void InitialUpdateCall()
	{
		Update_Score();
	}
	#endregion
	#region Regular HUD updates
	private void U_GroupSwitching()
	{
		switch(hudSettings.baseType)
		{
		case HUDBaseType.SHOOT:
			if (forceDisableRun_ShootGroup) 
			{
				if (shootGroup.rect.gameObject.activeSelf)shootGroup.rect.gameObject.SetActive (false);
				if (RapidFireButton.instance != null) {
					if (RapidFireButton.instance.pressedOn)
						RapidFireButton.instance.pressedOn = false;
				}
			} 
			else 
			{
				if(!shootGroup.rect.gameObject.activeSelf)shootGroup.rect.gameObject.SetActive (true);
			}
			if(runGroup.rect.gameObject.activeSelf)runGroup.rect.gameObject.SetActive (false);
			break;
		case HUDBaseType.KNIFE:
		case HUDBaseType.RUN:
			if (RapidFireButton.instance != null) {
				if (RapidFireButton.instance.pressedOn)
					RapidFireButton.instance.pressedOn = false;
			}
			if (forceDisableRun_ShootGroup) {
				if (runGroup.rect.gameObject.activeSelf)
					runGroup.rect.gameObject.SetActive (false);
			} else {
				if (!runGroup.rect.gameObject.activeSelf)
					runGroup.rect.gameObject.SetActive (true);
			}
			if (shootGroup.rect.gameObject.activeSelf)
				shootGroup.rect.gameObject.SetActive (false);
			if (hudSettings.baseType == HUDBaseType.KNIFE) {
				if (!runGroup.knifeSpecific.activeSelf)
					runGroup.knifeSpecific.SetActive (true);
			} else {
				if(runGroup.knifeSpecific.activeSelf)
					runGroup.knifeSpecific.SetActive (false);
			}
			break;
		}
	}
	FighterRole currentFFID;
    FighterName lastFighter;
    float currentPlayerHPDisplayValue;
	private void U_CurrentPlayerHPandPortrait()
	{
        //Debug.Log("whaaat");

        if (hudSettings.baseType != HUDBaseType.KNIFE)
        {
            currentPlayerGroup.HPBar.fillAmount = 1;
            if (currentPlayerGroup.portrait.sprite != GetFreedomFighterSprite(currentFFID, false))//true))
            {
                currentPlayerGroup.portrait.sprite = GetFreedomFighterSprite(currentFFID, false);//true);
                currentPlayerHPDisplayValue = PlayerInputController.instance.GetPlayerByRole (currentFFID).healthPoint / 100.0f;
            }
            else
            {
                currentPlayerHPDisplayValue = Mathf.Lerp(currentPlayerHPDisplayValue,PlayerInputController.instance.GetPlayerByRole (currentFFID).healthPoint / 100.0f,0.3f);
            }
            currentPlayerGroup.HPBar.fillAmount = currentPlayerHPDisplayValue;
        }
        else
        {
            currentPlayerGroup.HPBar.fillAmount = 1;
            if (currentPlayerGroup.portrait.sprite != GetFreedomFighterSprite(currentFFID, false))//true))
            {
                currentPlayerGroup.portrait.sprite = GetFreedomFighterSprite(currentFFID, false);//true);
            }
        }

      

	}
    private Sprite GetFreedomFighterSprite(FighterRole fighterRole, bool focusedVersion = false){
        FighterName fName= FighterName.None;
        if (PlayerInputController.instance != null)
            fName = PlayerInputController.instance.GetPlayerByRole(fighterRole).fighterName;
        else if (SneakyPlayerManager.instance != null)
            fName = SneakyPlayerManager.instance.GetSneakyPlayerByRole(fighterRole).fighterName;
        else
            Debug.Log("What the fuck!");
        int i_ff = (int)fName;
        Sprite retVal;
        if (!focusedVersion)
        {
            if ( i_ff >= playerSelectionButtonSprites.Count  || i_ff < 0)
                retVal = null;
            else
                retVal = playerSelectionButtonSprites[(int)fName];
        }
        else
        {
            if (i_ff >= playerPortraitSprites.Count || i_ff < 0)
                retVal = null;
            else
                retVal = playerPortraitSprites[(int)fName];
        }
        return retVal;
	}

	List<FighterRole> tempFFList = new List<FighterRole>();

	private void U_OtherPlayersHPandPortrait()
	{
		
        if (hudSettings.baseType == HUDBaseType.KNIFE) 
        {
			selectablePlayersGroup.rect.gameObject.SetActive (false);
			supportingPlayersGroup.rect.gameObject.SetActive (false);
			return;
		} 
//        else if(isUsingScope)
//        {
//            selectablePlayersGroup.rect.gameObject.SetActive (true);
//            supportingPlayersGroup.rect.gameObject.SetActive (false);
//        }
        else
        {
			selectablePlayersGroup.rect.gameObject.SetActive (true);
			supportingPlayersGroup.rect.gameObject.SetActive (true);
		}
		int avail_N = hudSettings.availablePlayerList.Count;
		if (avail_N > 1) {
			List<FighterRole> engagedfflist = AIDataManager.EngagedAllyList ();
			for (int i = 0; i < selectablePlayersGroup.perPlayerRefs.Length; i++) {
				if (i < avail_N) {
					if (!selectablePlayersGroup.perPlayerRefs [i].highlightRect.gameObject.activeSelf) 
						selectablePlayersGroup.perPlayerRefs [i].highlightRect.gameObject.SetActive (true);
                    
					if( selectablePlayersGroup.perPlayerRefs [i].portraitImage.sprite != GetFreedomFighterSprite (hudSettings.availablePlayerList [i]))
						selectablePlayersGroup.perPlayerRefs [i].portraitImage.sprite = GetFreedomFighterSprite (hudSettings.availablePlayerList [i]);
                    
					ThirdPersonController tpc = PlayerInputController.instance.GetPlayerByRole (hudSettings.availablePlayerList [i]);
					selectablePlayersGroup.perPlayerRefs [i].slicedHPImage.fillAmount = tpc.healthPoint/100.0f;

                    //selectablePlayersGroup.perPlayerRefs[i].switchToButton.interactable = !PlayerInputController.instance.IsCoverFireOn();

					if (currentFFID == tpc.fighterRole) {
						selectablePlayersGroup.perPlayerRefs [i].imageAnimator.SetFloat ("COLVAL", 0.0f);
					}
					else if (tpc.wasHitRecentlyHUDFLASHVERSION && (Time.time - tpc.lastHitTime < 4f)) 
                    {
						if (i == 1) 
                        {
                            if (player2IntroTTPending)
                            {
                                player2IntroTTPending = false;
                                HUDManager.TriggerToolTip (ToolTipType.Player2);
                            }
						}
						selectablePlayersGroup.perPlayerRefs [i].imageAnimator.SetFloat ("COLVAL", 1.0f);
					}
					else if (engagedfflist.Contains (hudSettings.availablePlayerList [i])) {
						selectablePlayersGroup.perPlayerRefs [i].imageAnimator.SetFloat ("COLVAL", 0.5f);
					}
					else {
						selectablePlayersGroup.perPlayerRefs [i].imageAnimator.SetFloat ("COLVAL", 0.0f);
					}

				} 
                else 
                {
					if (selectablePlayersGroup.perPlayerRefs [i].highlightRect.gameObject.activeSelf)
						selectablePlayersGroup.perPlayerRefs [i].highlightRect.gameObject.SetActive (false);
				}
			}
		} 
		else 
		{
			for (int i = 0; i < selectablePlayersGroup.perPlayerRefs.Length; i++) {
				if (selectablePlayersGroup.perPlayerRefs [i].highlightRect.gameObject.activeSelf) selectablePlayersGroup.perPlayerRefs [i].highlightRect.gameObject.SetActive (false);
			}
		}

		tempFFList.Clear ();
		tempFFList.AddRange (hudSettings.availablePlayerList);
		tempFFList.AddRange (hudSettings.supportingPlayerList);
		tempFFList.Remove (currentFFID);

		int sup_N = tempFFList.Count;
		for (int i = 0; i < supportingPlayersGroup.perPlayerRefs.Length; i++) {
			if (i < sup_N) {
				if (!supportingPlayersGroup.perPlayerRefs [i].highlightRect.gameObject.activeSelf) 
					supportingPlayersGroup.perPlayerRefs [i].highlightRect.gameObject.SetActive (true);
				//Debug.Log (supportingPlayersGroup.perPlayerRefs [i]);
                if( supportingPlayersGroup.perPlayerRefs [i].portraitImage.sprite != GetFreedomFighterSprite (tempFFList[i],false))//, true))
                    supportingPlayersGroup.perPlayerRefs [i].portraitImage.sprite = GetFreedomFighterSprite (tempFFList[i],false);//, true);
				supportingPlayersGroup.perPlayerRefs [i].slicedHPImage.fillAmount = PlayerInputController.instance.GetPlayerByRole (tempFFList[i]).healthPoint/100.0f;
			} else {
				if (supportingPlayersGroup.perPlayerRefs [i].highlightRect.gameObject.activeSelf)
					supportingPlayersGroup.perPlayerRefs [i].highlightRect.gameObject.SetActive (false);
			}
		}
	}
	public static void ClearSelectablePlayerAnimStates()
	{ 
		if (HUDManager.instance != null) {
			foreach (PerPlayerHUDRefs pp in HUDManager.instance.selectablePlayersGroup.perPlayerRefs) {
				pp.imageAnimator.SetTrigger ("FORCEWHITE");
			}
		}
	}


	#region KnifeSpecific
	public  bool animateStabButton = false;
	void U_StabButtonAnimation()
	{
		if (animateStabButton) {
			runGroup.stabAnimator.SetBool ("focusOn",true);
		} else {
			runGroup.stabAnimator.SetBool ("focusOn",false);
		}
	}
	void U_NoiseLevel () {
		runGroup.noiseLevelDisplay.fillAmount = SneakyPlayerManager.instance.GetNoiseLevel ()/SneakyPlayerManager.MAX_ALLOWABLE_NOISE_LEVEL;
	}
	#endregion
	#region ShootOrRun
	void U_AMMO()
	{
		shootGroup.noDragGroup.primaryFire_ProgressDial.fillAmount = PlayerInputController.instance.current_player.assignedWeapon.AmmoPercentage();
        shootGroup.noDragGroup.A_CountText.text = PlayerInputController.instance.current_player.assignedWeapon.RemainingAmmo().ToString();
	}
	void U_EnemyIndicators()
	{
		if (rightIndicatorOn)
		{
			if (Time.time - lastRightIndicationStartTime >= indicationTime)
			{
				rightIndicatorOn = false;
				shootGroup.enemyIndicatorGroup.rightIndicator.SetActive(false);
			}
		}
		if (leftIndicatorOn)
		{
			if (Time.time - lastLeftIndicationStartTime >= indicationTime)
			{
				leftIndicatorOn = false;
				shootGroup.enemyIndicatorGroup.leftIndicator.SetActive(false);
			}
		}
	}
	void U_SideMovementButtons()
	{
		//if (!PlayerInputController.instance.current_player.GetStationController ().IsMoving ()) {
		Portbliss.Station.StationController pssc = PlayerInputController.instance.current_player.GetStationController ();
        if ((pssc.leftWayOpen || pssc.rightWayOpen) && !isUsingScope) {
			if(!shootGroup.positionSwapGroup.leftMoveButton.gameObject.activeSelf) shootGroup.positionSwapGroup.leftMoveButton.gameObject.SetActive (true);
			if(!shootGroup.positionSwapGroup.rightMoveButton.gameObject.activeSelf) shootGroup.positionSwapGroup.rightMoveButton.gameObject.SetActive (true);

			shootGroup.positionSwapGroup.leftMoveButton.interactable = PlayerInputController.instance.current_player.GetStationController ().leftWayOpen;
			shootGroup.positionSwapGroup.rightMoveButton.interactable = PlayerInputController.instance.current_player.GetStationController ().rightWayOpen;
		} else {
			if(shootGroup.positionSwapGroup.leftMoveButton.gameObject.activeSelf) shootGroup.positionSwapGroup.leftMoveButton.gameObject.SetActive (false);
			if(shootGroup.positionSwapGroup.rightMoveButton.gameObject.activeSelf) shootGroup.positionSwapGroup.rightMoveButton.gameObject.SetActive (false);
		}
		//}
	}
    void U_A()
    {
        if (PlayerInputController.instance.current_player.IsReloading())
        {
            if (! shootGroup.noDragGroup.A_ProgressDialReload.gameObject.activeSelf) shootGroup.noDragGroup.A_ProgressDialReload.gameObject.SetActive(true);

            float filAmnt=  (shootGroup.noDragGroup.A_ProgressMax-shootGroup.noDragGroup.A_ProgressMin)*PlayerInputController.instance.current_player.GetReloadPercentage();
            //Debug.Log("Fill amount is: "+filAmnt.ToString());
            shootGroup.noDragGroup.A_ProgressDialReload.fillAmount = filAmnt;
        }
        else
        {
            if (shootGroup.noDragGroup.A_ProgressDialReload.gameObject.activeSelf) shootGroup.noDragGroup.A_ProgressDialReload.gameObject.SetActive(false);
        }
    }
	void U_D()
	{
		if (GeneralManager.instance.hideDisabled) 
        {
			if (shootGroup.noDragGroup.D_RectHideUnhide.gameObject.activeSelf) shootGroup.noDragGroup.D_RectHideUnhide.gameObject.SetActive (false);
		} 
        else 
        {
			if(!shootGroup.noDragGroup.D_RectHideUnhide.gameObject.activeSelf) shootGroup.noDragGroup.D_RectHideUnhide.gameObject.SetActive (true);
            if (PlayerInputController.instance.current_player.IsHiding() && shootGroup.noDragGroup.D_ButtonImage_HideUnhide .sprite != unhideSprite)
            {
                shootGroup.noDragGroup.D_ButtonImage_HideUnhide.sprite = unhideSprite;
            }
            if (!PlayerInputController.instance.current_player.IsHiding() && shootGroup.noDragGroup.D_ButtonImage_HideUnhide.sprite != hideSprite)
            {
                shootGroup.noDragGroup.D_ButtonImage_HideUnhide.sprite = hideSprite;
            }
		}
	}
    int healthKitCounter = 0;
    int coverFireCounter = 0;

    void U_B()
    {
        if (canUseScope)
        {
            if(!shootGroup.noDragGroup.B_Rect.gameObject.activeSelf) shootGroup.noDragGroup.B_Rect.gameObject.SetActive(true);
            shootGroup.noDragGroup.B_CountText.text = "";
            //Debug.Log(isUsingScope);
            if (isUsingScope)
                shootGroup.noDragGroup.B_ButtonImage.sprite = unscopeSprite;
            else
                shootGroup.noDragGroup.B_ButtonImage.sprite = scopeSprite;
            shootGroup.noDragGroup.B_Button.interactable = true;
        }
        else
        {
            if (GeneralManager.instance.healthKitDsiabled)
            {
                if (shootGroup.noDragGroup.B_Rect.gameObject.activeSelf)
                    shootGroup.noDragGroup.B_Rect.gameObject.SetActive(false);
            }
            else
            {
                if (!shootGroup.noDragGroup.B_Rect.gameObject.activeSelf)
                    shootGroup.noDragGroup.B_Rect.gameObject.SetActive(true);
            }


            shootGroup.noDragGroup.B_ButtonImage.sprite = hkitSprite;
            healthKitCounter = GeneralManager.instance.availableHealthKits;
            if (healthKitCounter > 0)
            {
                shootGroup.noDragGroup.B_CountText.text = healthKitCounter.ToString();
            }
            else
            {
                shootGroup.noDragGroup.B_CountText.text = "";
            }
            shootGroup.noDragGroup.B_Button.interactable = (healthKitCounter > 0);// && PlayerInputController.instance.current_player.CanThrowGranade();
        }
    }
    void U_C()
    {
        if (GeneralManager.instance.coverFireDisabled)
        {
            shootGroup.noDragGroup.C_Rect.gameObject.SetActive(false);
            shootGroup.noDragGroup.C_ProgressDial.gameObject.SetActive(false);
            return;
        }
        else
        {
            shootGroup.noDragGroup.C_Rect.gameObject.SetActive(true);
            shootGroup.noDragGroup.C_ProgressDial.gameObject.SetActive(true);
        }

        coverFireCounter =GeneralManager.instance.availableCoverFires;

        if (coverFireCounter > 0)
        {
            shootGroup.noDragGroup.C_CountText.text = coverFireCounter.ToString();
        }
        else
        {
            shootGroup.noDragGroup.C_CountText.text = "";
        }


        if (PlayerInputController.instance.IsCoverFireOn())
        {
            shootGroup.noDragGroup.C_ProgressDial.gameObject.SetActive(true);
            shootGroup.noDragGroup.C_ProgressDial.fillAmount = (shootGroup.noDragGroup.C_ProgressMax-shootGroup.noDragGroup.C_ProgressMin)*(1-PlayerInputController.instance.GetCoverFirePercent());
            shootGroup.noDragGroup.C_Button.interactable = true;
        }
        else
        {
            shootGroup.noDragGroup.C_ProgressDial.gameObject.SetActive(false);
            shootGroup.noDragGroup.C_Button.interactable = coverFireCounter > 0;
        }



    }

    void U_ScopeSpecific()
    {

        shootGroup.crossHair.gameObject.SetActive(!isUsingScope);
        scopeFilter.gameObject.SetActive(isUsingScope);
    }

	float bloodVisionStartHP = 65;
	float bloodOpacityAbsoluteTarget = 0;
	float bloodOpacityLerpedTarget = 0;
	float bloodOpacityShiftedTarget = 0;

	float bloodOpacityValue = 0;
	float opacityFlactuationMaxAmplitude = 0.30f;
	float opacityFlactuationCurrentAmplitude = 0;
	float flactuationDirection = 1;
	float flactuation = 0;
	float flactuationRate = 0.015f;
	Color tempCol;
	public void U_BloodBasedOnHP()
	{
		bloodOpacityAbsoluteTarget = Mathf.Clamp(((bloodVisionStartHP - PlayerInputController.instance.current_player.healthPoint) / bloodVisionStartHP), 0, 1);
		bloodOpacityLerpedTarget = Mathf.Lerp(bloodOpacityLerpedTarget, bloodOpacityAbsoluteTarget, 0.1f);
		bloodOpacityShiftedTarget = bloodOpacityLerpedTarget * (1 - opacityFlactuationMaxAmplitude);
		if (PlayerInputController.instance.current_player.healthPoint <= bloodVisionStartHP)
		{


			opacityFlactuationCurrentAmplitude = bloodOpacityLerpedTarget * opacityFlactuationMaxAmplitude;

			//Debug.Log("LerpedTarget: " + bloodOpacityLerpedTarget .ToString()+ " , curentA: "+ opacityFlactuationCurrentAmplitude.ToString());

			if (flactuation >= opacityFlactuationCurrentAmplitude)
			{
				flactuationDirection = -1;
			}
			else if (flactuation <= -opacityFlactuationCurrentAmplitude)
			{
				flactuationDirection = 1;
			}


			flactuation += flactuationRate * bloodOpacityAbsoluteTarget * flactuationDirection;
			bloodOpacityValue = bloodOpacityShiftedTarget + flactuation;
			//Debug.Log("flactuation: " + flactuation.ToString() + "val: " + bloodOpacityValue.ToString() + " , dir: " + flactuationDirection.ToString());
		}
		else
		{
			bloodOpacityValue = bloodOpacityShiftedTarget;
		}
		tempCol = bloodSplashBase.color;
		tempCol.a = Mathf.Clamp(bloodOpacityValue, 0, 1);
		bloodSplashBase.color = tempCol;
	}
	private void U_C4Stuff()
	{
		if (GeneralManager.instance.level == 2 && GeneralManager.instance.phase == 3) {
			if(!shootGroup.c4Group.rect.gameObject.activeSelf)shootGroup.c4Group.rect.gameObject.SetActive (true);
			shootGroup.c4Group.progressImage.fillAmount = GeneralManager.instance.levelProgress;
			if (GeneralManager.instance.levelProgress >= 0.99f) {
				shootGroup.c4Group.beepAnimator.speed = 1;
				//Debug.Log ("c4 beeping animation not set");
				shootGroup.c4Group.beepAnimator.SetTrigger ("BLAST");
			} else {
				float animSpeed = 0.56f;
				float factor = 1.2f;
				int iteration = Mathf.CeilToInt(GeneralManager.instance.levelProgress*0.999f * GeneralManager.totalWaves);
				//Debug.Log ("Ite: "+iteration.ToString()+",p: "+GeneralManager.instance.levelProgress.ToString()+",t: "+GeneralManager.totalWaves.ToString());
				//Debug.Log (iteration);
				for (int i = 0; i < iteration; i++) {
					//Debug.Log ("in the looooop");
					animSpeed *= factor;
				}

				//Debug.Log (animSpeed);
				shootGroup.c4Group.beepAnimator.speed = animSpeed;
			}

		} else {
			if(shootGroup.c4Group.rect.gameObject.activeSelf)shootGroup.c4Group.rect.gameObject.SetActive (false);
		}
	}
	#endregion

	//    //kp and wave panel
	//    public void U_EnemyLeft()
	//    {
	//		#if !TEMP_SKIP
	//        if (GameManagerScript.instance.gameWon) waveLeftDial.fillAmount = 1;
	//        else  waveLeftDial.fillAmount =((float) GameManagerScript.instance.currentWaveNumber-1 )/ GameConstants.rounds2win;
	//		#endif
	//    }
	//    public void U_WaveNumber()
	//    {
	//		#if !TEMP_SKIP
	//		WaveNumberText.text = GameManagerScript.instance.currentWaveNumber.ToString();
	//		#endif
	//    }
	#endregion  
    #endregion
	#region communication functions
	public void AnimateHKitButton()
	{
		shootGroup.noDragGroup.B_Animator.SetTrigger ("BLINK");
	}
	public void AnimateCoverFireButton()
	{
		shootGroup.noDragGroup.C_Animator.SetTrigger ("BLINK");
	}
	public void RightIndicatorShow()
	{
		if (GeneralManager.instance.leftRightIndicatorsDisabled) {
			return;
		}
		rightIndicatorOn = true;
		shootGroup.enemyIndicatorGroup.rightIndicator.SetActive(true);
		lastRightIndicationStartTime = Time.time; 
	}
	public void LeftIndicatorShow()
	{
		if (GeneralManager.instance.leftRightIndicatorsDisabled) {
			return;
		}
		leftIndicatorOn = true;
		shootGroup.enemyIndicatorGroup.leftIndicator.SetActive(true);
		lastLeftIndicationStartTime = Time.time;

	}
	IEnumerator DeactivateAfter(GameObject go, float time)
	{
		yield return new WaitForSeconds(time);
		go.SetActive(false);
	}
	#endregion
    #region inputs
	//internal int currentCharacterID;
	public void SwitchPosition(bool goLeft)
	{
		if (goLeft) {
			PlayerInputController.instance.GUI_MovePrevCoverPoint ();
		} else {

			PlayerInputController.instance.GUI_MoveNextCoverPoint ();
		}
	}
	public void SwitchPlayer(int id)
	{
        if (id >= hudSettings.availablePlayerList.Count || id < 0)
            Debug.LogError("Request to switch player had an unexpected index!");
        
        FighterRole ff= hudSettings.availablePlayerList[id];
		PlayerInputController.instance.GM_SwitchToPlayer(ff);
        //PlayerInputController.instance.GUI_SwitchToPlayer(ff);
	}
		

    public void CheckAndShoot()
    {
        if (RapidFireButton.instance.pressedOn)
        {
            PlayerInputController.instance.GUI_Shoot();
        }
    }
    #region ABCD
    #region button bindings
    public void Func_A()
    {
        Reload();
    }
	public void Func_B()
	{
        if (!canUseScope)
        {
            UseHealthKit();
        }
        else
        {
            ToggleScope();
        }

	}
    public void Func_C()
    {
        UseCoverFire();
    }
    public void Func_D()
    {
        Hide();
    }
    #endregion
    #region actual funtionalities
    private void Reload()
    {
        PlayerInputController.instance.GUI_Reload();
    }
    private void ToggleScope()
    {
        if (isUsingScope)
        {
            PlayerInputController.instance.GUI_SetCurrentPlayerState(PlayerState.Aim);
        }
        else
        {
            PlayerInputController.instance.GUI_SetCurrentPlayerState(PlayerState.Scope);
        }
    }
    private void UseHealthKit()
    {
        GeneralManager.instance.availableHealthKits--;
        PlayerInputController.instance.current_player.healthPoint += GameConstants.healthKitHealAmount;
    }
//    private void UseGrenade()
//    {            
//        GeneralManager.instance.availableHealthKits--;
//        InGameSoundManagerScript.instance.ShoutJoyBanglaAll ();
//        PlayerInputController.instance.GUI_Granade ();
//    }
    private void UseCoverFire()
    {
        if (PlayerInputController.instance.IsCoverFireOn())
        {
            return;
        }
        else if(GeneralManager.instance.availableCoverFires>0)
        {
            GeneralManager.instance.availableCoverFires--;
            PlayerInputController.instance.GUI_CoverFire(10f);
        }
        //PlayerInputController.instance.current_player.healthPoint += GameConstants.healthKitHealAmount;
    }
    private void Hide()
    {
        
        //PlayerInputController.instance.GUI_Hide();
        if (PlayerInputController.instance.isAiming)
        {
            PlayerInputController.instance.GUI_SetCurrentPlayerState(PlayerState.Hide);
        }
        else
        {
            PlayerInputController.instance.GUI_SetCurrentPlayerState(PlayerState.Aim);
        }

    }
    #endregion
    #endregion
    public void Pause()
    {
        InEndGameMenuManager.instance.DisplayPauseMenu();
    }
	public void Walk()
	{
        if (hudSettings.baseType == HUDBaseType.KNIFE)
        {
            SneakyPlayerManager.instance.Walk();
        }
        else
        {
            PlayerInputController.instance.current_player.GetComponent<Portbliss.Station.StationController>().SpeedUp();
        }

    }
	public void Stab()
	{
		SneakyPlayerManager.instance.Stab();
	}

    #endregion
	#region toolTips New
	IEnumerator ToolTipStartingSequence()
	{
		yield return new WaitForSeconds (1);
		if (!((GeneralManager.instance.level == 2) && (GeneralManager.instance.phase ==1))) {
			yield return new WaitForSeconds (5.5f);
			TriggerToolTip (ToolTipType.Aim);
			yield return new WaitForSeconds (1.0f);
			TriggerToolTip (ToolTipType.Shoot);
		} else {
			yield return new WaitForSeconds (4.0f);
			TriggerToolTip (ToolTipType.Noise);
			yield return new WaitForSeconds (1.0f);
			TriggerToolTip (ToolTipType.Stab);
		}

	}


	#region toolTipListUps
	public static void TriggerToolTip(ToolTipType ttType)
	{
		if (UserSettings.GetSingleToolTipStatus (ttType)) {
			if(!instance.queuedTT.Contains(ttType))
			instance.queuedTT.Add (ttType);
		}
	}

    public void DisplayTTNow(ToolTipType ttType)
    {
        switch (ttType) {
            //L1 P1
            case ToolTipType.Aim:
                ShowToolTipForThiRect(null, "<b><size=100><color=#ffa500ff>Swipe</color></size></b> the screen to rotate the aiming direction");
                break;
            case ToolTipType.Shoot:
                ShowToolTipForThiRect(shootGroup.noDragGroup.primaryFireRect, "Touch the Fire Button to <b><size=100><color=#ffa500ff>Shoot!</color></size></b>");
                break;  
            case ToolTipType.Hide:
                ShowToolTipForThiRect(shootGroup.noDragGroup.D_RectHideUnhide, "<b><size=100><color=#ffa500ff>Hide</color></size></b> to take less damage");
                break;
            case ToolTipType.Reload:
                ShowToolTipForThiRect(shootGroup.noDragGroup.A_RectReload, "Use the <b><size=100><color=#ffa500ff>Reload</color></size></b> button to refill");
                break;
                //L1 P2
            case ToolTipType.Grenadier:
                ShowToolTipForThiRect(GrenedierFlashingIconSampleRect, "Watch out for <b><size=100><color=#ffa500ff>Grenade Alerts</color></size></b>", true);
                break;
            case ToolTipType.PositionSwap:
                ShowToolTipForThiRect(shootGroup.positionSwapGroup.rect, "Change your positions to <b><size=100><color=#ffa500ff>Avoid Grenades</color></size></b>");
                break;
                //L1 P4
            case ToolTipType.Walk:
                ShowToolTipForThiRect(runGroup.walkButtonRect, "Tap the button to control your <b><size=100><color=#ffa500ff>Speed</color></size></b>");
                break;
                // L2 P1
            case ToolTipType.Noise:
                ShowToolTipForThiRect (runGroup.noiseLevelRect, "Walking fast will increase your <b><size=100><color=#ffa500ff>Noise Level</color></size></b>");
                break;
            case ToolTipType.Stab:
                ShowToolTipForThiRect (runGroup.stabButtonRect, "<b><size=100><color=#ffa500ff>Stab</color></size></b> the enemy when you are close enough");
                break;
            case ToolTipType.Vision:
                ShowToolTipForThiRect (null, "Avoid the enemies <b><size=100><color=#ffa500ff>Vision</color></size></b> at all costs!");
                break;
                //L2 P3
            case ToolTipType.Player2:
                {
                    FighterName fname = PlayerInputController.instance.GetPlayerByRole(FighterRole.Leader).fighterName;

                    if (fname == FighterName.Baker)
                    {
                        ShowToolTipForThiRect(selectablePlayersGroup.perPlayerRefs[1].highlightRect, "Enemies have enaged Bakers! Press the button to <b><size=100><color=#ffa500ff>Switch</color></size></b> between Players");
                    }
                    else
                    {
                        ShowToolTipForThiRect(selectablePlayersGroup.perPlayerRefs[1].highlightRect, "Enemies have enaged Dom! Press the button to <b><size=100><color=#ffa500ff>Switch</color></size></b> between Players");
                    }
                    return;
                }
                break;
            case ToolTipType.Player3:
                {
                    FighterName fname = PlayerInputController.instance.GetPlayerByRole(FighterRole.Sniper).fighterName;

                    if (fname == FighterName.JB)
                    {
                        ShowToolTipForThiRect(selectablePlayersGroup.perPlayerRefs[2].highlightRect, "<b><size=100><color=#ffa500ff>MORTAR!</color></size></b>. Switch to <b><size=100><color=#ffa500ff>JB</color></size></b> and take advantage of her scope!");
                    }
                    else
                    {
                        ShowToolTipForThiRect(selectablePlayersGroup.perPlayerRefs[2].highlightRect, "<b><size=100><color=#ffa500ff>MORTAR!</color></size></b>. Switch to <b><size=100><color=#ffa500ff>Phillips</color></size></b> and take advantage of his scope!");
                    }
                    return;
                }
                break;
            case ToolTipType.CoverFire:
                ShowToolTipForThiRect (shootGroup.noDragGroup.C_Rect, "Overwhelmed? Ask for <b><size=100><color=#ffa500ff>Cover-Fire</color></size></b>!");
                break;
            case ToolTipType.HealthKit:
                ShowToolTipForThiRect (shootGroup.noDragGroup.B_Rect, "Use <b><size=100><color=#ffa500ff>Health Kits</color></size></b> when you are low on health points!");
                break;
        }
        UserSettings.SetSingleToolTipStatus (ttType, false);
    }
	bool CanPlayTT()
	{

		//Debug.Log ("A");
		if (PlayerInputController.instance != null) {
			if (PlayerInputController.instance.current_player.GetStationController ().IsAutoMoving())
				return false;
		}
		else if (SneakyPlayerManager.instance !=null)
		{
			if (SneakyPlayerManager.instance.GetCurrentPlayer ().IsPlayerAutoMoving ()) {
				//Debug.Log ("debug");
				return false;
			}
		}

		bool result = !(queuedTT.Count <= 0 || waitingForTap);
		if (SlowMotionBullet.instance != null)
			if (SlowMotionBullet.instance.IsSlowMotionOn ())
				result = false;
		//Debug.Log (queuedTT.Count<=0);
		//Debug.Log (waitingForTap);
		return  result;
	}

	#endregion
	public List<ToolTipType> queuedTT = new List<ToolTipType>();
	public RectTransform TTPanel;
	public Text TTMessage;
	public bool waitingForTap = false;
	public Action tipEndingTapAction;
	public void ShowToolTipForThiRect(RectTransform selectedUIItem, string message, bool localElement = false)
	{
		//if(ShakeCamera.instance!=null)
		//ShakeCamera.instance.StopCameraShake();

		currentlySelectedUIItem = selectedUIItem;
		if (localElement) currentlySelectedUIItem.gameObject.SetActive(true);
		Transform selectedUIParent = null;
		if (currentlySelectedUIItem != null)
		{
			selectedUIParent = currentlySelectedUIItem.parent;
			currentlySelectedUIItem.SetParent(TTPanel);
		}
		TTPanel.gameObject.SetActive(true);
		TTMessage.text = message;
		float prevTimeScale = Time.timeScale;
		Time.timeScale = 0.01f;
		ToolTipAllowResumeTime = Time.time + minimumTTScreenDisableTime*Time.timeScale;
		waitingForTap = true;

		tipEndingTapAction = () => {
			if (localElement) currentlySelectedUIItem.gameObject.SetActive(false);
			if (currentlySelectedUIItem != null)
			{
				currentlySelectedUIItem.SetParent(selectedUIParent);
				currentlySelectedUIItem.transform.localScale = SavedSize;
			}
			TTPanel.gameObject.SetActive(false);
			Time.timeScale = prevTimeScale;
			waitingForTap = false;
			StartCoroutine(nextTT());
		};
	}
	IEnumerator nextTT()
	{
		yield return null;
		if (CanPlayTT())
		{
			DisplayTTNow(queuedTT[0]);
			queuedTT.RemoveAt(0);
		}
	}
	private bool waitingInitialized;
	private Vector3 SavedSize;
	private float SizeMultiplier;
	private RectTransform currentlySelectedUIItem;
	private float blinkRate;
	private float value;
	private float setValue;
	private float direction;
	private float ToolTipAllowResumeTime;
	private float minimumTTScreenDisableTime;
	private bool ToolTipUpdating()
	{
		if (CanPlayTT())
		{
			DisplayTTNow(queuedTT[0]);
			queuedTT.RemoveAt(0);
			return true;
		}
		//if (Time.time < ToolTipAllowResumeTime) { return true; }
		if (waitingForTap)
		{
			if ((Input.GetMouseButtonDown(0) || Input.touchCount>0) && Time.time > ToolTipAllowResumeTime)
			{
				tipEndingTapAction();
				tipEndingTapAction = null;
			}
			else
			{
				if (currentlySelectedUIItem == null) return true;
				if (!waitingInitialized)
				{
					waitingInitialized = true;
					SavedSize = currentlySelectedUIItem.transform.localScale;
					value = 0;
					direction = 1;
					setValue = 0;
				}
				value += direction * 2.0f * blinkRate * (1f / Time.timeScale) * Time.deltaTime;
				if (value > 1 || value < 0)
					direction = direction * -1;
				setValue = Mathf.Clamp(value, 0, 1);
				currentlySelectedUIItem.transform.localScale = Vector3.Lerp(SavedSize, SavedSize * SizeMultiplier, setValue);
			}
			return true;
		}
		else
		{
			waitingInitialized = false;
			return false;
		}
	}
	#endregion

	#if !TEMP_SKIP
    #region tooltip
    //===================================================================================
    IEnumerator ToolTipStartingSequence()
    {
        yield return new WaitForSeconds(2);
        if (UserSettings.TutorialOn && GameManagerScript.instance.aimTTpending)
        {
            GameManagerScript.instance.aimTTpending = false;
            ShowAimTT();
        }
        //yield return new WaitForSeconds(2);
        if (UserSettings.TutorialOn && GameManagerScript.instance.fireTTpending)
        {
            GameManagerScript.instance.fireTTpending = false;
            ShowFireTT();
        }
            
    }

    #region toolTipCallings


    public void ShowGrenadeUsableTT()
    {
        if (waitingForTap || SlowMotionBullet.instance.IsSlowMotionOn()) queuedTT.Add(ShowGrenadeUsableTT);
        else ShowToolTipForThiRect(nadeButtonRect, "Touch this button to <b><size=100><color=#ffa500ff>Throw Grenade</color></size></b>");
    }

    public void ShowBackUpUsableTT()
    {
        if (waitingForTap || SlowMotionBullet.instance.IsSlowMotionOn()) queuedTT.Add(ShowBackUpUsableTT);
        else ShowToolTipForThiRect( backupButtonRect, "Overwhelmed by the enemy? You can call for <b><size=100><color=#ffa500ff>Covering Fire</color></size></b> using this button.");
    }



    public void GrenadierIntroTT()
    {
        if (waitingForTap || SlowMotionBullet.instance.IsSlowMotionOn()) queuedTT.Add(GrenadierIntroTT);
        else ShowToolTipForThiRect(GrenedierFlashingIconSampleRect, "Watch out for <b><size=100><color=#ffa500ff>Grenade Alerts</color></size></b>", true);
    }
    public void ShowPlayer2TT()
    {
        if (waitingForTap || SlowMotionBullet.instance.IsSlowMotionOn()) queuedTT.Add(ShowPlayer2TT);
        else ShowToolTipForThiRect(selectablePlayersGroup.perPlayerRefs[1].highlightRect, "Your health is critically low!  Switch to <b><size=100><color=#ffa500ff>Shamsu!</color></size></b> Switching will allow to <b><size=100><color=#ffa500ff>Recover</color></size></b> from injury.");
    }
    public void ShowPlayer3TT()
    {
        if (waitingForTap || SlowMotionBullet.instance.IsSlowMotionOn()) queuedTT.Add(ShowPlayer3TT);
        else ShowToolTipForThiRect(selectablePlayersGroup.perPlayerRefs[2].highlightRect, "<b><size=100><color=#ffa500ff>MORTAR!</color></size></b>. Use <b><size=100><color=#ffa500ff>Taposh</color></size></b> to get a longer range advantage.");
    }
    #endregion


    public List<Action> queuedTT = new List<Action>();
    public RectTransform TTPanel;
    public Text TTMessage;
    public bool waitingForTap = false;
    public Action tipEndingTapAction;
    public void ShowToolTipForThiRect(RectTransform selectedUIItem, string message, bool localElement = false)
    {
        ShakeCamera.instance.StopCameraShake();
        currentlySelectedUIItem = selectedUIItem;
        if (localElement) currentlySelectedUIItem.gameObject.SetActive(true);
        Transform selectedUIParent = null;
        if (currentlySelectedUIItem != null)
        {
            selectedUIParent = currentlySelectedUIItem.parent;
            currentlySelectedUIItem.SetParent(TTPanel);
        }
        TTPanel.gameObject.SetActive(true);
        TTMessage.text = message;
        float prevTimeScale = Time.timeScale;
        Time.timeScale = 0.01f;
        ToolTipAllowResumeTime = Time.time + minimumTTScreenDisableTime*Time.timeScale;
        waitingForTap = true;

        tipEndingTapAction = () => {
            if (localElement) currentlySelectedUIItem.gameObject.SetActive(false);
            if (currentlySelectedUIItem != null)
            {
                currentlySelectedUIItem.SetParent(selectedUIParent);
                currentlySelectedUIItem.transform.localScale = SavedSize;
            }
            TTPanel.gameObject.SetActive(false);
            Time.timeScale = prevTimeScale;
            waitingForTap = false;
            StartCoroutine(nextTT());
        };
    }
    IEnumerator nextTT()
    {
        yield return null;
        if (queuedTT.Count > 0)
        {
            queuedTT[0]();
            queuedTT.RemoveAt(0);
        }
    }
    private bool waitingInitialized;
    private Vector3 SavedSize;
    private float SizeMultiplier;
    private RectTransform currentlySelectedUIItem;
    private float blinkRate;
    private float value;
    private float setValue;
    private float direction;
    private float ToolTipAllowResumeTime;
    private float minimumTTScreenDisableTime;
    private bool ToolTipUpdating()
    {
        if (!waitingForTap && queuedTT.Count > 0)
        {
            queuedTT[0]();
            queuedTT.RemoveAt(0);
            return true;
        }
        //if (Time.time < ToolTipAllowResumeTime) { return true; }
        if (waitingForTap)
        {
            if ((Input.GetMouseButtonDown(0) || Input.touchCount>0) && Time.time > ToolTipAllowResumeTime)
            {
                tipEndingTapAction();
                tipEndingTapAction = null;
            }
            else
            {
                if (currentlySelectedUIItem == null) return true;
                if (!waitingInitialized)
                {
                    waitingInitialized = true;
                    SavedSize = currentlySelectedUIItem.transform.localScale;
                    value = 0;
                    direction = 1;
                    setValue = 0;
                }
                value += direction * 2.0f * blinkRate * (1f / Time.timeScale) * Time.deltaTime;
                if (value > 1 || value < 0)
                    direction = direction * -1;
                setValue = Mathf.Clamp(value, 0, 1);
                currentlySelectedUIItem.transform.localScale = Vector3.Lerp(SavedSize, SavedSize * SizeMultiplier, setValue);
            }
            return true;
        }
        else
        {
            waitingInitialized = false;
            return false;
        }
    }
    #endregion
	#endif
	#region editor grouping classes
	[System.Serializable]
	public class ShootGroupClass
	{
		public RectTransform rect;
		public IndicatorGroupClass enemyIndicatorGroup;
		public NoDragGroupClass noDragGroup;
		public PositionSwapGroupClass positionSwapGroup;
		public C4GroupClass c4Group;
        public RectTransform crossHair;

		//public Text ammouCountText;
	}

	[System.Serializable]
	public class IndicatorGroupClass
	{
		public RectTransform rect;
		public GameObject leftIndicator;
		public GameObject rightIndicator;
	}
	[System.Serializable]
	public class PositionSwapGroupClass
	{
		public RectTransform rect;
		public Button leftMoveButton;
		public Button rightMoveButton;
	}
	[System.Serializable]
	public class C4GroupClass
	{
		public RectTransform rect;
		public Image progressImage;
		public Animator beepAnimator;
	}


	[System.Serializable]
	public class NoDragGroupClass
	{
        
		public RectTransform rect;
		public RectTransform primaryFireRect;
        public Image primaryFire_ProgressDial;



        public RectTransform A_RectReload;
        public Text A_CountText;
        public Image A_ProgressDialReload;
        public float A_ProgressMax = 1;
        public float A_ProgressMin = 0;


        public RectTransform B_Rect;
        public Button B_Button;
        public Animator B_Animator;
        public Text B_CountText;
        public Image B_ButtonImage;



        public RectTransform C_Rect;
        public Button C_Button;
        public Animator C_Animator;
        public Text C_CountText;
        public Image C_ProgressDial;
        public float C_ProgressMax = 1;
        public float C_ProgressMin = 0;


        public RectTransform D_RectHideUnhide;
        public Image D_ButtonImage_HideUnhide;
		//public Image backUpProgressDial;
	}

	[System.Serializable]
	public class RunGroupClass
	{
		public RectTransform rect;
		public GameObject knifeSpecific;
		public Animator stabAnimator;
		public RectTransform walkButtonRect;
		public RectTransform stabButtonRect;
		public RectTransform noiseLevelRect;
		public Image noiseLevelDisplay;
		private bool fd;
	}

	[System.Serializable]
	public class CurrentPlayerGroupClass
	{
		public RectTransform rect;
		public Image portrait;
		public Image HPBar;
	}
	[System.Serializable]
	public class SelectablePlayersGroupClass
	{
		public RectTransform rect;
		public PerPlayerHUDRefs[] perPlayerRefs;
	}
	[System.Serializable]
	public class SupportingPlayersGroupClass
	{
		public RectTransform rect;
		public PerPlayerHUDRefs[] perPlayerRefs;
	}
	[System.Serializable]
	public class SpriteRefClass
	{
		public Sprite hide;
		public Sprite unhide;
		public Sprite nura;
		public Sprite korim;
		public Sprite jamal;
		public Sprite kopila;
		public Sprite bodi;

	}
	[System.Serializable]
	public class PerPlayerHUDRefs
	{
		public RectTransform highlightRect;
		internal Button switchToButton;
		internal Image slicedHPImage;
		internal Image portraitImage;
		internal Animator imageAnimator;
	}
	#endregion
}
#region hudSettings
[System.Serializable]
public class HUDSettings
{
	public HUDBaseType baseType = HUDBaseType.SHOOT;
	//public HUDShootModuleSettings shootSettings;
	//public HUDRunModuleSettings runSettings;
	public List<FighterRole> availablePlayerList;
	public List<FighterRole> supportingPlayerList;

	public void Apply()
	{
		HUDManager.UpdateHUDSettings (this);
	}
	//public bool positionSwitchinAllowed{ get{ return (baseType == HUDBaseType.SHOOT) && shootSettings.positionSwitching; }}
}
//[System.Serializable]
//public class HUDShootModuleSettings
//{
//	public bool positionSwitching;
//
//}
//[System.Serializable]
//public class HUDRunModuleSettings
//{
//	public bool knifeMode;
//}
public enum HUDBaseType { SHOOT, RUN , KNIFE }
#endregion
public enum ToolTipType
{
	Aim=0,
	Shoot=1,
	Hide=2,
	Reload=3,
	Grenadier=4,
	PositionSwap=5,
	Walk=6,
	Noise=7,
	Vision=8,
	Stab=9,
	Player2=10,
	Player3=11,
	CoverFire=12,
	HealthKit=13
}