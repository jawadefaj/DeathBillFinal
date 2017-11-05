using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
public class SceneThreeScript : MonoBehaviour {

	public GameObject samsu;
	public GameObject kabir;
	public GameObject kabir_saturated;
	public GameObject bodi;
	public GameObject bodi_saturated;
	public GameObject dialogueBar;
	public GameObject nameBar;
	public Text name;
	public Text name2;
	private bool isSamsuFaded;
	private bool isSamsuBright;
	private bool isKabirFaded;
	private bool isKabirBright;
	private bool isSamsuActive;
	private float tempTime;
	private int i = 1;
	// Use this for initialization
	void Start () {
		DOTween.Init();
		tempTime = 0f;
		isSamsuFaded = false;
		isSamsuBright = false;
		isKabirFaded = false;
		isKabirBright = false;
		isSamsuActive = false;

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

		yield return new WaitForSeconds(3f);
		kabir.transform.DOScale(new Vector3(.95f,.9473684f,0),.5f);
		yield return new WaitForSeconds(.5f);
		//kabir_saturated.gameObject.GetComponent<Image>().DOFade(0,.001f);
		kabir.SetActive(false);
		kabir_saturated.SetActive(true);
		this.transform.DORotate(new Vector3(0,90,0), .5f);

		yield return new WaitForSeconds(.5f);
		bodi.gameObject.GetComponent<Image>().DOFade(255,.5f);
		nameBar.transform.DORotate(new Vector3(0,0,0), .5f);
		name2.color = new Color(255f,255f,255f,0f);
		name2.text = "Bodi";
		name2.DOFade(255,.5f);

		yield return new WaitForSeconds(2f);
		bodi.transform.DOScale(new Vector3(.95f,.9473684f,0),.5f);

		yield return new WaitForSeconds(.5f);
		bodi.SetActive(false);
		bodi_saturated.SetActive(true);
		nameBar.transform.DORotate(new Vector3(0,90,0), .5f);
		//this.transform.DORotate(new Vector3(0,90,0), .5f);
		//bodi.gameObject.GetComponent<Image>().DOFade(0,.5f);

		yield return new WaitForSeconds(.5f);
		kabir_saturated.gameObject.GetComponent<Image>().DOFade(0,.5f);
		yield return new WaitForSeconds(.5f);

		samsu.gameObject.GetComponent<Image>().DOFade(255,.5f);
		name.color = new Color(255f,255f,255f,0f);
		name.text = "Samsu";
		name.DOFade(255,.5f);
		this.transform.DORotate(new Vector3(0,0,0), .5f);

	}

}
