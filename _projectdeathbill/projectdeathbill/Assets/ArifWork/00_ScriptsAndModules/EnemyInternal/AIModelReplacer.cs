using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class AIModelReplacer : MonoBehaviour 
{
    public GameObject modelPrefabReference;
    public GameObject gunPrefabReference;
    [SerializeField]public AIPersonnel personnelScript;
    [SerializeField]public AIModelManager newModelManScript;
    public ModelStructure oldBodyReferences;
    public ModelStructure newBodyReferences;

}

[System.Serializable]
public struct BoneData
{
    public string _name;
    public Transform _boneRef;

    public BoneData(string name, Transform trans)
    {
        this._name = name;
        this._boneRef = trans;
    }
}
[System.Serializable]
public class ModelStructure
{
    public BoneData[] bones = new BoneData[]{
        new BoneData("Root", null),
        new BoneData("Hips", null),
        new BoneData("LeftUpLeg", null),
        new BoneData("LeftLeg", null),
        new BoneData("RightUpLeg", null),
        new BoneData("RightLeg", null),
        new BoneData("LeftArm", null),
        new BoneData("LeftForeArm", null),
        new BoneData("LeftHand", null),
        new BoneData("RightArm", null),
        new BoneData("RightForeArm", null),
        new BoneData("RightHand", null),
        new BoneData("RightHandThumb1", null),
        new BoneData("Spine", null),
        new BoneData("Spine1", null),
        new BoneData("Spine2", null),
        new BoneData("Head", null), 
        new BoneData("Gun", null)
    };

    public Transform root               {  get{ return bones[0]._boneRef;}      set{bones[0]._boneRef = value;}   }
    public Transform hip                {  get{ return bones[1]._boneRef;}      set{bones[1]._boneRef = value;}   }
    public Transform leftUpLeg          {  get{ return bones[2]._boneRef;}      set{bones[2]._boneRef = value;}   }
    public Transform leftLeg            {  get{ return bones[3]._boneRef;}      set{bones[3]._boneRef = value;}   }
    public Transform rightUpLeg         {  get{ return bones[4]._boneRef;}      set{bones[4]._boneRef = value;}   }
    public Transform rightLeg           {  get{ return bones[5]._boneRef;}      set{bones[5]._boneRef = value;}   }
    public Transform leftArm            {  get{ return bones[6]._boneRef;}      set{bones[6]._boneRef = value;}   }
    public Transform leftForeArm        {  get{ return bones[7]._boneRef;}      set{bones[7]._boneRef = value;}   }
    public Transform leftHand           {  get{ return bones[8]._boneRef;}      set{bones[8]._boneRef = value;}   }
    public Transform rightArm           {  get{ return bones[9]._boneRef;}      set{bones[9]._boneRef = value;}   }
    public Transform rightForeArm       {  get{ return bones[10]._boneRef;}     set{bones[10]._boneRef = value;}   }
    public Transform rightHand          {  get{ return bones[11]._boneRef;}     set{bones[11]._boneRef = value;}   }
    public Transform rightHandThumb1    {  get{ return bones[12]._boneRef;}     set{bones[12]._boneRef = value;}   }
    public Transform spine0             {  get{ return bones[13]._boneRef;}     set{bones[13]._boneRef = value;}   }
    public Transform spine1             {  get{ return bones[14]._boneRef;}     set{bones[14]._boneRef = value;}   }
    public Transform spine2             {  get{ return bones[15]._boneRef;}     set{bones[15]._boneRef = value;}   }
    public Transform head               {  get{ return bones[16]._boneRef;}     set{bones[16]._boneRef = value;}   }
    public Transform gun                {  get{ return bones[17]._boneRef;}     set{bones[17]._boneRef = value;}   }

    public static Transform FindEquivalentTransform(ModelStructure findFrom, ModelStructure sampleHolder, Transform t)
    {
        for (int i = 0; i < sampleHolder.bones.Length; i++)
        {
            if (t == sampleHolder.bones[i]._boneRef)
            {
                return findFrom.bones[i]._boneRef;
            }
        }
        return null;
    }

    public static void FillModel(ModelStructure model)
    {
        if (model.root == null)
        {
            Debug.LogError("model root not defined!");
            return;
        }
        for (int i = 1; i < model.bones.Length-1; i++)
        {
            if (model.bones[i]._boneRef == null)
            {
                model.bones[i]._boneRef = FindUnder(model.root, model.bones[i]._name);
                if (model.bones[i]._boneRef == null)
                    Debug.LogError(model.bones[i]._name + " not set!");
            }
        }
    }

    static Transform FindUnder(Transform mother, string name)
    {
        if (mother.childCount == 0)
        {
            return null;
        }
        else
        {
            foreach (Transform tr in mother) {
                if (tr.name.Contains(name))
                    return tr;
                else
                {
                    Transform found = FindUnder(tr,name);
                    if (found != null)
                        return found;
                }
            }
        }
        return null;
    }
        
}