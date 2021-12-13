using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using System;

public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [HideInInspector] public ItemDetails itemDetails;
    [HideInInspector] public int itemQuantity;

    private Camera mainCamera;
    private Transform itemsParent;
    private GameObject draggedItem;
    private string gamepadScheme = "Gamepad";

    [SerializeField] private InventoryBar inventoryBar = null;
    [SerializeField] private GameObject itemPrefab = null;

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
        if (itemDetails != null)
        {
            Vector2 mousePosition = GamepadCursor.Instance.CurrentControlScheme == gamepadScheme ? GamepadCursor.Instance.GetVirtualMousePosition() : Mouse.current.position.ReadValue();
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, -mainCamera.transform.position.z));

            GameObject itemGameObject = Instantiate(itemPrefab, worldPosition, Quaternion.identity, itemsParent);
            Item item = itemGameObject.GetComponent<Item>();
            item.ItemCode = itemDetails.itemCode;

            InventoryManager.Instance.RemoveItem(InventoryLocation.Player, item.ItemCode);
        }
    }
}
