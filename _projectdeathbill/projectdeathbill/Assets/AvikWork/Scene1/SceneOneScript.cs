using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
public class SceneOneScript: MonoBehaviour {

	public GameObject samsu;
	public GameObject samsu_faded;
	public GameObject kabir;
	public GameObject kabir_saturated;
	public GameObject taposh;
	public GameObject dialogueBar;
	public GameObject nameBar;
	public Text name;
    public Text name2;
    public Text dialogueText;
    private float delay = 1.2f;
    //public AudioClip audio1;
    private float textSpeed = .3f;
//	private float tempTime;
    private string temp;
//	private int i = 1;
    private string temp1="";
    //public AudioClip sceneOneAudio;
    private AudioSource audio;
	// Use this for initialization
	void Start () {
        audio = GetComponent<AudioSource>();
        if(UserSettings.SoundOn)
        {
            audio.Play();
        }
        else{
            audio.Stop();
        }
		DOTween.Init();
	
        StartCoroutine(handleTransition());
	}


    IEnumerator handleTransition()
    {
        yield return new WaitForSeconds(1f);
       
        samsu.gameObject.GetComponent<Image>().DOFade(255,.5f);
        dialogueBar.transform.DORotate(new Vector3(0,0,0), .5f);
        this.transform.DORotate(new Vector3(0,0,0), .5f);
        name.setText(LanguageManager.string_type.shamsu);

        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene1_samsu1_part0);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, textSpeed*2, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(.6f+delay);
        dialogueText.DOFade(0,.1f);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene1_samsu1_part1);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOFade(255,.001f);
        dialogueText.DOText(temp, textSpeed*3.5f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(textSpeed*3.5f + delay);
        dialogueText.DOFade(0,.1f);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene1_samsu1_part2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOFade(255,.001f);
        dialogueText.DOText(temp, textSpeed*3.5f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(textSpeed*3.5f + delay);
        dialogueText.DOFade(0,.1f);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene1_samsu1_part3);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOFade(255,.001f);
        dialogueText.DOText(temp, textSpeed*4, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(textSpeed*4 + delay);
        dialogueText.DOFade(255,.2f);
        samsu.transform.DOScale(new Vector3(.8947368421f,.8947368421f,0),.3f);
        yield return new WaitForSeconds(.3f);
        //kabir_saturated.gameObject.GetComponent<Image>().DOFade(0,.001f);
        samsu.SetActive(false);
        samsu_faded.SetActive(true);
        this.transform.DORotate(new Vector3(0,90,0), .5f);
     
        yield return new WaitForSeconds(.5f);
        kabir.gameObject.GetComponent<Image>().DOFade(255,.5f);
        nameBar.transform.DORotate(new Vector3(0,0,0), .5f);
        name2.color = new Color(255f,255f,255f,0f);
        name2.setText(LanguageManager.string_type.kabir);
        name2.DOFade(255,.5f);

        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene1_kabir_part1);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOFade(255,.001f);
        dialogueText.DOText(temp, textSpeed*7, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(textSpeed*7 + delay);
        dialogueText.DOFade(0,.1f);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene1_kabir_part2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOFade(255,.001f);
        dialogueText.DOText(temp, textSpeed*4, true, ScrambleMode.None, null);


        yield return new WaitForSeconds(textSpeed*4 + delay);
        dialogueText.DOFade(0,.1f);
        samsu_faded.gameObject.GetComponent<Image>().DOFade(0,.3f);
        kabir.transform.DOScale(new Vector3(.8947368421f,.8947368421f,0),.3f);
        yield return new WaitForSeconds(.3f);
        kabir.SetActive(false);
        kabir_saturated.SetActive(true);
        nameBar.transform.DORotate(new Vector3(0,90,0), .5f);

        yield return new WaitForSeconds(.5f);
        this.transform.DORotate(new Vector3(0,0,0), .5f);
        taposh.gameObject.GetComponent<Image>().DOFade(255,.5f);
        name.color = new Color(255f,255f,255f,0f);
        name.setText(LanguageManager.string_type.taposh);
        name.DOFade(255,.6f);

        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene1_taposh_part1);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOFade(255,.001f);
        dialogueText.DOText(temp, textSpeed*6, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(textSpeed*6 + delay);
        dialogueText.DOFade(0,.1f);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene1_taposh_part2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOFade(255,.001f);
        dialogueText.DOText(temp, 1.5f, true, ScrambleMode.None, null);



        yield return new WaitForSeconds(2f);
        CinematicsManager.vanish = true;
    }
	
}
