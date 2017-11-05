using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
public class SceneTwelveScript : MonoBehaviour {

	public GameObject kabir;
    public GameObject kabirDark;
    public GameObject shamsu;
    public GameObject shamsuDark;
    public GameObject anila;
   // public GameObject bg;
    public GameObject dialogueBar;
    public GameObject zeep;
    public Text dialogueText;
    public GameObject namebar;
	public Text name;
	public Text name2;
    private string temp;
    private string temp1 = "";
    private float delay = 1.2f;
    AudioSource audio;
   // private float textSpeed = 3f;
    
	void Start () {
		DOTween.Init();

        audio = GetComponent<AudioSource>();
        if(UserSettings.SoundOn)
        {
            audio.Play();
        }
        else{
            audio.Stop();
        }

		StartCoroutine(handleTransition());

		//this.transform.DORotate(new Vector3(0,0,0), .5f);
		//dialogueBar.transform.DORotate(new Vector3(0,0,0), .5f);
		//samsu.transform.DOScale(new Vector3(.9f,.9f,.9f),2f);
	}

	IEnumerator handleTransition()
	{
        yield return new WaitForSeconds(.5f);
        //bg.gameObject.GetComponent<Transform>().DOScale(new Vector3(2,2,0), 1f);
        zeep.gameObject.GetComponent<Transform>().DOLocalMove(new Vector3(150f, -224f, 0f), 2f);
        yield return new WaitForSeconds(2.2f);
        kabir.gameObject.GetComponent<Image>().DOFade(255,.7f);
        yield return new WaitForSeconds(.3f);
        this.transform.DORotate(new Vector3(0,0,0), .5f);
        dialogueBar.transform.DORotate(new Vector3(0,0,0), .5f);
        name.color = new Color(255f,255f,255f,0f);
        name.setText(LanguageManager.string_type.kabir);
        name.DOFade(255,.8f);

        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene12_kabir);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOFade(255,.001f);
        dialogueText.DOText(temp, 1.2f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.2f+delay);
        kabir.transform.DOScale(new Vector3(.944444f,.952941f,0),.3f);
        yield return new WaitForSeconds(.3f);
        kabir.SetActive(false);
        kabirDark.SetActive(true);
        this.transform.DORotate(new Vector3(0,90,0), .5f);
        yield return new WaitForSeconds(.3f);

        shamsu.gameObject.GetComponent<Image>().DOFade(255,.7f);
        yield return new WaitForSeconds(.3f);
        namebar.transform.DORotate(new Vector3(0,0,0), .5f);
        name2.color = new Color(255f,255f,255f,0f);
        name2.setText(LanguageManager.string_type.shamsu);
        name2.DOFade(255,.8f);
        dialogueText.DOFade(0,.4f);
       // dialogueBar.transform.DORotate(new Vector3(0,0,0), .5f);
       
        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene12_shamsu1);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOFade(255,.001f);
        dialogueText.DOText(temp, 1.4f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.4f+delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene12_shamsu2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(2f);
        shamsu.transform.DOScale(new Vector3(.944444f,.952941f,0),.3f);
        yield return new WaitForSeconds(.3f);
        shamsu.SetActive(false);
        shamsuDark.SetActive(true);
        namebar.transform.DORotate(new Vector3(0,90,0), .5f);
        kabirDark.gameObject.GetComponent<Image>().DOFade(0,.4f);
        dialogueText.DOFade(0,.4f);
        yield return new WaitForSeconds(.3f);

        anila.gameObject.GetComponent<Image>().DOFade(255,.7f);
        yield return new WaitForSeconds(.5f);
        this.transform.DORotate(new Vector3(0,0,0), .5f);
        name.color = new Color(255f,255f,255f,0f);
        name.setText(LanguageManager.string_type.anila);
        name.DOFade(255,.8f);

        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene12_anila);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOFade(255,.001f);
        dialogueText.DOText(temp, 1.2f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.2f + delay);
        CinematicsManager.vanish = true;

   
	}
		
		
}
