using UnityEngine;
using System.Collections;

public class Work_Level2A_Save : BaseWorker {

    public int passedID =5;
	protected override void OnStart()
    {
        Work_Level2A_LoadFromSave.startFrom = passedID + 1;
        Debug.Log("saving state "+Work_Level2A_LoadFromSave.startFrom );
        WorkFinished();
    }
}
