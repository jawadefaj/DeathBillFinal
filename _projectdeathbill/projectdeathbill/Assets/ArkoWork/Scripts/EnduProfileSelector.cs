using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnduProfileSelector : MonoBehaviour {

    private static int totalSelected = 0;

    public FighterName fighter;

    //component
    public RectTransform root;
    public Button profileBtn;
    public Button otherPair;
    public GameObject portraitBorder;
    public Image portrait; 
    public GameObject glow;
    public GameObject nameDisplay;
    public GameObject parameters;
    public Text posText;
    public Text fighterNameTxt;
    public Text gunNameTxt;
    public Image damageImg;
    public Image accurecyImg;
    public Image rangeImg;
    public Image fireRateImg;
    public Text clipSizeTxt;

    private FighterProfile fProfile;
    private float defY;
	
    void Awake()
    {
        defY = root.anchoredPosition.y;
    }

	void Start () {
        fProfile = FighterProfileDataHolder.instance.GetFighterProfile(fighter);

        Image profileImage = profileBtn.targetGraphic as Image;
        profileImage.sprite = fProfile.proPic;

        ChangeState(false);
	}

    void OnEnable()
    {
        profileBtn.onClick.AddListener(OnProfileBtnClicked);
    }

    void OnDisable()
    {
        profileBtn.onClick.RemoveListener(OnProfileBtnClicked);
    }
	
    void OnProfileBtnClicked()
    {
        ChangeState(true);
    }

    public void ChangeState(bool toSelected)
    {
        if (toSelected)
        {
            PlayerSelectionWork();
            root.anchoredPosition = new Vector2(root.anchoredPosition.x, defY);
        }
        else
        {
            root.anchoredPosition = new Vector2(root.anchoredPosition.x, defY-96);
        }

        profileBtn.gameObject.SetActive(!toSelected);
        otherPair.gameObject.SetActive(!toSelected);

        portraitBorder.SetActive(toSelected);
        portrait.gameObject.SetActive(toSelected);
        glow.SetActive(toSelected);
        nameDisplay.SetActive(toSelected);
        parameters.SetActive(toSelected);

        //change text color
        posText.color = toSelected?Color.green:Color.red;

        //change total selecxt
        ChangeTotalSelected(toSelected);
    }

    private void PlayerSelectionWork()
    {
        //update staffs
        portrait.sprite = fProfile.portrait;
        fighterNameTxt.text = fProfile.displayName;
        gunNameTxt.text = fProfile.gun.name;
        damageImg.fillAmount = fProfile.gun.damage;
        accurecyImg.fillAmount = fProfile.gun.accurecy;
        rangeImg.fillAmount = fProfile.gun.range;
        fireRateImg.fillAmount = fProfile.gun.fireRate;
        clipSizeTxt.text = fProfile.gun.clipSize.ToString();

        //tell endu manager who is selected
        EnduranceManager.instance.SelectPlayer(EnduranceManager.instance.GetPlayerPostID(fighter),fighter);
    }

    private void ChangeTotalSelected(bool isSelected)
    {
        int value = isSelected ? 1 : -1;

        totalSelected = Mathf.Clamp(totalSelected + value, 0, 3);

        if (totalSelected == 3)
            EnduranceManager.instance.ChangeGoButtonStatus(true);
        else
            EnduranceManager.instance.ChangeGoButtonStatus(false);
    }
}
