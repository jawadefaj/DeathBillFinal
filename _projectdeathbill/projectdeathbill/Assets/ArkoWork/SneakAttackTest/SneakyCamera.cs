using System;
using UnityEngine;
using System.Collections;

public class SneakyCamera : MonoBehaviour {

	public static SneakyCamera instance;

	
	private PanRect panRect;
	private Transform thisCamera;
	private bool isTransiting = false;
	private Vector3 cameraLerpPos = Vector3.zero;

	void Awake()
	{
		instance = this;
	}
	// Use this for initialization
	void Start () {

		thisCamera =  this.transform.GetChild(0);
        ResetLocal();
	}

	void Update()
	{
		if(!isTransiting)
		{
			thisCamera.transform.localPosition = Vector3.Lerp(thisCamera.transform.localPosition,cameraLerpPos,3f*Time.deltaTime);
		}
	}

	public void PanCamera(Vector2 deltaPos)
	{
		deltaPos *= 0.25f;
		Vector3 newPos = cameraLerpPos;
		newPos.x = Mathf.Clamp( newPos.x+deltaPos.x,-panRect.left,panRect.right);
		newPos.y = Mathf.Clamp( newPos.y+deltaPos.y,-panRect.bottom,panRect.top);

		cameraLerpPos = newPos;
	}

	public void SetPanRect(PanRect pr)
	{
		this.panRect = pr;
        ResetLocal();
	}

	public void ForceCameraSet(Transform target)
	{
        ResetLocal();

		this.transform.position = target.position;
		this.transform.rotation = target.rotation;
	}

    public void ResetLocal()
    {
        if(thisCamera==null) thisCamera =  this.transform.GetChild(0); 
        thisCamera.transform.localPosition = Vector3.zero;
    }

	private IEnumerator moveTo;
	public void TransitCamera(Transform to_camera, float _time, float sLerpOffset = 15f, Action callback=null)
	{
		if(moveTo!=null)
			StopCoroutine(moveTo);

		isTransiting = true;
		from = this.transform;
		to = to_camera;
		time = _time;
		upOffSet = sLerpOffset;
		moveTo = MoveTo(callback);

		StartCoroutine(moveTo);
	}

	private Transform from;
	private Transform to;
	private float time;
	private float upOffSet;

	IEnumerator MoveTo(Action callback) {

		float timer = 0.0f;  

		Vector3 fromPos = from.position;
		Quaternion fromRot = from.rotation;

		//calculate center for slep
		Vector3 center = (fromPos+to.position)*0.5f;
		center -= new Vector3(0,upOffSet,0);

		//edit center value to a perpendicular direction
		Vector3 a = to.position-fromPos;
		Vector3 b = center - fromPos;
		Vector3 bi_norm = Vector3.Cross(a,b);
		Vector3 norm = Vector3.Cross(a,bi_norm);
		norm.Normalize();
		center = (fromPos+to.position)*0.5f;
		center -= norm*upOffSet;

		//calculate slerp vector
		Vector3 riseRelCenter = fromPos-center;
		Vector3 setRelCenter = to.position-center;


		while (timer <= time) {
			float t = 1.0f + Mathf.Pow((timer / time - 1.0f), 3.0f);
			//from.transform.position = Vector3.Lerp(fromPos, to.position, t);
			from.transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, t);
			from.transform.position += center;
			from.transform.rotation = Quaternion.Slerp(fromRot,to.rotation,t);
			timer += Time.deltaTime;

			yield return null;
		}
		from.transform.position = to.position;
		from.transform.rotation = to.rotation;

		//free camera from any stored pan effect
		cameraLerpPos = thisCamera.transform.localPosition;
		isTransiting = false;

		if(callback!=null) callback();
	}


}
