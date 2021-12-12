using System;
using System.Collections.Generic;

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

    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdatedEvent;

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

    public static void CallInventoryUpdatedEvent(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if (InventoryUpdatedEvent != null)
        {
            InventoryUpdatedEvent(inventoryLocation, inventoryList);
        }
    }
}
