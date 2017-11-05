using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Work_SceneBlackOut : BaseWorker {

	internal Image blackCover
    {
        get{ 
            return HUDManager.instance.blackCoverImage;
        }
    }
	public bool fadeIn = true;
	private float rate = 0.5f;
	protected override void OnStart ()
	{
		if(fadeIn)
			StartCoroutine(GoBlack());
		else
			StartCoroutine(GoTransparent());
	}

	private IEnumerator GoBlack()
	{
		blackCover.gameObject.SetActive(true);
		HUDManager.instance.nonTTPanel.SetActive (false);

		blackCover.color *= new Color(1,1,1,0);

		do
		{
			blackCover.color += new Color(0,0,0,rate*Time.deltaTime);
			yield return null;
		}while (blackCover.color.a<0.99f);

		WorkFinished();
	}

	private IEnumerator GoTransparent()
	{
		blackCover.gameObject.SetActive(true);
		HUDManager.instance.nonTTPanel.SetActive (false);

		blackCover.color += new Color(0,0,0,1);

		do
		{
			blackCover.color -= new Color(0,0,0,rate*Time.deltaTime);
			yield return null;
		}while (blackCover.color.a>0.01f);

		blackCover.gameObject.SetActive(false);

		HUDManager.instance.nonTTPanel.SetActive (true);
		WorkFinished();
	}
}


