#if UNITY_ANDROID
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine.SocialPlatforms;

public class GPGSavedTestScript : MonoBehaviour{

	// Use this for initialization
	void Start () {
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
			// enables saving game progress.
			.EnableSavedGames()
			.Build();
		PlayGamesPlatform.InitializeInstance(config);
		PlayGamesPlatform.DebugLogEnabled = true;
		PlayGamesPlatform.Activate ();
		Social.localUser.Authenticate((bool success) => {
			if(success)
			{

				debugText.text = "login success";
			}
			else
			{
				debugText.text = "login failed";
			}
		});
	}


	public Text mainText;
	public Text debugText;

	public void ShowUI()
	{
		Debug.Log ("Show ui called");
		uint maxNumToDisplay = 5;
		bool allowCreateNew = false;
		bool allowDelete = true;

		ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
		savedGameClient.ShowSelectSavedGameUI("Select saved game",
			maxNumToDisplay,
			allowCreateNew,
			allowDelete,
			OnSavedGameSelected
		);
	}
	public void OnSavedGameSelected (SelectUIStatus status, ISavedGameMetadata game) {
		Debug.Log ("selection :" + status.ToString());
	}
	public void Save()
	{
		OpenSavedGame (OnSavedGameOpenedToSave);
	}
	public void Load()
	{
		OpenSavedGame (OnSavedGameOpenedToLoad);
	}
	public void Clear()
	{
		mainText.text = "__";
		debugText.text = "__";
	}
	public void Generate()
	{
		mainText.text = UnityEngine.Random.Range(0,99).ToString();
	}


	void OpenSavedGame(System.Action<SavedGameRequestStatus,ISavedGameMetadata> onopend)
	{
		ISavedGameClient savedgameclient = PlayGamesPlatform.Instance.SavedGame;
		savedgameclient.OpenWithAutomaticConflictResolution ("poop", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, onopend);
		ConflictCallback cc = conflictCall;
		savedgameclient.OpenWithManualConflictResolution ("poop", DataSource.ReadCacheOrNetwork, true, cc, onopend);
	}
	//ConflictCallback asda;
	public void conflictCall(IConflictResolver con, ISavedGameMetadata original, byte[] orgBytes, ISavedGameMetadata unmerged, byte[] unmergedBytes)
	{
		con.ChooseMetadata (original);
	}

	public void OnSavedGameOpenedToSave(SavedGameRequestStatus status, ISavedGameMetadata game) {
		if (status == SavedGameRequestStatus.Success) {
			debugText.text = "going to save";
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
}
#endif
