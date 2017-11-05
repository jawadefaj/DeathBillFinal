using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
public class SceneTwoScript : MonoBehaviour {

	public GameObject line1;
    public GameObject line2;
    public GameObject line3;
    public GameObject bg;
    public GameObject cross;
    public GameObject dialogueBar;
    public GameObject dialogueMask;
    public Text dialogueText;
//	public GameObject kabir_saturated;
//	public GameObject anila;
//	public GameObject anila_saturated;
//	public GameObject dialogueBar;
//	public GameObject nameBar;
	public Text name;
	public Text name2;
    private  string temp;
    private string temp1="";
    private float inc=0f;
    private float textSpeed = .3f;
    private float delay = 1.2f;
    AudioSource audio;
	// Use this for initialization
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

	float english_delay=0;
	IEnumerator handleTransition()
	{
		//((UserSettings.SelectedLanguage == Language.English) ? 0 : 0);
        yield return new WaitForSeconds(1f);
        bg.gameObject.GetComponent<Transform>().DOScale(new Vector3(2,2,0), 1f);
        yield return new WaitForSeconds(.3f);
        this.transform.DORotate(new Vector3(0,0,0), .5f);
        dialogueBar.transform.DORotate(new Vector3(0,0,0), .5f);
        name.setText(LanguageManager.string_type.shamsu);

        //Showing Text in Dialogue bar
        dialogueText.setText(LanguageManager.string_type.scene2_samsu1);
        temp = dialogueText.text; 
        dialogueText.text = "";
        yield return new WaitForSeconds(.8f);
        dialogueText.DOText(temp, 1.5f, true, ScrambleMode.None, null);

		yield return new WaitForSeconds (1.5f + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene2_samsu2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.8f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.8f + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene2_samsu3);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.8f, true, ScrambleMode.None, null);


        yield return new WaitForSeconds(3f);
        cross.SetActive(true);

        dialogueText.setText(LanguageManager.string_type.scene2_samsu4);
        temp = dialogueText.text; 
        dialogueText.text = "";
       // yield return new WaitForSeconds(.8f);
        dialogueText.DOText(temp, 1.8f, true, ScrambleMode.None, null);

        for(int i=0; i<2; i++)
        {
            yield return new WaitForSeconds(.5f);
            cross.gameObject.GetComponent<Transform>().DOScale(new Vector3(2.5f,2.5f,0), .5f);
            yield return new WaitForSeconds(.5f);
            cross.gameObject.GetComponent<Transform>().DOScale(new Vector3(1,1,0), .5f);
        }
        yield return new WaitForSeconds(.5f);
        cross.gameObject.GetComponent<Transform>().DOScale(new Vector3(1.8f,1.8f,0), .5f);
        yield return new WaitForSeconds(1f);

        dialogueText.setText(LanguageManager.string_type.scene2_samsu5);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.8f, true, ScrambleMode.None, null);

        for(int i=0; i<=40; i++)
        {
            line1.gameObject.GetComponent<Image>().fillAmount = 0f+inc;
            inc += .025f;
            yield return new WaitForSeconds(.025f);
        }
        inc =0f;
        yield return new WaitForSeconds(1.5f);

        dialogueText.setText(LanguageManager.string_type.scene2_samsu6);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.8f, true, ScrambleMode.None, null);
        for(int i=0; i<=40; i++)
        {
            line2.gameObject.GetComponent<Image>().fillAmount = 0f+inc;
            inc += .025f;
            yield return new WaitForSeconds(.025f);
        }
        inc = 0f;
        yield return new WaitForSeconds(1.5f);

        dialogueText.setText(LanguageManager.string_type.scene2_samsu7);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.5f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.5f+delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene2_samsu8);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.5f, true, ScrambleMode.None, null);


        for(int i=0; i<=40; i++)
        {
            line3.gameObject.GetComponent<Image>().fillAmount = 0f+inc;
            inc += .025f;
            yield return new WaitForSeconds(.025f);
        }

        yield return new WaitForSeconds(1f);

        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.5f);

        dialogueText.setText(LanguageManager.string_type.scene2_samsu9);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.2f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.2f+delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene2_samsu10);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.5f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.5f + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene2_samsu11);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, .6f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.5f);
		CinematicsManager.vanish = true;
    }
}
		
		

