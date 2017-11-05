using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
public class SceneFourScript : MonoBehaviour {

	public GameObject first;
	public GameObject second;
	public GameObject third;
	public GameObject dialogueBar;
	//public GameObject nameBar;
	public Text name;
    public Text dialogueText;
    private string temp;
    private string temp1= "";
    private float delay = 1.2f;
    AudioSource audio;
    //public AudioClip audioKick;
    //public AudioClip audioDoorOpen;
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
		yield return new WaitForSeconds(1f);
		first.gameObject.GetComponent<Transform>().DOShakePosition(1f, new Vector3(50f,1f,1f), 10, 90, false);
       // AudioSource.PlayClipAtPoint(audioKick,new Vector3(0,0,0));
		yield return new WaitForSeconds(.5f);
		//second.transform.DOScale(new Vector3(3f,3f,3f), .2f);
		//yield return new WaitForSeconds(.3f);
		second.SetActive(true);
		yield return new WaitForSeconds(.3f);
		second.gameObject.GetComponent<Transform>().DOShakePosition(1.2f, new Vector3(60f,25f,25f), 10, 90, false);
       // AudioSource.PlayClipAtPoint(audioDoorOpen,new Vector3(0,0,0));
		yield return new WaitForSeconds(1.4f);
		second.transform.DOScale(new Vector3(1.2f,1.2f,1f), 1f);
		yield return new WaitForSeconds(1.1f);
		//second.SetActive(false);
		first.SetActive(false);
		second.gameObject.GetComponent<Image>().DOFade(0f,.5f);
		yield return new WaitForSeconds(.4f);
		third.gameObject.GetComponent<Image>().DOFade(255f,1f);
		yield return new WaitForSeconds(1f);
		//samsu.gameObject.GetComponent<Image>().DOFade(255,.5f);
		dialogueBar.transform.DORotate(new Vector3(0,0,0), .5f);
        name.setText(LanguageManager.string_type.shamsu);
		this.transform.DORotate(new Vector3(0,0,0), .5f);

        yield return new WaitForSeconds(.6f);
        dialogueText.setText(LanguageManager.string_type.scene4_samsu1);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.1f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.1f + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene4_samsu2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, .8f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(.8f+delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene4_samsu3);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.2f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.2f + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene4_samsu4);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.4f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.4f + delay);
        CinematicsManager.vanish = true;

	}


}
