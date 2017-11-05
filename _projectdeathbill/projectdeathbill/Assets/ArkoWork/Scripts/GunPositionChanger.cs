using UnityEngine;
using System.Collections;

public class GunPositionChanger : StateMachineBehaviour {

	public GameObject modelPrefab;
	//private string thisModelTag = "mixamorig";
	//public string prefabTag = "mixamorig";
	//public string gunName = "";

	private bool isInitialized = false;
	private Transform leftHand = null;
	private Transform rightHand = null;
	//private string thisModel_leftHandAddress = "";
	//private string thisModel_rightHandAddress = "";
	//private string prefab_leftHandAddress = "";
	//private string prefab_rightHandAddress = "";
	private bool isRightHand = true;
	private Transform modelGun;
	private Transform transformGun;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

		//normal initialization. will be called once
		if(!isInitialized)
		{
            //thisModelTag = "";
            ThirdPersonController tpc = animator.gameObject.GetComponent<ThirdPersonController>();
            ModelStructureKeeper keeper = modelPrefab.GetComponent<ModelStructureKeeper>();

            if (keeper == null)
            {
                Debug.LogError("Assign ModelStructureKeeper script to your prefab");
                return;
            }
            /*if (!string.IsNullOrEmpty(tpc.modelTag))
                thisModelTag = tpc.modelTag+":";*/

            /*string temp_prefabTag = "";
            if (!string.IsNullOrEmpty(prefabTag))
                temp_prefabTag = prefabTag+":";

			thisModel_leftHandAddress = string.Format("{0}Hips/{0}Spine/{0}Spine1/{0}Spine2/{0}LeftShoulder/{0}LeftArm/{0}LeftForeArm/{0}LeftHand",thisModelTag);
			thisModel_rightHandAddress = string.Format("{0}Hips/{0}Spine/{0}Spine1/{0}Spine2/{0}RightShoulder/{0}RightArm/{0}RightForeArm/{0}RightHand",thisModelTag);
            prefab_leftHandAddress = string.Format("{0}Hips/{0}Spine/{0}Spine1/{0}Spine2/{0}LeftShoulder/{0}LeftArm/{0}LeftForeArm/{0}LeftHand",temp_prefabTag);
            prefab_rightHandAddress = string.Format("{0}Hips/{0}Spine/{0}Spine1/{0}Spine2/{0}RightShoulder/{0}RightArm/{0}RightForeArm/{0}RightHand", temp_prefabTag);
			*/

            leftHand = tpc.modelStructure.leftHand;
            rightHand = tpc.modelStructure.rightHand;

            isRightHand = (keeper.modelStructure.gun.parent == keeper.modelStructure.rightHand) ? true : false;
            modelGun = keeper.modelStructure.gun;
            transformGun = tpc.modelStructure.gun;

			isInitialized = true;
		}

		//action done on each enter
		if(isRightHand)
		{
			transformGun.parent = rightHand;
		}
		else
		{
			transformGun.parent = leftHand;
		}

		transformGun.localPosition = modelGun.localPosition;
		transformGun.localRotation = modelGun.localRotation;

	}

}
