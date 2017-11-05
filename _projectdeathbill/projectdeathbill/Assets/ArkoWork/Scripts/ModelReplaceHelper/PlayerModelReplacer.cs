using UnityEngine;
using System.Collections;

public class PlayerModelReplacer : MonoBehaviour {

    public GameObject modelPrefabReference;
    public GameObject gunPrefabReference;
    public bool useNewGunModel = false;
   
    //[HideInInspector]
    public ThirdPersonController playerController;
    //[HideInInspector]
    public ModelStructure oldBodyReferences;
    //[HideInInspector]
    public ModelStructure newBodyReferences;

    public GameObject newModelRef;
}
