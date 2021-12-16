using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerMovementSettings playerMovementSettings;
    [SerializeField] private SpriteRenderer equippedItemSpriteRenderer = null;

    private AnimationOverrides animationOverrides;
    private List<CharacterAttribute> characterAttributeCustomisationList;
    private CharacterAttribute toolCharacterAtribute;
    private CharacterAttribute bodyCharacterAtribute;

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
    private bool isUsingTool;
    private bool isDead;
    private bool rollTrigger;
    private bool jumpTrigger;
    private bool hurtTrigger;
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

        animationOverrides = GetComponentInChildren<AnimationOverrides>();
        toolCharacterAtribute = new CharacterAttribute(CharacterPartAnimator.tool, PartVariantType.none);
        bodyCharacterAtribute = new CharacterAttribute(CharacterPartAnimator.body, PartVariantType.none);

        characterAttributeCustomisationList = new List<CharacterAttribute>();

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
        isUsingTool = false;
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
                isUsingTool,
                isDead,
                rollTrigger,
                jumpTrigger,
                hurtTrigger
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
        rollTrigger = false;
        jumpTrigger = false;
        hurtTrigger = false;
    }

    private void PlayerMovementInput()
    {
        if (inputX != 0 || inputY != 0)
        {
            isIdle = false;
            isRunning = true;

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
        // For now lets just make it always run. Will enable the toggle later

        if (isIdle == false)
        {
            isRunning = true;
            movementSpeed = playerMovementSettings.runningSpeed;
            //if (isRunning)
            //{
            //    isWalking = false;
            //    movementSpeed = playerMovementSettings.runningSpeed;
            //}
            //else
            //{
            //    isWalking = true;
            //    movementSpeed = playerMovementSettings.walkingSpeed;
            //}
        }
    }

    private void PlayerFacingDirection()
    {
        float x = playerDirection == PlayerDirection.Left ? -1 : 1;
        transform.localScale = new Vector3(x, 1, 1);
    }

    private void PlayerMovement()
    {
        if (isUsingTool)
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
                isUsingTool,
                isDead,
                rollTrigger,
                jumpTrigger,
                hurtTrigger
            );
    }

    public void ShowCarriedItem(int itemCode)
    {
        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);
        if (itemDetails != null)
        {
            equippedItemSpriteRenderer.sprite = itemDetails.itemSprite;
            equippedItemSpriteRenderer.color = new Color(1, 1, 1, 1);

            bodyCharacterAtribute.partVariantType = PartVariantType.Carry;
            toolCharacterAtribute.partVariantType = PartVariantType.Carry;
            characterAttributeCustomisationList.Clear();
            characterAttributeCustomisationList.Add(bodyCharacterAtribute);
            characterAttributeCustomisationList.Add(toolCharacterAtribute);

            animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);
        }
    }

    public void ClearCarriedItem()
    {
        equippedItemSpriteRenderer.sprite = null;
        equippedItemSpriteRenderer.color = new Color(1, 1, 1, 0);

        bodyCharacterAtribute.partVariantType = PartVariantType.none;
        toolCharacterAtribute.partVariantType = PartVariantType.none;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(bodyCharacterAtribute);
        characterAttributeCustomisationList.Add(toolCharacterAtribute);

        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);
    }

    public Vector3 GetPlayerViewportPosition()
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
        // isRunning = !isRunning;

        // Always run for now
        isRunning = true;
    }
}
