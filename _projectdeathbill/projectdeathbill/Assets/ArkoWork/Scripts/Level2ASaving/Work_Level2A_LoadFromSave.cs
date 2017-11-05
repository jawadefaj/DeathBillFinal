using UnityEngine;
using System.Collections;

public class Work_Level2A_LoadFromSave : BaseWorker {

    //stages are 5,6,7

    public static int startFrom = 5;

    public GameManagerMaster master;

	protected override void OnStart()
    {
        WorkInfo wi;

        if (startFrom == 5)
        {
            //nothing special
        }
        else if (startFrom == 6)
        {
            wi = master.workList[4];
            wi.isActive = false;
            master.workList[4] = wi;
            //update score
            GeneralManager.instance.score=600;
            GeneralManager.instance.killCount = 3;
        }
        else if (startFrom == 7)
        {
            wi = master.workList[4];
            wi.isActive = false;
            master.workList[4] = wi;

            wi = master.workList[5];
            wi.isActive = false;
            master.workList[5] = wi;

            //update score
            GeneralManager.instance.score=1000;
            GeneralManager.instance.killCount = 5;
        }

        WorkFinished();
    }
}
