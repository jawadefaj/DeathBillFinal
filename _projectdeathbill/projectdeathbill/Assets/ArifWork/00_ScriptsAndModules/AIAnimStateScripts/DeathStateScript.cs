using UnityEngine;
using System.Collections;

public class DeathStateScript : StateMachineBehaviour {

    AIPersonnel aipersonnel;
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        aipersonnel = animator.transform.parent.GetComponent<AIPersonnel>();
        animator.SetBool("ISDEAD", true);
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        (aipersonnel as MonoBehaviour).StartCoroutine(DestroyAfter(animator, stateInfo.length + 2));
    }
    IEnumerator DestroyAfter(Animator anim, float time)
    {
        yield return new WaitForSeconds(time);
        if(aipersonnel.enemyType != EnemyType.MORTAR)
            Pool.Destroy(anim.transform.parent.gameObject);
        else
            Destroy(anim.transform.parent.gameObject);
    }
	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //    
    //}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
