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

    public static event Action<int, Season, int, string, int, int, int> AdvanceGameMinuteEvent;
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameHourEvent;
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameDayEvent;
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameSeasonEvent;
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameYearEvent;

    public static event Action BeforeSceneUnloadFadeOutEvent;
    public static event Action BeforeSceneUnloadEvent;
    public static event Action AfterSceneLoadEvent;
    public static event Action AfterSceneLoadFadeInEvent;

    public static event Action DropSelectedItemEvent;
    public static event Action RemoveSelectedItemFromInventoryEvent;

    public static event Action InstantiateCropPrefabsEvent;

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

    public static void CallAdvanceGameMinuteEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameMinuteEvent != null)
        {
            AdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    public static void CallAdvanceGameHourEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameHourEvent != null)
        {
            AdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    public static void CallAdvanceGameDayEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameDayEvent != null)
        {
            AdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    public static void CallAdvanceGameSeasonEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameSeasonEvent != null)
        {
            AdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    public static void CallAdvanceGameYearEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameYearEvent != null)
        {
            AdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        if (BeforeSceneUnloadFadeOutEvent != null)
        {
            BeforeSceneUnloadFadeOutEvent();
        }
    }

    public static void CallBeforeSceneUnloadEvent()
    {
        if (BeforeSceneUnloadEvent != null)
        {
            BeforeSceneUnloadEvent();
        }
    }

    public static void CallAfterSceneLoadEvent()
    {
        if (AfterSceneLoadEvent != null)
        {
            AfterSceneLoadEvent();
        }
    }

    public static void CallAfterSceneLoadFadeInEvent()
    {
        if (AfterSceneLoadFadeInEvent != null)
        {
            AfterSceneLoadFadeInEvent();
        }
    }

    public static void CallDropSelectedItemEvent()
    {
        if (DropSelectedItemEvent != null)
        {
            DropSelectedItemEvent();
        }
    }

    public static void CallRemoveSelectedItemFromInventoryEvent()
    {
        if (RemoveSelectedItemFromInventoryEvent != null)
        {
            RemoveSelectedItemFromInventoryEvent();
        }
    }

    public static void CallInstantiateCropPrefabsEvent()
    {
        if (InstantiateCropPrefabsEvent != null)
        {
            InstantiateCropPrefabsEvent();
        }
    }
}
