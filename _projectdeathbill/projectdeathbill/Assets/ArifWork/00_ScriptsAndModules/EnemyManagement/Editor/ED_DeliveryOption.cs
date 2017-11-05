using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(DeliveryOption))]
public class ED_DeliveryOption : Editor {

    DeliveryOption dOpt;
    Transform pI;
    Transform pO;
    Transform pointIlast;
    Transform pointOfirst;
    Transform handleILast;
    Transform handleOFirst;
    //Transform 
    
    void OnEnable()
    {
        dOpt = (DeliveryOption) target;
        pI = dOpt.roadDefinition.path_in.transform;
        pO = dOpt.roadDefinition.path_out.transform;
        pointIlast = pI.GetChild(pI.childCount-1);
        pointOfirst = pO.GetChild(0);
        handleILast = pointIlast.GetChild(1);
        handleOFirst = pointOfirst.GetChild(0);
    }


    float inRotationValue;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        inRotationValue =  EditorGUILayout.FloatField("In Rotation Value",inRotationValue);

        if (GUILayout.Button("SetRot", new GUILayoutOption[]{  GUILayout.Height(30) }))
        {
            Transform tempTrans;
            pO.position = pI.position;
            pO.rotation = pI.rotation;

            pointIlast.localRotation = Quaternion.Euler(0,inRotationValue,0);
            handleILast.localPosition = new Vector3(0,0,2);

            pointOfirst.parent = pointIlast;
            pointOfirst.localPosition = Vector3.zero;
            pointOfirst.localRotation = Quaternion.identity;
            pointOfirst.parent = pO;
            pointOfirst.SetAsFirstSibling();
            handleOFirst.localPosition = new Vector3(0,0,-2);
        }
    }
}
