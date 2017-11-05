using UnityEngine;
using System.Collections;

public class GunPost {

	public FighterName[] eligiblePlayers;
	public FighterName selectedPlayer;
	public string postID;

	public GunPost()
	{

	}

	public GunPost(FighterName[] eligibleFighters, string _postID)
	{
		eligiblePlayers = eligibleFighters;
		postID = _postID;
		selectedPlayer = eligiblePlayers [0];
	}

	public bool Equals(GunPost gp)
	{
		return string.Equals (postID, gp.postID);
	}

	public static bool operator == (GunPost gp1, GunPost gp2)
	{
		return gp1.Equals (gp2);
	}

	public static bool operator != (GunPost gp1, GunPost gp2)
	{
		return !gp1.Equals (gp2);
	}
}
