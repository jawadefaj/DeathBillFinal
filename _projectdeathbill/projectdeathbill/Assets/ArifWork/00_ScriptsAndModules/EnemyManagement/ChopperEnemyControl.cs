using UnityEngine;
using System.Collections;

public class ChopperEnemyControl : MonoBehaviour {

    float time = 0;
	float climbingSpeed;
	float rotationSpeed;
	int randomNum;

    System.Action actOnTriggerEnter;
    Transform prnt;
    public void Init(System.Action actOnTriggerEnter, Transform par)
    {
        this.gameObject.SetActive(true);
        prnt = par;
        this.transform.parent = par;
        this.transform.localPosition = new Vector3(0,0,0);

        this.gameObject.transform.Rotate (this.transform.up*Random.Range(180.0f,-180.0f));


        climbingSpeed = Handy.Deviate(4f,0.05f);
        rotationSpeed = Handy.Deviate(60f,0.5f);

        randomNum = Random.Range (1,80);
        if (randomNum % 2 == 0) {
            rotationSpeed *= -1;
        }
        this.actOnTriggerEnter = actOnTriggerEnter;
    }
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;
	    this.gameObject.transform.Rotate (Vector3.up * Time.deltaTime * rotationSpeed);
        Vector3 vec = this.transform.localPosition;
        vec.y -= (Time.deltaTime * climbingSpeed);
        this.transform.localPosition = vec;
	}


	void OnTriggerEnter(Collider col)
	{
        if (actOnTriggerEnter != null)
            actOnTriggerEnter();
//		stop = true;
        Pool.Destroy(this.gameObject);
	}
}
