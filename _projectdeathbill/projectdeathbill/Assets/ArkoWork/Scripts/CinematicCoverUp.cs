using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CinematicCoverUp : MonoBehaviour {

	public static CinematicCoverUp instance;

	public Image topCover;
	public Image downCover;

	private Vector2 pos;
	private float speed = 90f;
	private bool isCoverShowing = false;

	void Awake()
	{
		instance = this;
	}

	void Start () {
		//topCover.gameObject.SetActive(true);
		//downCover.gameObject.SetActive(true);

		GoToUpPos();
	}

	public void FlagDown()
	{
		StopOtherTransitions();
		StartCoroutine("GradualyDown");
	}

	public void FlagUp()
	{
		StopOtherTransitions();
		StartCoroutine("GradualyUp");
	}

	public bool IsCoverShowing()
	{
		return isCoverShowing;
	}

	private void StopOtherTransitions()
	{
		StopCoroutine("GradualyDown");
		StopCoroutine("GradualyUp");
	}

	private void GoToDownPos()
	{
		pos = topCover.rectTransform.anchoredPosition;
		pos.y = -topCover.rectTransform.rect.height/2f;
		topCover.rectTransform.anchoredPosition = pos;

		pos = downCover.rectTransform.anchoredPosition;
		pos.y = downCover.rectTransform.rect.height/2f;
		downCover.rectTransform.anchoredPosition =pos;
	}

	private void GoToUpPos()
	{
		pos = topCover.rectTransform.anchoredPosition;
		pos.y = topCover.rectTransform.rect.height/2f;
		topCover.rectTransform.anchoredPosition = pos;

		pos = downCover.rectTransform.anchoredPosition;
		pos.y = -downCover.rectTransform.rect.height/2f;
		downCover.rectTransform.anchoredPosition =pos;
	}

	private IEnumerator GradualyDown()
	{
		GoToUpPos();

		isCoverShowing = true;
		float totalDown = 0;

		do{
			pos = topCover.rectTransform.anchoredPosition;
			pos.y -= speed*Time.deltaTime;
			topCover.rectTransform.anchoredPosition = pos;

			pos = downCover.rectTransform.anchoredPosition;
			pos.y += speed*Time.deltaTime;
			downCover.rectTransform.anchoredPosition = pos;

			totalDown += speed*Time.deltaTime;

			yield return null;
		}while(totalDown<topCover.rectTransform.rect.height);
			
	}

	private IEnumerator GradualyUp()
	{
		GoToDownPos();

		float totalDown = 0;

		do{
			pos = topCover.rectTransform.anchoredPosition;
			pos.y += speed*Time.deltaTime;
			topCover.rectTransform.anchoredPosition = pos;

			pos = downCover.rectTransform.anchoredPosition;
			pos.y -= speed*Time.deltaTime;
			downCover.rectTransform.anchoredPosition = pos;

			totalDown += speed*Time.deltaTime;

			yield return null;
		}while(totalDown<topCover.rectTransform.rect.height);

		isCoverShowing = false;
	}
}
