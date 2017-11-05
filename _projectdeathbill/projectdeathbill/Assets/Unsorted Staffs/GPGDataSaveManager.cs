//#define GPG_DATASAVER

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
#endif
using UnityEngine.SocialPlatforms;

public class GPGDataSaveManager:MonoBehaviour{

    static void OnOptionChecked(bool isGPGOk)
    {
        

        SocialAuthAndroid(isGPGOk);
    }

    static void ActivateGPG(bool savedGamesEnabled)
    {
        #if UNITY_ANDROID
        if (!gpgActivationCalled)
        {
            PlayGamesClientConfiguration config;
            if (savedGamesEnabled)
            {
                config = new PlayGamesClientConfiguration.Builder()
                .EnableSavedGames()
                .Build();
            }
            else
            {
                config = new PlayGamesClientConfiguration.Builder()
                .Build();
            }
            PlayGamesPlatform.InitializeInstance(config);
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
            gpgActivationCalled = true;
        }
        #endif
    }

    static void SocialAuthAndroid(bool gpg_datasaver_ok)
    {
        ActivateGPG(gpg_datasaver_ok);

        Social.localUser.Authenticate((bool success) => {
            if(success && gpg_datasaver_ok)
            {
                #if UNITY_ANDROID
                UserSettings.IsGPGDataSaverOK = true;
                OpenSavedGame(OnSavedGameOpenedToLoad);
                #endif
            }
        });


    }

	void Awake()
	{
        SetLoadedData("");

        #if UNITY_ANDROID
        if (UserSettings.IsGPGDataSaverOK == false)
            ExternalOptionChecker.StartOptionChecking(OnOptionChecked);
        else
            SocialAuthAndroid(true);	
        #endif
	}

	public const string fileName = "PersonalSaveData";
	public static bool gpgActivationCalled = false;
	public static byte[] dataToSave;
	public static void SaveData(string data)
	{
		if (Social.localUser.authenticated) 
		{
			dataToSave = System.Text.Encoding.UTF8.GetBytes (data);
            #if UNITY_ANDROID
            #if GPG_DATASAVER
			OpenSavedGame (OnSavedGameOpenedToSave);
            #endif
            #endif
		}
	}
	public static void SetLoadedData(string data)
	{
		UserGameData.LoadGame(data);
	}
	public static bool ShouldUseOriginal(byte[] originalBytes,byte[] unmergedBytes)
	{
		//arbitrate if should use original or unmerged
		string org = System.Text.Encoding.UTF8.GetString(originalBytes);
		string unm = System.Text.Encoding.UTF8.GetString(unmergedBytes);
		UserGameData orgOb = new UserGameData(org);
		UserGameData unmOb = new UserGameData(unm);
		return orgOb.IsHigherThan(unmOb);
	}

    #if UNITY_ANDROID
	public static void OpenSavedGame(System.Action<SavedGameRequestStatus,ISavedGameMetadata> OnOpenComplete)
	{
		ISavedGameClient savedgameclient = PlayGamesPlatform.Instance.SavedGame;
		ConflictCallback OnConflict = ResolveConflict;
		savedgameclient.OpenWithManualConflictResolution ("poop", DataSource.ReadCacheOrNetwork, true, OnConflict, OnOpenComplete);
	}
	public static void ResolveConflict(IConflictResolver con, ISavedGameMetadata original, byte[] originalBytes, ISavedGameMetadata unmerged, byte[] unmergedBytes)
	{
		string orignalString = System.Text.Encoding.UTF8.GetString(originalBytes);
		string unmergedString = System.Text.Encoding.UTF8.GetString(unmergedBytes);
		con.ChooseMetadata (ShouldUseOriginal(originalBytes,unmergedBytes)?original:unmerged);
	}

	public static void OnSavedGameOpenedToSave(SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			ISavedGameClient savedgameclient = PlayGamesPlatform.Instance.SavedGame;
			TimeSpan t = game.TotalTimePlayed;
			TimeSpan t2 = new TimeSpan (0,0,Mathf.CeilToInt(Time.time));
			t.Add (t2);
			SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
			builder = builder
				.WithUpdatedPlayedTime(t)
				.WithUpdatedDescription("Saved game at " + DateTime.Now);
			SavedGameMetadataUpdate updatedMetadata = builder.Build();
			savedgameclient.CommitUpdate (game,
				updatedMetadata,
				dataToSave,
				OnSavedGameWritten
			);
		} else {
			Debug.Log ("Failed to open game or writing!");
		}
	}
	public static void OnSavedGameWritten (SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
		} else {
			Debug.Log ("Failed to write game data!");
		}
	}

	public static void OnSavedGameOpenedToLoad(SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
			savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
		} else {
			SetLoadedData ("");
			Debug.Log ("Failed to open game for reading");
		}
	}
	public static void OnSavedGameDataRead (SavedGameRequestStatus status, byte[] data) {
		if (status == SavedGameRequestStatus.Success) {
			SetLoadedData (System.Text.Encoding.UTF8.GetString(data));
		} else {
			SetLoadedData ("");
			Debug.Log ("Failed to read data!");
		}
	}
    #endif
	/*
	public void OnSavedGameOpenedToSave(SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			ISavedGameClient savedgameclient = PlayGamesPlatform.Instance.SavedGame;
			TimeSpan t = game.TotalTimePlayed;
			TimeSpan t2 = new TimeSpan (0,0,Mathf.CeilToInt(Time.time));
			t.Add (t2);
			SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
			builder = builder
				.WithUpdatedPlayedTime(t)
				.WithUpdatedDescription("Saved game at " + DateTime.Now);
			SavedGameMetadataUpdate updatedMetadata = builder.Build();
			savedgameclient.CommitUpdate (game,
				updatedMetadata,
				System.Text.Encoding.UTF8.GetBytes(mainText.text),
				OnSavedGameWritten
			);
		} else {
			debugText.text = "failed to save" + status.ToString();
		}
	}
	public void OnSavedGameWritten (SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			debugText.text = "Written";
		} else {
			debugText.text = "failed to write";
		}
	}
	public void OnSavedGameOpenedToLoad(SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			debugText.text = "going to load from : " + game.TotalTimePlayed.TotalSeconds.ToString ();
			ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
			savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
		} else {

			debugText.text = "failed to load"+ status.ToString();
			// handle error
		}
	}
	public void OnSavedGameDataRead (SavedGameRequestStatus status, byte[] data) {
		if (status == SavedGameRequestStatus.Success) {

			debugText.text = "going to read";
			mainText.text = System.Text.Encoding.UTF8.GetString(data);
		} else {

			debugText.text = "failed to read"+ status.ToString();
			// handle error
		}
	}
	*/
}
