using UnityEngine;
using System.Collections;

namespace Portbliss.LevelManagment
{
	public class Level {

		CheckPoint[] _checkPoints;
		bool isMagna;	//Is it free to play

		public Level(CheckPoint[] levelCheckPoints, bool magna)
		{
			_checkPoints = levelCheckPoints;
			isMagna = magna;
		}
			
		public CheckPoint GetCheckPoint(int chkPointNo)
		{
			if(chkPointNo>=_checkPoints.Length)
			{
				Debug.LogError("No Such CheckPoint Exist. Returning the first checkpoint.");
				return _checkPoints[0];
			}
			return _checkPoints[chkPointNo];
		}

        public int GetCheckPointCount()
        {
            return _checkPoints.Length;
        }

		public CheckPoint GetNextCheckPoint(CheckPoint now)
		{
			for(int i=0;i<_checkPoints.Length;i++)
			{
				if(_checkPoints[i].IsEqual(now))
				{
					if(i<_checkPoints.Length-1) return _checkPoints[i+1];
				}
			}
			return new CheckPoint("","");
		}

		//lock/unlock equals to 
		public bool IsKopila()
		{
			return SecurePlayerPrefs.GetString("lvl1","0")=="0"?false:true;
		}

		public bool IsMagna()
		{
			return isMagna;
		}
	}
}
