using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCanAttack : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(Settings.canAttack, false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(Settings.canAttack, true);
    }
}
