using System;
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
    bool useAxeTrigger,
    bool usePickaxeTrigger,
    bool useHammerTrigger,
    bool useShovelTrigger,
    bool useWateringTrigger
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
        bool useAxeTrigger,
        bool usePickaxeTrigger,
        bool useHammerTrigger,
        bool useShovelTrigger,
        bool useWateringTrigger
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
                useAxeTrigger,
                usePickaxeTrigger,
                useHammerTrigger,
                useShovelTrigger,
                useWateringTrigger
            );
        }
    }
}
