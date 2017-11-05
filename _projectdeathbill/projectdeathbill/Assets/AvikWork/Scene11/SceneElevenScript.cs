using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
public class SceneElevenScript : MonoBehaviour {

	public GameObject samsu;
	public GameObject samsu_faded;
	public GameObject kabir;
	public GameObject kabir_saturated;
	public GameObject bodi;
	public GameObject bodi_saturated;
	public GameObject anila;
	public GameObject dialogueBar;
	public GameObject nameBar;
    public Text dialogueText;
	public Text name;
	public Text name2;
	private float tempTime;
    private string temp;
    private string temp1 = "";
	private int i = 1;
    private float textSpeed = 4f;
    private float delay = 1.2f;
    AudioSource audio;
	// Use this for initialization
	void Start () {
		DOTween.Init();
		tempTime = 0f;

        audio = GetComponent<AudioSource>();
        if(UserSettings.SoundOn)
        {
            audio.Play();
        }
        else{
            audio.Stop();
        }

		StartCoroutine(handleTransition());

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
        dialogueText.setText(LanguageManager.string_type.scene11_shamsu1);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.2f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.2f+delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene11_shamsu2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.9f, true, ScrambleMode.None, null);

		yield return new WaitForSeconds(1.9f +delay);
		samsu.transform.DOScale(new Vector3(.95f,.9473684f,0),.3f);
		yield return new WaitForSeconds(.3f);
		samsu.gameObject.GetComponent<Image>().DOFade(0,.001f);
		//samsu.SetActive(false);
		samsu_faded.SetActive(true);
		this.transform.DORotate(new Vector3(0,90,0), .5f);
        dialogueText.DOFade(0,.5f);

		yield return new WaitForSeconds(.5f);
		bodi.gameObject.GetComponent<Image>().DOFade(255,1f);
		nameBar.transform.DORotate(new Vector3(0,0,0), .5f);
		name2.color = new Color(255f,255f,255f,0f);
        name2.setText(LanguageManager.string_type.bodi);
		name2.DOFade(255,.5f);
	    
        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene11_bodi);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOFade(255,.001f);
        dialogueText.DOText(temp, 1.5f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.5f + delay);
        CinematicsManager.vanish = true;


		
    }
}