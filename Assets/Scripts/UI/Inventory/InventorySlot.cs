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

    [SerializeField] private InventoryBar inventoryBar = null;
    [SerializeField] private GameObject itemPrefab = null;
    [SerializeField] private GameObject inventoryTextBoxPrefab = null;
    [SerializeField] private int slotNumber;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        itemsParent = GameObject.FindGameObjectWithTag(Tags.ItemsParent).transform;
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
            Vector2 mousePosition = GamepadCursor.Instance.CurrentControlScheme == gamepadScheme ? GamepadCursor.Instance.GetVirtualMousePosition() : Mouse.current.position.ReadValue();
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -mainCamera.transform.position.z));

            GameObject itemGameObject = Instantiate(itemPrefab, worldPosition, Quaternion.identity, itemsParent);
            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = itemDetails.itemCode;

            InventoryManager.Instance.RemoveItem(InventoryLocation.Player, item.ItemCode);

            if (InventoryManager.Instance.FindItemInInventory(InventoryLocation.Player, item.ItemCode) == -1)
            {
                ClearSelectedItem();
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
        inventoryBar.SetHighlightedInventorySlots();

        InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.Player, itemDetails.itemCode);
    }

    public void ClearSelectedItem()
    {
        inventoryBar.ClearHighlightOnInventorySlots();
        isSelected = false;
        InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.Player);
    }
}
