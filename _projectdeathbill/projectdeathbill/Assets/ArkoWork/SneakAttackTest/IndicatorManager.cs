using UnityEngine;
using System.Collections;

public enum IndicatorType
{
	BoxIndicator =0,
	PlaneIndicator =1,
}
public class IndicatorManager : MonoBehaviour {

	public static IndicatorManager instance;
	public Indicator[] indicators;

	private GameObject currentIndicator;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		for(int i=0;i<indicators.Length;i++)
		{
			indicators[i].indicatorObject.SetActive(false);
		}
	}

	public void ShowIndicator(IndicatorType indicatorType, Transform taregt)
	{
		if(currentIndicator != null)
		{
			currentIndicator.transform.SetParent(null);
			currentIndicator.SetActive(false);
		}

		for(int i=0;i<indicators.Length;i++)
		{
			if(indicators[i].type == indicatorType)
			{
				indicators[i].indicatorObject.SetActive(true);
				indicators[i].indicatorObject.transform.SetParent(taregt);
				indicators[i].indicatorObject.transform.localPosition = Vector3.zero;
				indicators[i].indicatorObject.transform.Translate(indicators[i].ofset,Space.World);
				currentIndicator = indicators[i].indicatorObject;
				//indicators[i].indicatorObject.GetComponent<Animator>().SetBool("Play",true);
				return;
			}
		}
	}

	public void HideIndicator()
	{
		if(currentIndicator != null)
		{
			currentIndicator.transform.SetParent(null);
			currentIndicator.SetActive(false);
		}
	}

	[System.Serializable]
	public struct Indicator
	{
		public IndicatorType type;
		public GameObject indicatorObject;
		public Vector3 ofset;
	}
}
