using UnityEngine;

public class MovementAnimationParameterControl : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventHandler.MovementEvent += SetAnimationParameters;
    }

    private void OnDisable()
    {
        EventHandler.MovementEvent -= SetAnimationParameters;    
    }

    private void SetAnimationParameters(
        float inputX,
        float inputY,
        bool isIdle,
        bool isWalking,
        bool isRunning,
        bool isInteracting,
        bool isWatering,
        bool isUsingHands,
        bool isDead,
        bool canAttack,
        bool canUseTool,
        bool attackTrigger,
        bool rollTrigger,
        bool jumpTrigger,
        bool hurtTrigger,
        bool useToolTrigger,
        bool useAxeTrigger,
        bool usePickaxeTrigger,
        bool useHammerTrigger,
        bool useShovelTrigger,
        bool useWateringTrigger
    )
    {

        animator.SetBool(Settings.isIdle, isIdle);
        animator.SetBool(Settings.isWalking, isWalking);
        animator.SetBool(Settings.isRunning, isRunning);
        animator.SetBool(Settings.isInteracting, isInteracting);
        animator.SetBool(Settings.isWatering, isWatering);
        animator.SetBool(Settings.isUsingHands, isUsingHands);
        animator.SetBool(Settings.isDead, isDead);
        animator.SetBool(Settings.canAttack, canAttack);
        animator.SetBool(Settings.canUseTool, canUseTool);


        if (attackTrigger == true)
        {
            animator.SetTrigger(Settings.attackTrigger);
        }

        if (rollTrigger == true)
        {
            animator.SetTrigger(Settings.rollTrigger);
        }

        if (jumpTrigger == true)
        {
            animator.SetTrigger(Settings.jumpTrigger);
        }

        if (hurtTrigger == true)
        {
            animator.SetTrigger(Settings.hurtTrigger);
        }

        if (useToolTrigger == true)
        {
            animator.SetTrigger(Settings.useToolTrigger);
        }

        if (useAxeTrigger == true)
        {
            animator.SetTrigger(Settings.useAxeTrigger);
        }

        if (usePickaxeTrigger == true)
        {
            animator.SetTrigger(Settings.usePickaxeTrigger);
        }

        if (useHammerTrigger == true)
        {
            animator.SetTrigger(Settings.useHammerTrigger);
        }

        if (useShovelTrigger == true)
        {
            animator.SetTrigger(Settings.useShovelTrigger);
        }

        if (useWateringTrigger == true)
        {
            animator.SetTrigger(Settings.useWateringTrigger);
        }
    }

    public void AnimationEventPlayFootstepSound()
    {

    }
}
