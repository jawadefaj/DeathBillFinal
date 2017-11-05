using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class MainMenuGUI : MonoBehaviour {

	#region VARIABLES

	//outside buttong
	public string fileName;
	public Button fbLikeit;
	public Button tweetLikeit;
	public Button quitBtn;
	public AudioSource bgMusic;
	//public Button hiddenRetriveBtn;
	public Button notificationButton;
	private UpdatedContentNotificationAnimator notiBtn_AnimControll;
	//public Button leaderBtn;
	//public Button supportUs;

	//main menu window items

	public Button mainmenu_btn_story;
	public Button mainmenu_btn_settings;
	public Button mainmenu_btn_credits;

	//settings window items
	public Button settings_btn_sound;
	public Button settings_btn_tutorial;
	public Button settings_btn_language;
	//public Button settings_btn_hubSelection;
	public Button settings_btn_back;
	public Slider settings_slider_sensivity;
	//public Image settings_sound_image;
	public Sprite OptionOnImage;
	public Sprite OptionOffImage;
	//public Sprite soundOnImage;
	//public Sprite soundOffImage;

	//language selection window item
	public Button language_englishBtn;
	public Button language_banglaBtn;

	//credit window items
	public Button credits_btn_back;
	public ScrollRect textSlider;
	public Image[] knobSlider;

	//retrive window items
	public Text retrive_emailViewer;
	public Text retrive_statusViewr;
	public Button retrive_btn;
	public Button retrive_backBtn;

	//hub selection window item
	//public Button hud_allignment0;
	//public Button hud_allignment1;
	//public Button hud_btn_back;

	//support us window
	//public Button support_watchAdd;
	//public Button support_donateUs;
	//public Button support_back;

	//window collection
	public GameObject windowMainMenu;
	public GameObject windowSettings;
	public GameObject windowHudSelection;
	public GameObject windowCredit;
	public GameObject windowSupport;
	public GameObject windowLanguageSelect;
	public GameObject windowRetrive;

	private GameObject currentWindow = null;
	private GameObject previousWindow = null;

	const string KEY_FIRST_TIME = "firstTime";

	#endregion

	#region MONOBEHAVIOUR METHONDS
	void Start () {

		#if UNITY_EDITOR
		if(UserGameData.instance == null) UserGameData.LoadGame("");
		#endif

		//subscribe for news notification
		notiBtn_AnimControll = notificationButton.gameObject.GetComponent<UpdatedContentNotificationAnimator>();
		notificationButton.image.enabled = false;
		notificationButton.transform.FindChild("Icon").gameObject.SetActive(false);
		GamePromotionManager.instance.OnAdNewsPending += () => {notiBtn_AnimControll.attractAttention = true;};
		GamePromotionManager.instance.OnPromotionManagerReady += () => {
			notificationButton.image.enabled = true;
			notificationButton.transform.FindChild("Icon").gameObject.SetActive(true);
		};
		//self check
		if(GamePromotionManager.instance.isReady)
		{
			notificationButton.image.enabled = true;
			notificationButton.transform.FindChild("Icon").gameObject.SetActive(true);
		}

		//hide hidden retrive button
		//hiddenRetriveBtn.gameObject.SetActive(false);

		//never turn off screen
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		//check if first time. then show movie
		if(PlayerPrefs.GetInt(KEY_FIRST_TIME,1)==1)
		{
            mainmenu_btn_storyF();
            PlayerPrefs.SetInt(KEY_FIRST_TIME,0);
            //Debug.Log("movie");
		}
		else
		{
            //Debug.Log("No movie");
		}
        
        SwitchToWindow(windowMainMenu);

		if(UserSettings.SoundOn)
		{
			bgMusic.loop = true;
			bgMusic.Play();
		}
			
        //Send scene entry hit
        AnalyticsManager.instance.TrackSceneEntryEvent("MainMenu");
	}

	void OnEnable()
	{
		fbLikeit.onClick.AddListener(fbLikeitF);
		tweetLikeit.onClick.AddListener(tweetLikeitF);
		quitBtn.onClick.AddListener(()=> {Application.Quit();});
		notificationButton.onClick.AddListener(notificationButtonF);
		//supportUs.onClick.AddListener(supportUsF);

		//main menu items
		mainmenu_btn_settings.onClick.AddListener(mainmenu_btn_settingsF);
		mainmenu_btn_credits.onClick.AddListener(mainmenu_btn_creditsF);
		mainmenu_btn_story.onClick.AddListener(mainmenu_btn_storyF);

		//settings window item
		settings_slider_sensivity.onValueChanged.AddListener(settings_slider_sensivityF);
		settings_btn_sound.onClick.AddListener(settings_btn_soundF);
		settings_btn_tutorial.onClick.AddListener(settings_btn_tutorialF);
		settings_btn_back.onClick.AddListener(BackFunction);
		settings_btn_language.onClick.AddListener(UpdateAndShowLanguageSelectionWindow);
		//hiddenRetriveBtn.onClick.AddListener(UpdateAndShowRetriveWindow);
		//settings_btn_hubSelection.onClick.AddListener(settings_btn_hubSelectionF);

		//retrive window element 
		retrive_btn.onClick.AddListener(retrive_btnF);
		retrive_backBtn.onClick.AddListener(BackFunction);

		//language selection window item
		language_banglaBtn.onClick.AddListener(language_banglaBtnF);
		language_englishBtn.onClick.AddListener(language_englishBtnF);

		//credit window element
		credits_btn_back.onClick.AddListener(BackFunction);
		textSlider.onValueChanged.AddListener(creditScrollValueChange);

		//support us window
		//support_watchAdd.onClick.AddListener(support_watchAddF);
		//support_donateUs.onClick.AddListener(support_donateUsF);
		//support_back.onClick.AddListener(BackFunction);

	}

	void OnDisable()
	{
		fbLikeit.onClick.RemoveAllListeners();
		tweetLikeit.onClick.RemoveAllListeners();
		quitBtn.onClick.RemoveAllListeners();
		notificationButton.onClick.RemoveAllListeners();
		//supportUs.onClick.RemoveAllListeners();

		//main menu item
		mainmenu_btn_settings.onClick.RemoveAllListeners();
		mainmenu_btn_credits.onClick.RemoveAllListeners();
		mainmenu_btn_story.onClick.RemoveAllListeners();

		//settings window item
		settings_slider_sensivity.onValueChanged.RemoveAllListeners();
		settings_btn_sound.onClick.RemoveAllListeners();
		settings_btn_tutorial.onClick.RemoveAllListeners();
		settings_btn_back.onClick.RemoveAllListeners();
		settings_btn_language.onClick.RemoveAllListeners();
		///hiddenRetriveBtn.onClick.RemoveAllListeners();
		//settings_btn_hubSelection.onClick.RemoveAllListeners();

		//retrive window element 
		retrive_btn.onClick.RemoveAllListeners();
		retrive_backBtn.onClick.RemoveAllListeners();

		//language selection window item
		language_banglaBtn.onClick.RemoveAllListeners();
		language_englishBtn.onClick.RemoveAllListeners();

		//credit window element
		credits_btn_back.onClick.RemoveAllListeners();
		textSlider.onValueChanged.RemoveAllListeners();

		//support us window
		//support_watchAdd.onClick.RemoveAllListeners();
		//support_donateUs.onClick.RemoveAllListeners();
		//support_back.onClick.RemoveAllListeners();
	}

	void Update()
	{
		#if UNITY_ANDROID
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					BackFunction();
				}
		#endif
	}
	#endregion

	#region GUI EVENT FUNCTIONS

	void notificationButtonF()
	{
		notiBtn_AnimControll.attractAttention = false;
		GamePromotionManager.instance.ShowAd();
	}
	void retrive_btnF()
	{
		App42StorageManager.instance.CheckForLevelUnlockPermission();
		StartCoroutine(LevelCheckStatusUpdate());
	}

	IEnumerator LevelCheckStatusUpdate()
	{
		retrive_btn.interactable = false;
		retrive_statusViewr.text = "Fetching Data ...";

		int i=0;
		float rate = 1f;
		float nextChange = -1;

		do
		{
			if(nextChange<0)
			{
				if(i==0)
					retrive_statusViewr.text = "Fetching Data ";
				else if (i==1)
					retrive_statusViewr.text = "Fetching Data .";
				else if (i==2)
					retrive_statusViewr.text = "Fetching Data ..";
				else if (i==3)
					retrive_statusViewr.text = "Fetching Data ...";
				else
					retrive_statusViewr.text = "Fetching Data ...";

				i++;
				if(i>3) i=0;

				nextChange = rate;
			}

			nextChange-=Time.deltaTime;

			yield return null;
		}while(App42StorageManager.instance.dataRetriveStatus == DataRetriveStatus.Working);

		retrive_statusViewr.text = App42StorageManager.instance.dataRetriveStatus.ToString();
		retrive_btn.interactable = true;
	}

	void language_banglaBtnF()
	{
		//update user settings to bangla
		UserSettings.SelectedLanguage = Language.Bangla;
		UpdateLanguageOptions();
		SwitchToWindow(windowMainMenu);
		PlayerPrefs.SetInt (KEY_FIRST_TIME, 0);
	}

	void language_englishBtnF()
	{
		//update user settigns to english
		UserSettings.SelectedLanguage = Language.English;
		UpdateLanguageOptions();
		SwitchToWindow(windowMainMenu);
		PlayerPrefs.SetInt(KEY_FIRST_TIME,0);
	}

	void mainmenu_btn_storyF()
	{
		//play video
		Handheld.PlayFullScreenMovie(fileName, Color.black, FullScreenMovieControlMode.CancelOnInput);
	}

	void support_donateUsF()
	{
		Application.OpenURL("http://www.firstfundbd.com/archives/projects/heroes-of-71");
	}

	void support_watchAddF()
	{
		//if(GoogleAnalyticsTracker.instance!=null)
		//	GoogleAnalyticsTracker.instance.ReportAddClickButtonEvent();

        StopCoroutine("ShowAdd");
        StartCoroutine("ShowAdd");
	}

    IEnumerator ShowAdd()
    {
		yield return null;
		/*
        while (!Advertisement.IsReady())
            yield return null;

        Advertisement.Show();*/
    }
	void supportUsF()
	{
		SwitchToWindow(windowSupport);
	}

	void fbLikeitF()
	{
		//Application.OpenURL("https://www.facebook.com/heroesof71/");
	}

	void tweetLikeitF()
	{
		//Application.OpenURL("https://twitter.com/heroes_of_71");
	}

	void mainmenu_btn_facebookF()
	{
		/*if(FB.IsLoggedIn== false)
			FBManager.instance.LoginFacebook();
		else
			FBManager.instance.FeedPost("Heroes of 71");*/
	}

	void creditScrollValueChange(Vector2 value)
	{
		//Debug.Log (value);
		if(value.y>0.67f)
		{
			knobSlider[0].color = new Color(1f,1f,1f,1f);
			knobSlider[1].color =  new Color(1f,1f,1f,0.27f);
			knobSlider[2].color =  new Color(1f,1f,1f,0.27f);
		}
		else if (value.y>0.33f)
		{
			knobSlider[0].color =  new Color(1f,1f,1f,0.27f);
			knobSlider[1].color =  new Color(1f,1f,1f,1f);
			knobSlider[2].color =  new Color(1f,1f,1f,0.27f);;
		}
		else
		{
			knobSlider[0].color =  new Color(1f,1f,1f,0.27f);
			knobSlider[1].color =  new Color(1f,1f,1f,0.27f);
			knobSlider[2].color =  new Color(1f,1f,1f,1f);
		}
	}
	void mainmenu_btn_creditsF()
	{
		SwitchToWindow(windowCredit);
	}

	/*void selectHud0F()
	{
		UserSettings.HubChoice =0;
		UpdateHudSelectionWindow();
	}

	void selectHud1F()
	{
		UserSettings.HubChoice =1;
		UpdateHudSelectionWindow();
	}*/
	               
	/*void settings_btn_hubSelectionF()
	{
		SwitchToWindow(windowHudSelection);
		UpdateHudSelectionWindow();
	}*/

	void settings_btn_tutorialF()
	{
		UserSettings.TutorialOn = !UserSettings.TutorialOn;

		if(UserSettings.TutorialOn)
		{
			settings_btn_tutorial.gameObject.GetComponentInChildren<Text>().text = "Tutorial On";
			settings_btn_tutorial.gameObject.GetComponent<Image>().sprite = OptionOnImage;
		}
		else
		{
			settings_btn_tutorial.gameObject.GetComponentInChildren<Text>().text = "Tutorial Off";
			settings_btn_tutorial.gameObject.GetComponent<Image>().sprite = OptionOffImage;
		}
	}

	void settings_btn_soundF()
	{
		UserSettings.SoundOn = !UserSettings.SoundOn;
		if(UserSettings.SoundOn)
		{
			settings_btn_sound.gameObject.GetComponentInChildren<Text>().text = "Sound On";
			settings_btn_sound.gameObject.GetComponent<Image>().sprite = OptionOnImage;
			//settings_sound_image.sprite = soundOnImage;

			//play the sound
			bgMusic.Play();
		}
		else
		{
			settings_btn_sound.gameObject.GetComponentInChildren<Text>().text = "Sound Off";
			settings_btn_sound.gameObject.GetComponent<Image>().sprite = OptionOffImage;
			//settings_sound_image.sprite = soundOffImage;

			//off sound
			bgMusic.Stop();
		}
	}

	void settings_slider_sensivityF(float value)
	{
		UserSettings.Sensivity = value;
	}

	void mainmenu_btn_settingsF()
	{
		SwitchToWindow (windowSettings);
		UpdateSettingsWindow();

		//switch on hidden retrive button over lederboard
		//hiddenRetriveBtn.gameObject.SetActive(true);
	}


	#endregion

	#region PRIVATE METHODS

	private void UpdateAndShowRetriveWindow()
	{
		retrive_emailViewer.text = string.Concat("Your ID: ",GetMailAddress());
		retrive_statusViewr.text = "If you have lost any game progress, contact us at: contact@portbliss.org with your id to retrieve your data.";

		SwitchToWindow(windowRetrive);

	}

	public string GetMailAddress()
	{
		#if UNITY_EDITOR
		return "portbliss";
		#elif UNITY_ANDROID
		AndroidJavaClass UnityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject UnityPlayerActivity = UnityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

		AndroidJavaClass emailGetterClass = new AndroidJavaClass("org.portbliss.getemail.EmailGetter"); 
		string email = emailGetterClass.CallStatic<string>("getEmail", UnityPlayerActivity);

		if(string.IsNullOrEmpty(email)) return "";
		else
		return email;

		#endif
	}


	private void UpdateAndShowLanguageSelectionWindow()
	{
		//hide current window
		SwitchToWindow(windowLanguageSelect);

		//update options
		UpdateLanguageOptions();
	}

	private void UpdateLanguageOptions()
	{
		//get it from user settings
		bool bangla = (UserSettings.SelectedLanguage == Language.Bangla);

		if(bangla)
		{
			language_banglaBtn.image.sprite = OptionOnImage;
			language_englishBtn.image.sprite = OptionOffImage;
		}
		else
		{
			language_banglaBtn.image.sprite = OptionOffImage;
			language_englishBtn.image.sprite = OptionOnImage;
		}
	}

	private void UpdateSettingsWindow()
	{
		float s = UserSettings.Sensivity;
		settings_slider_sensivity.minValue = GameConstants.minSensivity;
		settings_slider_sensivity.maxValue = GameConstants.maxSensivity;
		settings_slider_sensivity.value = s;

		if(UserSettings.SoundOn)
		{
			settings_btn_sound.gameObject.GetComponentInChildren<Text>().text = "Sound On";
			settings_btn_sound.gameObject.GetComponent<Image>().sprite = OptionOnImage;
			//settings_sound_image.sprite = soundOnImage;
		}
		else
		{
			settings_btn_sound.gameObject.GetComponentInChildren<Text>().text = "Sound Off";
			settings_btn_sound.gameObject.GetComponent<Image>().sprite = OptionOffImage;
			//settings_sound_image.sprite = soundOffImage;
		}

		if(UserSettings.TutorialOn)
		{
			settings_btn_tutorial.gameObject.GetComponentInChildren<Text>().text = "Tutorial On";
			settings_btn_tutorial.gameObject.GetComponent<Image>().sprite = OptionOnImage;
		}
		else
		{
			settings_btn_tutorial.gameObject.GetComponentInChildren<Text>().text = "Tutorial Off";
			settings_btn_tutorial.gameObject.GetComponent<Image>().sprite = OptionOffImage;
		}
	}

	private void BackFunction()
	{
		//SwitchToWindow(previousWindow);
		if(currentWindow == windowMainMenu)
		{
			//do you want to quit
		}
		else if (currentWindow == windowSettings)
		{
			SwitchToWindow(windowMainMenu);

			//hide hiddent game retrive button
			//hiddenRetriveBtn.gameObject.SetActive(false);
		}
		else if (currentWindow == windowHudSelection)
		{
			SwitchToWindow(windowSettings);
		}
		else if (currentWindow == windowCredit)
		{
			SwitchToWindow(windowMainMenu);
		}
		else if (currentWindow == windowRetrive)
		{
			SwitchToWindow(windowSettings);
		}
		else if (currentWindow == windowSupport)
		{
			SwitchToWindow(windowMainMenu);
		}
	}

	private void SwitchToWindow(GameObject window)
	{
		if(currentWindow!=null)
		{
			previousWindow = currentWindow;
			currentWindow.SetActive(false);
		}
		window.SetActive(true);
		currentWindow = window;
	}
	#endregion

}
