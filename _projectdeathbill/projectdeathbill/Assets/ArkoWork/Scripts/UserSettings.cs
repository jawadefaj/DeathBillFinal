using UnityEngine;
using System.Collections;

public class UserSettings {

	private static bool isInitialized = false;
	private static bool isSoundOn = false;
	private static bool isTutorialOn = false;
	private static float _sensivity = 1f;
	private static int _hubChoice = 0;
    private static int _highScore = 0;
	private static int[] _scores = new int[]{0,0,0,0,0,0,0};
	private static char[] _toolTipOptions = new char[14];
//	private static char[] _toolTipOptions
//	{
//		set
//		{
//			_toolTipOption_int = value;
//			Debug.Log ("was set with length: " + _toolTipOption_int.Length);
//		}
//		get
//		{ 
//			return _toolTipOption_int;
//		}
//
//	}

    private static bool _isGPGDataSaverOk = false;
	private static string _toopTipOpt_On = "11111111111111";
	private static string _toopTipOpt_Off = "00000000000000";
	private static string tag = "tag";


	private static Language _selectedLanguage = Language.Bangla; 
	private static string _keyForSelectedLanguage = "SelectedLanguage";
	//initialization
	private static void Initialize()
	{

		//sound
		int a = PlayerPrefs.GetInt("SoundOn",1);
		if(a==1) isSoundOn = true;
		else isSoundOn = false;

		//tutorial
		a = PlayerPrefs.GetInt("TutorialOn",1);
		if(a==1) isTutorialOn = true;
		else isTutorialOn = false;

		//sensivity
		_sensivity = PlayerPrefs.GetFloat("Sensivity",GameConstants.defaultSensivity);

		//hub choice
		_hubChoice = PlayerPrefs.GetInt("HubChoice",0);

        //high score
        _highScore = PlayerPrefs.GetInt("HighScore", 0);

		//scores
		for(int i=0;i<_scores.Length;i++)
		{
			//Debug.Log(SecurePlayerPrefs.GetString((tag+i.ToString()),"0"));
			_scores[i] = int.Parse(SecurePlayerPrefs.GetString((tag+i.ToString()),"0"));
		}

		//tool tip options
		_toolTipOptions = PlayerPrefs.GetString("ToolTips",_toopTipOpt_On).ToCharArray();
		

		_selectedLanguage = (Language) PlayerPrefs.GetInt (_keyForSelectedLanguage,0);

        //data saver settings
        a = PlayerPrefs.GetInt("GPG",0);
        if(a==1) _isGPGDataSaverOk = true;
        else _isGPGDataSaverOk = false;

        isInitialized = true;
	}
	                             
	//sound settings
	public static bool SoundOn
	{
		get
		{
			if(!isInitialized) Initialize ();
			return isSoundOn;
		}
		set
		{
			isSoundOn = value;
			if(value == true)
				PlayerPrefs.SetInt("SoundOn",1);
			else
				PlayerPrefs.SetInt("SoundOn",0);

			PlayerPrefs.Save();
		}
			
	}

    //GPG Data saverr settings
    public static bool IsGPGDataSaverOK
    {
        get
        {
            if (!isInitialized)
                Initialize();
            return _isGPGDataSaverOk;
        }
        set
        {
            _isGPGDataSaverOk = value;
            if(value == true)
                PlayerPrefs.SetInt("GPG",1);
            else
                PlayerPrefs.SetInt("GPG",0);

            PlayerPrefs.Save();
        }
    }

	//show tutorial settings
	public static bool TutorialOn
	{
		get
		{
			if(!isInitialized) Initialize ();
			return isTutorialOn;
		}
		set
		{

			isTutorialOn = value;
			if (value == true) {
				PlayerPrefs.SetInt ("TutorialOn", 1);
				//turn on all options
				_toolTipOptions = _toopTipOpt_On.ToCharArray();
				PlayerPrefs.SetString ("ToolTips", _toopTipOpt_On);
			} 
			else {
				PlayerPrefs.SetInt ("TutorialOn", 0);
				//turn of all options
				_toolTipOptions = _toopTipOpt_Off.ToCharArray();
				PlayerPrefs.SetString ("ToolTips", _toopTipOpt_Off);
			}

			PlayerPrefs.Save();
		}
	}

	//Sensivity Settings
	public static float Sensivity
	{
		get
		{
			if(!isInitialized) Initialize ();
			//Debug.Log ("Sensivity value returned "+ _sensivity);
			return _sensivity;
		}
		set
		{
			value = Mathf.Clamp(value,GameConstants.minSensivity,GameConstants.maxSensivity);
			_sensivity = value;
			PlayerPrefs.SetFloat ("Sensivity",value);
			//Debug.Log ("Sensivity value set "+ _sensivity);

			PlayerPrefs.Save();
		}
	}

	//Hub Choice Settings
	public static int HubChoice
	{
		get
		{
			if(!isInitialized) Initialize ();
			return _hubChoice;
		}
		set
		{
			_hubChoice = value;
			PlayerPrefs.SetInt("HubChoice",value);

			PlayerPrefs.Save();
		}
	}

    //High Score
    public static int HighScore
    {
        get
        {
            if (!isInitialized) Initialize();
            return _highScore;
        }
        set
        {
            if (!isInitialized) Initialize();
            if (_highScore < value)
            {
                _highScore = value;
                PlayerPrefs.SetInt("HighScore", value);
            }

			PlayerPrefs.Save();
        }
    }

	//get all mini level score
	[System.Obsolete("Use UserGameData.GetScore() Instead",false)]
	public static int GetScore(int Level , int Phase)
	{
		int lvl = (Level-1)*4 + (Phase-1);
		if(!isInitialized) Initialize();

		return _scores[lvl];
	}

	[System.Obsolete("Use UserGameData.SetScore() Instead",false)]
	public static void SetScore(int Level , int Phase, int score)
	{
		int lvl = (Level-1)*4 + (Phase-1);
		if(!isInitialized) Initialize();

		//check for high
		if(_scores[lvl]<score)
		{
			_scores[lvl] = score;
			SecurePlayerPrefs.SetString((tag+lvl.ToString()),score.ToString());
		}

		PlayerPrefs.Save();
	}

	[System.Obsolete("Use UserGameData.GetLevel1Score() Instead",false)]
	public static int GetLevel1Score()
	{
		if(!isInitialized) Initialize();

		int p =0;
		for(int i=0;i<=3;i++)
			p+= _scores[i];

		return p;
	}

	[System.Obsolete("Use UserGameData.GetLevel2Score() Instead",false)]
	public static int GetLevel2Score()
	{
		if(!isInitialized) Initialize();

		int p =0;
		for(int i=4;i<=6;i++)
			p+= _scores[i];

		return p;
	}

	[System.Obsolete("Use UserGameData.GetTotalScore() Instead",false)]
	public static int GetTotalScore()
	{
		return GetLevel1Score () + GetLevel2Score ();
	}

	public static bool GetSingleToolTipStatus(ToolTipType toolTip)
	{
		if (!isInitialized)
			Initialize ();

		int i = (int)toolTip;

//		Debug.Log (i);
//		Debug.Log (_toolTipOptions.Length);

		if (char.Equals(_toolTipOptions [i],'1'))
			return true;
		else
			return false;
	}

	public static void SetSingleToolTipStatus(ToolTipType toolTip, bool value)
	{
		if (!isInitialized)
			Initialize ();

		int i = (int)toolTip;
		char c = value == true ? '1' : '0';

		_toolTipOptions [i] = c;
		PlayerPrefs.SetString ("ToolTips",new string(_toolTipOptions));
			
		PlayerPrefs.Save();
	}

	public static Language SelectedLanguage
	{

		get
		{ 
			if(!isInitialized) Initialize ();
			return _selectedLanguage;
		}
		set
		{
			_selectedLanguage = value;
			PlayerPrefs.SetInt (_keyForSelectedLanguage, (int) value);
		}
	}
}

public enum Language
{
	Bangla =0,
	English =1
}