using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Facebook.Unity;

public class InEndGameMenuManager : MonoBehaviour
{
	public GameObject PauseMenu;
	//public Button sensiButton_DisabledForPauseMenu;

	public Image BackImage;
	public GameObject EndMenu;
	public Text mainText;

	//public Text headerText;
	public GameObject joyBanglaText;
	public Text scoreTitleText;
	public Text scoreValueText;
	public Text bestTitleText;
	public Text bestValueText;
	public Text totalScoreText;
	public Text killsText;
	public GameObject KillsITEM;
	public Text HSText;
	public GameObject HSTEXTITEM;


	public GameObject winElements;
	public GameObject loseElements;

    public GameObject loseKata;


	public static InEndGameMenuManager instance;

	void Awake()
	{
		instance = this;
		backScreenBlackeningOn = false;
		Color tempCol = BackImage.color;
		tempCol.a = 0f;
		BackImage.color = tempCol;
	}
	void Update()
	{
		if (backScreenBlackeningOn) {
			Color tempCol = BackImage.color;
			tempCol.a += (0.5f*Time.deltaTime)/Time.timeScale;
			BackImage.color = tempCol;
		}
	}
	bool backScreenBlackeningOn = false;
	public void InitEndGameMenu(bool won)
	{
		BackImage.gameObject.SetActive(true);
		backScreenBlackeningOn = true;
		Time.timeScale = 0.01f;
		EndMenu.SetActive (true);
		if (!won) {
			loseElements.SetActive (true);
			if (HUDManager.hudSettings.baseType == HUDBaseType.KNIFE) {
				switch (SneakyPlayerManager.instance.nextTargetPersonel.rajakarAlertReason) {
				case AIPersonnel.AlertReason.HEARD:
					mainText.text = "The enemy heard your footsteps!";
					break;
				case AIPersonnel.AlertReason.TOUCHED:
					mainText.text = "You went too close to the enemy!";
					break;
				case AIPersonnel.AlertReason.SEEN:
					mainText.text = "The enemy saw you!";
					break;
				}
				scoreTitleText.text = "Phase Score:";
				scoreValueText.text = GeneralManager.instance.score.ToString ();
				bestTitleText.text = "Phase Best Score:";
				bestValueText.text = UserGameData.instance.GetScore (GeneralManager.instance.level, GeneralManager.instance.phase).ToString();
				totalScoreText.text = UserGameData.instance.GetTotalScore ().ToString();
				killsText.text = GeneralManager.instance.killCount.ToString ();
				HSTEXTITEM.SetActive (false);
			} else {

                if (GeneralManager.instance.level == 3 && GeneralManager.instance.phase == 1)
                {
                    loseKata.SetActive(false);
                    mainText.text = "Heroes Never Die!";
                    scoreTitleText.text = "Endurance Score:";
                    scoreValueText.text = GeneralManager.instance.score.ToString ();
                    bestTitleText.text = "Best Score:";
                    bestValueText.text = UserGameData.instance.GetScore (GeneralManager.instance.level, GeneralManager.instance.phase).ToString();
                    totalScoreText.text = UserGameData.instance.GetTotalScore ().ToString();
                    killsText.text = GeneralManager.instance.killCount.ToString ();
                    HSText.text = GeneralManager.instance.headShotCount.ToString ();
                }
                else
                {
                    loseKata.SetActive(true);
                    mainText.text = "Mission Failed!";
                    scoreTitleText.text = "Area Score:";
                    scoreValueText.text = GeneralManager.instance.score.ToString ();
                    bestTitleText.text = "Area Best Score:";
                    bestValueText.text = UserGameData.instance.GetScore (GeneralManager.instance.level, GeneralManager.instance.phase).ToString();
                    totalScoreText.text = UserGameData.instance.GetTotalScore ().ToString();
                    killsText.text = GeneralManager.instance.killCount.ToString ();
                    HSText.text = GeneralManager.instance.headShotCount.ToString ();
                }

			}
		} else {
			winElements.SetActive (true);
            if (GeneralManager.instance.level == 2 && GeneralManager.instance.phase == 3 ) {
				joyBanglaText.SetActive (true);
				mainText.text = "";
			} else {
				joyBanglaText.SetActive (false);
				mainText.text = "Mission Completed!";
			}
			//mainText.text = "Mission Completed!";
			scoreTitleText.text = "Level Score:";
			scoreValueText.text = GeneralManager.savedLevelScore.ToString();
			bestTitleText.text = "Level Best Score:";
			if (GeneralManager.instance.level == 1) {
				bestValueText.text = UserGameData.instance.GetLevel1Score().ToString();
			} else {
				bestValueText.text = UserGameData.instance.GetLevel2Score().ToString();
			}
			totalScoreText.text = UserGameData.instance.GetTotalScore ().ToString();

			KillsITEM.SetActive (false);
			HSTEXTITEM.SetActive (false);
		}

		InGameSoundManagerScript.KillAllPossibleSounds();
		if (UserSettings.SoundOn)
		{
			ClipInfo ci;
			if (won)
			{
				ci = BaseAudioKeeper.GetClipInfoWithID (ClipID.victoryClip);

			}
			else
			{
                if (GeneralManager.instance.level == 3 && GeneralManager.instance.phase == 1)
                {
                    ci = BaseAudioKeeper.GetClipInfoWithID(ClipID.victoryClip);
                }
                else
                {
                    ci = BaseAudioKeeper.GetClipInfoWithID(ClipID.defeatClip);
                }
			}
			InGameSoundManagerScript.instance.selfAudioSource.clip = ci.clip;
			InGameSoundManagerScript.instance.selfAudioSource.volume = ci.volume;
			InGameSoundManagerScript.instance.selfAudioSource.Play();
		}			

	}
	private float savedTimeScale = 0;
	public void DisplayPauseMenu()
	{
		savedTimeScale = Time.timeScale;
		Time.timeScale = 0;
		PauseMenu.SetActive(true);

		//ShakeCamera.instance.StopCameraShake();

		//sensiButton_DisabledForPauseMenu.interactable = (HUDManager.hudSettings.baseType != HUDBaseType.KNIFE);

		InGameSoundManagerScript.instance.SetPausedState(true);
		SetUpSoundButtonState();
	}
	// buttons
	public void Resume()
	{
		Time.timeScale = savedTimeScale;
		PauseMenu.SetActive(false);
		InGameSoundManagerScript.instance.SetPausedState(false);
	}
	public void Restart()
	{
        AnalyticsManager.instance.TrackLevelRestartEvent(GeneralManager.instance.level, GeneralManager.instance.phase);

		LevelManager.ReloadLevel ();
	}
	public void MainMenu()
	{
		SceneLoader.LoadScene(GameConstants.mainMenu);
	}

	public void EndGameContinue()
	{
		WeaponLoader.ClearWeapon ( true);
	}
	public void FacebookShareOnButton()
	{
		#if !NOTFINAL
		if (FB.IsLoggedIn)
		{
		JustShare();
		}
		else
		{
		FBManager.instance.OnLoggedInDone += FBloginCallback;
		FBManager.instance.LoginFacebook();
		}
		#else
		Debug.Log("not implemented");
		#endif

	}
	private void FBloginCallback()
	{

		#if !NOTFINAL
		JustShare();
		FBManager.instance.OnLoggedInDone -= FBloginCallback;
		#else
		Debug.Log("not implemented");
		#endif

	}
	private void JustShare()
	{			
		#if !NOTFINAL
		if (GeneralManager.instance.gameWon)
		{
			if(GeneralManager.instance.level==1)
			{
				FBManager.instance.FeedPost("I have successfully rescued Mr. President with score " + GeneralManager.savedLevelScore +
					" And a grand score of "+ UserGameData.instance.GetTotalScore() +"! Do you think you can do it too??");
			}
			else
			{
				FBManager.instance.FeedPost("I have successfully blown the warehouse up with score " + GeneralManager.savedLevelScore +
					" And a grand score of "+ UserGameData.instance.GetTotalScore() +"! Do you think you can do it too??");
			}
		}
		else
		{
			FBManager.instance.FeedPost("I have scored a grand total of "+ UserGameData.instance.GetTotalScore() +"! Are you brave enough to take the challenge?");
		}
		#else
		Debug.Log("not implemented");
		#endif		

	}


	public Text soundStateText;
	public Image soundStateLogoImage;
	public Sprite soundOnLogoSprite;
	public Sprite soundOffLogoSprite;
	public Image soundStateBGImage;
	public Sprite soundOnBGSprite;
	public Sprite soundOffBGSprite;
	public void SoundStateToggleButton()
	{
		bool tempBool = UserSettings.SoundOn;
		tempBool = !tempBool;
		UserSettings.SoundOn = tempBool;
		SetUpSoundButtonState();

	}
	void SetUpSoundButtonState()
	{
		if (UserSettings.SoundOn)
		{
			soundStateLogoImage.sprite = soundOnLogoSprite;
			soundStateBGImage.sprite = soundOnBGSprite;
			soundStateText.text = "On";
		}
		else
		{
			InGameSoundManagerScript.KillAllPossibleSounds();
			soundStateLogoImage.sprite = soundOffLogoSprite;
			soundStateBGImage.sprite = soundOffBGSprite;
			soundStateText.text = "Off";
		}
	}

	public Slider sensitivitySlider;
	public void SensiOnSliderShow()
	{
		sensitivitySlider.maxValue = GameConstants.maxSensivity;
		sensitivitySlider.minValue = GameConstants.minSensivity;
		sensitivitySlider.value = UserSettings.Sensivity;
		sensitivitySlider.onValueChanged.AddListener((float value)=> { SensiSliderValue(value); });
	}
	public void SensiOnSliderHide()
	{
		sensitivitySlider.onValueChanged.RemoveAllListeners();
	}
	public void SensiSliderValue(float value)
	{
		UserSettings.Sensivity = value;
	}
}
