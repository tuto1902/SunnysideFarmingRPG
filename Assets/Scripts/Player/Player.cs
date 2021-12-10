using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerMovementSettings playerMovementSettings;

    private float inputX;
    private float inputY;
    private float movementSpeed;
    private PlayerDirection playerDirection;

    private Rigidbody2D rigidBody2D;
    private Camera mainCamera;

    private bool isIdle;
    private bool isWalking;
    private bool isRunning;
    private bool isWatering;
    private bool isUsingHands;
    private bool isAttacking;
    private bool isDead;
    private bool attackTrigger;
    private bool rollTrigger;
    private bool jumpTrigger;
    private bool hurtTrigger;
    private bool useToolTrigger;
    private bool useAxeTrigger;
    private bool usePickaxeTrigger;
    private bool useHammerTrigger;
    private bool useShovelTrigger;
    private bool playerInputDisabled = false;

    public bool PlayerInputDisabled
    {
        get => playerInputDisabled;
        set => playerInputDisabled = value;
    }

    protected override void Awake()
    {
        base.Awake();

        rigidBody2D = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        inputManager.moveEvent += InputManager_OnMoveEvent;
        inputManager.toggleRunEvent += InputManager_OnToggleRun;
    }

    private void OnDestroy()
    {
        inputManager.moveEvent -= InputManager_OnMoveEvent;
        inputManager.toggleRunEvent -= InputManager_OnToggleRun;
    }

    private void Start()
    {
        isIdle = true;
        isAttacking = false;
        playerDirection = PlayerDirection.Right;
    }

    private void Update()
    {
        if (playerInputDisabled == false)
        {
            ResetAnimationTriggers();
            PlayerMovementInput();
            PlayerWalkInput();
            PlayerFacingDirection();

            EventHandler.CallMovementEvent(
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

    private void FixedUpdate()
    {
        PlayerMovement();
        PlayerFriction();
    }

    private void ResetAnimationTriggers()
    {
        attackTrigger = false;
        rollTrigger = false;
        jumpTrigger = false;
        hurtTrigger = false;
        useToolTrigger = false;
        useAxeTrigger = false;
        usePickaxeTrigger = false;
        useHammerTrigger = false;
        useShovelTrigger = false;
    }

    private void PlayerMovementInput()
    {
        if (inputX != 0 || inputY != 0)
        {
            isIdle = false;
            isWalking = true;

            movementSpeed = playerMovementSettings.walkingSpeed;

            if (inputX < 0)
            {
                playerDirection = PlayerDirection.Left;
            }
            else if (inputX > 0)
            {
                playerDirection = PlayerDirection.Right;
            }
        }
        else if (inputX == 0 && inputY == 0)
        {
            isIdle = true;
            isWalking = false;
            isRunning = false;
        }
    }

    private void PlayerWalkInput()
    {
        if (isIdle == false)
        {
            if (isRunning)
            {
                isWalking = false;
                movementSpeed = playerMovementSettings.runningSpeed;
            }
            else
            {
                isWalking = true;
                movementSpeed = playerMovementSettings.walkingSpeed;
            }
        }
    }

    private void PlayerFacingDirection()
    {
        float x = playerDirection == PlayerDirection.Left ? -1 : 1;
        transform.localScale = new Vector3(x, 1, 1);
    }

    private void PlayerMovement()
    {
        if (isAttacking)
        {
            rigidBody2D.velocity = Vector2.zero;
            return;
        }

        if (playerInputDisabled == false)
        {
            Vector2 targetSpeed = new Vector2(inputX * movementSpeed, inputY * movementSpeed);
            float speedDifferenceX = targetSpeed.x - rigidBody2D.velocity.x;
            float speedDifferenceY = targetSpeed.y - rigidBody2D.velocity.y;

            float movementX = Mathf.Pow(Mathf.Abs(speedDifferenceX) * playerMovementSettings.acceleration, playerMovementSettings.velocityPower) * Mathf.Sign(speedDifferenceX);
            float movementY = Mathf.Pow(Mathf.Abs(speedDifferenceY) * playerMovementSettings.acceleration, playerMovementSettings.velocityPower) * Mathf.Sign(speedDifferenceY);

            Vector2 movement = new Vector2(movementX, movementY);

            rigidBody2D.AddForce(movement);
        }
    }

    private void PlayerFriction()
    {
        if (Mathf.Abs(inputX) < 0.01 && Mathf.Abs(inputY) < 0.01)
        {
            float frictionAmountX = Mathf.Min(Mathf.Abs(rigidBody2D.velocity.x), Mathf.Abs(playerMovementSettings.frictionAmount));
            float frictionAmountY = Mathf.Min(Mathf.Abs(rigidBody2D.velocity.y), Mathf.Abs(playerMovementSettings.frictionAmount));

            frictionAmountX *= Mathf.Sign(rigidBody2D.velocity.x);
            frictionAmountY *= Mathf.Sign(rigidBody2D.velocity.y);

            Vector2 frictionAmount = new Vector2(frictionAmountX, frictionAmountY);

            rigidBody2D.AddForce(Vector2.one * -frictionAmount, ForceMode2D.Impulse);
        }
    }

    public void EnablePlayerInput()
    {
        playerInputDisabled = false;
    }

    public void DisablePlayerInput()
    {
        playerInputDisabled = true;
    }

    public void ResetMovement()
    {
        inputX = 0;
        inputY = 0;
        isIdle = true;
        isWalking = false;
        isRunning = false;
    }

    public void DisablePlayerInputAndResetMovement()
    {
        DisablePlayerInput();
        ResetMovement();

        EventHandler.CallMovementEvent(
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

    public Vector3 GetPlayerViewportposition()
    {
        return mainCamera.WorldToViewportPoint(transform.position);
    }

    private void InputManager_OnMoveEvent(Vector2 movementInput)
    {
        inputX = movementInput.x;
        inputY = movementInput.y;
    }

    private void InputManager_OnToggleRun()
    {
        isRunning = !isRunning;
    }
}
