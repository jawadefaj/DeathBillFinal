using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GunPostCollection {

	public GunPost[] gunPosts;

	public GunPostCollection(GunPost[] _gunPosts)
	{
		gunPosts = _gunPosts;
		SetDefaultPlayerAtPosts ();
	}

	#region Public Functionality
	public List<FighterName> GetEligiblePlayers(string postID)
	{
		return GetEligiblePlayers (GetGunPostFromID (postID));
	}

	public List<FighterName> GetEligiblePlayers(GunPost gunPost)
	{
		return new List<FighterName> (gunPost.eligiblePlayers);
	}

	public List<FighterName> GetAvailablePlayers(string postID)
	{
		return GetAvailablePlayers (GetGunPostFromID (postID));
	}

	public List<FighterName> GetAvailablePlayers(GunPost gunPost)
	{
		List<FighterName> pList = new List<FighterName> (gunPost.eligiblePlayers);

		for (int i = 0; i < gunPosts.Length; i++)
		{
			if (gunPosts [i] != gunPost)
			{
				if (pList.Contains (gunPosts [i].selectedPlayer))
					pList.Remove (gunPosts [i].selectedPlayer);
			}
		}

		if (pList.Count < 1)
		{
			Debug.LogError ("No more available player!");
		}

		return pList;
	}

	public void SetPlayerToPost(string postID, FighterName fighter)
	{
		SetPlayerToPost (GetGunPostFromID (postID),fighter);
	}

	public void SetPlayerToPost(GunPost gunPost, FighterName fighter)
	{
		List<FighterName> availableList = GetAvailablePlayers (gunPost);

		if (availableList.Contains (fighter))
		{
			gunPost.selectedPlayer = fighter;
		}
		else
		{
			Debug.LogError ("The player is not eligible to sit on the post!");
		}

	}

	public FighterName GetSelectedPlayerAtPost(string postID)
	{
		return GetSelectedPlayerAtPost (GetGunPostFromID (postID));
	}

	public FighterName GetSelectedPlayerAtPost(GunPost gunPost)
	{
		return gunPost.selectedPlayer;
	}
	#endregion

	#region Private Methods
	private void SetDefaultPlayerAtPosts()
	{
		List<FighterName> usedFighters = new List<FighterName> ();

		for (int i = 0; i < gunPosts.Length; i++)
		{
			for (int j = 0; j < gunPosts [i].eligiblePlayers.Length; j++)
			{
				if (usedFighters.Contains (gunPosts [i].eligiblePlayers [j]))
				{
					if (j == gunPosts [i].eligiblePlayers.Length - 1)
					{
						Debug.LogError ("Automatic player selection failed!");
						gunPosts [i].selectedPlayer = gunPosts [i].eligiblePlayers [0];
						usedFighters.Add (gunPosts [i].selectedPlayer);
					}
					continue;
				} 
				else
				{
					gunPosts [i].selectedPlayer = gunPosts [i].eligiblePlayers [j];
					usedFighters.Add (gunPosts [i].selectedPlayer);
					//Debug.Log (string.Format("At post {0} seletec player is {1}", gunPosts[i].postID,gunPosts[i].selectedPlayer.ToString()));
					break;
				}
			}
		}
	}

	private GunPost GetGunPostFromID(string postID)
	{
		for (int i = 0; i < gunPosts.Length; i++)
		{
			if (string.Equals (postID, gunPosts [i].postID))
				return gunPosts [i];
		}

		Debug.LogError ("No valid gun post found by the id "+ postID);

		return new GunPost ();
	}
		
	#endregion
}
