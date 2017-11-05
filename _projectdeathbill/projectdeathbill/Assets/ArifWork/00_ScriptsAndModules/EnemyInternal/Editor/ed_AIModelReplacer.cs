using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using Portbliss.EditorTools;

[CustomEditor(typeof(AIModelReplacer))]
public class ed_AIModelReplacer : Editor {
    AIModelReplacer scriptRef;
    #region properties
    ModelStructure modelOld
    {
        get
        {
            return scriptRef.oldBodyReferences;
        }
    }
    ModelStructure modelNew
    {
        get
        {
            return scriptRef.newBodyReferences;
        }
    }

    AIPersonnel personnelScript{ 
        set{
            if (scriptRef != null)
                scriptRef.personnelScript = value;
        }
        get
        {
            if (scriptRef != null)
                return scriptRef.personnelScript;
            else
                return null;
        }
    }
    AIModelManager modelManNew{ 
        set{
            if (scriptRef != null)
                scriptRef.newModelManScript = value;
        }
        get
        {
            if (scriptRef != null)
                return scriptRef.newModelManScript;
            else
                return null;
        }
    }
    AIModelManager modelManInUse{ 
        get
        {
            if (scriptRef != null)
                return scriptRef.personnelScript.selfModel;
            else
                return null;
        }
    }
    #endregion

    bool viewOldBones = false;
    bool viewNewBones = false;

    void OnEnable()
    {
        scriptRef = (AIModelReplacer) target;
        personnelScript = scriptRef.GetComponent<AIPersonnel>();
        if (modelOld != null)
        {
            modelOld.root = personnelScript.selfModel.transform;
            FillModel(modelOld);
            modelOld.gun = personnelScript.selfModel.Gun;
        }
    }
    #region inspector setup
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        SerializedObject s_object = new SerializedObject(target);
        SerializedProperty modelPrefabRef = s_object.FindProperty("modelPrefabReference");
        SerializedProperty gunPrefabRef = s_object.FindProperty("gunPrefabReference");
        //SerializedProperty personelScript = s_object.FindProperty("personnelScript");
        //SerializedProperty newModelManScript = s_object.FindProperty("newModelManScript");
        SerializedProperty OldObjboneList= s_object.FindProperty("oldBodyReferences.bones");
        SerializedProperty newObjboneList = s_object.FindProperty("newBodyReferences.bones");

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(modelPrefabRef, new GUIContent("New Model (FBX/Prefab)"));
        EditorGUILayout.PropertyField(gunPrefabRef, new GUIContent("New Gun (FBX/Prefab)"));
        //EditorGUILayout.PropertyField(personelScript, new GUIContent(personelScript.name));
        //EditorGUILayout.PropertyField(newModelManScript, new GUIContent(newModelManScript.name));

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

        if (scriptRef.newModelManScript != null)
        {
            EditorGUILayout.Space();
            viewNewBones = EditorGUILayout.Foldout(viewNewBones, "New Bone Structure");
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
        }

        if(scriptRef.newModelManScript == null) 
        {
            if (scriptRef.modelPrefabReference != null)if (GUILayout.Button("Add Model", GUILayout.Height(30)))
            {
                AddModelFromPrefab();
            }
        }
        else{
            if(GUILayout.Button("TransferData",GUILayout.Height(30)))
            {
                TransferData();
            }

            if(GUILayout.Button("FinishUp",GUILayout.Height(30)))
            {
                FinishUp();
            }
        }


        s_object.ApplyModifiedProperties();
    }
    #endregion

    void FinishUp()
    {
        modelNew.root.gameObject.SetLayer("Enemy",true);
        GameObject oldref = personnelScript.selfModel.gameObject;
        personnelScript.selfModel = scriptRef.newModelManScript;
        scriptRef.newModelManScript = null;
        personnelScript.muzzleFlashParticle = FindUnder(modelNew.gun,"Particle System").GetComponent<ParticleSystem>();
        personnelScript.headReference = modelNew.head;

        scriptRef.oldBodyReferences = scriptRef.newBodyReferences;
        scriptRef.newBodyReferences = new ModelStructure();

        if (EditorUtility.DisplayDialog("Irreversible Process!!!", "Do you want to remove old model?", "i am feeling lucky!", "what? no!"))
        {
            DestroyImmediate(oldref);
        }
        else
        {
            oldref.SetActive(false);
        }
    }
        
    void AddModelFromPrefab()
    {
        GameObject modelOb = Instantiate(scriptRef.modelPrefabReference);
        modelOb.name = "AI_Model";
        modelOb.transform.parent = personnelScript.transform;
        modelOb.transform.localPosition = Vector3.zero;
        modelOb.transform.localRotation = Quaternion.identity;
        modelManNew = modelOb.AddComponent<AIModelManager>();
        LoadSkeleton();

    }

    void TransferData()
    {
        Duplicate_RootNode();
        Duplicate_Hips();
        Duplicate_LeftUpLeg();
        Duplicate_LeftLeg();
        Duplicate_RightUpLeg();
        Duplicate_RightLeg();
        Duplicate_Spine();
        Duplicate_LeftArm();
        Duplicate_LeftForeArm();
        Duplicate_RightArm();
        Duplicate_RightForeArm();
        Duplicate_Head();


    }
    void LoadSkeleton()
    {
        modelOld.root = personnelScript.selfModel.transform;
        FillModel(modelOld);
        modelOld.gun = personnelScript.selfModel.Gun;
        modelNew.root = modelManNew.transform;
        FillModel(modelNew);
    }



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
    Transform RecreateSimilar_AtNew(Transform t, bool isEmpty)
    {
        Transform newParent = ModelStructure.FindEquivalentTransform(modelNew,modelOld,t.parent);
        return Handy.Duplicate_WithLocalVals(t,newParent,isEmpty);
    }
    #region added
    void Duplicate_RootNode()
    {
        if (modelOld.root == null || modelNew.root == null)
        {
            Debug.LogError("One of the root nodes are missing reference");
            return;
        }
        Animator animNew = modelNew.root.GetComponent<Animator>();
        Avatar newAnimAvtar = animNew.avatar;

        CopyComponents(modelNew.root,modelOld.root, new System.Type[]
            {
                typeof(Transform),
                typeof(Animator),
                typeof(AIModelManager),
                //typeof(RootMotion.FinalIK.AimIK),
                //typeof(RootMotion.FinalIK.FullBodyBipedIK),
                typeof(RootMotion.FinalIK.Recoil)
            }
        );
        animNew.avatar = newAnimAvtar;
        AIModelManager aimm = modelNew.root.GetComponent<AIModelManager>();
        aimm.nadeReleasePoint = RecreateSimilar_AtNew(aimm.nadeReleasePoint, true);
        aimm.GunPositionRightHand = RecreateSimilar_AtNew(aimm.GunPositionRightHand,true);
        aimm.GunPositionLeftHand = RecreateSimilar_AtNew(aimm.GunPositionLeftHand,true);
        aimm.Gun = Handy.Duplicate_WithLocalVals(aimm.Gun,aimm.GunPositionRightHand,true);
        modelNew.gun = aimm.Gun.transform;
        if (scriptRef.gunPrefabReference != null)
        {
            Transform gm = modelNew.gun.FindChild("GunMesh");
            if (gm == null)
            {
                GameObject go = GameObject.Instantiate(scriptRef.gunPrefabReference);
                go.name = "GunMesh";
                go.transform.parent = modelNew.gun;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
            }
        }
        aimm.lineRenderer = RecreateSimilar_AtNew(aimm.lineRenderer.transform, false).GetComponent<LineRenderer>();

        AimIK aimik = modelNew.root.GetComponent<AimIK>();
        if (aimik == null)aimik = modelNew.root.gameObject.AddComponent<AimIK>();
        aimik.solver.transform = modelNew.gun;
        IKSolver.Bone[] bones = new IKSolver.Bone[3]{new IKSolver.Bone(),new IKSolver.Bone(),new IKSolver.Bone()}; 
        bones[0].transform = modelNew.spine0;
        bones[1].transform = FindDirect(bones[0].transform,"Spine");
        bones[2].transform = FindDirect(bones[1].transform,"Spine");
        aimik.solver.bones = bones;


        FullBodyBipedIK fbbik = modelNew.root.GetComponent<FullBodyBipedIK>();
        if (fbbik == null)fbbik = modelNew.root.gameObject.AddComponent<FullBodyBipedIK>();

        Recoil recoil = modelNew.root.GetComponent<Recoil>();
        if (recoil != null)
        {
            recoil.SetIKRefs(aimik,fbbik);
        }
        //modelNew.root.GetComponent<FullBodyBipedIK>().;

    }
    void Duplicate_Hips()
    {
        if (modelOld.hip == null || modelNew.hip == null)
        {
            Debug.LogError("One of the hip nodes are missing reference");
            return;
        }

        CopyComponents(modelNew.hip,modelOld.hip, new System.Type[]
            {
                typeof(Rigidbody),
                typeof(BoxCollider),
                typeof(IBI_EnemyLimbs)
            }
        );
         }
    void Duplicate_LeftUpLeg()
    {
        if (modelOld.leftUpLeg == null || modelNew.leftUpLeg == null)
        {
            Debug.LogError("One of the leftUpLeg nodes are missing reference");
            return;
        }

        CopyComponents(modelNew.leftUpLeg,modelOld.leftUpLeg, new System.Type[]
            {
                typeof(Rigidbody),
                typeof(CharacterJoint),
                typeof(CapsuleCollider),
                typeof(IBI_EnemyBody)
            }
        );
        modelNew.leftUpLeg.GetComponent<CharacterJoint>().connectedBody = modelNew.hip.GetComponent<Rigidbody>();
    }
    void Duplicate_LeftLeg()
    {
        if (modelOld.leftLeg == null || modelNew.leftLeg == null)
        {
            Debug.LogError("One of the leftLeg nodes are missing reference");
            return;
        }

        CopyComponents(modelNew.leftLeg,modelOld.leftLeg, new System.Type[]
            {
                typeof(Rigidbody),
                typeof(CharacterJoint),
                typeof(CapsuleCollider),
                typeof(IBI_EnemyBody)
            }
        );
        modelNew.leftLeg.GetComponent<CharacterJoint>().connectedBody = modelNew.leftUpLeg.GetComponent<Rigidbody>();
    }
    #endregion

    void Duplicate_RightUpLeg()
    {
        if (modelOld.rightUpLeg == null || modelNew.rightUpLeg == null)
        {
            Debug.LogError("One of the rightUpLeg nodes are missing reference");
            return;
        }

        CopyComponents(modelNew.rightUpLeg,modelOld.rightUpLeg, new System.Type[]
            {
                typeof(Rigidbody),
                typeof(CharacterJoint),
                typeof(CapsuleCollider),
                typeof(IBI_EnemyBody)
            }
        );
        modelNew.rightUpLeg.GetComponent<CharacterJoint>().connectedBody = modelNew.hip.GetComponent<Rigidbody>();
    }

    void Duplicate_RightLeg()
    {
        if (modelOld.rightLeg == null || modelNew.rightLeg == null)
        {
            Debug.LogError("One of the rightLeg nodes are missing reference");
            return;
        }

        CopyComponents(modelNew.rightLeg,modelOld.rightLeg, new System.Type[]
            {
                typeof(Rigidbody),
                typeof(CharacterJoint),
                typeof(CapsuleCollider),
                typeof(IBI_EnemyBody)
            }
        );
        modelNew.rightLeg.GetComponent<CharacterJoint>().connectedBody = modelNew.rightUpLeg.GetComponent<Rigidbody>();
    }

    void Duplicate_LeftArm()
    {
        if (modelOld.leftArm == null || modelNew.leftArm == null)
        {
            Debug.LogError("One of the leftarm nodes are missing reference");
            return;
        }

        CopyComponents(modelNew.leftArm,modelOld.leftArm, new System.Type[]
            {
                typeof(Rigidbody),
                typeof(CharacterJoint),
                typeof(CapsuleCollider),
                typeof(IBI_EnemyLimbs)
            }
        );
        modelNew.leftArm.GetComponent<CharacterJoint>().connectedBody = modelNew.spine0.GetComponent<Rigidbody>();
    }

    void Duplicate_LeftForeArm()
    {
        if (modelOld.leftForeArm == null || modelNew.leftForeArm == null)
        {
            Debug.LogError("One of the leftforearm nodes are missing reference");
            return;
        }

        CopyComponents(modelNew.leftForeArm,modelOld.leftForeArm, new System.Type[]
            {
                typeof(Rigidbody),
                typeof(CharacterJoint),
                typeof(CapsuleCollider),
                typeof(IBI_EnemyLimbs)
            }
        );
        modelNew.leftForeArm.GetComponent<CharacterJoint>().connectedBody = modelNew.leftArm.GetComponent<Rigidbody>();

    }

    //No component is attached to LeftHand :(

    void Duplicate_RightArm()
    {
        if (modelOld.rightArm == null || modelNew.rightArm == null)
        {
            Debug.LogError("One of the rightarm nodes are missing reference");
            return;
        }

        CopyComponents(modelNew.rightArm,modelOld.rightArm, new System.Type[]
            {
                typeof(Rigidbody),
                typeof(CharacterJoint),
                typeof(CapsuleCollider),
                typeof(IBI_EnemyLimbs)
            }
        );
        modelNew.rightArm.GetComponent<CharacterJoint>().connectedBody = modelNew.spine0.GetComponent<Rigidbody>();

    }

    void Duplicate_RightForeArm()
    {
        if (modelOld.rightForeArm == null || modelNew.rightForeArm == null)
        {
            Debug.LogError("One of the rightforearm nodes are missing reference");
            return;
        }

        CopyComponents(modelNew.rightForeArm,modelOld.rightForeArm, new System.Type[]
            {
                typeof(Rigidbody),
                typeof(CharacterJoint),
                typeof(CapsuleCollider),
                typeof(IBI_EnemyLimbs)
            }
        );
        modelNew.rightForeArm.GetComponent<CharacterJoint>().connectedBody = modelNew.rightArm.GetComponent<Rigidbody>();

    }

    void Duplicate_Spine()
    {
        if (modelOld.spine0 == null || modelNew.spine0 == null)
        {
            Debug.LogError("One of the spine nodes are missing reference");
            return;
        }

        CopyComponents(modelNew.spine0,modelOld.spine0, new System.Type[]
            {
                typeof(Rigidbody),
                typeof(CharacterJoint),
                typeof(BoxCollider),
                typeof(IBI_EnemyBody)
            }
        );
        modelNew.spine0.GetComponent<CharacterJoint>().connectedBody = modelNew.hip.GetComponent<Rigidbody>();

    }

    void Duplicate_Head()
    {
        if (modelOld.head == null || modelNew.head == null)
        {
            Debug.LogError("One of the head nodes are missing reference");
            return;
        }

        CopyComponents(modelNew.head,modelOld.head, new System.Type[]
            {
                typeof(Rigidbody),
                typeof(CharacterJoint),
                typeof(SphereCollider),
                typeof(IBI_EnemyHead)
            }
        );
        modelNew.head.GetComponent<CharacterJoint>().connectedBody = modelNew.spine0.GetComponent<Rigidbody>();

    }

    #endregion
}
