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

    private void Start()
    {
        animator.SetBool(Settings.canAttack, true);
        animator.SetBool(Settings.canUseTool, true);
    }

    private void SetAnimationParameters(
        float inputX,
        float inputY,
        bool isIdle,
        bool isWalking,
        bool isRunning,
        bool isWatering,
        bool isUsingHands,
        bool isDead,
        bool attackTrigger,
        bool rollTrigger,
        bool jumpTrigger,
        bool hurtTrigger,
        bool useToolTrigger,
        bool useAxeTrigger,
        bool usePickaxeTrigger,
        bool useHammerTrigger,
        bool useShovelTrigger
    )
    {

        animator.SetBool(Settings.isIdle, isIdle);
        animator.SetBool(Settings.isWalking, isWalking);
        animator.SetBool(Settings.isRunning, isRunning);
        animator.SetBool(Settings.isWatering, isWatering);
        animator.SetBool(Settings.isUsingHands, isUsingHands);
        animator.SetBool(Settings.isDead, isDead);

        bool canAttack = animator.GetBool(Settings.canAttack);
        bool canUseTool = animator.GetBool(Settings.canUseTool);
        bool isInteracting = animator.GetBool(Settings.isInteracting);

        if (isInteracting == false)
        {
            if (canAttack == true && attackTrigger == true)
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

            if (canUseTool == true && useToolTrigger == true)
            {
                animator.SetTrigger(Settings.useToolTrigger);

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
            }
        }
    }

    public void AnimationEventPlayFootstepSound()
    {

    }
}
