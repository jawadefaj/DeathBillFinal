using UnityEngine;
using System.Collections;
using Portbliss.Station;

public class Work_AIMoveToStation : BaseWorker {

	public FighterRole fighterID;
	public bool waitOnStationReach = true;
    public bool useStartDelay = false;
    public float startDelayTime = 0f;
	private PlayerAI _ai;

	protected override void OnStart ()
	{
        StartCoroutine(IE_Start());
	}

    IEnumerator IE_Start()
    {
        ThirdPersonController tpc = PlayerInputController.instance.GetPlayerByRole (fighterID);
        PlayerAI ai = tpc.GetAI ();
        if(ai!=null)
        {
            if (useStartDelay)
                yield return new WaitForSeconds(startDelayTime);
            
            _ai = ai;
            ai.MoveStation();

            if(waitOnStationReach)
                ai.gameObject.GetComponent<StationController>().OnStationReached += StationReached;
            else
                WorkFinished();
        }
        else
        {
            Debug.LogError("No Player AI found!!");
        }
    }

	private void StationReached()
	{
		_ai.gameObject.GetComponent<StationController>().OnStationReached -= StationReached;
		WorkFinished();
	}
}
