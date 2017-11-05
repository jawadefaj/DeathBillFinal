using UnityEngine;
using Portbliss.Station;
using System.Collections;
using System.Collections.Generic;

public class PlayerAI : MonoBehaviour {

	public bool useViewAngle = true;
	[Tooltip("Use full of the view angle")]
	public float viewAngle = 90f;
    public LayerMask rayLayer;

    //private AIPersonnel PF_enmPrsnl;
    private AIPersonnel currentTargetAI;
    private Transform targetPoint
    {
        get
        {
            if (currentTargetAI == null)
                return null;
            else if(currentTargetAI.status.dead)
            {
                currentTargetAI = null;
                return null;
            }
            else
                return currentTargetAI.headReference;
        }
    }


	private bool isAIOn = true;
	private ThirdPersonController playerController;
	private StationController sc;
	private bool moveToNextStation = false;
	private bool ceaseFire = false;
	private float nextCoverChange = 3f;
    private bool hasPermissionToShoot
    {
        get
        {
            return AIDataManager.areAllEnemiesAlert;
        }
    }

	private float fireTime =0;

	void OnEnable () {
        rayLayer = LayerMask.GetMask(new string[] {"Obstackle"});
		sc = this.GetComponent<StationController>();
		playerController = this.GetComponent<ThirdPersonController>();
	}

	void Update()
	{
        if (playerController.IsDead())
            return;
        
        bool badAI = (playerController.fighterName == FighterName.Hillary || playerController.fighterName == FighterName.Trump);
        badAI = false;
		if(isAIOn)
		{
            if (!IsTargetValid(currentTargetAI))
            {
                TryToAssignNewTarget();         //if fails to get one it will be null
            }
			


			if(moveToNextStation)
			{
				if(sc!=null)
					moveToNextStation = !sc.MoveToNextStation();
				return;
			}

            if (IsReadyToFire() && currentTargetAI != null && !ceaseFire && !badAI)
            {
                //playerController.AILookAt(target.position);
                playerController.assignedWeapon.AIFire(targetPoint);

                //to keep us on aim position
                //playerController.GetAnimator().SetBool("aim", true);
            }
            else
            {
                //go to hide
                //playerController.GetAnimator().SetBool("aim", false);
            }

		}

		if(Input.GetKeyDown(KeyCode.KeypadEnter))
			MoveStation();
	}

    #region AI target validity and selection
    enum TargetValidity
    {
        NULL_REF,
        DEAD,
        RANGE,
        BLOCKED,
        ANGLE,
        VALID
    }
    private TargetValidity GetTargetValidity(AIPersonnel targetAi)
    {
        if (targetAi == null)
        {
            return TargetValidity.NULL_REF;
        }
        else if (targetAi.status.dead)
        {
            return TargetValidity.DEAD;
        }

        Transform aimSpot = targetAi.headReference;
        Vector3 fromPoint = this.transform.position;
        Vector3 toPoint = aimSpot.position;
        fromPoint.y += 1f;
        Vector3 direction = (toPoint-fromPoint).normalized;

        float maxAllowDistance = playerController.assignedWeapon.AIMaxShootingRangeSq;
        if(Vector3.SqrMagnitude(toPoint-fromPoint)>maxAllowDistance)
        {
            return TargetValidity.RANGE;
        }

        if(Physics.Raycast(fromPoint,direction,maxAllowDistance,rayLayer))
        {
            return TargetValidity.BLOCKED;
        }

        if (useViewAngle)
        {
            float internalAngle = 0;

            Vector3 playerForward = playerController.gameObject.transform.forward;
            Vector2 fwd = new Vector2(playerForward.x, playerForward.z);
            Vector2 dir = new Vector2(direction.x, direction.z);
            internalAngle = Vector2.Angle(fwd, dir);

            if (internalAngle > viewAngle / 2f)
            {
                return TargetValidity.ANGLE;
            }
        }

        return TargetValidity.VALID;
    }
    private bool IsTargetValid(AIPersonnel enemyAI)
    {
        if (GetTargetValidity(enemyAI) == TargetValidity.VALID)
            return  true;
        else
            return false;
    }
	private void TryToAssignNewTarget()
	{
        if (!hasPermissionToShoot)
        {
            currentTargetAI = null;
            return;
        }

        List<AIPersonnel> enemyList = AIDataManager.nonMainEnemyList;
        if (enemyList.Count == 0)
            enemyList = AIDataManager.activeEnemyList;
            
        for (int i = 0; i < enemyList.Count; i++)
        {
            AIPersonnel aip = enemyList[i];
            if (IsTargetValid(aip))
            {
                currentTargetAI = aip;
                return;
            }
            else
            {
                continue;
            }
        }
        currentTargetAI = null;
	}
    #endregion

	public void SwitchFire(bool isOn)
	{
		ceaseFire = !isOn;
	}

	public bool IsReadyToFire()
	{
		if(sc!=null)
		{
			return !sc.IsMoving();
		}

		return false;
	}

	public void MoveStation()
	{
		moveToNextStation = true;
	}

    public bool IsAIOn()
    {
		return isAIOn;
    }

    public Transform GetAICurrentTarget()
    {
        return targetPoint;
    }
}
