using UnityEngine;
using System.Collections;

namespace Portbliss.LevelManagment
{
	public struct CheckPoint
	{
		public string sceneName;
		public string identifier;

		//cinematics
		public bool startCinematicsPending;
		public bool endCinematicsPending;
		public bool gamePlayPending;

		public int startCinematicsIndex;
		public int endCinematicsIndex;

        //Optional player
        public bool hasOptionalPlayer;

		public CheckPoint(string _sceneName, string id, bool canHaveOptionalPlayer=false, int iStartCine=-1, int iEndCine=-1)
		{
			this.sceneName = _sceneName;
			this.identifier = id;
            this.hasOptionalPlayer = canHaveOptionalPlayer;

			startCinematicsPending = true;
			endCinematicsPending = true;
			gamePlayPending = true;

			startCinematicsIndex = iStartCine;
			endCinematicsIndex = iEndCine;
		}

		public bool IsEqual(CheckPoint sample)
		{
			if(string.Equals(sample.identifier,this.identifier)) return true;
			else return false;
		}

		public void ClearAllFlags()
		{
			startCinematicsPending = true;
			endCinematicsPending = true;
			gamePlayPending = true;
		}

		public int GetSelfIndex()
		{
			string s = this.identifier[this.identifier.Length-1].ToString();

			switch(s)
			{
			case "a":
				return 0;
			case "b":
				return 1;
			case "c":
				return 2;
			case "d":
				return 3;
			default:
				return 0;
			}
		}

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(sceneName);
        }
	}
}
