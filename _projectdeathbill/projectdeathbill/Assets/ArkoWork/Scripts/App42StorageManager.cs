using UnityEngine;
using System.Collections;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.storage;
using SimpleJSON;

public enum DataRetriveStatus
{
	DoingNothing =0,
	Working =1,
	CompletedWithSuccess =2,
	CompletedWithErrorCode100 =3,	//valid email address not found
	CompletedWithErrorCode200 =3,	//can not retrive data from storage
	CompletedWithErrorCode300 =4,	//Invalid data format
	CompletedWithErrorCode400 =5,	//Invalid storage object
}

public class App42StorageManager : MonoBehaviour {

	public static App42StorageManager instance;

	private string apiKey = "d176ae4b7bf311dcd6c8a2b64f0d8a3b329c84b07e5ec8c2946180c4634d7c98";
	private string secrectKey = "b4bd76832ed6e36037d31219512840ab535e97d56648db743f8d8dcfdc0fb3e9";

	private string databaseName = "HEROESOF71";
	private string collectionName = "DataRetriveCenter";

	private StorageService storageService;
	private DataCheckCallBack chkCallback = new DataCheckCallBack();

	public DataRetriveStatus dataRetriveStatus = DataRetriveStatus.DoingNothing;

	void Awake()
	{
		instance = this;
		//DontDestroyOnLoad(this);
	}

	void Start () {
		App42API.Initialize(apiKey,secrectKey);

		storageService = App42API.BuildStorageService();

	}
	
	public void CheckForLevelUnlockPermission()
	{
		dataRetriveStatus = DataRetriveStatus.Working;

		string key = "id";
		string value = GetMailAddress();

		if(string.IsNullOrEmpty(value))
		{
			dataRetriveStatus = DataRetriveStatus.CompletedWithErrorCode100;
			return;
		}

		storageService.FindDocumentByKeyValue(databaseName,collectionName,key,value, chkCallback);
	}

	public void OpenUpLevels(string command)
	{
		string[] levelsToOpen = command.Split(',');

		foreach(string s in levelsToOpen)
		{
			int lvl = int.Parse(s.Split('-')[0]);
			int chk = int.Parse(s.Split('-')[1]);

			UserGameData.instance.UnlockStage(lvl-1,chk-1);
		}

		App42StorageManager.instance.dataRetriveStatus = DataRetriveStatus.CompletedWithSuccess;
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


	public class DataCheckCallBack:App42CallBack
	{
		public void OnSuccess (object response)
		{
			if(response is Storage)
			{
				Storage storage = (Storage)response;
				Storage.JSONDocument doc = storage.jsonDocList[0];

				//got the data here
				JObject jsData = JSON.Parse(doc.GetJsonDoc());
				string openCommand = "";

				try
				{
					openCommand = jsData["open"].ToString().Split('\"')[1];
					//Debug.Log(openCommand);
				}
				catch(System.Exception ex)
				{
					Debug.Log(ex.Message);
					openCommand = "";
					App42StorageManager.instance.dataRetriveStatus = DataRetriveStatus.CompletedWithSuccess;
					return;
				}
					
				App42StorageManager.instance.OpenUpLevels(openCommand);
				return;
			}

			App42StorageManager.instance.dataRetriveStatus = DataRetriveStatus.CompletedWithErrorCode400;

		}

		public void OnException (System.Exception ex)
		{
			Debug.Log(ex.Message);
			if(App42StorageManager.instance!=null)
				App42StorageManager.instance.dataRetriveStatus = DataRetriveStatus.CompletedWithErrorCode200;
		}
	}
}
