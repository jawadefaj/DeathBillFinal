using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#elif UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif

public class SocialManagerScript : MonoBehaviour {
	public static string GP_LEADERBOARD_01_ID = "CgkI-96kpZYfEAIQAA";//"CgkI-9Dtu98EEAIQAA";

	public static string GC_LEADERBOARD_01_ID = "deathbill.leaderboard";
	public static TimeScope GC_LEADERBOARD_01_TIMESCOPE = TimeScope.AllTime;

	public UnityEngine.UI.Button leaderBoardButton;
	public UnityEngine.UI.Button achievementButton;



	void Start () {
        #if UNITY_IOS
        Social.localUser.Authenticate(null);
        #endif
		if (leaderBoardButton != null) {
			leaderBoardButton.onClick.AddListener(ShowLeaderboard);
		}
  	}
	void FixedUpdate () {
        if (leaderBoardButton != null)
        {
            leaderBoardButton.interactable = Social.localUser.authenticated;
            GrayOut(Social.localUser.authenticated);
        }
		if(achievementButton!=null) achievementButton.interactable = Social.localUser.authenticated;
	}
    public UnityEngine.UI.Image grayOutImageTarget;
    public UnityEngine.UI.Text grayOutTextTarget;

    void GrayOut(bool active)
    {
        if (active)
        {
            grayOutImageTarget.color = Color.white;
            grayOutTextTarget.color = Color.white;
        }
        else
        {
            grayOutImageTarget.color = Color.gray;
            grayOutTextTarget.color = Color.gray;
        }
    }
//	static void Init(System.Action act)
//	{
//		Social.localUser.Authenticate((bool success) => {
//			if(act!=null && success) act();
//		});
//	}
	public void ShowLeaderboard(){
        
		if(!Social.localUser.authenticated){
            Debug.LogError("Leaderboard Show Requested at unauthenticated state!!!.. Check button implementation!!");
//			Init(()=>{
//				#if UNITY_ANDROID
//				PlayGamesPlatform.Instance.ShowLeaderboardUI(GP_LEADERBOARD_01_ID);
//				#elif UNITY_IOS
//				GameCenterPlatform.ShowLeaderboardUI(GC_LEADERBOARD_01_ID,GC_LEADERBOARD_01_TIMESCOPE);
//				#endif
//
//			});
		}
		else
		{
			#if UNITY_ANDROID
			PlayGamesPlatform.Instance.ShowLeaderboardUI(GP_LEADERBOARD_01_ID);
			#elif UNITY_IOS
			GameCenterPlatform.ShowLeaderboardUI(GC_LEADERBOARD_01_ID,GC_LEADERBOARD_01_TIMESCOPE);
			#endif
		}

	}
	public static void PostScoreToLeaderboard01(int score){

		if (!Social.localUser.authenticated)return;
		#if UNITY_ANDROID
		Social.ReportScore(score, GP_LEADERBOARD_01_ID, (bool success) => {});
		#elif UNITY_IOS
		Social.ReportScore(score, GC_LEADERBOARD_01_ID, (bool success) => {});
		#endif
	}
}
