using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EnduranceManager : MonoBehaviour {

    public static EnduranceManager instance;

    public const string GP_SHORT = "short";
	public const string GP_MID = "mid";
	public const string GP_LONG = "long";

	public static FighterName ShortRangePlayer
	{
		get 
		{
			if (!isInitialized)
				Initialize ();
			return gp_collection.GetSelectedPlayerAtPost (GP_SHORT);
		}
	}

	public static List<FighterName> ShortRangePlayers
	{
		get 
		{
			if (!isInitialized)
				Initialize ();
			return gp_collection.GetEligiblePlayers (GP_SHORT);
		}
	}

	public static FighterName MidRangePlayer
	{
		get 
		{
			if (!isInitialized)
				Initialize ();
			return gp_collection.GetSelectedPlayerAtPost (GP_MID);
		}
	}

	public static List<FighterName> MidRangePlayers
	{
		get 
		{
			if (!isInitialized)
				Initialize ();
			return gp_collection.GetEligiblePlayers (GP_MID);
		}
	}

	public static FighterName LongRangePlayer
	{
		get 
		{
			if (!isInitialized)
				Initialize ();
			return gp_collection.GetSelectedPlayerAtPost (GP_LONG);
		}
	}

	public static List<FighterName> LongRangePlayers
	{
		get 
		{
			if (!isInitialized)
				Initialize ();
			return gp_collection.GetEligiblePlayers (GP_LONG);
		}
	}

	private static GunPostCollection gp_collection;
	private static bool isInitialized = false;

    void Awake()
    {
        instance = this;
    }

	void Start()
	{
		Initialize ();
	}

    void OnEnable()
    {
        goBtn.onClick.AddListener(OnGoBtnClicked);
    }

    void OnDisable()
    {
        goBtn.onClick.RemoveListener(OnGoBtnClicked);
    }

	private static void Initialize()
	{
		GunPost gunPost_shortRange = new GunPost (new FighterName[]{ FighterName.Hillary, FighterName.Trump },GP_SHORT);
    	GunPost gunpost_midRange = new GunPost (new FighterName[]{FighterName.Dom, FighterName.Baker },GP_MID);
		GunPost gunpost_longRange = new GunPost (new FighterName[]{FighterName.Philips, FighterName.JB },GP_LONG);

        //GunPost gunPost_shortRange = new GunPost (new FighterName[]{ FighterName.Trump, FighterName.Hillary },GP_SHORT);
        //GunPost gunpost_midRange = new GunPost (new FighterName[]{FighterName.Baker, FighterName.Dom},GP_MID);
        //GunPost gunpost_longRange = new GunPost (new FighterName[]{FighterName.JB, FighterName.Philips},GP_LONG);

		gp_collection = new GunPostCollection (new GunPost[]{gunPost_shortRange,gunpost_midRange,gunpost_longRange });
		isInitialized = true;
	}

    #region GUI Section

    public GameObject[] outsideObjects;
    public Button goBtn;

    public void OnGoBtnClicked()
    {
        LevelManager.LoadLevel (LevelID.Level03,0);
    }

    public void BackButtonFucntion()
    {
        ChangeOutsideObjectStatus(true);
    }

    public void UpdatePlayerSelectionWindow()
    {
        ChangeOutsideObjectStatus(false);
    }

    public void SelectPlayer(string toPost, FighterName fName)
    {
        gp_collection.SetPlayerToPost(toPost, fName);
    }

    public void ChangeGoButtonStatus(bool isReady)
    {
        goBtn.interactable = isReady;
    }

    public string GetPlayerPostID(FighterName fName)
    {
        if (ShortRangePlayers.Contains(fName))
            return GP_SHORT;
        if (MidRangePlayers.Contains(fName))
            return GP_MID;
        if (LongRangePlayers.Contains(fName))
            return GP_LONG;

        Debug.LogError(string.Format("No fighter by the name {0} is not in the gun post list", fName.ToString()));
        return "";
    }

    private void ChangeOutsideObjectStatus(bool isActive)
    {
        foreach (GameObject go in outsideObjects)
            go.SetActive(isActive);
    }
    #endregion
}
