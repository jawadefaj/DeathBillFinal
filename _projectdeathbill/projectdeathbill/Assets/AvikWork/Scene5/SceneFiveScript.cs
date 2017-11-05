using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
public class SceneFiveScript : MonoBehaviour {

	public GameObject kabir;
	public GameObject dialogueBar;
	public GameObject nameBar;
    public Text dialogueText;
	public Text name;
	public Text name2;
    private string temp;
    private string temp1 = "";
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

	IEnumerator handleTransition()
	{
		//kabir.gameObject.GetComponent<Image>().DOFade(255,.5f);
		yield return new WaitForSeconds(1f);
		dialogueBar.transform.DORotate(new Vector3(0,0,0), .5f);
		nameBar.transform.DORotate(new Vector3(0,0,0), .5f);
		name2.color = new Color(255f,255f,255f,0f);
        name2.setText(LanguageManager.string_type.shamsu);
		name2.DOFade(255,.8f);

        dialogueText.setText(LanguageManager.string_type.scene5_samsu);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.2f, true, ScrambleMode.None, null);

		yield return new WaitForSeconds(1.2f + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
		kabir.gameObject.GetComponent<Image>().DOFade(255,1f);
        nameBar.transform.DORotate(new Vector3(0,90,0), .5f);
	    

		yield return new WaitForSeconds(.5f);
		this.transform.DORotate(new Vector3(0,0,0), .5f);
		name.color = new Color(255f,255f,255f,0f);
        name.setText(LanguageManager.string_type.kabir);
		name.DOFade(255,.7f);

        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene5_kabir_part1);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.8f, true, ScrambleMode.None, null);

//        yield return new WaitForSeconds(1.5f);
//        dialogueText.DOText(temp1, 4f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.8f+delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene5_kabir_part2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.5f, true, ScrambleMode.None, null);

		//this.transform.DORotate(new Vector3(0,90,0), .5f);
		//bodi.gameObject.GetComponent<Image>().DOFade(0,.5f);

        yield return new WaitForSeconds(1.5f+delay);
        CinematicsManager.vanish = true;

	
	}


		
}
