using UnityEngine;
using System.Collections;

public class StaticObjectRemover : MonoBehaviour {

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void OnApplicationQuit()
    {
        //Remove all static object reference here
        //UserGameData.instance = null;
    }
}
