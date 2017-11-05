using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.MiniJSON;
using Facebook.Unity;
using System;


public class FBManager : MonoBehaviour {

	public static FBManager instance;

	private static List<object>                 friends         = null;
	private static Dictionary<string, string>   profile         = null;
	private static List<object>                 scores          = null;
	private static Dictionary<string, Texture>  friendImages    = new Dictionary<string, Texture>();

	public static List<FacebookFriend> myFriends = new List<FacebookFriend> ();

	//my infos
	public static string myName="";
	public static long myScore=0;

	public bool isInit = false;

	//events
	public delegate void ActionComplete();
	public delegate void ActionCompleteBool(bool success);
	public event ActionComplete OnLoggedInDone;
	public event ActionComplete OnBasicInfoGetDone;
	public event ActionComplete OnScoreBoardGetDone;
	public event ActionCompleteBool onSaveScoreDone;
	public event ActionComplete OnLoggedOutDone;
	public event ActionComplete OnInitializeDone;

	void Awake()
	{
		DontDestroyOnLoad(this);
		if (FBManager.instance ==null)
			FBManager.instance=this;
		else
			Destroy (this);
	}

	void Start()
	{
		InitializeFacebook();
	}

	public void InitializeFacebook()
	{
		FB.Init(SetInit, OnHideUnity);
	}

	void OnHideUnity(bool isShown)
	{

	}

	void SetInit()
	{
		isInit = true;
		if(OnInitializeDone != null) OnInitializeDone();
		//Debug.Log ("Init complt");
		if (FB.IsLoggedIn) 
		{
			OnLoggedIn();
		}
	}

	void OnLoggedIn()
	{
		//loggid in complt
		if(OnLoggedInDone!=null) OnLoggedInDone ();
		// Reqest player info and profile picture
		//FB.API("/me?fields=id,first_name,friends.limit(100).fields(first_name,id)", Facebook.HttpMethod.GET, APICallback);
	}

//	public void SaveNewScore ( Int32 score)
//	{
//		if (score > myScore) 
//		{
//			myScore = score;
//			if (FB.IsLoggedIn)
//			{
//				var query = new Dictionary<string, string>();
//				query["score"] = score.ToString ();
//				FB.API("/me/scores", Facebook.HttpMethod.POST, SaveScoreCallback , query);
//			}
//		}
//	}

	public void FeedPost (string link_name)//string _pictureUrl,
	{
		if (FB.IsLoggedIn) 
		{
			/*FB.ShareLink(contentURL:new Uri("https://play.google.com/store/apps/details?id=com.portbliss.ho71.retaliation"),
			             contentTitle:"HEROES OF 71",
			             contentDescription:link_name,
			             photoURL:new Uri("http://heroesof71.portbliss.org/anila-in-action.jpg"),
			             callback: null);*/
			
			FB.FeedShare(link: new Uri("https://play.google.com/store/apps/details?id=com.portbliss.deathbill"),
				linkName: "War On Terror: Deathbill",
				linkDescription: link_name,
				picture: new Uri("http://portbliss.org/deathbill/fb_photo.png"),
				mediaSource:"",
				callback: null);
		}

	}

	void ShareCallback(IResult r)
	{
		//Debug.Log (r.Text);
	}
//
//	void SaveScoreCallback(FBResult r)
//	{
//		if (r.Error == null)
//			onSaveScoreDone (true);
//		else
//			onSaveScoreDone(false);
//	}

	/*void APICallback(IResult result)
	{
		if (result.Error != null)
		{
			//Debug.LogError(result.Error);
			// Let's just try again
			FB.API("/me?fields=id,first_name,friends.limit(100).fields(first_name,id)", Facebook.HttpMethod.GET, APICallback);
			return;
		}
		
		profile = Util.DeserializeJSONProfile(result.RawResult);
		myName =profile["first_name"];

		//get friends
		friends = Util.DeserializeJSONFriends(result.RawResult);
		
		//basic info got
		if(OnBasicInfoGetDone!=null) OnBasicInfoGetDone ();
	}*/

//	public void QueryScores()
//	{
//		FB.API("/app/scores?fields=score,user.limit(20)", Facebook.HttpMethod.GET, ScoresCallback);
//	}
//
//	void ScoresCallback(FBResult result) 
//	{
//		Util.Log("ScoresCallback");
//		if (result.Error != null)
//		{
//			Util.LogError(result.Error);
//			return;
//		}
//		
//		scores = new List<object>();
//		List<object> scoresList = Util.DeserializeScores(result.Text);
//		
//		foreach(object score in scoresList) 
//		{
//			var entry = (Dictionary<string,object>) score;
//			var user = (Dictionary<string,object>) entry["user"];
//			
//			string userId = (string)user["id"];
//			
//			if (string.Equals(userId,FB.UserId))
//			{
//				// This entry is the current player
//				int playerHighScore = getScoreFromEntry(entry);
//				Util.Log("Local players score on server is " + playerHighScore);
//
//				//this is my hogh score
//				myScore = playerHighScore;
//			}
//			
//			scores.Add(entry);
//			/*
//			if (!friendImages.ContainsKey(userId))
//			{
//				// We don't have this players image yet, request it now
//				LoadPictureAPI(Util.GetPictureURL(userId, 128, 128),pictureTexture =>
//				               {
//					if (pictureTexture != null)
//					{
//						friendImages.Add(userId, pictureTexture);
//					}
//				});
//			}*/
//		}
//		
//		// Now sort the entries based on score
//		scores.Sort(delegate(object firstObj,
//		                     object secondObj)
//		            {
//			return -getScoreFromEntry(firstObj).CompareTo(getScoreFromEntry(secondObj));
//		}
//
//		);
//
//		//size score object
//		myFriends.Clear ();
//		if(scores != null)
//		{
//			foreach(object scoreEntry in scores) 
//			{
//				Dictionary<string,object> entry = (Dictionary<string,object>) scoreEntry;
//				Dictionary<string,object> user = (Dictionary<string,object>) entry["user"];
//
//				
//				string name     = ((string) user["name"]).Split(new char[]{' '})[0] + "\n";
//				string score     = getScoreFromEntry (entry).ToString ();
//				string id = (string)user["id"];
//
//				Texture picture=null;
//				/*if (friendImages.TryGetValue((string) user["id"], out picture)) 
//				{
//
//				}*/
//
//				FacebookFriend frnd = new FacebookFriend(id,name,picture,Int32.Parse (score));
//				myFriends.Add (frnd);
//			}
//			
//		}
//
//		OnScoreBoardGetDone ();
//	}

	/*private int getScoreFromEntry(object obj)
	{
		Dictionary<string,object> entry = (Dictionary<string,object>) obj;
		return Convert.ToInt32(entry["score"]);
	}*/

	public void LoginFacebook()
	{
		if (isInit)
		{
			if (!FB.IsLoggedIn)
			{
				FB.LogInWithPublishPermissions(new List<string>() { "publish_actions" }, this.LoginCallback);
			}
		}
	}

	public void LogoutFacebook()
	{
		if(FB.IsLoggedIn)
		{
			FB.LogOut();
			OnLoggedOutDone();
		}
	}

	/// <summary>
	/// Share your score in Facebook with your frineds
	/// </summary>
	/// <param name="_scoreToShear">The score you want to shear</param>
	/// <param name="_pictureUrl">picture url you want to attach</param>
	///<param name="_linkName">link name</param>


	void LoginCallback(IResult result)
	{
		if (FB.IsLoggedIn)
		{
			OnLoggedIn ();
		}
	}


	

}

public class FacebookFriend
{
	public string name;
	public Texture image;
	public Int32 score;
	private string id;

	public FacebookFriend(string _id,string _name, Texture _texture, Int32 _score)
	{
		this.name = _name;
		this.image = _texture ;
		this.score = _score;
		this.id = _id;
	}

	public string GetImageUrl()
	{
		return "http://graph.facebook.com/"+id +"/picture?type=square";
	}
}
