using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCanUseTool : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(Settings.canUseTool, false);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(Settings.canUseTool, true);
    }
}
