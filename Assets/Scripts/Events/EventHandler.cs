using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void MovementDelegate(
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
        if (MovementEvent != null)
        {
            MovementEvent(
                inputX,
                inputY,
                isIdle,
                isWalking,
                isRunning,
                isWatering,
                isUsingHands,
                isDead,
                attackTrigger,
                rollTrigger,
                jumpTrigger,
                hurtTrigger,
                useToolTrigger,
                useAxeTrigger,
                usePickaxeTrigger,
                useHammerTrigger,
                useShovelTrigger
            );
        }
    }
}
