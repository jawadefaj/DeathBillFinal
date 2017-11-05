using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FighterProfileDataHolder : MonoBehaviour {

    public static FighterProfileDataHolder instance;

    public FighterProfile[] fighters;

    void Awake()
    {
        instance = this;
    }

    public FighterProfile GetFighterProfile(FighterName fName)
    {
        foreach(FighterProfile f in fighters)
        {
            if (f.fName == fName)
                return f;
        }
        Debug.LogError(string.Format("No fighter by the name {0} is not found", fName.ToString()));
        return new FighterProfile();
    }
}

[System.Serializable]
public struct FighterProfile
{
    public string displayName;
    public FighterName fName;
    public Sprite proPic;
    public Sprite portrait;

    public GunProfile gun;

}

[System.Serializable]
public struct GunProfile
{
    public string name;
    [RangeAttribute(0f,1f)]
    public float damage;
    [RangeAttribute(0f,1f)]
    public float accurecy;
    [RangeAttribute(0f,1f)]
    public float range;
    [RangeAttribute(0f,1f)]
    public float fireRate;
    public int clipSize;

}
