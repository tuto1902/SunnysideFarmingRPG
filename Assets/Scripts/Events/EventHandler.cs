using System.Collections.Generic;
using UnityEngine;

public delegate void MovementDelegate(
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
    bool useAxetrigger,
    bool usePickaxeTrigger,
    bool useHammerTrigger,
    bool useShovelTrigger
);

public static class EventHandler
{
    public static event MovementDelegate MovementEvent;

    public static void CallMovementEvent(
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
        bool useAxetrigger,
        bool usePickaxeTrigger,
        bool useHammerTrigger,
        bool useShovelTrigger
    )
    {
        if (MovementEvent != null)
        {
            MovementEvent(
                inputX,
                inputY,
                isIdle,
                isWalking,
                isRunning,
                isInteracting,
                isWatering,
                isUsingHands,
                isDead,
                canAttack,
                canUseTool,
                attackTrigger,
                rollTrigger,
                jumpTrigger,
                hurtTrigger,
                useToolTrigger,
                useAxetrigger,
                usePickaxeTrigger,
                useHammerTrigger,
                useShovelTrigger
            );
        }
    }
}
