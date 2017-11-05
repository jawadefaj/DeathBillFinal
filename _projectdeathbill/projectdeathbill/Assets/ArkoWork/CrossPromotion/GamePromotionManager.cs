using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class GamePromotionManager : MonoBehaviour {

	public static GamePromotionManager instance;

	public string url;

	public Button playNow;
	public Button quit;
	public Image adImage;

	internal bool isReady = false;
	private Texture2D adPoster;
	private Canvas thisCanvas;
	private bool onceTry = true;
	private bool notiNotSent = false;

	public delegate void NewMessageEventHandeler();
	public event NewMessageEventHandeler OnAdNewsPending;
	public event NewMessageEventHandeler OnPromotionManagerReady;

	void Awake()
	{
		DontDestroyOnLoad(this);
		instance = this;
		thisCanvas = this.GetComponent<Canvas>();
	}

	IEnumerator Start()
	{
		isReady = false;

		//hide ourselves
		thisCanvas.enabled = false;
		thisCanvas.sortingOrder = 9999;

		//read data from web url
		WWW newsChecker = new WWW(url);
		do
		{
			//Debug.Log("loading txt file");
			yield return null;
		}while(newsChecker.isDone == false);

		if(!string.IsNullOrEmpty(newsChecker.error))
		{
			Debug.Log("error occured "+ newsChecker.error);
			yield break;
		}

		string dataFromWeb = newsChecker.text;

		//read data from player prefs
		string dataFromLocal = PlayerPrefs.GetString("AdData", "");

		Debug.Log("data from web : "+ dataFromWeb);
		Debug.Log("data from locat: "+ dataFromLocal);

		//checking
		if(string.IsNullOrEmpty(dataFromWeb)) dataFromWeb = AdData.GetDefaultJSON();
		if(string.IsNullOrEmpty(dataFromLocal)) dataFromLocal = AdData.GetDefaultJSON();

		//conostruct 2 object
		AdData fromWeb = JsonUtility.FromJson<AdData>(dataFromWeb);
		AdData fromLocal = JsonUtility.FromJson<AdData>(dataFromLocal);

		Debug.Log(fromWeb.ToString());
		Debug.Log(fromLocal.ToString());

		//check for equality
		string localImgeAdd = Path.Combine(Application.persistentDataPath,"ad_image");
		if(fromWeb.IsEqual(fromLocal))
		{
			//load image from saved file
			if(System.IO.File.Exists(localImgeAdd))
			{
				byte[] imageBytes = System.IO.File.ReadAllBytes(localImgeAdd);
				adPoster = new Texture2D(0,0);
				adPoster.LoadImage(imageBytes);
				isReady = true;

				//same old
				if(!fromLocal.hasSeenYet)
				{
					//one news is pending
					if(OnAdNewsPending!=null)
					{
						OnAdNewsPending();
					}
					else
					{
						notiNotSent = true;
					}
				}
			}
		}
		else
		{
			//new news

			//download and save the image
			WWW imageDownloader = new WWW(fromWeb.imageUrl);

			do
			{
				//Debug.Log("loading image file");
				yield return null;
			}while(imageDownloader.isDone==false);

			if(string.IsNullOrEmpty(imageDownloader.error))
			{
				//store first
				fromWeb.hasSeenYet = false;
				PlayerPrefs.SetString("AdData", JsonUtility.ToJson(fromWeb));

				adPoster = imageDownloader.texture;

				System.IO.File.WriteAllBytes(localImgeAdd,imageDownloader.bytes);

				//now ready to display
				isReady = true;

				//trigger event
				if(OnAdNewsPending!=null) 
				{
					OnAdNewsPending();
				}
				else
				{
					notiNotSent = true;
				}


			}
			else
			{
				Debug.LogError("image download failed");
				Debug.Log(imageDownloader.error);
			}

		}
	}

	void OnEnable()
	{
		playNow.onClick.AddListener(playNowF);
		quit.onClick.AddListener(quitF);
	}

	void OnDisable()
	{
		playNow.onClick.RemoveAllListeners();
		quit.onClick.RemoveAllListeners();
	}

	void Update()
	{
		if(onceTry)
		{
			if(isReady)
			{
				if(OnPromotionManagerReady!=null) 
				{
					OnPromotionManagerReady();
					onceTry = false;
				}
			}
		}

		if(notiNotSent)
		{
			if(OnAdNewsPending != null)
			{
				notiNotSent = false;
				OnAdNewsPending();
			}
		}
	}

	void playNowF()
	{
		AdData fromLocal = JsonUtility.FromJson<AdData>(PlayerPrefs.GetString("AdData"));
		fromLocal.hasSeenYet = true;
		PlayerPrefs.SetString("AdData", JsonUtility.ToJson(fromLocal));

		Application.OpenURL(fromLocal.downloadUrl);

		quitF();
	}

	void quitF()
	{
		this.thisCanvas.enabled = false;
	}

	public void ShowAd()
	{
		if(!isReady) return;

		thisCanvas.enabled = true;

		Sprite newSP = Sprite.Create(adPoster,new Rect(0,0,adPoster.width,adPoster.height),new Vector2(0.5f,0.5f));

		adImage.sprite = newSP;

	}


	public class AdData
	{
		public string imageUrl = "";
		public string downloadUrl = "";
		public bool hasSeenYet = false;

		public bool IsEqual(AdData obj)
		{
			return (obj.downloadUrl == downloadUrl);
		}

		public static string GetDefaultJSON()
		{
			return JsonUtility.ToJson(new AdData());
		}

		public override string ToString ()
		{
			return JsonUtility.ToJson(this);
		}
	}
}
