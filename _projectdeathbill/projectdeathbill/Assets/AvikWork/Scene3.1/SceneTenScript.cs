using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
public class SceneTenScript : MonoBehaviour {

	public GameObject samsu;
	public GameObject dialogueBar;
	//public GameObject nameBar;
    public Text dialogueText;
	public Text name;
	private int i = 1;
    private string temp;
    private string temp1 = "";
    private float textSpeed = .3f;
    private float delay = 1.2f;
    AudioSource audio;
	// Use this for initialization
	void Start () {
		DOTween.Init();
		//tempTime = 0f;

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
        
		samsu.gameObject.GetComponent<Image>().DOFade(255,.5f);
		dialogueBar.transform.DORotate(new Vector3(0,0,0), .5f);
		this.transform.DORotate(new Vector3(0,0,0), .5f);
        name.setText(LanguageManager.string_type.shamsu);

        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene10_shamsu1);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, textSpeed*5, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(textSpeed * 5 + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene10_shamsu2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, textSpeed*7, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(textSpeed * 7 + delay);
        CinematicsManager.vanish = true;
	}


}
