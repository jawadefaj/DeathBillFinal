using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Portbliss.Station;

public class PlayerInputController : MonoBehaviour {

	public static PlayerInputController instance;

	public ThirdPersonController[] players;
	//[HideInInspector]
	public List<ThirdPersonController> aiPlayers = new List<ThirdPersonController>();
    public RectTransform hudRect;
    public UnityEngine.UI.Text DebugText;

	private Camera main_Camera;
	public ThirdPersonController current_player;
	private Weapon weapon;
	internal int index;
	internal bool isAiming = false;
    private bool isTargetSetDone = true;

	private float touchFactor = 0;
    void Awake()
    {
		instance = this;
    }

	void Start () {
		main_Camera = Camera.main;
		touchFactor = 60.0f / Screen.height;
	
		//initialize player
		//SwitchToPlayer(0);
        //AIPersonnel.targetPlayer = current_player.GetTargetReference();
        index = 0;
        aiPlayers.Clear();

	}
    private bool isZoomedIn = false;

    private bool isMouseDragging = false;
    Vector3 lastPos = Vector3.zero;
    Vector3 thisPos = Vector3.zero;
    Vector2 lastTPos = Vector2.zero;
    Vector2 thisTPos = Vector2.zero;
    Vector2 deltaTPos = Vector2.zero;
	bool isFingerLocked = false;
	int fingerID = 0;

    void Update()
    {
		PlayerAimControlUpdate();

	#if UNITY_EDITOR
		if(Input.GetKeyDown(KeyCode.K))
		{
            GetPlayerByRole(FighterRole.Sniper).TakeImapct(new RaycastHit(),1000,HitSource.ENEMY);
		}

        if (Input.GetMouseButtonDown(1))
        {
            if(CanCurrentPlayerScope())
            {
                if(isAiming) 
                    GUI_SetCurrentPlayerState(PlayerState.Hide);
                else
                    GUI_SetCurrentPlayerState(PlayerState.Scope);
            }
            else
            {
                if(isAiming) 
                    GUI_SetCurrentPlayerState(PlayerState.Hide);
                else
                    GUI_SetCurrentPlayerState(PlayerState.Aim);
            }

        }

        if (Input.GetKeyDown(KeyCode.R))
		{
			GUI_Reload();
		}
        
        if (Input.GetKeyDown(KeyCode.G))
		{
			GUI_Granade ();
		}

        if (Input.GetKey(KeyCode.Space)||RapidFireButton.instance.pressedOn)
        {
            GUI_Shoot();
        }

		if(Input.GetKeyDown(KeyCode.N))
		{
			GUI_MoveNextStation();
		}

		if(Input.GetKeyDown(KeyCode.LeftBracket))
		{
			GUI_MovePrevCoverPoint();
		}

		if(Input.GetKeyDown(KeyCode.RightBracket))
		{
			GUI_MoveNextCoverPoint();
		}

		if(Input.GetKeyDown(KeyCode.S))
		{
			SwitchPlayer();
		}

        if(Input.GetKeyDown(KeyCode.C))
        {
            GUI_CoverFire(5);
        }
			
	#endif

    }

    public void PlayerAimControlUpdate()
    {
#if UNITY_EDITOR

        if (Input.GetMouseButtonDown(0))
        {
            isMouseDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseDragging = false;
            lastPos = Vector3.zero;
        }

		if (isMouseDragging && Time.timeScale>0.9f)
        {
            thisPos = Input.mousePosition;

            if (lastPos == Vector3.zero) lastPos = thisPos;
			current_player.RotateCameraHorizontal(GetNonLinearInput( (thisPos.x - lastPos.x)  *touchFactor)*UserSettings.Sensivity);
			current_player.RotateCameraVertical(GetNonLinearInput( (thisPos.y - lastPos.y)  *touchFactor )*UserSettings.Sensivity);

            lastPos = thisPos;

        }
#endif
#if UNITY_ANDROID || UNITY_IOS
        //for touch

        if (Input.touchCount > 0 && Time.timeScale>0.9f)
        {  
            for (int i = 0; i < Mathf.Min(2,  Input.touchCount); i++)
            {
                Touch t = Input.touches[i];

                //Touch t = Input.touches[0];
                if (!isFingerLocked && t.phase == TouchPhase.Began)
                {
                    if (!RectTransformUtility.RectangleContainsScreenPoint(hudRect, t.position))
                    {
                        isFingerLocked = true;
                        fingerID = t.fingerId;
                    }
                }

                if (isFingerLocked && fingerID == t.fingerId)
                { 
                    if (t.phase == TouchPhase.Moved)
                    {
                        thisTPos = t.position;
                        if (lastTPos == Vector2.zero) { lastTPos = thisTPos; }
                        deltaTPos = thisTPos - lastTPos;

						//if(Mathf.Abs(deltaTPos.x)> Screen.width*0.6f) return;

                        current_player.RotateCameraHorizontal(GetNonLinearInput(deltaTPos.x *touchFactor) * UserSettings.Sensivity);
                        current_player.RotateCameraVertical(GetNonLinearInput(deltaTPos.y *touchFactor) * UserSettings.Sensivity);

                        lastTPos = thisTPos;
                    }

					else if (t.phase == TouchPhase.Ended)
					{
						isFingerLocked = false;
						lastTPos = Vector2.zero;
					}
                }
            }
        }
        else
        {
            isFingerLocked = false;
            lastTPos = Vector2.zero;
        }

#endif
    }

   
    private float GetNonLinearInput(float linearInput)
    {

        if(linearInput>=0)
            return Mathf.Pow(linearInput, 1.45f);
        else
            return -Mathf.Pow(-linearInput, 1.45f);
    }

	public PlayerAI GetAiPlayer(FighterRole id)
	{
		foreach(ThirdPersonController tpc in aiPlayers)
		{
			if (tpc.fighterRole == id) {
				return tpc.GetAI ();
			}
		}

		return null;
	}

    public ThirdPersonController GetPlayerByID(FighterName id)
    {
        for (int i = 0; i < players.Length; i++) {
            if (players [i].fighterName == id) {
                return players [i];
            }
        }
        return null;
    }

	public ThirdPersonController GetPlayerByRole(FighterRole id)
	{
		for (int i = 0; i < players.Length; i++) {
			if (players [i].fighterRole == id) {
				return players [i];
			}
		}
		return null;
	}


    public void SwitchPlayer()
	{
		int counter = 0;
		do
		{
	        index++;
			counter++;

	        if (index == players.Length)
	            index = 0;

			//check for death
			if(players[index].IsDead() == false)
			{
				GM_SwitchToPlayer(players[index].fighterRole);
				return;
			}
		}while(counter<5);

		GameOver();

	}
	private static bool savedGlobalZoomValueWasUsedFlag = false;
	private static float savedGlobalZoomValue;

	public void GM_SwitchToPlayer(FighterRole id)
	{
		HUDManager.ClearSelectablePlayerAnimStates ();
		for(int i=0;i<players.Length;i++)
		{
			if(players[i].fighterRole==id)
			{
                //if(players[i].assignedWeapon.canZoom) 
                    ImprovedCameraCntroller.instance.SetZoomCameraValue(players[i].assignedWeapon.personalZoomValue);
				SwitchToPlayer(i,true);

				return;
			}
		}

		Debug.LogError("The fighter does not exist");
	}

    public void GUI_SwitchToPlayer(FighterRole id)
    {
        HUDManager.ClearSelectablePlayerAnimStates ();
        for(int i=0;i<players.Length;i++)
        {
            if(players[i].fighterRole==id)
            {
                //if(players[i].assignedWeapon.canZoom) 
                ImprovedCameraCntroller.instance.SetZoomCameraValue(players[i].assignedWeapon.personalZoomValue);
                SwitchToPlayer(i,false);

                return;
            }
        }

        Debug.LogError("The fighter does not exist");
    }

	private void SwitchToPlayer(int index, bool mustSwitch = false)
	{
		if(!mustSwitch)
		{
            if(current_player!=null)
            {
               if(current_player.IsThrowingGranade()) return;
            }

            //if (isCoverFireOn)
            //    return;
        }
		if(players[index].IsDead()) return;
    

        //stop camera shake
        //ShakeCamera.instance.StopCameraShake();

        if (current_player != null)
		{
            current_player.SetControll(false);
            if (isCoverFireOn)
                current_player.AttachAI(false);
		}

		current_player = players[index];
        current_player.SetControll(true);
        if (isCoverFireOn)
            current_player.RemoveAI();

		//main_Camera.transform.parent = players[index].transform;
		weapon = current_player.assignedWeapon;
       

		if(current_player.canSwitchBetweenCameras)
		{
			players[index].SwitchToHeadCameraView();
			isAiming = false;
		}
		else
		{
			players[index].SwitchToShoulderCameraView();
			isAiming = true;
		}

        //set tergat
        isTargetSetDone = false;
        StartCoroutine(SwitchAgroToPlayerAfter(3.5f));
		//reset camera
        ImprovedCameraCntroller.instance.ResetFieldOfView();
		//set index
		this.index = index;

    }

    IEnumerator SwitchAgroToPlayerAfter(float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);
		if (AIDataManager.instance.zonalTargetingCurrentPlayerPreference !=1) {
			AIDataManager.UrgentTargetUpdates (1);
		}
        //AIPersonnel.targetPlayer = current_player.GetTargetReference();
    }

	public void GameOver()
	{
		Debug.LogError("Game over bitch!");
		//Time.timeScale =0;
	}

	public Transform GetPlayerTargetPoint(FighterRole id)
	{
		for(int i=0;i<players.Length;i++)
		{
			if(players[i].fighterRole == id)
				return players[i].GetTargetReference();
		}

		for(int i=0;i<aiPlayers.Count;i++)
		{
			if(aiPlayers[i].fighterRole == id)
				return aiPlayers[i].GetTargetReference();
		}

        Debug.LogError("No such player exist in player controller. Requested Role ID "+ id.ToString());
		return null;
	}

	public List<int> GetOffLinePlayerIndex()
    {
		List<int> availablePlayers = new List<int>();

		if(index!=0 && !players[0].IsDead()) availablePlayers.Add(0);
		if(index!=1 && !players[1].IsDead()) availablePlayers.Add(1);
		if(index!=2 && !players[2].IsDead()) availablePlayers.Add(2);
		       
		return availablePlayers;
    }

    public bool CanCurrentPlayerScope()
    {
        return current_player.assignedWeapon.hasScope;
    }

    public bool IsCurrentPlayerScoping()
    {
        return current_player.isScopeOn;
    }

	/*public bool IsAiming()
	{
		return current_player.GetAnimator().GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("Aim");
	}*/

	//====================================Controll Functions===================================


    public void GUI_Shoot()
    {
        if(ImprovedCameraCntroller.instance.IsPrimaryMovementOn()) return;

		if (!isAiming)
        {
            if (current_player.assignedWeapon.hasScope)
            {
                GUI_SetCurrentPlayerState(PlayerState.Scope);
            }
            else
            {
                current_player.SwitchToShoulderCameraView();
                isAiming = true;
                return;
            }

        }
        if (isTargetSetDone == false)
        {
            //AIPersonnel.targetPlayer = current_player.GetTargetReference();
            isTargetSetDone = true;
        }
        weapon.Fire();
    }

    public void GUI_Reload()
    {
        if(ImprovedCameraCntroller.instance.IsPrimaryMovementOn()) return;
        current_player.Reload();
    }

    public void GUI_Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

	public void GUI_Granade()
	{
		current_player.ThrowGranade();
		isAiming = false;
	}

	public bool GUI_MoveNextStation()
	{
		if(current_player.IsReloading() || current_player.IsShooting() || current_player.IsThrowingGranade())
			return false;

		isAiming = false;
        current_player.SetPlayerScope(false);
		
		StationController sc = current_player.GetStationController();
		if(sc!=null)
            return sc.MoveToNextStation(null,false,(bool isSuccess)=>{
                if(isSuccess)
                    current_player.SwitchToHeadCameraView();
            });
		else
		{
			Debug.LogError("No station controller attached");
			return false;
		}
	}

	public bool GUI_MoveNextStationManual()
	{
		if(current_player.IsReloading() || current_player.IsShooting() || current_player.IsThrowingGranade())
			return false;

		isAiming = false;
        current_player.SetPlayerScope(false);

		StationController sc = current_player.GetStationController();
		if(sc!=null)
			return sc.MoveToNextStation(null,true);
		else
		{
			Debug.LogError("No station controller attached");
			return false;
		}
	}

	public void GUI_MoveNextCoverPoint()
	{
		if(current_player.IsShooting() || current_player.IsThrowingGranade())
			return;

		if(current_player.IsReloading()) current_player.CancelReload();

		StationController sc = current_player.GetStationController();
		if(sc!=null)
		{
			sc.MoveToNextMovePoint();
            current_player.SetPlayerScope(false);
		}
	}

	public void GUI_MovePrevCoverPoint()
	{
		if(current_player.IsShooting() || current_player.IsThrowingGranade() )
			return;

		if(current_player.IsReloading()) current_player.CancelReload();


		StationController sc = current_player.GetStationController();
		if(sc!=null)
		{
			sc.MoveToPrevMovePoint();
            current_player.SetPlayerScope(false);
		}
	}

	public void GUI_SpeedUp()
	{
		instance.current_player.GetStationController ().SpeedUp ();
	}


    public void GUI_SetCurrentPlayerState(PlayerState state)
    {
        if (current_player.GetStationController().IsMoving())
        {
            Debug.LogWarning("Player is moving. Can not perform action");
            return;
        }

        if (state == PlayerState.Scope)
        {
            current_player.SetPlayerScope(true);
            isAiming = true;
            return;
        }
        else
        {
            if (current_player.canSwitchBetweenCameras == false)
            {
                Debug.LogWarning("The player does not has option to switch between cameras");
                return;
            }
            else
            {
                if (isAiming && state == PlayerState.Aim)
                {
                    if (current_player.isScopeOn)
                    {
                        current_player.SetPlayerScope(false);
                    }
                    else
                    {
                        Debug.LogWarning("Player is already aiming");
                    }

                    return;
                }
                else if (!isAiming && state == PlayerState.Hide)
                {
                    Debug.LogWarning("Player is already hiding");
                    return;
                }
                else
                {
                    if (isAiming == false)
                        current_player.SwitchToShoulderCameraView();
                    else
                        current_player.SwitchToHeadCameraView();

                    isAiming = !isAiming;
                    current_player.SetPlayerScope(false);
                    //Debug.Log("scope false set");
                }
            }
        }
    }

    private bool isCoverFireOn = false;
    private float coverFireTotalTime = 30f;
    private float coverFireRemainingTime = 0;
    public void GUI_CoverFire(float duration)
    {
        if (!isCoverFireOn)
        {
            isCoverFireOn = true;
            coverFireTotalTime = duration;
			//InGameSoundManagerScript.instance.ForAmericaShoutAll ();
            StartCoroutine("StartCoverFire");
        }
    }

    IEnumerator StartCoverFire()
    {
        isCoverFireOn = true;

        //attch ai
        for (int i = 0; i < players.Length; i++)
        {
            if (current_player != players[i])
            {
                players[i].AttachAI(false);
            }
        }

            coverFireRemainingTime = coverFireTotalTime;

            do
            {
                yield return null;
                coverFireRemainingTime-=Time.deltaTime;
            } while(coverFireRemainingTime > 0);

        //remove ai
        for (int i = 0; i < players.Length; i++)
        {
            if (current_player != players[i])
            {
                players[i].RemoveAI();
            }
        }

        isCoverFireOn = false;
    }

    public bool IsCoverFireOn()
    {
        return isCoverFireOn;
    }

    public float GetCoverFirePercent()
    {
        float done = coverFireTotalTime - coverFireRemainingTime;
        return done / coverFireTotalTime;
    }

}

public enum PlayerState
{
    Hide=0,
    Aim=1,
    Scope=2,
}
