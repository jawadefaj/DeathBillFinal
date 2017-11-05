using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIPlayerDataManager : MonoBehaviour {
    public float scenePlayerAIDamageMultiplier = 0.1f;
    public List<AIPlayerDataStruct> dataList = new List<AIPlayerDataStruct>();

    public static AIPlayerDataManager instance;

    void Awake()
    {
        instance = this;
    }

    public static AIPlayerDataStruct GetDataStruct(FighterName _fName)
    {
        if (AIPlayerDataManager.instance == null)
        {
            Debug.LogError("There is no ai player data manager in the scene");
            return new AIPlayerDataStruct();
        }

        for (int i = 0; i < AIPlayerDataManager.instance.dataList.Count; i++)
        {
            if (AIPlayerDataManager.instance.dataList[i].fName == _fName)
                return AIPlayerDataManager.instance.dataList[i];
        }

        Debug.LogError("No data exist for the desired role "+ _fName.ToString());
        return new AIPlayerDataStruct();
    }
}


[System.Serializable]
public struct AIPlayerDataStruct
{
    public FighterName fName;

    //shooting
    public float aiAccuracy ;

    public int avgBulletFireInAI;
    public float AIBulletFireVariaton ;

    public float avgDelayTimeInAI;
    public float AIDelayVariaton ;

    public float AIMaxShootingRangeSq;

    //Regen Rate
    public float regenRate_Aggressive; //aim, fire, nade
    public float regenRate_NonAggressive ; //hide,run, reloading
    public float regenRate_OffControl ; // not active player
    public float regenRate_ai ;

    public void SetDefaultValue()
    {
        aiAccuracy = 0.4f;
        avgBulletFireInAI = 10;
        AIBulletFireVariaton = 0.1f;
        avgDelayTimeInAI = 1f;
        AIDelayVariaton = 0.1f;
        AIMaxShootingRangeSq = 400f;
        regenRate_Aggressive = 1f;
        regenRate_NonAggressive = 5f;
        regenRate_OffControl = 8f;
        regenRate_ai = 2.5f;
    }
}
