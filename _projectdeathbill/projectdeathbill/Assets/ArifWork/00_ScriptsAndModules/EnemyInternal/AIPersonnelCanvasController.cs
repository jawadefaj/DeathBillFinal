using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AIPersonnelCanvasController : MonoBehaviour {

    Transform mainCamTransform;
    public RectTransform HPBar;
    public RectTransform HPBarParent;
    public RectTransform NadeAlert;
	public GameObject targetIcon;
    public Color fullHPColor;
    public Color noHPColor;


	Image HPBarImage; 
	Image HPBarParentImage;
    Image nadeAlertImage;
    
	AIPersonnel ai;

    Vector3 baseScale;
    Camera mCam;

    void Start()
    {
        baseScale = this.transform.localScale;  
    }
    void OnEnable()
    {
//        if (GeneralManager.instance.runningForPromo) {
//            gameObject.SetActive(false);
//            return;
//        }
		ai = transform.parent.GetComponent<AIPersonnel> ();
		mainCamTransform = GeneralManager.instance.camTrans;
		if(HPBarImage == null )HPBarImage = HPBar.GetComponent<Image>();
		if(HPBarParentImage == null )HPBarParentImage = HPBarParent.GetComponent<Image>();
        if (NadeAlert != null)
        {
            NadeAlert.gameObject.SetActive(false);
            nadeAlertImage = NadeAlert.GetComponent<Image>();
        }
       
    }
        
    void LateUpdate() {
        if(mainCamTransform != null)
        {
            transform.rotation = mainCamTransform.rotation;

            float M = 1;
            if (Camera.main != null)
            {
                float Rx = Vector3.Distance(transform.position,Camera.main.transform.position);
                float Ms = 1;
                if(Rx> AIDataManager.instance.uiZooomStandardDistance)
                    Ms = (Camera.main.fieldOfView * Rx) / (AIDataManager.instance.uiZooomStandardDistance * 60);
                M = Mathf.Lerp(1,Ms,AIDataManager.instance.uiZoomRealness); 
            }
            transform.localScale = baseScale*M;
        }
            
    }
    
    Vector2 tempVec2 = new Vector2();
    public void UpdateHP(float remainingHPPercentage)
    {
		if (remainingHPPercentage <= 0) { HPBarParent.gameObject.SetActive(false); return; }
		else { 
			if (ai != null) {
				if(ai.enemyType != EnemyType.RAJAKAR)
				HPBarParent.gameObject.SetActive (true);
			}
		}
//        tempVec2 = HPBar.sizeDelta;
//        tempVec2.x = remainingHPPercentage * HPBarParent.rect.width;
//        HPBar.sizeDelta = tempVec2;
//        tempVec2 = HPBar.anchoredPosition;
//        tempVec2.x = HPBar.sizeDelta.x / 2;
//        HPBar.anchoredPosition = tempVec2;

		HPBarImage.fillAmount = remainingHPPercentage;
		HPBarParentImage.fillAmount = 1 - remainingHPPercentage;
        Color col = Color.Lerp(noHPColor, fullHPColor,remainingHPPercentage);
        HPBarImage.color = col;

    }

    
    public void TurnOnNadeAlert()
    {
        NadeAlert.gameObject.SetActive(true);
        StartCoroutine("BlinkNadeAlertColor");

    }
    public void TurnOffNadeAlert()
    {
        NadeAlert.gameObject.SetActive(false);
        StopCoroutine("BlinkNadeAlertColor");
    }
    float blinkPeriod = 0.2f;
    IEnumerator BlinkNadeAlertColor()
    {
        while (true)
        {
            nadeAlertImage.color = Color.white;
            yield return new WaitForSeconds(blinkPeriod);
            nadeAlertImage.color = Color.red;
            yield return new WaitForSeconds(blinkPeriod);
        }
    }
}
