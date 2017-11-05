using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
public class SceneSixScript : MonoBehaviour {

	public GameObject kabir;
	public GameObject kabir_saturated;
	public GameObject anila;
	public GameObject anila_saturated;
    public GameObject anilaEdited;
    public GameObject kabirEdited;
    public GameObject kabirEditedFade;
	public GameObject dialogueBar;
	public GameObject nameBar;
    public Text dialogueText;
	public Text name;
	public Text name2;
    private float delay = 1.2f;
    private string temp;
    private string temp1 = "";
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

	IEnumerator handleTransition()
	{
		kabir.gameObject.GetComponent<Image>().DOFade(255,.5f);
		dialogueBar.transform.DORotate(new Vector3(0,0,0), .5f);
		this.transform.DORotate(new Vector3(0,0,0), .5f);
        name.setText(LanguageManager.string_type.kabir);

        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene6_kabir1_part1);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.6f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.6f + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene6_kabir1_part2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1f, true, ScrambleMode.None, null);

		yield return new WaitForSeconds(2f);
		kabir.transform.DOScale(new Vector3(.95f,.9473684f,0),.5f);
		yield return new WaitForSeconds(.5f);
		//kabir_saturated.gameObject.GetComponent<Image>().DOFade(0,.001f);
		kabir.SetActive(false);
		kabir_saturated.SetActive(true);
		this.transform.DORotate(new Vector3(0,90,0), .5f);


		yield return new WaitForSeconds(.5f);
		anila.gameObject.GetComponent<Image>().DOFade(255,.5f);
		nameBar.transform.DORotate(new Vector3(0,0,0), .5f);
		name2.color = new Color(255f,255f,255f,0f);
        name2.setText(LanguageManager.string_type.anila);
		name2.DOFade(255,.5f);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene6_anila1_part0);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1f + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene6_anila1_part1);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1f + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene6_anila1_part2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1f + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene6_anila1_part3);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.2f, true, ScrambleMode.None, null);

		yield return new WaitForSeconds(1.2f + delay);
		anila.transform.DOScale(new Vector3(.9444444f,.94117647f,0),.5f);
		anila.gameObject.GetComponent<Image>().DOFade(0,.5f);
		yield return new WaitForSeconds(.5f);
		anila_saturated.SetActive(true);
		//anila_saturated.gameObject.GetComponent<Image>().DOFade(255,.5f);
		name2.DOFade(0,.5f);
		nameBar.transform.DORotate(new Vector3(0,90,0), .5f);

		yield return new WaitForSeconds(1f);
		anila.SetActive(false);
		anila.gameObject.GetComponent<Image>().DOFade(255,.5f);

//		//activate kabir
//		yield return new WaitForSeconds(1f);
//		this.transform.DORotate(new Vector3(0,0,0), .5f);
//		kabir_saturated.transform.DOScale(new Vector3(1.055556f,1.05714285f,0),.5f);
//		kabir.transform.DOScale(new Vector3(1f,1f,0),.4f);
//		yield return new WaitForSeconds(.4f);
//		kabir.SetActive(true);
//		name2.color = new Color(255f,255f,255f,0f);
//		name2.DOFade(255,.5f);
//		kabir_saturated.SetActive(false);
        kabir_saturated.gameObject.GetComponent<Image>().DOFade(0,.5f);
        yield return new WaitForSeconds(.8f);
        kabirEdited.gameObject.GetComponent<Image>().DOFade(255,.5f);
        this.transform.DORotate(new Vector3(0,0,0), .5f);


        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene6_kabir2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.2f, true, ScrambleMode.None, null);

		yield return new WaitForSeconds(1.2f + delay);
        kabirEdited.transform.DOScale(new Vector3(.95f,.9473684f,0),.3f);
		//kabir_saturated.transform.DOScale(new Vector3(1f,1f,0),.2f);
		yield return new WaitForSeconds(.3f);
        kabirEdited.SetActive(false);
        kabirEditedFade.SetActive(true);
		this.transform.DORotate(new Vector3(0,90,0), .5f);
       // dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);

//		yield return new WaitForSeconds(1f);
//		anila_saturated.transform.DOScale(new Vector3(1.0588235f,1.0625f,0),.5f);
//		anila.transform.DOScale(new Vector3(1f,1f,0),.5f);
//		anila.SetActive(true);
//		name2.DOFade(255,.6f);
//		nameBar.transform.DORotate(new Vector3(0,0,0), .4f);

        anila_saturated.gameObject.GetComponent<Image>().DOFade(0,.5f);
        yield return new WaitForSeconds(.8f);

        anilaEdited.gameObject.GetComponent<Image>().DOFade(255,.5f);
        dialogueBar.transform.DORotate(new Vector3(0,0,0), .5f);
        name2.DOFade(255,.6f);
        name2.setText(LanguageManager.string_type.anila);
        nameBar.transform.DORotate(new Vector3(0,0,0), .4f);

        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene6_anila2_part1);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.5f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.5f + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene6_anila2_part2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.5f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.5f+delay);
        CinematicsManager.vanish = true;

	}
		
		
}
