using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
public class SceneSevenScript : MonoBehaviour {

	public GameObject samsu;
	public GameObject samsu_faded;
    public GameObject samsuEdited;
	public GameObject kabir;
	public GameObject kabir_saturated;
	public GameObject bodi;
	public GameObject bodi_saturated;
	public GameObject anila;
	public GameObject anila_saturated;
	public GameObject dialogueBar;
	public GameObject nameBar;
    public Text dialogueText;
	public Text name;
	public Text name2;
    private string temp;
    private string temp1 = "";
	private float tempTime;
    private float delay = 1.2f;
	private int i = 1;
    AudioSource audio;
	// Use this for initialization
	void Start () {
		DOTween.Init();
		tempTime = 0f;
		StartCoroutine(handleTransition());
        audio = GetComponent<AudioSource>();
        if(UserSettings.SoundOn)
        {
            audio.Play();
        }
        else{
            audio.Stop();
        }
		this.transform.DORotate(new Vector3(0,0,0), .5f);
		dialogueBar.transform.DORotate(new Vector3(0,0,0), .5f);
		//samsu.transform.DOScale(new Vector3(.9f,.9f,.9f),2f);
	}

	IEnumerator handleTransition()
	{
		samsu.gameObject.GetComponent<Image>().DOFade(255,.5f);
		dialogueBar.transform.DORotate(new Vector3(0,0,0), .5f);
		this.transform.DORotate(new Vector3(0,0,0), .5f);
        name.setText(LanguageManager.string_type.shamsu);

        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene7_shamsu1_part0);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.2f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.2f+delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene7_shamsu1_part1);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.2f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.2f + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene7_shamsu1_part2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.8f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.8f + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene7_shamsu1_part3);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.2f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.2f + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene7_shamsu1_part4);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.4f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.4f + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene7_shamsu1_part5);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.2f, true, ScrambleMode.None, null);



		yield return new WaitForSeconds(2f);
		samsu.transform.DOScale(new Vector3(.95f,.9473684f,0),.5f);
		yield return new WaitForSeconds(.5f);
		samsu.gameObject.GetComponent<Image>().DOFade(0,.001f);
		//samsu.SetActive(false);
		samsu_faded.SetActive(true);
		this.transform.DORotate(new Vector3(0,90,0), .5f);

		yield return new WaitForSeconds(.5f);
		bodi.gameObject.GetComponent<Image>().DOFade(255,.5f);
		nameBar.transform.DORotate(new Vector3(0,0,0), .5f);
		name2.color = new Color(255f,255f,255f,0f);
        name2.setText(LanguageManager.string_type.bodi);
		name2.DOFade(255,.5f);

        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene7_bodi1);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.6f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.6f + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene7_bodi2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.2f, true, ScrambleMode.None, null);


		yield return new WaitForSeconds(1.2f + delay);
		nameBar.transform.DORotate(new Vector3(0,90,0), .5f);
		samsu_faded.gameObject.GetComponent<Image>().DOFade(0,.5f);
		yield return new WaitForSeconds(.4f);
		kabir.gameObject.GetComponent<Image>().DOFade(255,.5f);
		this.transform.DORotate(new Vector3(0,90,0), .5f);
		name.color = new Color(255f,255f,255f,0f);
        name.setText(LanguageManager.string_type.kabir);
		name.DOFade(255,.5f);
		this.transform.DORotate(new Vector3(0,0,0), .5f);

        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene7_kabir1_part1);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(2f);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene7_kabir1_part2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.5f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.5f + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene7_kabir1_part3);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.1f, true, ScrambleMode.None, null);



		yield return new WaitForSeconds(1.1f+ delay);
		kabir.transform.DOScale(new Vector3(.95f,.9473684f,0),.5f);
		yield return new WaitForSeconds(.5f);
		kabir.SetActive(false);
		kabir_saturated.SetActive(true);
		this.transform.DORotate(new Vector3(0,90,0), .5f);
		bodi.gameObject.GetComponent<Image>().DOFade(0,.5f);



		yield return new WaitForSeconds(1f);
		anila.gameObject.GetComponent<Image>().DOFade(255,.5f);
		nameBar.transform.DORotate(new Vector3(0,0,0), .5f);
		name2.color = new Color(255f,255f,255f,0f);
        name2.setText(LanguageManager.string_type.anila);
		name2.DOFade(255,.5f);

        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene7_anila1);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.4f, true, ScrambleMode.None, null);

      


		//activate kabir
		yield return new WaitForSeconds(2f);
		nameBar.transform.DORotate(new Vector3(0,90,0), .5f);
		//samsu_faded.gameObject.GetComponent<Image>().DOFade(0,.5f);
		kabir_saturated.transform.DOScale(new Vector3(1.055556f,1.05714285f,0),.5f);
		kabir.transform.DOScale(new Vector3(1f,1f,0),.5f);
		anila.transform.DOScale(new Vector3(.95f,.9473684f,0),.5f);
		//yield return new WaitForSeconds(.5f);
		yield return new WaitForSeconds(.5f);
		anila_saturated.SetActive(true);
		anila.SetActive(false);
		kabir_saturated.SetActive(false);
		kabir.SetActive(true);
		//kabir.gameObject.GetComponent<Image>().DOFade(255,.5f);
		this.transform.DORotate(new Vector3(0,90,0), .5f);
		name.color = new Color(255f,255f,255f,0f);
        name.setText(LanguageManager.string_type.kabir);
		name.DOFade(255,.5f);
		this.transform.DORotate(new Vector3(0,0,0), .5f);

        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene7_kabir2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.2f, true, ScrambleMode.None, null);

       


		yield return new WaitForSeconds(1.8f);
		kabir.transform.DOScale(new Vector3(.95f,.9473684f,0),.5f);
		kabir_saturated.transform.DOScale(new Vector3(1f,1f,0),.5f);
		yield return new WaitForSeconds(.5f);
		kabir.SetActive(false);
		kabir_saturated.SetActive(true);
		this.transform.DORotate(new Vector3(0,90,0), .5f);
		yield return new WaitForSeconds(.5f);
		anila.transform.DOScale(new Vector3(1f,1f,0),.5f);
		anila_saturated.transform.DOScale(new Vector3(1.055556f,1.05714285f,0),.5f);
		yield return new WaitForSeconds(.5f);
		anila.SetActive(true);
		anila_saturated.SetActive(false);
		nameBar.transform.DORotate(new Vector3(0,0,0), .5f);
		name2.color = new Color(255f,255f,255f,0f);
        name2.setText(LanguageManager.string_type.anila);
		name2.DOFade(255,.5f);

        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene7_anila2_part1);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.8f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.8f+ delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene7_anila2_part2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.8f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.8f+delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene7_anila2_part3);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.8f, true, ScrambleMode.None, null);

		yield return new WaitForSeconds(2.3f);
		kabir_saturated.gameObject.GetComponent<Image>().DOFade(0,.5f);
//		yield return new WaitForSeconds(1f);
//		samsu.gameObject.GetComponent<Image>().DOFade(0,.5f);
		//kabir_saturated.SetActive(false);
		//samsu.SetActive(true);
		yield return new WaitForSeconds(1f);
        anila.transform.DOScale(new Vector3(.9444444f,.9411764f,0),.5f);
        nameBar.transform.DORotate(new Vector3(0,90,0), .5f);
        anila_saturated.transform.DOScale(new Vector3(1f,1f,0),.2f);
        yield return new WaitForSeconds(.5f);
        anila.SetActive(false);
        anila_saturated.SetActive(true);

		samsuEdited.gameObject.GetComponent<Image>().DOFade(255,.5f);
		this.transform.DORotate(new Vector3(0,0,0), .5f);
		name.color = new Color(255f,255f,255f,0f);
        name.setText(LanguageManager.string_type.shamsu);
		name.DOFade(255,.5f);

        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene7_shamsu2_part1);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1f, true, ScrambleMode.None, null);
		
        yield return new WaitForSeconds(1f + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene7_shamsu2_part2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.5f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.5f+delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene7_shamsu2_part3);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.8f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.8f+delay);
        CinematicsManager.vanish = true;

	}


		
}
