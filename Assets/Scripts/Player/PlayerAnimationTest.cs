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
    public bool isWatering;
    public bool isUsingHands;
    public bool isUsingTool;
    public bool isDead;
    public bool rollTrigger;
    public bool jumpTrigger;
    public bool hurtTrigger;


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

        EventHandler.CallMovementEvent(
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

        ResetAnimationParameters();
    }

    private void ResetAnimationParameters()
    {
        rollTrigger = false;
        jumpTrigger = false;
        hurtTrigger = false;

        isDead = false;
    }
}
