using UnityEngine;
using System.Collections;

public class ShakeCamera1 : MonoBehaviour {
    public static ShakeCamera1 instance;

	private float magnitude = 3f;
	private float duration = 1f;


	private Transform main_camera;
	private bool isShaking = false;
	private bool isFixingRotation = false;

	private Transform tempTransfrom;

	private float mag;


    void Awake()
    {
        instance = this;
		main_camera = Camera.main.transform;
		tempTransfrom = new GameObject().transform;
    }
	void Start () {

	}

	IEnumerator IEShakeCamera()
	{
		//cancel shake camera  reasons
		if(Time.timeScale <1f) yield break;

		mag = magnitude;
		isShaking = true;
        shakeStartTime = Time.time;
        yield return new WaitForSeconds(duration);
		isShaking = false;
		isFixingRotation = true;

		//smoothly rotate to our base rotation
		StartCoroutine("SmoothlyRotateToBase");
	}

	private float time =0.8f;
	IEnumerator SmoothlyRotateToBase() {
		float timer = 0.0f;  

		Quaternion fromRot = main_camera.transform.localRotation;
		
		while (timer <= time) {
			float t = 1.0f + Mathf.Pow((timer / time - 1.0f), 3.0f);
			main_camera.transform.localRotation = Quaternion.Slerp(fromRot,tempTransfrom.localRotation,t);
			timer += Time.deltaTime;
			
			yield return null;
		}

		main_camera.transform.localRotation = tempTransfrom.localRotation;
		isFixingRotation = false;
	}

	Vector3 temp;
	Vector3 baseRot;
    private float shakeStartTime;

	void Update () {
        if (isShaking)
        {
            mag = Mathf.Lerp(magnitude, 0, (Time.time - shakeStartTime)/ duration);
            temp = new Vector3(Random.Range(-0.1f * mag, 0.1f * mag), Random.Range(-0.1f * mag, 0.1f * mag), Random.Range(-0.1f * mag, 0.1f * mag));
			baseRot = main_camera.transform.localRotation.eulerAngles;
            baseRot += temp;
			main_camera.transform.localRotation = Quaternion.Euler(baseRot);
        }
//		if(Input.GetKeyDown(KeyCode.L))
//		{
//			ShakeTheCam(5,2);
//		}

	}

	public void StopCameraShake()
	{
		if(isShaking || isFixingRotation)
		{
			isShaking = false;
			isFixingRotation = false;
			StopCoroutine("IEShakeCamera");
			StopCoroutine("SmoothlyRotateToBase");

			Debug.Log("camera shake was stopped");
			if(main_camera.transform.parent != null)
				main_camera.transform.localRotation = tempTransfrom.localRotation;
		}
	}

    public void ShakeTheCam(float shakeMagnitude, float shakeDuration)
    {
		//StopCoroutine("IEShakeCamera");
		if(isShaking) return;

		if(PlayerInputController.instance == null) return;

		if(PlayerInputController.instance.current_player.GetStationController().IsMoving()) return;

		magnitude = shakeMagnitude;
		duration = shakeDuration;

		//do not start shaking if camera is parentless
		if(main_camera.parent == null) return;

		if(!isShaking)
		{
			if(tempTransfrom==null)
				tempTransfrom = new GameObject().transform;
		}

		//storing main cameras positon and rotaion
		tempTransfrom.parent = main_camera.parent;
		tempTransfrom.position = main_camera.position;
		tempTransfrom.localRotation = main_camera.localRotation;

        StartCoroutine("IEShakeCamera");
    }

	public void AddVerticalCameraMovement(float amount)
	{
		if(isShaking)
		{
			tempTransfrom.Rotate(Vector3.right * (-amount), Space.Self);
		}
	}

	public bool IsShaking()
	{
		return (isShaking || isFixingRotation);
	}
}
