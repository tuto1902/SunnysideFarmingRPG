using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTest : MonoBehaviour
{
    public float inputX;
    public float inputY;
    public bool isIdle;
    public bool isWalking;
    public bool isRunning;
    public bool isInteracting;
    public bool isWatering;
    public bool isUsingHands;
    public bool isDead;
    public bool canAttack;
    public bool canUseTool;
    public bool attackTrigger;
    public bool rollTrigger;
    public bool jumpTrigger;
    public bool hurtTrigger;
    public bool useToolTrigger;
    public bool useAxeTrigger;
    public bool usePickaxeTrigger;
    public bool useHammerTrigger;
    public bool useShovelTrigger;
    public bool useWateringTrigger;


    // Start is called before the first frame update
    void Start()
    {
        isIdle = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (jumpTrigger || hurtTrigger)
        {
            isIdle = true;
            isWatering = false;
            isUsingHands = false;
        }

        if (isRunning)
        {
            isIdle = false;
            isWalking = false;
        }

        if (isWalking)
        {
            isIdle = false;
            isRunning = false;
        }

        if (isWatering)
        {
            isIdle = false;
            isRunning = false;
            isWalking = false;
        }

        if (isUsingHands)
        {
            isIdle = false;
            isRunning = false;
            isWalking = false;
            isWatering = false;
        }

        if (isWalking == false && isRunning == false && isWatering == false && isUsingHands == false)
        {
            isIdle = true;
        }

        EventHandler.CallMovementEvent(inputX, inputY, isIdle, isWalking, isRunning, isInteracting, isWatering, isUsingHands, isDead, canAttack, canUseTool, attackTrigger, rollTrigger, jumpTrigger, hurtTrigger, useToolTrigger, useAxeTrigger, usePickaxeTrigger, useHammerTrigger, useShovelTrigger, useWateringTrigger);

        ResetAnimationParameters();
    }

    private void ResetAnimationParameters()
    {
        attackTrigger = false;
        rollTrigger = false;
        jumpTrigger = false;
        hurtTrigger = false;

        if (useToolTrigger && useAxeTrigger)
        {
            //useToolTrigger = false;
            useAxeTrigger = false;
        }

        if (useToolTrigger && usePickaxeTrigger)
        {
            //useToolTrigger = false;
            usePickaxeTrigger = false;
        }

        if (useToolTrigger && useHammerTrigger)
        {
            //useToolTrigger = false;
            useHammerTrigger = false;
        }

        if (useToolTrigger && useShovelTrigger)
        {
            //useToolTrigger = false;
            useShovelTrigger = false;
        }

        if (useToolTrigger && useWateringTrigger)
        {
            //useToolTrigger = false;
            useWateringTrigger = false;
        }

        isDead = false;
    }
}
