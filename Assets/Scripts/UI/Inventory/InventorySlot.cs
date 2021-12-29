using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [HideInInspector] public bool isSelected = false;
    [HideInInspector] public ItemDetails itemDetails;
    [HideInInspector] public int itemQuantity;

    private Camera mainCamera;
    private Transform itemsParent;
    private GameObject draggedItem;
    private string gamepadScheme = "Gamepad";
    private Canvas parentCanvas;
    private GridCursor gridCursor = null;

    [SerializeField] private InventoryBar inventoryBar = null;
    [SerializeField] private GameObject itemPrefab = null;
    [SerializeField] private GameObject inventoryTextBoxPrefab = null;
    [SerializeField] private int slotNumber;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += SceneLoaded;
        EventHandler.DropSelectedItemEvent += DropItemAtMousePosition;
        EventHandler.RemoveSelectedItemFromInventoryEvent += RemoveSelectedItemFromInventory;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= SceneLoaded;
        EventHandler.DropSelectedItemEvent -= DropItemAtMousePosition;
        EventHandler.RemoveSelectedItemFromInventoryEvent -= RemoveSelectedItemFromInventory;
    }

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void SceneLoaded()
    {
        itemsParent = GameObject.FindGameObjectWithTag(Tags.ItemsParent).transform;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        gridCursor = FindObjectOfType<GridCursor>();
    }

    private void ClearCursors()
    {
        gridCursor.DisableCursor();
        gridCursor.SelctedItemType = ItemType.none;
    }

    private void RemoveSelectedItemFromInventory()
    {
        if (itemDetails != null && isSelected)
        {
            int itemCode = itemDetails.itemCode;

            InventoryManager.Instance.RemoveItem(InventoryLocation.Player, itemCode);
            if (InventoryManager.Instance.FindItemInInventory(InventoryLocation.Player, itemCode) == -1)
            {
                ClearSelectedItem();
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemDetails != null)
        {
            Player.Instance.DisablePlayerInputAndResetMovement();
            draggedItem = Instantiate(inventoryBar.draggedItem, inventoryBar.transform);
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = inventorySlotImage.sprite;
            SetSelectedItem();
            if (GamepadCursor.Instance.CurrentControlScheme == gamepadScheme)
            {
                GamepadCursor.Instance.CanFadeOut = false;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedItem != null)
        {
            if (GamepadCursor.Instance.CurrentControlScheme == gamepadScheme)
            {
                draggedItem.transform.position = GamepadCursor.Instance.GetVirtualMousePosition();
            }
            else
            {
                draggedItem.transform.position = Mouse.current.position.ReadValue();
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggedItem != null)
        {
            Destroy(draggedItem);

            if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlot>() != null)
            {
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlot>().slotNumber;

                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.Player, slotNumber, toSlotNumber);

                DestroyInventoryTextBox();

                ClearSelectedItem();
            }
            else
            {
                if (itemDetails.canBeDropped)
                {
                    DropItemAtMousePosition();
                }
            }

            Player.Instance.EnablePlayerInput();

            if (GamepadCursor.Instance.CurrentControlScheme == gamepadScheme)
            {
                GamepadCursor.Instance.CanFadeOut = true;
            }
        }
    }

    private void DropItemAtMousePosition()
    {
        if (itemDetails != null && isSelected)
        {

            if (gridCursor.CursorPositionIsValid)
            {
                GameObject itemGameObject;
                if (GamepadCursor.Instance.CurrentControlScheme == Settings.gamepadScheme)
                {
                    Vector3Int cursorGridPosition = gridCursor.GetGridPositionForCursor();
                    Vector3 cursorPosition = new Vector3(cursorGridPosition.x + Settings.gridCellSize / 2, cursorGridPosition.y + Settings.gridCellSize / 2, -mainCamera.transform.position.z);
                    itemGameObject = Instantiate(itemPrefab, cursorPosition, Quaternion.identity, itemsParent);
                }
                else
                {
                    Vector2 mousePosition = Mouse.current.position.ReadValue();
                    Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x + Settings.gridCellSize/2, mousePosition.y + Settings.gridCellSize / 2, -mainCamera.transform.position.z));
                    itemGameObject = Instantiate(itemPrefab, worldPosition, Quaternion.identity, itemsParent);
                }

                Item item = itemGameObject.GetComponent<Item>();
                item.ItemCode = itemDetails.itemCode;

                InventoryManager.Instance.RemoveItem(InventoryLocation.Player, item.ItemCode);

                if (InventoryManager.Instance.FindItemInInventory(InventoryLocation.Player, item.ItemCode) == -1)
                {
                    ClearSelectedItem();
                }
            }

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemQuantity != 0)
        {
            inventoryBar.inventoryTextBoxGameObject = Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
            inventoryBar.inventoryTextBoxGameObject.transform.SetParent(parentCanvas.transform, false);

            InventoryTextBox inventoryTextBox = inventoryBar.inventoryTextBoxGameObject.GetComponent<InventoryTextBox>();

            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);

            inventoryTextBox.SetTextBoxes(itemDetails.itemDescription, itemTypeDescription, "", itemDetails.itemLongDescription, "", "");

            if (inventoryBar.IsInventoryBarAtBottom)
            {
                inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                inventoryBar.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 70, transform.position.z);
            }
            else
            {
                inventoryBar.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
                inventoryBar.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y - 70, transform.position.z);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyInventoryTextBox();
    }

    private void DestroyInventoryTextBox()
    {
        if (inventoryBar.inventoryTextBoxGameObject != null)
        {
            Destroy(inventoryBar.inventoryTextBoxGameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (isSelected == true)
            {
                ClearSelectedItem();
            }
            else
            {
                if (itemQuantity > 0)
                {
                    SetSelectedItem();
                }
            }
        }
    }

    public void SetSelectedItem()
    {
        inventoryBar.ClearHighlightOnInventorySlots();
        isSelected = true;
        inventoryBar.selectedSlot = slotNumber;
        inventoryBar.SetHighlightedInventorySlots();

        gridCursor.ItemUseGridRadius = itemDetails.itemUseGridRadius;

        if (itemDetails.itemUseGridRadius > 0)
        {
            gridCursor.EnableCursor();
        }
        else
        {
            gridCursor.DisableCursor();
        }

        gridCursor.SelctedItemType = itemDetails.itemType;

        InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.Player, itemDetails.itemCode);

        if (itemDetails.canBeCarried == true)
        {
            Player.Instance.ShowCarriedItem(itemDetails.itemCode);
        }
        else
        {
            Player.Instance.ClearCarriedItem();
        }
    }

    public void ClearSelectedItem()
    {
        ClearCursors();
        inventoryBar.ClearHighlightOnInventorySlots();
        isSelected = false;
        inventoryBar.selectedSlot = -1;
        InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.Player);

        Player.Instance.ClearCarriedItem();
    }
}
