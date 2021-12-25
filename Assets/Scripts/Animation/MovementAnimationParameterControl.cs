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
        bool isUsingTool,
        bool isDead,
        bool rollTrigger,
        bool jumpTrigger,
        bool hurtTrigger
    )
    {

        animator.SetBool(Settings.isIdle, isIdle);
        animator.SetBool(Settings.isWalking, isWalking);
        animator.SetBool(Settings.isRunning, isRunning);
        animator.SetBool(Settings.isWatering, isWatering);
        animator.SetBool(Settings.isUsingHands, isUsingHands);
        animator.SetBool(Settings.isUsingTool, isUsingTool);
        animator.SetBool(Settings.isDead, isDead);

        if (isUsingTool)
        {
            animator.SetBool(Settings.canUseTool, true);
        }

        bool isInteracting = animator.GetBool(Settings.isInteracting);

        if (isInteracting == false)
        {
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
        }
    }

    public void AnimationEventPlayFootstepSound()
    {

    }
}
