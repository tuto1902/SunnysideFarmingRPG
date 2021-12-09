using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleIsInteracting : StateMachineBehaviour
{
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(Settings.isInteracting, true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(Settings.isInteracting, false);
    }
}
