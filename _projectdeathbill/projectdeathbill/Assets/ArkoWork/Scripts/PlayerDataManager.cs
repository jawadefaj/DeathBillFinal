using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerDataManager : MonoBehaviour {

    public List<PlayerDataStruct> dataList = new List<PlayerDataStruct>();

    public static PlayerDataManager instance;

    void Awake()
    {
        instance = this;
    }

    public static PlayerDataStruct GetPlayerDataStruct(FighterName _name)
    {
        if(PlayerDataManager.instance ==null)
        {
            Debug.LogError("No Player Data structure manager is added on the scene");
            return new PlayerDataStruct();
        }

        for (int i = 0; i < PlayerDataManager.instance.dataList.Count; i++)
        {
            if (PlayerDataManager.instance.dataList[i].fName == _name)
                return PlayerDataManager.instance.dataList[i];
        }

        Debug.LogError("No data exist for the desired role "+ _name.ToString());
        return new PlayerDataStruct();
    }
}

[System.Serializable]
public struct PlayerDataStruct
{
    public FighterName fName;
    public Vector3 headCameraPos;
    public Vector3 shoulderCameraPos;

    public Quaternion headCameraRot;
    public Quaternion shoulderCameraRot;

    public float personalZoomValue;

    public void SetPersonalZoomValue(float v)
    {
        this.personalZoomValue = v;
    }
}
