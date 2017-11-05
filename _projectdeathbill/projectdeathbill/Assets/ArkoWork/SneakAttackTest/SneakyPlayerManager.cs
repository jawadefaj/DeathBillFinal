using UnityEngine;
using System.Collections;
using Portbliss.SneakyStation;

public class SneakyPlayerManager : MonoBehaviour {

	public static SneakyPlayerManager instance;

	public SneakyPlayer[] players;

	private SneakyPlayer currentPlayer;
	private Transform nextTarget;
	internal AIPersonnel nextTargetPersonel;
	private bool isStabPending;
	private bool isMouseDragging = false;
	private bool isUsingAiToFindPath = false;
	private Vector2 lastPos;
	private float touchFactor;
	private float _noiseLevel =0f;
	private bool isGameOver = false;

	private const float MAX_AFFECTING_DISTANCE_SQ = 100; // squared
	public const float MAX_ALLOWABLE_NOISE_LEVEL = 0.75f;
	private const float MIN_APPROACHING_DISTANCE = 0.2f;
	public const float STAB_RANGE_SQ = 4f;

	public System.Action<bool> OnWalkStateChanged;

	void Awake()
	{
		instance = this;
	}
		
	void Start () {
		currentPlayer = players[0];
		touchFactor = 60f / Screen.height;
		currentPlayer.ActivatePlayer();
	}

	public void SwitchToPlayer(FighterRole toPlayer)
	{
		for(int i=0;i<players.Length;i++)
		{
			if(players[i].fighterRole == toPlayer)
			{
				currentPlayer = players[i];
				currentPlayer.ActivatePlayer();
				return;
			}
		}
	}

	public bool MoveToNextStation()
	{
		if(isGameOver) return false;

		return currentPlayer.GetSneakyStationController().MoveToNextStation();
	}

	public bool MoveToNextStationManual()
	{
		if(isGameOver) return false;

		return currentPlayer.GetSneakyStationController().MoveToNextStation(null,true);
	}

	public SneakyPlayer GetCurrentPlayer()
	{
		return currentPlayer;
	}

    public SneakyPlayer GetSneakyPlayerByName(FighterName _name)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].fighterName == _name)
                return players[i];
        }

        Debug.LogError("No player by the asked name is assigned in player manager");
        return null;
    }

    public SneakyPlayer GetSneakyPlayerByRole(FighterRole role)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].fighterRole == role)
                return players[i];
        }

        Debug.LogError("No player by the asked role is assigned in player manager");
        return null;
    }

	public void SetStabTarget(AIPersonnel target)
	{
		if(target==null) Debug.LogError("Where the fuck is my target?????");
		nextTarget = target.transform;
		nextTargetPersonel = target;
		isStabPending = true;
	}

	public void ClearStabTarget()
	{
		nextTarget = null;
		nextTargetPersonel = null;
		isStabPending = false;

		//remove indicator
		IndicatorManager.instance.HideIndicator();
	}

	public Transform GetStabTarget()
	{
		if(isStabPending)
		{
			return nextTarget;
		}
		else
		{
			//Debug.Log("Illigal call made");
			return null;
		}
	}

	public bool IsStabKillingPending()
	{
		return isStabPending;
	}

	public bool CanWalk()
	{
		if(isGameOver) return false;

		return Work_SneakyMoveNextStationManual.canWalk || isUsingAiToFindPath;
	}

	public void Walk()
	{
		if(isGameOver) return;

		if(isUsingAiToFindPath)
		{
			currentPlayer.GetMovementController().SpeedUp();
		}
		else
		{
			if(Work_SneakyMoveNextStationManual.canWalk)
			{
				currentPlayer.GetSneakyStationController().SpeedUp();
			}
		}
	}

	public void SwitchToAIPathFindingWalk(Transform target, bool isManualWalk)
	{
		if(isGameOver) return;

		isUsingAiToFindPath = true;
		currentPlayer.GetMovementController().SetTarget(target==null?nextTargetPersonel.transform:target);

		if(isManualWalk)
			currentPlayer.GetMovementController().StartManualWalk();
		else
			currentPlayer.GetMovementController().StartAutoWalk();
	}

	public void AiPathFindingWalkDone()
	{
		isUsingAiToFindPath = false;
	}

	public void Stab()
	{
		if(isGameOver) return;
		currentPlayer.GetMovementController().walkSpeed = 0f;
		currentPlayer.StabEnemy(nextTarget,null);
	}

	public void GetBackToCover()
	{
		if(isUsingAiToFindPath)
		{
			currentPlayer.GetMovementController().GetBackToPreviousCover();
		}
		else
		{
			currentPlayer.GetSneakyStationController().GetBackToPreviousCover();
		}
	}

	public float GetNoiseLevel()
	{
		return _noiseLevel;
	}

	public void OnGameOver()
	{
		if(isGameOver) return;

		isGameOver = true;

		StartCoroutine(ShowAdd());
	}

	IEnumerator ShowAdd()
	{
		GeneralManager.EndGame(false, false);
		yield return new WaitForSeconds(2f*Time.timeScale);

        //show ad
        if (GeneralManager.adShowIndex == 1)
        {
            IronSourceManager.ShowSmartAd();
        }
        GeneralManager.adShowIndex++;
        if (GeneralManager.adShowIndex >= GeneralManager.adShowInterval)
        {
            GeneralManager.adShowIndex = 0;
        }
        /*
		//call mainmenu
		if(LocationFinder.GetCountry() == Country.Bangladesh)
		{
			if(!GeneralManager.instance.gmaInstance.WillShowInterStitialAdNow())
			{
				if(UnityEngine.Advertisements.Advertisement.IsReady())
				{
					//GoogleMobileAdsScript.unityAdsRunningTrigger =true;
					UnityEngine.Advertisements.Advertisement.Show();
				}
			}

		}
		else
		{
			if(UnityEngine.Advertisements.Advertisement.IsReady())
			{
				//GoogleMobileAdsScript.unityAdsRunningTrigger =true;
				UnityEngine.Advertisements.Advertisement.Show();
			}
			else{
				GeneralManager.instance.gmaInstance.WillShowInterStitialAdNow();
			}
		}
        */
	}

	// Update is called once per frame
	void Update () {

		CameraPanInput();

		CalculateNoiseLevel();

		CheckMinDistance();
	}

	private void CheckMinDistance()
	{
		HUDManager.instance.animateStabButton = false;
		if(nextTargetPersonel!=null)
		{
			if (!nextTargetPersonel.status.dead) 
			{
				Vector3 playerPos = currentPlayer.transform.position;
				Vector3 enemyPos = nextTargetPersonel.transform.position;

				float sq_distance = Vector3.SqrMagnitude (playerPos - enemyPos);

				//for indicator
				if (sq_distance < STAB_RANGE_SQ) {
					HUDManager.instance.animateStabButton = true;
					//HUDManager.TriggerToolTip(ToolTipType.Stab);
				} 

				//for alerting
				if (sq_distance < MIN_APPROACHING_DISTANCE) {
					nextTargetPersonel.Alert (panic: true, reason: AIPersonnel.AlertReason.TOUCHED);
					isGameOver = true;
					//Debug.LogWarning ("You are too close");
				}
			}
		}
	}

	public void CalculateNoiseLevel()
	{
		SneakyStationController ssc = currentPlayer.GetSneakyStationController();

		float distanceRatio = (MAX_AFFECTING_DISTANCE_SQ - GetSqDistanceFormTarget())/MAX_AFFECTING_DISTANCE_SQ;

		if(isUsingAiToFindPath)
			_noiseLevel = Mathf.Lerp(_noiseLevel, Mathf.Clamp( distanceRatio*currentPlayer.GetMovementController().GetSpeedRatio(),0f,1f),10f*Time.deltaTime);
		else
			_noiseLevel = Mathf.Lerp(_noiseLevel, Mathf.Clamp( distanceRatio*ssc.GetSpeedRatio(),0f,1f),10f*Time.deltaTime);

		//check noise level and alert enemy
//		if ( !currentPlayer.isStabing &&_noiseLevel > (MAX_ALLOWABLE_NOISE_LEVEL / 2) ) {
//			HUDManager.TriggerToolTip (ToolTipType.Noise);
//		}
		if(_noiseLevel>MAX_ALLOWABLE_NOISE_LEVEL && !currentPlayer.isStabing)
		{
			if (nextTargetPersonel != null) {
				if (nextTargetPersonel.GetComponent<AINoobRajakarPersonnel> () == null) {
					nextTargetPersonel.Alert (panic: true, reason: AIPersonnel.AlertReason.HEARD);
					isGameOver = true;
				}
			}
			//Debug.LogWarning ("You made too much noise");
		}
	}

	private float GetSqDistanceFormTarget()
	{
		if(nextTarget == null) return 100f;
		else
		{
			return Vector3.SqrMagnitude((nextTarget.position-currentPlayer.transform.position));
		}
	}

	void CameraPanInput()
	{

		if (Input.GetMouseButtonDown(0))
		{
			isMouseDragging = true;
		}

		if (Input.GetMouseButtonUp(0))
		{
			isMouseDragging = false;
			lastPos = Vector2.zero;
		}

		if (isMouseDragging && Time.timeScale>0.9f)
		{
			Vector2 thisPos = Input.mousePosition;

			if (lastPos == Vector2.zero) lastPos = thisPos;

			thisPos = thisPos - lastPos;
			thisPos *= (touchFactor*1f);

			if(isGameOver) return;

			SneakyCamera.instance.PanCamera(thisPos);

			lastPos = Input.mousePosition;

		}

		#if UNITY_EDITOR
		if(Input.GetKeyDown(KeyCode.B))
		{
			GetBackToCover();
		}

		#endif
	}
}
