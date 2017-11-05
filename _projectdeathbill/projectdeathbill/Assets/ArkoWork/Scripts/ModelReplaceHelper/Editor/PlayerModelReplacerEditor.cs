using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using SWS;
using Portbliss.Station;
using RootMotion.FinalIK;

[CustomEditor(typeof(PlayerModelReplacer))]
public class PlayerModelReplacerEditor : Editor {

    private PlayerModelReplacer modelReplacer;
    private GameObject newPlayerModel
    {
        get
        {
            return modelReplacer.newModelRef;
        }
        set
        {
            modelReplacer.newModelRef = value;
        }
    }
    //private GameObject newGunModel;
    private bool viewOldBones = false;
    private bool viewNewBones = false;

    private GameObject oldPlayerModel
    {
        get
        {
            return modelReplacer.gameObject;
        }
    }
    
    private ModelStructure oldModelStructure
    {
        get
        {
            return modelReplacer.oldBodyReferences;
        }
    }

    private ModelStructure newModelStructure
    {
        get
        {
            return modelReplacer.newBodyReferences;
        }
    }

    private ThirdPersonController oldPlayerController
    {
        get
        {
            if (modelReplacer.playerController != null)
                return modelReplacer.playerController;
            else
            {
                Debug.LogError("Old model ThirdPersonController not found");
                return null;
            }
        }
    }

    private Transform oldGunModel
    {
        get
        {
            if (oldPlayerController != null)
            {
                return oldPlayerController.assignedWeapon.transform;
            }
            else
            {
                Debug.LogError("Old model Gun not found");
                return null;
            }
        }
    }

    void OnEnable()
    {
        modelReplacer = (PlayerModelReplacer)target;
    }

    public override void OnInspectorGUI()
    {
        SerializedObject s_object = new SerializedObject(target);
        SerializedProperty modelPrefabRef = s_object.FindProperty("modelPrefabReference");
        SerializedProperty gunPrefabRef = s_object.FindProperty("gunPrefabReference");
        SerializedProperty OldObjboneList= s_object.FindProperty("oldBodyReferences.bones");
        SerializedProperty newObjboneList = s_object.FindProperty("newBodyReferences.bones");
        SerializedProperty useNewGunModel = s_object.FindProperty("useNewGunModel");

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(modelPrefabRef, new GUIContent(modelPrefabRef.name));
        EditorGUILayout.PropertyField(useNewGunModel, new GUIContent("Use New Gun Model"));

        if(useNewGunModel.boolValue)
            EditorGUILayout.PropertyField(gunPrefabRef, new GUIContent(gunPrefabRef.name));


        EditorGUILayout.Space();
        viewOldBones = EditorGUILayout.Foldout(viewOldBones, "Old Bone Structure");
        if (viewOldBones)
        {
            for (int i = 0; i < OldObjboneList.arraySize; i++)
            {
                SerializedProperty bone = OldObjboneList.GetArrayElementAtIndex(i);
                SerializedProperty boneTransform = bone.FindPropertyRelative("_boneRef");
                SerializedProperty name = bone.FindPropertyRelative("_name");
                EditorGUILayout.PropertyField(boneTransform, new GUIContent(name.stringValue));
            }
        }


        EditorGUILayout.Space();
        viewNewBones =  EditorGUILayout.Foldout(viewNewBones, "New Bone Structure");
        if (viewNewBones)
        {
            for (int i = 0; i < newObjboneList.arraySize; i++)
            {
                SerializedProperty bone = newObjboneList.GetArrayElementAtIndex(i);
                SerializedProperty boneTransform = bone.FindPropertyRelative("_boneRef");
                SerializedProperty name = bone.FindPropertyRelative("_name");
                EditorGUILayout.PropertyField(boneTransform, new GUIContent(name.stringValue));
            }
        }

        if (modelReplacer.modelPrefabReference != null)
        {
            if (GUILayout.Button("Load Model",GUILayout.Height(30)))
            {
                LoadModel();
            }

            if (GUILayout.Button("Transfer data",GUILayout.Height(30)))
            {
                TransferData();
            }


        }
        else
        {
            EditorGUILayout.HelpBox("Add a new Model to initiate transfer process", MessageType.Info);
        }

        s_object.ApplyModifiedProperties();
    }

    private void LoadModel()
    {
        GameObject modelOb = Instantiate(modelReplacer.modelPrefabReference);
        modelOb.name = "New_"+ modelReplacer.gameObject.name;
        modelOb.transform.parent = modelReplacer.transform;
        modelOb.transform.localPosition = Vector3.zero;
        modelOb.transform.localRotation = Quaternion.identity;
        modelOb.transform.parent = null;
        newPlayerModel = modelOb;

        modelReplacer.playerController = modelReplacer.gameObject.GetComponent<ThirdPersonController>();

        LoadSkeleton();
    }

    private void LoadSkeleton()
    {
        oldModelStructure.root = modelReplacer.transform;
        FillModel(oldModelStructure);
        oldModelStructure.gun = oldGunModel;

        newModelStructure.root = newPlayerModel.transform;
        FillModel(newModelStructure);

        viewNewBones = true;
        viewOldBones = true;
    }


    private void TransferData()
    {
        SetUp_Animator();
        SetUp_Colliders();
        SetUp_GunModel();
        SetUp_RootComponents();
    }



    #region SETUP
    private void SetUp_Animator()
    {
        Animator newAnimator = newPlayerModel.GetComponent<Animator>();

        if (newAnimator == null)
        {
            Debug.LogWarning("Your model does not contain any Animator component.");
            newAnimator = newPlayerModel.AddComponent<Animator>();
        }

        Avatar av = newAnimator.avatar;

        CopyComponent(newPlayerModel, oldPlayerModel, typeof(Animator));

        newAnimator.avatar = av;
    }

    private void SetUp_Colliders()
    {
        List<Transform> colliders = FindTransformContainingComponent(oldPlayerModel.transform, typeof(PlayerBulletImpactTaker));

        foreach (Transform t in colliders)
            RecreateSimilar_AtNew(t, false);
    }


    private void SetUp_GunModel()
    {
        //gun creation
        if (modelReplacer.useNewGunModel)
        {
            //if the gun model is provided then use that or assume that the gun model is already attached in the model
            if (modelReplacer.gunPrefabReference != null)
            {
                newModelStructure.gun = RecreateSimilar_AtNew(oldModelStructure.gun, false, modelReplacer.gunPrefabReference);
            }
            else
            {
                //we are assuming that you have given us the referenece already
                if(newModelStructure.gun==null)
                    Debug.LogError("The gun model refenrence is not assigned. Setting up model will terminate and will throw some error.");
            }

            //create the particles and shooting target point also
            GameObject new_particleSystem;
            GameObject new_lineRenderer;
            GameObject new_shellExtractionPoint;

            Weapon old_weapon = oldPlayerModel.GetComponentInChildren<Weapon>();
            BulletShellGenarator old_bsg = oldPlayerModel.GetComponent<BulletShellGenarator>();

            new_particleSystem = (GameObject)Instantiate(old_weapon.muzzleFlash.gameObject, old_weapon.muzzleFlash.transform.position, old_weapon.muzzleFlash.transform.rotation);
            new_lineRenderer = (GameObject)Instantiate(old_weapon.lineRenderer.gameObject, old_weapon.lineRenderer.transform.position, old_weapon.lineRenderer.transform.rotation);
            new_shellExtractionPoint = (GameObject)Instantiate(old_bsg.shellInstantiatePoint.gameObject, old_bsg.shellInstantiatePoint.position, old_bsg.shellInstantiatePoint.rotation);

            new_particleSystem.transform.SetParent(newModelStructure.gun);
            new_lineRenderer.transform.SetParent(newModelStructure.gun);
            new_shellExtractionPoint.transform.SetParent(newModelStructure.gun);

            new_shellExtractionPoint.name = old_bsg.shellInstantiatePoint.name;

            //fix references
            CopyComponent(newModelStructure.gun.gameObject,oldModelStructure.gun.gameObject,typeof(Weapon));

            Weapon new_weapon = newPlayerModel.GetComponentInChildren<Weapon>();
            new_weapon.muzzleFlash = newPlayerModel.GetComponentInChildren<ParticleSystem>();
            new_weapon.lineRenderer = newPlayerModel.GetComponentInChildren<LineRenderer>();
        }
        else
        {
            //use gun from old model
            newModelStructure.gun = RecreateSimilar_AtNew(oldModelStructure.gun, false, oldModelStructure.gun.gameObject);
        }
      
    }

    private void SetUp_RootComponents()
    {
        //copy common component
        CopyComponents(newPlayerModel, oldPlayerModel, new System.Type[]
            {
                typeof(ThirdPersonController),
                typeof(BulletShellGenarator),
                typeof(splineMove),
                typeof(StationController),
                typeof(MoveAnimator),
                typeof(Recoil)
            });

        //Create other gameobjects
        Transform h_camera = RecreateSimilar_AtNew(oldPlayerController.headViewCamera, false);
        Transform s_camera = RecreateSimilar_AtNew(oldPlayerController.shoulderViewCamera, false);
        Transform newTarget = RecreateSimilar_AtNew(oldPlayerController.targetReference, true);

        //setup Third Person Controller
        ThirdPersonController tpc = newPlayerModel.GetComponent<ThirdPersonController>();
        Weapon weapon = newModelStructure.gun.GetComponent<Weapon>();
        tpc.headViewCamera = h_camera;
        tpc.shoulderViewCamera = s_camera;
        tpc.targetReference = newTarget;
        tpc.assignedWeapon = weapon;

        /*string tag = newModelStructure.hip.name;
        string[] parts = tag.Split(':');
        if (parts.Length == 2)
            tag = parts[0];
        else
        {
            tag = "";
            Debug.LogWarning("No tag found for the model");
        }
        tpc.modelTag = tag;*/

        tpc.modelStructure = newModelStructure;

        //Setup bullet extraction point
        BulletShellGenarator bsg = newPlayerModel.GetComponent<BulletShellGenarator>();
        string oldShellExtPointName = oldPlayerModel.GetComponent<BulletShellGenarator>().shellInstantiatePoint.name;
        bsg.shellInstantiatePoint = newModelStructure.gun.transform.Find(oldShellExtPointName);
        if (bsg.shellInstantiatePoint == null)
            Debug.LogWarning("No shell extraction point found");

        //Setup Aim IK
        AimIK aimik = newPlayerModel.GetComponent<AimIK>();
        if (aimik == null)aimik = newPlayerModel.gameObject.AddComponent<AimIK>();
        aimik.solver.transform = newModelStructure.gun;
        IKSolver.Bone[] bones = new IKSolver.Bone[3]{new IKSolver.Bone(),new IKSolver.Bone(),new IKSolver.Bone()}; 
        bones[0].transform = newModelStructure.spine0;
        bones[1].transform = FindDirect(bones[0].transform,"Spine");
        bones[2].transform = FindDirect(bones[1].transform,"Spine");
        aimik.solver.bones = bones;


        FullBodyBipedIK fbbik = newModelStructure.root.GetComponent<FullBodyBipedIK>();
        if (fbbik == null)fbbik = newModelStructure.root.gameObject.AddComponent<FullBodyBipedIK>();

        Recoil recoil = newModelStructure.root.GetComponent<Recoil>();
        if (recoil != null)
        {
            recoil.SetIKRefs(aimik,fbbik);
        }
    }

    #endregion

    #region model Loading
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

    static Transform FindDirect(Transform mother, string name)
    {
        foreach (Transform tr in mother)
        {
            if (tr.name.Contains(name))
                return tr;
        }
        return null;
    }

    static List<Transform> FindTransformContainingComponent(Transform mother, System.Type t)
    {
        List<Transform> t_list = new List<Transform>();

        Component[] components = mother.GetComponentsInChildren(t);
        foreach (Component c in components)
            t_list.Add(c.transform);

        return t_list;
    }

    static void FillModel(ModelStructure model)
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
    #endregion


    #region duplication
    void CopyComponents(Transform destination, Transform original, System.Type[] types)
    {
        CopyComponents(destination.gameObject,original.gameObject,types);
    }
    void CopyComponents(GameObject destination, GameObject original, System.Type[] types)
    {
        foreach (System.Type t in types)
        {
            CopyComponent(destination,original,t);
        }
    }
    void CopyComponent(GameObject target, GameObject sample, System.Type type)
    {
        Component comp = target.GetComponent(type); 
        if (comp == null) comp = target.AddComponent(type);
        EditorUtility.CopySerialized(sample.GetComponent(type),comp);
    }
    #endregion
    #region Specific Duplication
    Transform RecreateSimilar_AtNew(Transform t, bool isEmpty, GameObject objToCopy=null)
    {
        //Debug.Log(t.parent.name);
        Transform newParent = ModelStructure.FindEquivalentTransform(newModelStructure,oldModelStructure,t.parent);
        //Debug.Log(newParent);
        return Handy.Duplicate_WithLocalVals(objToCopy==null?t:objToCopy.transform,newParent,isEmpty);
    }
    #endregion
}
