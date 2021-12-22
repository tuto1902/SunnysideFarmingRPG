using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridCursor : MonoBehaviour
{
    private Canvas canvas;
    private Grid grid;
    private Camera mainCamera;

    [SerializeField] private Image cursorImage;
    [SerializeField] private RectTransform cursorRectTransform;
    [SerializeField] private Sprite greenCursorSprite;
    [SerializeField] private Sprite redCursorSprite;

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

    private Vector3Int GetGridPositionForPlayer()
    {
        return grid.WorldToCell(Player.Instance.transform.position);
    }

    private Vector3Int GetGridPositionForCursor()
    {
        Vector3 cursorPosition;
        if (GamepadCursor.Instance.CurrentControlScheme == Settings.gamepadScheme)
        {
            cursorPosition = GamepadCursor.Instance.GetVirtualMousePosition();
        }
        else
        {
            cursorPosition = Mouse.current.position.ReadValue();
        }

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(cursorPosition.x, cursorPosition.y, -mainCamera.transform.position.z));
        return grid.WorldToCell(worldPosition);
    }
}
