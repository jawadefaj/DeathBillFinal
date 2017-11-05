using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class SlowMotionBullet : MonoBehaviour {

    public static SlowMotionBullet instance;

    public GameObject bullet;
    public Canvas canvas;
    public float standardMinFactor =10;
    public float standardMaxFactor = 20;
    public float standardDistance = 30;
    float invStandardDistance;
    public float minFactor
    {
        get
        {
            if (distanceFromTarget < standardDistance)
            {
                return standardMinFactor * distanceFromTarget * invStandardDistance;
            }
            else
            {
                return standardMinFactor;
            }
        }        
    }
    public float maxFactor    
    {
        get
        {
            if (distanceFromTarget > standardDistance)
            {
                return standardMaxFactor * distanceFromTarget * invStandardDistance;
            }
            else
            {
                return standardMaxFactor;
            }
        }        
    }
    public Transform bullet_camera;

    private GameObject mainCamera;
    private bool isPlaying = false;
    private bool isDoningEndingTask = false;

    private float distanceFromTarget
    {
        get
        {
            return (destination - startPoint).magnitude;
        }
    }

    private Vector3 destination
    {
        get
        {
            return hitTarget.position;
        }
    }



    private float savedDamagePower =0;
    private RaycastHit savedHitInfo;
    //
    public Transform hitAIroot;
    public Transform hitTarget;
    private Vector3 startPoint;


    void Awake()
    {
        instance = this;
        invStandardDistance = 1 / standardDistance;
    }

    void Start () {

        mainCamera = Camera.main.gameObject;
        bullet_camera.gameObject.SetActive(false);
        bullet.SetActive(false);
    }

    // Update is called once per frame
    float nowDistamce=0f;
    float prevDistance=0f;
    float factor = 10f;

    void Update () {

        if(isPlaying == true)
        {
            //rotate bullet
            bullet.transform.up = (destination-bullet.transform.position).normalized;
            bullet.transform.Rotate(Vector3.up*(1f/Time.timeScale)*Time.deltaTime*1f, Space.Self);

            bullet.transform.Translate(Vector3.up*factor*(1f/Time.timeScale)*Time.deltaTime,Space.Self);

            //calculate time scale
            //Debug.Log(destination);
            nowDistamce = Vector3.Distance(bullet.transform.position,destination);
            //Debug.Log(factor);
            factor = minFactor + (maxFactor-minFactor)*(1f-(nowDistamce/distanceFromTarget));

            if((nowDistamce<0.1f) || (prevDistance-nowDistamce)<0)
            {
                isDoningEndingTask = true;
                StartCoroutine(BulletImpact(savedHitInfo));
                isPlaying = false;
            }

            if(nowDistamce<distanceFromTarget*0.1f)
            {
                if(bullet!= null)
                {
                    bullet_camera.parent = null;
                    bullet_camera.LookAt(bullet.transform);
                }
            }

            prevDistance = nowDistamce;
        }

    }

    private IEnumerator BulletImpact(RaycastHit hit)
    {
        Time.timeScale = 1f;

        //tell them that are hit
        iBulletImpact it = hit.transform.gameObject.GetComponent<iBulletImpact>();
        if(it != null)
            it.TakeImapct(hit, savedDamagePower, HitSource.PLAYER);

        //Deactivate bullet
        bullet.SetActive(false);

        yield return new WaitForSeconds(1f);
        mainCamera.GetComponent<Camera>().enabled = true;
        mainCamera.GetComponent<AudioListener>().enabled = true;

        bullet_camera.transform.parent = null;
        bullet_camera.gameObject.SetActive(false);

        canvas.gameObject.SetActive(true);

        isDoningEndingTask = false;
    }

    public void PlaySlowMotionBullet(Vector3 fromPoint,float damagePower, Transform hitAIroot,  RaycastHit sampleHit)
    {
        //target setup
        this.hitAIroot = hitAIroot;
        hitTarget = new GameObject().transform;
        hitTarget.position = sampleHit.point;
        hitTarget.parent = hitAIroot;

        startPoint = fromPoint;

        //Vector3 toPoint = sampleHit.point;
        //calculate distance
        //destination = toPoint;
        //distanceFromTarget = Vector3.Distance(fromPoint,toPoint);
        savedDamagePower = damagePower;
        savedHitInfo = sampleHit;

        //CameraController.instance.sto

        Time.timeScale = 0.1f;

        //reset some settings
        factor = minFactor;
        nowDistamce =0;
        prevDistance =float.MaxValue;

        //bullet instantiate
        bullet.SetActive(true);
        bullet.transform.position = fromPoint;
        bullet.transform.up = (destination-fromPoint).normalized;

        //camera
        //bullet_camera = (new GameObject()).transform;
        //bullet_camera.gameObject.AddComponent<Camera>();
        //bullet_camera.gameObject.AddComponent<MotionBlur>();
        //bullet_camera.gameObject.GetComponent<Camera>().depth = 10;
        bullet_camera.gameObject.SetActive(true);
        bullet_camera.transform.position = fromPoint - ((destination-fromPoint).normalized)*2f;
        bullet_camera.transform.Translate(Vector3.up*0.5f,Space.Self);
        bullet_camera.transform.Translate(Vector3.right*0.5f,Space.Self);
        bullet_camera.transform.LookAt(bullet.transform.position);
        bullet_camera.transform.transform.parent = bullet.transform;

        mainCamera.GetComponent<Camera>().enabled = false;
        mainCamera.GetComponent<AudioListener>().enabled = false;

        //turn off canvas
        canvas.gameObject.SetActive(false);



        //prevent all other input
        RapidFireButton.instance.pressedOn = false;

        isPlaying = true;
        isDoningEndingTask = false;
    }

    public bool IsSlowMotionOn()
    {
        return (isPlaying || isDoningEndingTask);
    }
}
