using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Work_FillImages : BaseWorker {
	public List<FillImageSet> fillImageSettings =  new List<FillImageSet>();
	private int index = 0;


	protected override void OnStart ()
	{
		CineManager.instance.StartCoroutine (StartFilling(fillImageSettings[index++]));
		WorkFinished ();
	}

	IEnumerator StartFilling(FillImageSet fImg)
	{
		float fillRate = 1;
		if (fImg.fillTime > 0) {
			fillRate = Time.deltaTime / fImg.fillTime;
		} 
		yield return new WaitForSeconds (fImg.startDelay);
		do
		{
			fImg.image.fillAmount += fillRate;
			yield return null;
		}
		while(fImg.image.fillAmount<1);
	}

}
[System.Serializable]
public class FillImageSet
{
	public Image image;
	public float startDelay;
	public float fillTime;
}
