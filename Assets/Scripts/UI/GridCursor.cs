using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GridCursor : MonoBehaviour
{
    private Canvas canvas;
    private Grid grid;
    private Camera mainCamera;

    [SerializeField] private Image cursorImage;
    [SerializeField] private RectTransform cursorRectTransform;
    [SerializeField] private Sprite greenCursorSprite;
    [SerializeField] private Sprite redCursorSprite;
    [SerializeField] private CropDetailsList cropDetailsList = null;

    private bool _cursorPositionIsValid = false;
    private int _itemUseGridRadius = 0;
    private ItemType _selectedItemType;
    private bool _cursorIsEnabled = false;

    public bool CursorPositionIsValid {
        get => _cursorPositionIsValid;
        set => _cursorPositionIsValid = value;
    }

    public int ItemUseGridRadius
    {
        get => _itemUseGridRadius;
        set => _itemUseGridRadius = value;
    }

    public ItemType SelctedItemType
    {
        get => _selectedItemType;
        set => _selectedItemType = value;
    }

    public bool CursorIsEnabled
    {
        get => _cursorIsEnabled;
        set => _cursorIsEnabled = value;
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
    }

    private void SceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if (CursorIsEnabled)
        {
            DisplayCursor();
        }
    }

    public void EnableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 1f);
        CursorIsEnabled = true;
    }

    public void DisableCursor()
    {
        cursorImage.color = new Color(1f, 1f, 1f, 0f);
        CursorIsEnabled = false;
    }

    private Vector3Int DisplayCursor()
    {
        if (grid == null)
        {
            return Vector3Int.zero;
        }

        Vector3Int gridPosition = GetGridPositionForCursor();
        Vector3Int playerGridPosition = GetGridPositionForPlayer();

        SetCursorValidity(gridPosition, playerGridPosition);

        cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

        return gridPosition;
    }

    private void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
    {
        SetCursorToValid();

        if (Mathf.Abs(cursorGridPosition.x - playerGridPosition.x) > ItemUseGridRadius || Mathf.Abs(cursorGridPosition.y - playerGridPosition.y) > ItemUseGridRadius)
        {
            SetCursorToInvalid();
            return;
        }

        ItemDetails itemDetails = InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.Player);
        if (itemDetails == null)
        {
            SetCursorToInvalid();
            return;
        }

        GridPropertyDetails gridPropertyDetails = GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);
        if (gridPropertyDetails != null)
        {
            switch (itemDetails.itemType)
            {
                case ItemType.Seed:
                    if (IsCursorValidForSeed(gridPropertyDetails) == false)
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                case ItemType.Commodity:
                    if (IsCursorValidForCommodity(gridPropertyDetails) == false)
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                case ItemType.WateringTool:
                case ItemType.DiggingTool:
                case ItemType.ChoppingTool:
                case ItemType.BreakingTool:
                    if (IsCursorValidForTool(gridPropertyDetails, itemDetails) == false)
                    {
                        SetCursorToInvalid();
                        return;
                    }
                    break;
                case ItemType.none:
                    break;
                case ItemType.count:
                    break;
                default:
                    break;
            }
        }
        else
        {
            SetCursorToInvalid();
            return;
        }
    }

    private bool IsCursorValidForTool(GridPropertyDetails gridPropertyDetails, ItemDetails itemDetails)
    {
        switch (itemDetails.itemType)
        {
            case ItemType.DiggingTool:
                if (gridPropertyDetails.isDiggable && gridPropertyDetails.daysSinceDug == -1)
                {
                    Vector3 cursorWorldPosition = new Vector3(GetWorldPositionForCursor().x + 0.5f, GetWorldPositionForCursor().y + 0.5f, 0f);

                    List<Item> itemList = new List<Item>();

                    HelperMethods.GetComponentsAtBoxLocation<Item>(out itemList, cursorWorldPosition, Settings.cursorSize, 0f);

                    bool foundReapable = false;
                    foreach (Item item in itemList)
                    {
                        if (InventoryManager.Instance.GetItemDetails(item.ItemCode).itemType == ItemType.ReapableScenary)
                        {
                            foundReapable = true;
                            break;
                        }
                    }

                    if (foundReapable)
                    {
                        return false;
                    }

                    return true;
                }
                else if (gridPropertyDetails.seedItemCode != -1)
                {
                    CropDetails cropDetails = cropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);
                    if (cropDetails != null)
                    {
                        if (gridPropertyDetails.growthDays >= cropDetails.growthDays[cropDetails.growthDays.Length - 1])
                        {
                            if (cropDetails.CanUseToolToHarvesCrop(itemDetails.itemCode))
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            case ItemType.WateringTool:
                if (gridPropertyDetails.daysSinceDug > -1 && gridPropertyDetails.daysSinceWatered == -1)
                {
                    return true;
                }

                return false;
            case ItemType.ChoppingTool:
                if (gridPropertyDetails.seedItemCode != -1)
                {
                    CropDetails cropDetails = cropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);
                    if (cropDetails != null)
                    {
                        if (gridPropertyDetails.growthDays >= cropDetails.growthDays[cropDetails.growthDays.Length - 1])
                        {
                            if (cropDetails.CanUseToolToHarvesCrop(itemDetails.itemCode))
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            case ItemType.BreakingTool:
                if (gridPropertyDetails.seedItemCode != -1)
                {
                    CropDetails cropDetails = cropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);
                    if (cropDetails != null)
                    {
                        if (gridPropertyDetails.growthDays >= cropDetails.growthDays[cropDetails.growthDays.Length - 1])
                        {
                            if (cropDetails.CanUseToolToHarvesCrop(itemDetails.itemCode))
                            {
                                return true;
                            }
                        }
                    }
                }
                return false;
            default:
                return false;
        }
    }

    private Vector3 GetWorldPositionForCursor()
    {
        return grid.CellToWorld(GetGridPositionForCursor());
    }

    private bool IsCursorValidForCommodity(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    private bool IsCursorValidForSeed(GridPropertyDetails gridPropertyDetails)
    {
        return gridPropertyDetails.canDropItem;
    }

    private void SetCursorToValid()
    {
        cursorImage.sprite = greenCursorSprite;
        CursorPositionIsValid = true;
    }

    private void SetCursorToInvalid()
    {
        cursorImage.sprite = redCursorSprite;
        CursorPositionIsValid = false;
    }

    private Vector2 GetRectTransformPositionForCursor(Vector3Int gridPosition)
    {
        Vector3 gridWorldPosition = grid.CellToWorld(gridPosition);
        Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition);
        return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas);
    }

    public Vector3Int GetGridPositionForPlayer()
    {
        return grid.WorldToCell(Player.Instance.transform.position);
    }

    public Vector3Int GetGridPositionForCursor()
    {
        if (GamepadCursor.Instance.CurrentControlScheme == Settings.gamepadScheme)
        {
            PlayerDirection playerDirection = Player.Instance.PlayerDirection;
            Vector3Int playerGridPosition = GetGridPositionForPlayer();
            Vector3Int gridPosition = new Vector3Int(playerDirection == PlayerDirection.Left ? playerGridPosition.x - 1 : playerGridPosition.x + 1, playerGridPosition.y, 0);
            return gridPosition;
        }
        else
        {
            Vector3 cursorPosition = Mouse.current.position.ReadValue();
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(cursorPosition.x, cursorPosition.y, -mainCamera.transform.position.z));
            return grid.WorldToCell(worldPosition);
        }

    }
}
