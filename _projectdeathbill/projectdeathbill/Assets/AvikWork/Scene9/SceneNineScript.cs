using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
public class SceneNineScript : MonoBehaviour {

	public GameObject bg;
	public GameObject bgDark;
	public GameObject jungleLeft;
	public GameObject jungleRight;
	public GameObject rajakar;
	public GameObject rajakarBlurred;
	public GameObject samsu;
	public GameObject kabir;
	public GameObject kabir_saturated;
	public GameObject bodi;
	public GameObject bodi_saturated;
	public GameObject dialogueBar;
	public GameObject nameBar;
    public Text dialogueText;
	public Text name;
	public Text name2;
    private float delay = 1.2f;
    private string temp;
    private string temp1= "";
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
		yield return new WaitForSeconds(1f);
		bg.gameObject.GetComponent<Image>().DOFade(0,2f);
		bgDark.gameObject.GetComponent<Image>().DOFade(1,.5f);
		yield return new WaitForSeconds(.8f);
        jungleLeft.gameObject.GetComponent<Transform>().DOLocalMoveX(0f, .5f);
        jungleRight.gameObject.GetComponent<Transform>().DOLocalMoveX(0f,.5f);
//		jungleLeft.gameObject.GetComponent<Transform>().DOMoveX(265f, .5f);
//		jungleRight.gameObject.GetComponent<Transform>().DOMoveX(295f, .5f);

		yield return new WaitForSeconds(.8f);
		kabir.gameObject.GetComponent<Image>().DOFade(255,.5f);
		dialogueBar.transform.DORotate(new Vector3(0,0,0), .5f);
		this.transform.DORotate(new Vector3(0,0,0), .5f);
        name.setText(LanguageManager.string_type.kabir);


        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene9_kabir);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 2f, true, ScrambleMode.None, null);



		yield return new WaitForSeconds(2f+delay);
		kabir.transform.DOScale(new Vector3(.95f,.9473684f,0),.3f);
		yield return new WaitForSeconds(.3f);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
		//kabir_saturated.gameObject.GetComponent<Image>().DOFade(0,.001f);
		kabir.SetActive(false);
		kabir_saturated.SetActive(true);
		this.transform.DORotate(new Vector3(0,90,0), .5f);
//
		yield return new WaitForSeconds(.5f);
		bodi.gameObject.GetComponent<Image>().DOFade(255,.5f);
		nameBar.transform.DORotate(new Vector3(0,0,0), .5f);
		name2.color = new Color(255f,255f,255f,0f);
        name2.setText(LanguageManager.string_type.bodi);
		name2.DOFade(255,.5f);

        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene9_bodi1);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, .7f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(.7f + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene9_bodi2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.4f, true, ScrambleMode.None, null);

		yield return new WaitForSeconds(2.4f);
		bodi.transform.DOScale(new Vector3(.95f,.9473684f,0),.3f);

		yield return new WaitForSeconds(.3f);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
		bodi.SetActive(false);
		bodi_saturated.SetActive(true);
		nameBar.transform.DORotate(new Vector3(0,90,0), .5f);
		kabir_saturated.gameObject.GetComponent<Image>().DOFade(0,.7f);
		yield return new WaitForSeconds(.8f);
		samsu.gameObject.GetComponent<Image>().DOFade(255,.5f);
		this.transform.DORotate(new Vector3(0,0,0), .5f);
		name.color = new Color(255f,255f,255f,0f);
        name.setText(LanguageManager.string_type.shamsu);
		name.DOFade(255,.8f);

        yield return new WaitForSeconds(.7f);
        dialogueText.setText(LanguageManager.string_type.scene9_shamsu1);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.9f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(2.4f);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene9_shamsu2);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.8f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.8f + delay);
        dialogueText.DOText(temp1, .2f, true, ScrambleMode.None, null);
        yield return new WaitForSeconds(.4f);
        dialogueText.setText(LanguageManager.string_type.scene9_shamsu3);
        temp = dialogueText.text; 
        dialogueText.text = "";
        dialogueText.DOText(temp, 1.2f, true, ScrambleMode.None, null);

        yield return new WaitForSeconds(1.2f + delay);
        CinematicsManager.vanish = true;

}
}