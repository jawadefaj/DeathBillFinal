using UnityEngine;
using System;
using System.Collections;

public class ChopperDropArea : MonoBehaviour {
    public ZoneBlock attachedZoneBlock;


    float timeDif;
	int enemyCount;
	float varyWithin;
    Transform spwnerPoint1;
    Transform spwnerPoint2;
    Transform enemySample;

    Action firstGuydroppedAct;
    Action readyToLeaveAct;
    int tempCount;

    public void StartTheMachinary(int howMany, float interval, float variance, Transform chopperBody, Action OnFirstGuyDropped, Action OnReadyToLeave)
    {
        Debug.Log("Machinary!");
        enemyCount = howMany;
        timeDif = interval;
        varyWithin = variance;
        tempCount = 0;
        spwnerPoint1 = chopperBody.GetChild(1);
        spwnerPoint2 = chopperBody.GetChild(2);
        enemySample = chopperBody.GetChild(3);

        firstGuydroppedAct = OnFirstGuyDropped;
        readyToLeaveAct = OnReadyToLeave;

        StartCoroutine (GetTheRope());

    }

	IEnumerator GetTheRope()
	{
		
		yield return new WaitForSeconds (1f);

//		rope1 = curChopper.gameObject.transform.GetChild (0).gameObject;
        spwnerPoint1.gameObject.SetActive (true);
        spwnerPoint1.transform.rotation = Quaternion.identity;
//		print (rope1.name);
//
//		rope2 = curChopper.gameObject.transform.GetChild (1).gameObject;
        spwnerPoint2.gameObject.SetActive (true);
        spwnerPoint2.transform.rotation = Quaternion.identity;
//		print (rope2.name);

		yield return new WaitForSeconds (.5f);
		StartCoroutine(repeatClimbing());
	}
    IEnumerator LoseTheRope()
    {
        yield return new WaitForSeconds (1f);

        //      rope1 = curChopper.gameObject.transform.GetChild (0).gameObject;
        spwnerPoint1.gameObject.SetActive (false);
        //      print (rope1.name);
        //
        //      rope2 = curChopper.gameObject.transform.GetChild (1).gameObject;
        spwnerPoint2.gameObject.SetActive (false);
        //      print (rope2.name);

        yield return new WaitForSeconds (.5f);
        if (readyToLeaveAct != null)
            readyToLeaveAct();
    }

//	void OnTriggerEnter(Collider col)
//	{
//        Debug.Log("triggering.........");
//		tempCount++;
//        Debug.Log(tempCount);
//        if (tempCount == 1)
//        {
//            if (firstGuydroppedAct != null)
//            {
//                firstGuydroppedAct();
//            }
//            //Debug.Log("first guy");
//        }
//        else if (tempCount == enemyCount) {
//            StartCoroutine(LoseTheRope());
//		}
//	}
    void ActOnTriggerEnter()
    {
        tempCount++;
        if (tempCount == 1)
        {
            if (firstGuydroppedAct != null)
            {
                firstGuydroppedAct();
            }
            //Debug.Log("first guy");
        }
        else if (tempCount == enemyCount) {
            StartCoroutine(LoseTheRope());
        }
    }

	IEnumerator repeatClimbing()
	{
		

		for (int i = 0; i < enemyCount; i++) {
			
			float updateDif = timeDif + timeDif * UnityEngine.Random.Range (-varyWithin,varyWithin);
            //updateDif = Handy.Deviate(timeDif,varyWithin);
			//print ("Time differece : " + updateDif);
            GameObject go;
			if (i % 2 == 0) {
                go = Pool.Instantiate (enemySample.gameObject, spwnerPoint1.position, Quaternion.identity);
                go.GetComponent<ChopperEnemyControl>().Init(ActOnTriggerEnter,spwnerPoint1);
            } else {
                go = Pool.Instantiate (enemySample.gameObject, spwnerPoint2.position, Quaternion.identity);
                go.GetComponent<ChopperEnemyControl>().Init(ActOnTriggerEnter,spwnerPoint2);
            }

			yield return new WaitForSeconds (updateDif);
		}


	}
}
