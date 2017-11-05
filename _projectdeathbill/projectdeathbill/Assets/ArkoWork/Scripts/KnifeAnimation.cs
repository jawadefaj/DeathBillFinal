using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class KnifeAnimation : MonoBehaviour {

    [FormerlySerializedAs("actor")]
	public FighterRole actorRole;
    public FighterName actorName;
	public Transform fighterTransform;
	public Transform rajakarTransform;
	public Renderer rajakarLungi;

	private Animator fighterAnimator;
	private Animator rajakarAnimator;

	void Start()
	{
		fighterAnimator = fighterTransform.gameObject.GetComponent<Animator>();
		rajakarAnimator = rajakarTransform.gameObject.GetComponent<Animator>();
		this.gameObject.SetActive(false);
	}

	public void Play()
	{
		fighterAnimator.SetTrigger("Play");
		rajakarAnimator.SetTrigger("Play");
	}

	public void DeactivateModel()
	{
		this.gameObject.SetActive(false);

	}

	public void Reposition(Transform targetTransform)
	{
		this.gameObject.SetActive(true);

		Transform originalParent = fighterTransform.parent;

		//reverse parenting
		fighterTransform.SetParent(null);
		originalParent.SetParent(fighterTransform);

		//reposition to target position
		fighterTransform.position = targetTransform.position;
		fighterTransform.rotation = targetTransform.rotation;

		//reparent again
		originalParent.SetParent(null);
		fighterTransform.SetParent(originalParent);

		//play rajakar idle
		rajakarAnimator.SetTrigger("Spawn");
	}

	public void SetRajakarLungiTexture(Texture texture)
	{
		//rajakarLungi.materials[0].mainTexture = texture;
	}

	public void RepositionsAndPlay(Transform t_transform, Texture lungiTexture)
	{
		SetRajakarLungiTexture(lungiTexture);
		Reposition(t_transform);
		Play();
	}
}
