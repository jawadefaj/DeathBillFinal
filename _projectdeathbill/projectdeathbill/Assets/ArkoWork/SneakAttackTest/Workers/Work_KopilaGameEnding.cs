using UnityEngine;
using System.Collections;

public class Work_KopilaGameEnding : BaseWorker {

	private SneakyPlayer kopila;
    private Transform kopila_rightHand;
    private Transform kopila_gun;
	public Transform kopila_chaku;

	public AINoobRajakarPersonnel rajakar;
	public Transform rajakar_shoot_target;

	private Animator kopila_animator;
	private GameObject _camera;

	protected override void OnStart ()
	{
        kopila = SneakyPlayerManager.instance.GetSneakyPlayerByRole(FighterRole.Support);
        kopila_animator = kopila.gameObject.GetComponent<Animator>();
        ThirdPersonController tpc = kopila.gameObject.GetComponent<ThirdPersonController>();
        kopila_rightHand = tpc.modelStructure.rightHand;
        kopila_gun = tpc.modelStructure.gun;

		StartCoroutine("GameEnding");
	}

	IEnumerator GameEnding()
	{
		//clear indicator
		IndicatorManager.instance.HideIndicator();

		//turn of tt panel
		HUDManager.instance.forceDisableRun_ShootGroup = true;

		//create a camera
		_camera = new GameObject("Ending Camera");
		Camera c = _camera.AddComponent<Camera>();
		c.depth = 12;
		_camera.transform.position = rajakar.transform.position;
		_camera.transform.rotation = rajakar.transform.rotation;
		_camera.transform.Translate(Vector3.up*2f);
		_camera.transform.Translate(Vector3.back*3f);
		//_camera.transform.Rotate(Vector3

		//alert rajakar
		rajakar.AlertNoobRajakar();
		yield return new WaitForSeconds(2.38f);

        rajakar.gameObject.GetComponent<SneakySoundTriggerer>().OnRajakarFleeing();

        yield return new WaitForSeconds(0.62f);
		//prepare kopila
		kopila_chaku.gameObject.SetActive(false);
		kopila_gun.SetParent(kopila_rightHand);
		kopila_animator.SetBool("Aim",true);

		//play rajakar sound

		yield return new WaitForSeconds(2f);

		//bring back the camera
		_camera.transform.position = kopila.transform.position;
		_camera.transform.rotation = kopila.transform.rotation;
		_camera.transform.Translate(Vector3.up*2f);
		_camera.transform.Translate(Vector3.back*3f);


		//now shoot
		yield return new WaitForSeconds(0.5f);

		ThirdPersonController tpc = kopila.gameObject.GetComponent<ThirdPersonController>();
		tpc.assignedWeapon.SecondaryInitialize(tpc);
		tpc.Shoot(0,0,true,true,rajakar_shoot_target);

		yield return new WaitForSeconds(0.5f);
		rajakar.KillNoobRajakar();
		yield return null;

		yield return new WaitForSeconds (2f);
		WorkFinished ();
	}
}
