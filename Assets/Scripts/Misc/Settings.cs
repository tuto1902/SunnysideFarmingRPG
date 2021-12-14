using UnityEngine;

public static class Settings
{
    public static int isIdle;
    public static int isWalking;
    public static int isRunning;
    public static int isInteracting;
    public static int isWatering;
    public static int isUsingHands;
    public static int isDead;
    public static int attackTrigger;
    public static int rollTrigger;
    public static int jumpTrigger;
    public static int hurtTrigger;
    public static int useToolTrigger;
    public static int useAxeTrigger;
    public static int usePickaxeTrigger;
    public static int useHammerTrigger;
    public static int useShovelTrigger;
    public static int useWateringTrigger;
    public static int canAttack;
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
        isDead = Animator.StringToHash("isDead");
        attackTrigger = Animator.StringToHash("attackTrigger");
        rollTrigger = Animator.StringToHash("rollTrigger");
        jumpTrigger = Animator.StringToHash("jumpTrigger");
        hurtTrigger = Animator.StringToHash("hurtTrigger");
        useToolTrigger = Animator.StringToHash("useToolTrigger");
        useAxeTrigger = Animator.StringToHash("useAxeTrigger");
        usePickaxeTrigger = Animator.StringToHash("usePickaxeTrigger");
        useHammerTrigger = Animator.StringToHash("useHammerTrigger");
        useShovelTrigger = Animator.StringToHash("useShovelTrigger");
        useWateringTrigger = Animator.StringToHash("useWateringTrigger");
        canAttack = Animator.StringToHash("canAttack");
        canUseTool = Animator.StringToHash("canUseTool");
    }
}
