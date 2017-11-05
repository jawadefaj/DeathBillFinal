using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KnifeAnimationManager : MonoBehaviour {

	public static KnifeAnimationManager instance;
	public Image blackCover
    {
        get
        {
            return HUDManager.instance.blackCoverImage;
        }
    }
	public KnifeAnimation[] animations;
	//[Tooltip("Enter animation in a serial")]
	public AnimationEntry[] animationEntry;

	private int index =0;
	private GameObject _animationCamera;
	private Transform mainCamera;
	private Transform _canvas;
	private KnifeAnimation currentAnimation;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		//clear the cameras
		for(int i=0;i<animationEntry.Length;i++)
		{
			Destroy(animationEntry[i].animationCamera.GetComponent<GUILayer>());
			Destroy(animationEntry[i].animationCamera.GetComponent<FlareLayer>());
			Destroy(animationEntry[i].animationCamera.GetComponent<Camera>());
			Destroy(animationEntry[i].animationCamera.GetComponent<AudioListener>());

			if(animationEntry[i].postAnimationCamera != null)
			{
				Destroy(animationEntry[i].postAnimationCamera.GetComponent<GUILayer>());
				Destroy(animationEntry[i].postAnimationCamera.GetComponent<FlareLayer>());
				Destroy(animationEntry[i].postAnimationCamera.GetComponent<Camera>());
				Destroy(animationEntry[i].postAnimationCamera.GetComponent<AudioListener>());
			}
		}

		mainCamera = Camera.main.transform;
		_canvas = blackCover.transform.root;

		//create a new camera for us
		GameObject go = new GameObject("AnimationCamera");
		go.transform.SetParent(this.transform);
		go.AddComponent<Camera>();
		go.GetComponent<Camera>().depth = 11;
		_animationCamera = go;
		_animationCamera.SetActive(false);
	}

	public void PlayKnifeKillingAnimation(Transform playerTransform)
	{
		//turn of canvas
		_canvas.gameObject.SetActive(false);

		for(int i=0;i<animations.Length;i++)
		{
			if(animations[i].actorRole == animationEntry[index].forFighter)
			{
                //For optional player case
                if (animationEntry[index].forFighter == FighterRole.Leader)
                {
                    //skipping tjis animation if not the correct animation actor
                    if (animations[i].actorName != LevelManager.OptionalPlayer)
                        continue;
                }

				//this the target actor
				animations[i].RepositionsAndPlay(playerTransform,LungiManager.instance.GetUsedLungi());

				//reposition our camera
				_animationCamera.transform.position = animationEntry[index].animationCamera.position;
				_animationCamera.transform.rotation = animationEntry[index].animationCamera.rotation;
				_animationCamera.SetActive(true);

				currentAnimation = animations[i];

				//increse next index
				index++;
				if(animationEntry[index-1].hasAutoWalkAtEnd)
				{
					HUDManager.instance.forceDisableRun_ShootGroup = true;
				}
				if(animationEntry[index-1].sceneBlackOutAtEnd)
				{
					HUDManager.instance.nonTTPanel.gameObject.SetActive(false);
				}
				return;
			}
		}
	}

	public void OnAnimationPlayDone()
	{
		
		//if we have a post animation camera then switch main camera to it
		if(animationEntry[index-1].postAnimationCamera != null)
		{
			SneakyCamera.instance.ForceCameraSet(animationEntry[index-1].postAnimationCamera);
		}

		//do we want to black out our scene?
		if(animationEntry[index-1].sceneBlackOutAtEnd)
		{
			//SceneBlackOut();
			//Stay as it is
			SneakyPlayerManager.instance.GetCurrentPlayer().OnStabKillingDone(false);

			HUDManager.instance.nonTTPanel.gameObject.SetActive(false);
		}
		else
		{
			currentAnimation.DeactivateModel();
			SneakyPlayerManager.instance.GetCurrentPlayer().OnStabKillingDone(true);
			_animationCamera.SetActive(false);
		}
		//turn on canvas



		_canvas.gameObject.SetActive(true);
	}

	public void ResetSettings()
	{
		currentAnimation.DeactivateModel();
		_animationCamera.SetActive(false);
	}

	public void SetAnimationIndex(int value)
	{
		index = value;
	}

	private void SceneBlackOut()
	{
		StartCoroutine(CameraSwitchOff());
	}

	private IEnumerator CameraSwitchOff()
	{
		yield return new WaitForSeconds(2f);
		_animationCamera.SetActive(false);
	}

	[System.Serializable]
	public struct AnimationEntry
	{
		public FighterRole forFighter;
		public Transform animationCamera;
		public Transform postAnimationCamera;
		//public Texture rajakarLungi;
		public bool sceneBlackOutAtEnd;
		public bool hasAutoWalkAtEnd;
	}
}
