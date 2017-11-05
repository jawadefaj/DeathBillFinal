using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Work_JBCinematics_L1P3 : BaseWorker {

    public GameObject cine_jb;
    public GameObject cine_camera;
	Animator cineCamAnim;
    private GameObject jb;

	public float totalTime = 13;
	public float zoomTime = 7;

    private Image blackCover
    {
        get
        {
            return HUDManager.instance.blackCoverImage;
        }
    }
    private float rate = 0.5f;

	protected override void OnStart()
    {
        jb = PlayerInputController.instance.GetPlayerByID(FighterName.JB).gameObject;
        jb.gameObject.SetActive(false);
        StartCoroutine(IE_Start());

		cineCamAnim = cine_camera.GetComponent<Animator> ();
    }

    IEnumerator IE_Start()
    {
        StartCoroutine(GoTransparent());

		yield return new WaitForSeconds(zoomTime);
		cineCamAnim.SetTrigger ("ZOOM");
		yield return new WaitForSeconds(totalTime-zoomTime);

        StartCoroutine(GoBlack());
    }

    private IEnumerator GoTransparent()
    {
        blackCover.gameObject.SetActive(true);
        HUDManager.instance.nonTTPanel.SetActive (false);

        blackCover.color += new Color(0,0,0,1);

        do
        {
            blackCover.color -= new Color(0,0,0,rate*Time.deltaTime*2f);
            yield return null;

            if(blackCover.color.a<0.5f)
                cine_jb.GetComponent<Animator>().SetTrigger("Play");

        }while (blackCover.color.a>0.01f);

        blackCover.gameObject.SetActive(false);
       
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

        cine_jb.SetActive(false);
        cine_camera.SetActive(false);
        jb.gameObject.SetActive(true);
        WorkFinished();
    }
}
