using UnityEngine;

public static class Settings
{
    public static int isIdle;
    public static int isWalking;
    public static int isRunning;
    public static int isInteracting;
    public static int isWatering;
    public static int isUsingHands;
    public static int isUsingTool;
    public static int isDead;
    public static int rollTrigger;
    public static int jumpTrigger;
    public static int hurtTrigger;
    public static int canUseTool;

    public static string WateringTool = "Watering Can";
    public static string DiggingTool = "Shovel";
    public static string ChoppingTool = "Axe";
    public static string BreakingTool = "Pickaxe";
    public static string CollectingTool = "Basket";
    public static string BuildingTool = "Hammer";
    public static string Weapon = "Sword";

    static Settings()
    {
        isIdle = Animator.StringToHash("isIdle");
        isWalking = Animator.StringToHash("isWalking");
        isRunning = Animator.StringToHash("isRunning");
        isInteracting = Animator.StringToHash("isInteracting");
        isWatering = Animator.StringToHash("isWatering");
        isUsingHands = Animator.StringToHash("isUsingHands");
        isUsingTool = Animator.StringToHash("isUsingTool");
        isDead = Animator.StringToHash("isDead");
        rollTrigger = Animator.StringToHash("rollTrigger");
        jumpTrigger = Animator.StringToHash("jumpTrigger");
        hurtTrigger = Animator.StringToHash("hurtTrigger");
        canUseTool = Animator.StringToHash("canUseTool");
    }
}
