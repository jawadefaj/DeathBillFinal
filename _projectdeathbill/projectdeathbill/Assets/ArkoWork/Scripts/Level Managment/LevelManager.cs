using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Portbliss.LevelManagment;
using UnityEngine.SceneManagement;

public enum LevelID
{
	Level01,
	Level02,
	Level03,
}

public static class LevelManager {

	private static Dictionary<LevelID,Level> gameLevels;
	private static bool isInitialized = false;
	private static CheckPoint to_be_loaded_chkPoint;
	private static LevelID to_be_loaded_level;
    private static FighterName _optPlayer = FighterName.None;

    public static FighterName OptionalPlayer
    {
        get
        {
            if(to_be_loaded_chkPoint.IsEmpty())
                return FighterName.Hillary;
           
            if (to_be_loaded_chkPoint.hasOptionalPlayer)
                return _optPlayer;
            else
            {
                Debug.LogError("The level defination does not contain optional player parameter. Are you sure you want to use the optional player in this level?");
                return FighterName.None;
            }
        }
        set
        {
            _optPlayer = value;
        }
    }

	public static void LoadLevel(LevelID lvlID, int chkPoint)
	{
		if(!isInitialized) Initialize();

		if(gameLevels.ContainsKey(lvlID))
		{
			Level lvl = gameLevels[lvlID];
			to_be_loaded_chkPoint = lvl.GetCheckPoint(chkPoint);
			to_be_loaded_level = lvlID;

			//reset all
			to_be_loaded_chkPoint.ClearAllFlags();

			//play start cine
			to_be_loaded_chkPoint.startCinematicsPending = false;
			if(to_be_loaded_chkPoint.startCinematicsIndex != -1)
			{
				LoadCinematics(to_be_loaded_chkPoint.startCinematicsIndex);
			}
			else
			{
				to_be_loaded_chkPoint.gamePlayPending = false;
				SceneLoader.LoadScene (to_be_loaded_chkPoint.sceneName);
			}

		}
		else
		{
			Debug.LogError("No such level exits in the database");
		}
	}

	public static void LoadLevel(string hash)
	{
		if(!isInitialized) Initialize();

		string id = hash[3].ToString();
		int i_id = int.Parse(id);

		LevelID lvlID = (LevelID)(i_id-1);

        char id2 = hash[4];
        int i_id2 = System.Convert.ToInt32(id2);
        i_id2 -= 97;

        if (i_id2 < 0 || i_id2 > 25)
        {
            Debug.LogError("Level code parsing error");
            return;
        }

		LoadLevel(lvlID,i_id2);
	}

	public static void Initialize()
	{
		if (isInitialized)
			return;
		
        LoadLevelData();

		//By default only level 1-A is unlocked
		UserGameData.instance.UnlockStage(0,-1);
		UserGameData.instance.UnlockStage(0,0);

		isInitialized = true;
	}

    private static void LoadLevelData()
    {
        //load level data
        gameLevels = new Dictionary<LevelID,Level>();

        //level 01
        CheckPoint[] level01Checkpoints = new CheckPoint[4];
        level01Checkpoints[0] = new CheckPoint(GameConstants.level01aSceneName,"lvl1a",false,0);
        level01Checkpoints[1] = new CheckPoint(GameConstants.level01bSceneName,"lvl1b");
        level01Checkpoints[2] = new CheckPoint(GameConstants.level01cSceneName,"lvl1c",false,-1,1);
        level01Checkpoints[3] = new CheckPoint(GameConstants.level01dSceneName,"lvl1d",false,-1,2);
        gameLevels.Add(LevelID.Level01, new Level(level01Checkpoints,true));

        //Level 02
        CheckPoint[] level02Checkpoints = new CheckPoint[3];
        level02Checkpoints[0] = new CheckPoint(GameConstants.level02aSceneName,"lvl2a",true,3);
        level02Checkpoints[1] = new CheckPoint(GameConstants.level02bSceneName,"lvl2b",true,4);
        level02Checkpoints[2] = new CheckPoint(GameConstants.level02cSceneName,"lvl2c",true,5);
        gameLevels.Add(LevelID.Level02, new Level(level02Checkpoints,true));

        //Add more level data here for initialization

        //Level 03
        CheckPoint[] level03Checkpoints = new CheckPoint[1];
        level03Checkpoints [0] = new CheckPoint (GameConstants.enduranceLevel,"lvl3a");
        gameLevels.Add (LevelID.Level03, new Level (level03Checkpoints,true));
    }

    //This following function clear the current level, load next level or send hash for next execution order like goto main menu or goto buy menu
	public static string Clear()
	{
		if(!isInitialized) Initialize();

		//do we need to play this chk point ending cinematics 
		if(!string.IsNullOrEmpty(to_be_loaded_chkPoint.identifier))
		{
			if(to_be_loaded_chkPoint.startCinematicsPending)
			{
				to_be_loaded_chkPoint.startCinematicsPending = false;

				if(to_be_loaded_chkPoint.startCinematicsIndex!=-1)
				{
					LoadCinematics(to_be_loaded_chkPoint.startCinematicsIndex);
					return "";
				}
			}
			else if (to_be_loaded_chkPoint.gamePlayPending)
			{
				to_be_loaded_chkPoint.gamePlayPending = false;
				SceneLoader.LoadScene(to_be_loaded_chkPoint.sceneName);
				return "";
			}
			else if(to_be_loaded_chkPoint.endCinematicsPending)
			{
				to_be_loaded_chkPoint.endCinematicsPending = false;

				if(to_be_loaded_chkPoint.endCinematicsIndex!=-1)
				{
					LoadCinematics(to_be_loaded_chkPoint.endCinematicsIndex);
					return "";
				}
			}

			//we have no where to go now

			//does it have next chk point
			CheckPoint cp = gameLevels[to_be_loaded_level].GetNextCheckPoint(to_be_loaded_chkPoint);
			cp.ClearAllFlags();

			if(!string.IsNullOrEmpty(cp.identifier))
			{
				//open nxt chk point for play. Important point is we are considering all checkpoint is free to play. We need to buy levels only        
				UserGameData.instance.UnlockStage((int)to_be_loaded_level,cp.GetSelfIndex());
			}
			else
			{
				//current level is clear. Proceed to next level

				//Were we playing the last level?
				if (to_be_loaded_level == (LevelID)((System.Enum.GetValues (typeof(LevelID)).Length) - 1)) {
					//go main menu
					cp.identifier = "m";
				} 
				else 
				{
					int nextLvl_int = (((int)to_be_loaded_level) + 1);
					LevelID nextLvl_id = (LevelID)nextLvl_int;
					Level nextLvl = gameLevels [nextLvl_id];

					//the next level is ready to be played. However we still need to decide if it is free to play?
					UserGameData.instance.UnlockStage(nextLvl_int,-1);
					UserGameData.instance.UnlockStage(nextLvl_int,0);

					//we are going to main menu in any way
					cp.identifier = "l";
				}
			}

			return cp.identifier;

		}
		else
		{
			//invalid call
			return "";
		}




	}

	public static void ReloadLevel()
	{
		if(!isInitialized) Initialize();

		if(string.IsNullOrEmpty(to_be_loaded_chkPoint.identifier))
		{
			Debug.LogWarning("No level added. You can not reload this level");
		}
		else
		{
			//reload level
			SceneLoader.LoadScene(to_be_loaded_chkPoint.sceneName);
		}
	}

	public static Level GetLevel(int id)
	{
		if(!isInitialized) Initialize();

		return gameLevels [(LevelID)id];
	}

    public static int GetLevelCount()
    {
        return gameLevels.Count;
    }

    public static Dictionary<LevelID,Level> GetLevelData()
    {
        LoadLevelData();
       
        Dictionary<LevelID,Level> data = new Dictionary<LevelID, Level>();

        foreach (KeyValuePair<LevelID,Level> kvp in gameLevels)
            data.Add(kvp.Key, kvp.Value);

        return data;
    }

	private static void LoadCinematics(int index)
	{
		CinematicsManager.cineIndex = index;
		SceneLoader.LoadScene(GameConstants.cinematicsScene);
	}
}
