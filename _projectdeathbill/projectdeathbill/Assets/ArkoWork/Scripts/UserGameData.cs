using UnityEngine;
using System.Collections;
using SimpleJSON;

public enum LevelStatus
{
	NotReady =0,
	Ready =1,
	Bought =2,
}

public class UserGameData {

	//--- Only One Instance is real
	public static UserGameData instance;

	//--- Keys
	public const string KEY_ONE = "2";      //KEY_BOUGHT
	public const string KEY_TWO = "1";      //KEY_Ready
	public const string KEY_ZERO = "0";     //KEY_NOT_READY
	public const string KEY_GAMEDATA = "gameData";

	private string _gameData = "pp";

	//--- Constructor
	public UserGameData(string gameData)
	{
		_gameData = gameData;

		JObject jData = JSON.Parse(_gameData);

		//trying to add a new key could return a error for invalid json format
		try
		{
			jData.Add("k","v");
		}
		catch(System.Exception ex)
		{
			Debug.LogWarning(ex.Message);
			JSONClass jsClass = new JSONClass();
			jsClass.Add("k","v");
			_gameData = jsClass.ToString();
		}

		Debug.Log (_gameData);
		//Sample value
		//{"k":"v", "lvl1":"1", "lvl1a":"1", "lvl1a_s":"2070", "lvl1b":"1", "lvl1b_s":"2120", "lvl1c":"1", "lvl1c_s":"1770", "lvl1d":"1", "lvl1d_s":"1990", "lvl2":"1", "lvl2a":"1", "lvl2a_s":"1200", "lvl2b":"1", "lvl2b_s":"1825"}
	}

    #if UNITY_EDITOR
    public void ManipulateData(string data)
    {
       _gameData = data;
       SaveGame();
    }
    #endif

	//--- Set Game Data
	public void UnlockStage(int levelID, int chkID)
	{
		string key = GetLevelKey(levelID,chkID);

		UpdateValue(key,KEY_TWO);

		if(!(levelID ==0 && chkID ==0))
			SaveGame();
	}

	public void AccuireStage(int levelID)
	{
		string key = GetLevelKey (levelID);
        string key2 = GetLevelKey(levelID, 0);

		UpdateValue (key, KEY_ONE);
        UpdateValue(key2, KEY_TWO);

		SaveGame ();
	}

	public void SetScore(int level, int chkPoint, int score)
	{
		string key = GetScoreKey(GetLevelKey(level-1,chkPoint-1));

		int s = GetLevelScore(key);

		if(score>s)
		{
			UpdateValue(key,score.ToString());
			SaveGame();
		}
	}

	//---- Get Game Data
	public string GetGameData()
	{
		return _gameData;
	}

	public int GetTotalScore()
	{
        return GetLevel1Score()+GetLevel2Score()+GetLevel3Score();
	}

    public int GetLevel3Score()
    {
        return GetScore(3, 1);
    }

	public int GetLevel2Score()
	{
		return GetScore(2,1)+GetScore(2,2)+GetScore(2,3);
	}

	public int GetLevel1Score()
	{
		return GetScore(1,1)+GetScore(1,2)+GetScore(1,3)+GetScore(1,4);
	}

	public int GetScore(int level, int chkPoint)
	{
		string key = GetScoreKey(GetLevelKey(level-1,chkPoint-1));

		return GetLevelScore(key);
	}

	/*public bool IsLevelUnlocked(int levelID)
	{
        //FOR TESTING AL LEVELS ARE UNLOCKED
        return true;

		return IsCheckPointUnlocked(levelID,-1);
	}*/

	public bool IsCheckPointUnlocked(int levelID, int chkID)
	{
        //FOR TESTING AL LEVELS ARE UNLOCKED
       //return true;

		if(string.IsNullOrEmpty(_gameData))
		{
			if(levelID ==0 && chkID ==0) 
			{
				//UnlockStage(0,0);
				return true;
			}
			else return false;
		}
		else
		{
			//has some game data
			JObject jData = JSON.Parse(_gameData);
			string key = chkID>-1? GetLevelKey(levelID,chkID):GetLevelKey(levelID);

			if(levelID ==0 && chkID ==0) 
			{
				//UnlockStage(0,0);
				return true;
			}
			else
				return GetLevelStatus(key);
		}
	}

	//--- Game Loading and Saving
	public static UserGameData LoadGame(string cloudData)
	{
		UserGameData fromCloud = new UserGameData(cloudData);
		UserGameData fromPrefs ;

		//for supporting any old game version
		if(SecurePlayerPrefs.HasKey("tag0"))
		{
			//accuire the old data and delete it for good
			JSONClass oldData = new JSONClass();

			//accuire level load
			oldData.Add(GetLevelKey(0),"1");
			oldData.Add(GetLevelKey(0,1),"1");
			oldData.Add(GetLevelKey(0,2), SecurePlayerPrefs.GetString("lvl1b","0"));
			oldData.Add(GetLevelKey(0,3), SecurePlayerPrefs.GetString("lvl1c","0"));
			oldData.Add(GetLevelKey(0,4), SecurePlayerPrefs.GetString("lvl1d","0"));

			oldData.Add(GetLevelKey(1), SecurePlayerPrefs.GetString("lvl2","0"));
			oldData.Add(GetLevelKey(1,0), SecurePlayerPrefs.GetString("lvl2a","0"));
			oldData.Add(GetLevelKey(1,1), SecurePlayerPrefs.GetString("lvl2b","0"));
			oldData.Add(GetLevelKey(1,2), SecurePlayerPrefs.GetString("lvl2c","0"));

			//accuire scores
			oldData.Add(GetScoreKey(GetLevelKey(0,0)),UserSettings.GetScore(1,1));
			oldData.Add(GetScoreKey(GetLevelKey(0,1)),UserSettings.GetScore(1,2));
			oldData.Add(GetScoreKey(GetLevelKey(0,2)),UserSettings.GetScore(1,3));
			oldData.Add(GetScoreKey(GetLevelKey(0,3)),UserSettings.GetScore(1,4));

			oldData.Add(GetScoreKey(GetLevelKey(1,0)),UserSettings.GetScore(2,1));
			oldData.Add(GetScoreKey(GetLevelKey(1,1)),UserSettings.GetScore(2,2));
			oldData.Add(GetScoreKey(GetLevelKey(1,2)),UserSettings.GetScore(2,3));

			//save new format
			SecurePlayerPrefs.SetString(KEY_GAMEDATA,oldData.ToString());

			//delete old one
			SecurePlayerPrefs.DeleteKey("tag0");

			fromPrefs = new UserGameData(oldData.ToString());
		}
		else
		{
			fromPrefs = new UserGameData(SecurePlayerPrefs.GetString(KEY_GAMEDATA));
		}

		//decide which to load
		if(fromCloud.IsHigherThan(fromPrefs))
		{
			Debug.Log("choosing cloud data");
			Debug.Log(fromCloud.GetGameData());
			instance = fromCloud;
			return fromCloud;
		}
		else
		{
			Debug.Log("choosing player pref data");
			Debug.Log(fromPrefs.GetGameData());
			instance = fromPrefs;
			return fromPrefs;
		}
	}

	public void SaveGame()
	{
		//to cloud
		GPGDataSaveManager.SaveData(_gameData);
		//to player prefs
		SecurePlayerPrefs.SetString(KEY_GAMEDATA,_gameData);
	}

	//--- Conflict Resolver
	public bool IsHigherThan(UserGameData otherGame)
	{
		int thisGameRank = GetRank();
		int otherGameRank = otherGame.GetRank();

		if(thisGameRank>otherGameRank)
		{
			return true;
		}
		else if (thisGameRank<otherGameRank)
		{
			return false;
		}
		else
		{
			if(GetTotalScore() > otherGame.GetTotalScore())
				return true;
			else
				return false;
		}
	}

	//--- Private Methods

	private int GetRank()
	{
		int rank =0;

		if(IsCheckPointUnlocked(0,0)) rank++;
		if(IsCheckPointUnlocked(0,1)) rank++;
		if(IsCheckPointUnlocked(0,2)) rank++;
		if(IsCheckPointUnlocked(0,3)) rank++;

		if(IsCheckPointUnlocked(1,0)) rank++;
		if(IsCheckPointUnlocked(1,1)) rank++;
		if(IsCheckPointUnlocked(1,2)) rank++;

		return rank;
	}

	private void UpdateValue(string key, string value)
	{
		JObject jData = JSON.Parse(_gameData);
		jData.Add(key,value);

		_gameData = jData.ToString();
	}

	private int GetLevelScore(string key)
	{
		//value from this object data
		string value = "";
		int valueFromThisObject = 0;
		JObject jData = JSON.Parse(_gameData);

		try
		{
			value = jData[key].ToString();
			if(!string.IsNullOrEmpty(value)) value = value.Split('\"')[1];
			else value = "0";
		}
		catch(System.Exception ex)
		{
			value = "0";
			//Debug.LogError(ex.Message);
		}
			
		valueFromThisObject = int.Parse(value);

		return valueFromThisObject;
	}
		
	public LevelStatus GetLevelStatus(int levelID)
	{
        //FOR TESTING AL LEVELS ARE UNLOCKED
       //return LevelStatus.Ready;

		if(string.IsNullOrEmpty(_gameData))
		{
			if(levelID ==0) 
			{
				return LevelStatus.Ready;
			}
			else return LevelStatus.NotReady;
		}
		else
		{
			//has some game data
			JObject jData = JSON.Parse(_gameData);
			string key = GetLevelKey(levelID-1);

			if (levelID == 0) {
				return LevelStatus.Ready;
			} 
			else 
			{
				//value from this object data
				string value = "";
				LevelStatus valueFromThisObject = LevelStatus.NotReady;

				try
				{
					value = jData[key].ToString();
					if(!string.IsNullOrEmpty(value)) value = value.Split('\"')[1];
				}
				catch(System.Exception ex)
				{
					value = "";
					Debug.LogError(ex.Message);
				}

				if (string.Equals (value, KEY_TWO))
					valueFromThisObject = LevelStatus.Ready;
				else if (string.Equals (value, KEY_ZERO))
					valueFromThisObject = LevelStatus.NotReady;
				else if (string.Equals (value, KEY_ONE))
					valueFromThisObject = LevelStatus.Bought;
				else
				{
                    Debug.LogWarning(string.Format("Invalid data format found or key does not exist. Value = {0}, Key = {1}",value,key));
					valueFromThisObject = LevelStatus.NotReady;
				}

				return valueFromThisObject;
			}
		}
	}

	private bool GetLevelStatus(string key)
	{
		//value from this object data
		string value = "";
		bool valueFromThisObject = false;
		JObject jData = JSON.Parse(_gameData);

		try
		{
			value = jData[key].ToString();
			if(!string.IsNullOrEmpty(value)) value = value.Split('\"')[1];
		}
		catch(System.Exception ex)
		{
			value = "";
			//Debug.LogError(ex.Message);
		}

		if(string.Equals(value,KEY_TWO))
			valueFromThisObject = true;
		else if (string.Equals(value,KEY_ZERO))
			valueFromThisObject = false;
		else
		{
			Debug.LogWarning(string.Format("Invalid data format found or key does not exist. Value = {0}, Key = {1}",value,key));
			valueFromThisObject = false;
		}

		return valueFromThisObject;
	}

    public string GetRawData()
    {
        return _gameData;
    }

	private static string GetLevelKey(int lvl)
	{
		return string.Concat("lvl",(lvl+1).ToString());
	}

	private static string GetLevelKey(int lvl, int chkp)
	{
		string chkKey = "";

		switch(chkp)
		{
		case 0:
			chkKey = "a";
			break;

		case 1: 
			chkKey = "b";
			break;

		case 2: 
			chkKey = "c";
			break;

		case 3: 
			chkKey = "d";
			break;
		}

		return string.Concat(GetLevelKey(lvl),chkKey);
	}

	private static string GetScoreKey(string levelKey)
	{
		return string.Concat(levelKey,"_s");
	}
}
