using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerMovementSettings playerMovementSettings;
    [SerializeField] private SpriteRenderer equippedItemSpriteRenderer = null;

    private AnimationOverrides animationOverrides;
    private List<CharacterAttribute> characterAttributeCustomisationList;
    private CharacterAttribute toolCharacterAttribute;
    private CharacterAttribute bodyCharacterAttribute;
    private CharacterAttribute hairCharacterAttribute;

    private float inputX;
    private float inputY;
    private float movementSpeed;
    private PlayerDirection playerDirection;

    private Rigidbody2D rigidBody2D;
    private Camera mainCamera;
    private GridCursor gridCursor;

    private WaitForSeconds afterUseToolAnimationPause;
    private WaitForSeconds useToolAnimationPause;

    private WaitForSeconds afterLiftToolAnimationPause;
    private WaitForSeconds liftToolAnimationPause;

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

    public PlayerDirection PlayerDirection
    {
        get => playerDirection;
    }

    protected override void Awake()
    {
        base.Awake();

        rigidBody2D = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        animationOverrides = GetComponentInChildren<AnimationOverrides>();
        toolCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.tool, PartVariantType.none);
        bodyCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.body, PartVariantType.none);
        hairCharacterAttribute = new CharacterAttribute(CharacterPartAnimator.hair, PartVariantType.none);
        

        characterAttributeCustomisationList = new List<CharacterAttribute>();

        inputManager.moveEvent += InputManager_OnMoveEvent;
        inputManager.toggleRunEvent += InputManager_OnToggleRun;
        inputManager.playerClick += InputManager_OnPlayerClick;
        inputManager.useToolEvent += InputManager_OnUseTool;
    }

    private void OnDestroy()
    {
        inputManager.moveEvent -= InputManager_OnMoveEvent;
        inputManager.toggleRunEvent -= InputManager_OnToggleRun;
        inputManager.playerClick -= InputManager_OnPlayerClick;
        inputManager.useToolEvent -= InputManager_OnUseTool;
    }

    private void Start()
    {
        isIdle = true;
        isUsingTool = false;
        playerDirection = PlayerDirection.Right;
        gridCursor = FindObjectOfType<GridCursor>();
        afterUseToolAnimationPause = new WaitForSeconds(Settings.afterUseToolAnimationPause);
        useToolAnimationPause = new WaitForSeconds(Settings.useToolAnimationPause);
        afterLiftToolAnimationPause = new WaitForSeconds(Settings.afterLiftToolAnimationPause);
        liftToolAnimationPause = new WaitForSeconds(Settings.liftToolAnimationPause);
    }

    private void Update()
    {
        if (playerInputDisabled == false)
        {
            ResetAnimationTriggers();
            PlayerMovementInput();
            PlayerWalkInput();
            PlayerFacingDirection();
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
            gridCursor.DisableCursor();

            movementSpeed = playerMovementSettings.walkingSpeed;

            if (inputX < -0.05f)
            {
                playerDirection = PlayerDirection.Left;
            }
            else if (inputX > 0.05f)
            {
                playerDirection = PlayerDirection.Right;
            }
        }
        else if (inputX == 0 && inputY == 0)
        {
            isIdle = true;
            isWalking = false;
            isRunning = false;
            ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.Player);
            if (itemDetails != null && itemDetails.itemUseGridRadius > 0)
            {
                gridCursor.EnableCursor();
            }
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

            bodyCharacterAttribute.partVariantType = PartVariantType.Carry;
            toolCharacterAttribute.partVariantType = PartVariantType.Carry;
            characterAttributeCustomisationList.Clear();
            characterAttributeCustomisationList.Add(bodyCharacterAttribute);
            characterAttributeCustomisationList.Add(toolCharacterAttribute);

            animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);
        }
    }

    public void ClearCarriedItem()
    {
        equippedItemSpriteRenderer.sprite = null;
        equippedItemSpriteRenderer.color = new Color(1, 1, 1, 0);

        bodyCharacterAttribute.partVariantType = PartVariantType.none;
        toolCharacterAttribute.partVariantType = PartVariantType.none;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(bodyCharacterAttribute);
        characterAttributeCustomisationList.Add(toolCharacterAttribute);

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

    private void InputManager_OnPlayerClick()
    {
        if (isUsingTool)
        {
            return;
        }

        if (playerInputDisabled)
        {
            return;
        }

        if (GamepadCursor.Instance.CurrentControlScheme == Settings.gamepadScheme)
        {
            return;
        }

        if (gridCursor.CursorIsEnabled)
        {
            Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();
            Vector3Int playerGridPosition = gridCursor.GetGridPositionForPlayer();

            ProcessPlayerClickInput(cursorGridPosition, playerGridPosition);
        }
    }

    private void InputManager_OnUseTool()
    {
        if (GamepadCursor.Instance.CurrentControlScheme != Settings.gamepadScheme)
        {
            return;
        }

        if (gridCursor.CursorIsEnabled)
        {
            Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();
            Vector3Int playerGridPosition = gridCursor.GetGridPositionForPlayer();
            ProcessPlayerClickInput(cursorGridPosition, playerGridPosition);
        }
    }

    private void ProcessPlayerClickInput(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        ResetMovement();

        Vector3Int playerDirection = GetPlayerClickDirection(cursorGridPosition, playerGridPosition);
        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);

        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.Player);

        if (itemDetails != null)
        {
            switch(itemDetails.itemType)
            {
                case ItemType.Seed:
                    ProcessPlayerClickInputSeed(itemDetails);
                    break;
                case ItemType.Commodity:
                    ProcessPlayerClickInputCommodity(itemDetails);
                    break;
                case ItemType.DiggingTool:
                case ItemType.WateringTool:
                    ProcessPlayerClickInputTool(gridPropertyDetails, itemDetails, playerDirection);
                    break;
                case ItemType.none:
                    break;
                case ItemType.count:
                    break;
                default:
                    break;
            }
        }
    }

    
    private Vector3Int GetPlayerClickDirection(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        if (cursorGridPosition.x > playerGridPosition.x)
        {
            return Vector3Int.right;
        }
        else
        {
            return Vector3Int.left;
        }
        // Also check for up and down when using 4 directional animations
    }

    private void ProcessPlayerClickInputCommodity(ItemDetails itemDetails)
    {
        if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void ProcessPlayerClickInputSeed(ItemDetails itemDetails)
    {
        if (itemDetails.canBeDropped && gridCursor.CursorPositionIsValid)
        {
            EventHandler.CallDropSelectedItemEvent();
        }
    }

    private void ProcessPlayerClickInputTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails, Vector3Int playerDirection)
    {
        switch(itemDetails.itemType)
        {
            case ItemType.DiggingTool:
                if (gridCursor.CursorPositionIsValid)
                {
                    DigGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;
            case ItemType.WateringTool:
                if (gridCursor.CursorPositionIsValid)
                {
                    WaterGroundAtCursor(gridPropertyDetails, playerDirection);
                }
                break;
            default:
                break;
        }
    }

    private void DigGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        StartCoroutine(DigGroundAtCursorCoroutine(playerDirection, gridPropertyDetails));
    }

    private void WaterGroundAtCursor(GridPropertyDetails gridPropertyDetails, Vector3Int playerDirection)
    {
        StartCoroutine(WaterGroundAtCursorCoroutine(playerDirection, gridPropertyDetails));
    }

    private IEnumerator DigGroundAtCursorCoroutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        PlayerInputDisabled = true;
        isUsingTool = true;

        toolCharacterAttribute.partVariantType = PartVariantType.Shovel;
        bodyCharacterAttribute.partVariantType = PartVariantType.Shovel;
        hairCharacterAttribute.partVariantType = PartVariantType.Shovel;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        characterAttributeCustomisationList.Add(bodyCharacterAttribute);
        characterAttributeCustomisationList.Add(hairCharacterAttribute);

        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        if (playerDirection == Vector3Int.left)
        {
            this.playerDirection = PlayerDirection.Left;
        }
        else
        {
            this.playerDirection = PlayerDirection.Right;
        }

        PlayerFacingDirection();

        yield return useToolAnimationPause;

        if (gridPropertyDetails.daysSinceDug == -1)
        {
            gridPropertyDetails.daysSinceDug = 0;
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);
        GridPropertiesManager.Instance.DisplayDugGround(gridPropertyDetails);

        yield return afterUseToolAnimationPause;

        playerInputDisabled = false;
        isUsingTool = false;
    }

    private IEnumerator WaterGroundAtCursorCoroutine(Vector3Int playerDirection, GridPropertyDetails gridPropertyDetails)
    {
        PlayerInputDisabled = true;
        isUsingTool = true;

        toolCharacterAttribute.partVariantType = PartVariantType.Watering;
        bodyCharacterAttribute.partVariantType = PartVariantType.Watering;
        hairCharacterAttribute.partVariantType = PartVariantType.Watering;
        characterAttributeCustomisationList.Clear();
        characterAttributeCustomisationList.Add(toolCharacterAttribute);
        characterAttributeCustomisationList.Add(bodyCharacterAttribute);
        characterAttributeCustomisationList.Add(hairCharacterAttribute);

        animationOverrides.ApplyCharacterCustomisationParameters(characterAttributeCustomisationList);

        if (playerDirection == Vector3Int.left)
        {
            this.playerDirection = PlayerDirection.Left;
        }
        else
        {
            this.playerDirection = PlayerDirection.Right;
        }

        PlayerFacingDirection();

        yield return liftToolAnimationPause;

        if (gridPropertyDetails.daysSinceWatered == -1)
        {
            gridPropertyDetails.daysSinceWatered = 0;
        }

        GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY, gridPropertyDetails);
        GridPropertiesManager.Instance.DisplayWateredGround(gridPropertyDetails);

        yield return afterLiftToolAnimationPause;

        playerInputDisabled = false;
        isUsingTool = false;
    }
}
