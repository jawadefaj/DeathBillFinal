using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public struct WeaponButtonContainer
{
    public Button weapon;
    public Button[] subWeapons;
    public GameObject subWeaponContainer;
    public GameObject weaponAdd;
    public Sprite openSprite;
    public Sprite closeSprite;
    public Sprite buySprite;
}

public class WeaponLoader : MonoBehaviour {

    public WeaponButtonContainer[] weaponList;
    [Space]
	public GameObject baseContainer;
	public GameObject weaponContainer;
	public GameObject[] staticGroup;
	public Sprite acquiredGunImg;
	public Sprite notAcquiredGUnImg;
    [Space]
    [Header("Optional Player Section")]
    public GameObject optionalPlayerChoiceWindow;

    [HideInInspector][SerializeField]
    public string modifiedGameData = "";
    [HideInInspector][SerializeField]
    public bool useModifiedGameData = false;

	private int weaponChoosed;
	private int subWeaponChoosed;
	private bool isManadatoryToPlayPrevLvlBeforeBuy = true;

	public static bool loadOnStart = false;

	// Use this for initialization
	void Start () {

		//for test
		//UserGameData.LoadGame("");


		LevelManager.Initialize();
		//UpdateWeaponChoice();

		if(loadOnStart)
		{
			loadOnStart = false;
        	OpenWeaponBox();         
		}	

        #if UNITY_EDITOR
        if(useModifiedGameData)
            UserGameData.instance.ManipulateData(modifiedGameData);
        #endif
	}

	public void OpenWeaponBox()
	{
		baseContainer.SetActive(false);
		weaponContainer.SetActive(true);
		UpdateWeaponChoice();
	}

	public void AccuireSubWeapon(int no)
	{
		//string key = string.Format("{0}{1}{2}",baseWeaponName,weaponCatagoryName[weaponChoosed-1],weaponSubCatagoryName[no-1]);

        //only for level 2A saving functionality reset
        Work_Level2A_LoadFromSave.startFrom = 5;

		if (IsLoaded (weaponChoosed-1,no-1)) 
        {
            Portbliss.LevelManagment.CheckPoint cp = LevelManager.GetLevel(weaponChoosed - 1).GetCheckPoint(no - 1);
            if (cp.hasOptionalPlayer)
            {
                //show optional player choose option
                weaponList[weaponChoosed - 1].subWeaponContainer.SetActive(false);
                optionalPlayerChoiceWindow.SetActive(true);
                subWeaponChoosed = no;
            }
            else
            {
                LevelID id = (LevelID)(weaponChoosed - 1);
                LevelManager.LoadLevel (id, no - 1);
            }
			
		} 
        else 
        {
			Debug.LogError ("can not load");
		}
	}

    //when optional player has been choosed
    public void OnOptionalPlayerChoosed(int no)
    {
        //value must be 0 or 1?
        if(no==0) 
            LevelManager.OptionalPlayer = FighterName.Trump;
        else if (no == 1)
            LevelManager.OptionalPlayer = FighterName.Hillary;

        //simple loading
        LevelID id = (LevelID)(weaponChoosed - 1);
        LevelManager.LoadLevel (id, subWeaponChoosed - 1);
    }



	//To buy a level
	public void EarnWeapon(int no)
	{
		//TODO 
		//Attach the game buy code here. Then accuire the stage
		UserGameData.instance.AccuireStage (no);
		UpdateWeaponChoice ();
        Debug.Log("Level purchased");
        Debug.Log(UserGameData.instance.GetGameData());
        weaponList[weaponChoosed - 1].weaponAdd.SetActive(false);
		weaponContainer.SetActive (true);
	}
        
	public void AccuireWeapon(int no)
	{
		Debug.Log (UserGameData.instance.GetLevelStatus (no));

        if(LevelManager.GetLevel (no-1).IsMagna())
        {
            if (UserGameData.instance.GetLevelStatus (no) == LevelStatus.Ready) 
            {
                weaponContainer.SetActive (false);
                weaponList[no - 1].subWeaponContainer.SetActive(true);
                UpdateSubWeaponChoice ();
                weaponChoosed = no;
            }
        }
        else
        {
            if (UserGameData.instance.GetLevelStatus (no) == LevelStatus.Bought) {
                weaponContainer.SetActive (false);
                weaponList[no - 1].subWeaponContainer.SetActive(true);
                UpdateSubWeaponChoice ();
                weaponChoosed = no;
            } 
            else 
            {
                //go to advertise window
                weaponContainer.SetActive (false);
                weaponList[no - 1].weaponAdd.SetActive(true);
                ChangeStaticGroupState (false);
                weaponChoosed = no;
            }
        }

	}

	private void UpdateWeaponChoice()
    {
        for (int i = 0; i < weaponList.Length; i++)
        {
            LevelStatus lvlStat = UserGameData.instance.GetLevelStatus (i+1);
            Button weaponBtn = weaponList[i].weapon;

            if (lvlStat == LevelStatus.Ready) 
            {
                if (LevelManager.GetLevel (i).IsMagna ()) 
                {
                    weaponBtn.image.sprite = weaponList[i].openSprite;
                    weaponBtn.interactable = true;
                } else 
                {
                    //show buy level icon
                    weaponBtn.image.sprite = weaponList[i].buySprite;
                    weaponBtn.interactable = true;
                }

            } 
            else if (lvlStat == LevelStatus.Bought) 
            {
                weaponBtn.image.sprite = weaponList[i].openSprite;
                weaponBtn.interactable = true;
            }
            else  
            {
                weaponBtn.image.sprite = weaponList[i].closeSprite;
                weaponBtn.interactable = false;
            }
        }     
	}

	private void UpdateSubWeaponChoice()
	{
		string key;

        for (int i = 0; i < weaponList.Length; i++)
        {
            for (int j = 0; j < weaponList[i].subWeapons.Length; j++)
            {
                if (IsLoaded(i, j))
                {
                    weaponList[i].subWeapons[j].interactable = true;
                    weaponList[i].subWeapons[j].GetComponentInChildren<Text>().text = (j+1).ToString();
                    weaponList[i].subWeapons[j].image.sprite = acquiredGunImg;
                    Debug.Log("1 button activated");
                }
                else
                {
                    weaponList[i].subWeapons[j].interactable = false;
                    weaponList[i].subWeapons[j].GetComponentInChildren<Text>().text = "";
                    weaponList[i].subWeapons[j].image.sprite = notAcquiredGUnImg;
                    Debug.Log("1 button deactivated");
                }
            }
        }
            
	}

	private bool IsLoaded(int lvl, int chk)
	{
		return UserGameData.instance.IsCheckPointUnlocked(lvl,chk);
	}

	public void GoBack()
	{
        for (int i = 0; i < weaponList.Length; i++)
        {
            if (weaponList[i].subWeaponContainer.activeSelf)
            {
                weaponList[i].subWeaponContainer.SetActive(false);
                weaponContainer.SetActive(true);
                return;
            }

            if (weaponList[i].weaponAdd != null)
            {
                if (weaponList[i].weaponAdd.activeSelf)
                {
                    weaponList[i].weaponAdd.SetActive(false);
                    weaponContainer.SetActive(true);
                    return;
                }
            }
        }

		if (weaponContainer.activeSelf)
		{
			weaponContainer.SetActive(false);
			baseContainer.SetActive(true);
		}

        if (optionalPlayerChoiceWindow.activeSelf)
        {
            optionalPlayerChoiceWindow.SetActive(false);
            weaponList[weaponChoosed - 1].subWeaponContainer.SetActive(true);
        }
	}

	public void ChangeStaticGroupState(bool active)
	{
		foreach(GameObject g in staticGroup)
			g.SetActive(active);
	}

	public static void ClearWeapon(bool giveThisBooleanAValueForNoReason)
	{
		//Debug.LogError("Call came from "+ (comingFromGame==true?"Game":"Cine"));

		string hash = LevelManager. Clear();

		if(string.IsNullOrEmpty(hash))
		{
			//no action
			//Debug.Log("No hash value came.");
		}
		else
		{
			if(string.Equals(hash,"m"))
			{
				SceneLoader.LoadScene(GameConstants.mainMenu);
			}
			else if (string.Equals(hash,"l"))
			{
				//go to main menu with buying option
				loadOnStart = true;
				SceneLoader.LoadScene(GameConstants.mainMenu);
			}
			else
			{
                LevelManager.LoadLevel(hash);
			}

		}

	}
		
}
