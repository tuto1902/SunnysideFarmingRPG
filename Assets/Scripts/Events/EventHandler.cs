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
    bool isUsingTool,
    bool isDead,
    bool rollTrigger,
    bool jumpTrigger,
    bool hurtTrigger    
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
        bool isUsingTool,
        bool isDead,
        bool rollTrigger,
        bool jumpTrigger,
        bool hurtTrigger
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
                isUsingTool,
                isDead,
                rollTrigger,
                jumpTrigger,
                hurtTrigger
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
