using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryBar : MonoBehaviour
{
    [SerializeField] private Sprite transparent16x16Sprite = null;
    [SerializeField] private InventorySlot[] inventorySlots = null;
    [SerializeField] private InputManager inputManager = null;

    private RectTransform rectTransform;
    private bool _isInventoryBarAtBottom = true;

    [HideInInspector] public int selectedSlot = -1;
    [HideInInspector] public GameObject inventoryTextBoxGameObject;

    public bool IsInventoryBarAtBottom
    {
        get => _isInventoryBarAtBottom;
        set => _isInventoryBarAtBottom = value;
    }

    public GameObject draggedItem;

    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += OnInventoryUpdated;
        inputManager.itemSelectLeft += ItemSelectLeft;
        inputManager.itemSelectRight += ItemSelectRight;
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= OnInventoryUpdated;
        inputManager.itemSelectLeft -= ItemSelectLeft;
        inputManager.itemSelectRight -= ItemSelectRight;
    }

    public void DestroyCurrentlyDraggedItems()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].draggedItem != null)
            {
                Destroy(inventorySlots[i].draggedItem);
            }
        }
    }

    public void ClearCurrentlySelectedItems()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].isSelected)
            {
                inventorySlots[i].ClearSelectedItem();
            }
        }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        SwitchInventoryBarPosition();
    }

    private void SwitchInventoryBarPosition()
    {
        Vector3 playerViewportPosition = Player.Instance.GetPlayerViewportPosition();

        if (playerViewportPosition.y < 0.25f)
        {
            rectTransform.pivot = new Vector2(0.5f, 1);
            rectTransform.anchorMin = new Vector2(0.5f, 1);
            rectTransform.anchorMax = new Vector2(0.5f, 1);
            rectTransform.anchoredPosition = new Vector2(0, -2.5f);

            IsInventoryBarAtBottom = false;
        }
        else if (playerViewportPosition.y > 0.25f)
        {
            rectTransform.pivot = new Vector2(0.5f, 0);
            rectTransform.anchorMin = new Vector2(0.5f, 0);
            rectTransform.anchorMax = new Vector2(0.5f, 0);
            rectTransform.anchoredPosition = new Vector2(0, 2.5f);

            IsInventoryBarAtBottom = true;
        }
    }

    private void OnInventoryUpdated(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if (inventoryLocation == InventoryLocation.Player)
        {
            ClearInventorySlots();

            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (i < inventoryList.Count)
                {
                    int itemCode = inventoryList[i].itemCode;

                    ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);

                    if (itemDetails != null)
                    {
                        inventorySlots[i].inventorySlotImage.sprite = itemDetails.itemSprite;
                        inventorySlots[i].textMeshProUGUI.text = inventoryList[i].itemQuantity.ToString();
                        inventorySlots[i].itemDetails = itemDetails;
                        inventorySlots[i].itemQuantity = inventoryList[i].itemQuantity;
                        SetHighlightedInventorySlots(i);
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }

    private void ClearInventorySlots()
    {
        if (inventorySlots.Length > 0)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                inventorySlots[i].inventorySlotImage.sprite = transparent16x16Sprite;
                inventorySlots[i].textMeshProUGUI.text = "";
                inventorySlots[i].itemDetails = null;
                inventorySlots[i].itemQuantity = 0;
                SetHighlightedInventorySlots(i);
            }
        }
    }

    public void ClearHighlightOnInventorySlots()
    {
        if (inventorySlots.Length > 0)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                if (inventorySlots[i].isSelected)
                {
                    inventorySlots[i].isSelected = false;
                    inventorySlots[i].inventorySlotHighlight.color = new Color(1, 1, 1, 0);
                    InventoryManager.Instance.ClearSelectedInventoryItem(InventoryLocation.Player);
                }
            }
        }
    }

    public void SetHighlightedInventorySlots()
    {
        if (inventorySlots.Length > 0)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                SetHighlightedInventorySlots(i);
            }
        }
    }

    private void SetHighlightedInventorySlots(int itemPosition)
    {
        if (inventorySlots.Length > 0 && inventorySlots[itemPosition].itemDetails != null)
        {
            if (inventorySlots[itemPosition].isSelected)
            {
                inventorySlots[itemPosition].inventorySlotHighlight.color = new Color(1, 1, 1, 1);

                InventoryManager.Instance.SetSelectedInventoryItem(InventoryLocation.Player, inventorySlots[itemPosition].itemDetails.itemCode);
            }
        }
    }

    private void ItemSelectLeft()
    {
        
        List<InventoryItem> inventoryList = InventoryManager.Instance.inventoryLists[(int)InventoryLocation.Player];
        if (inventoryList.Count > 0)
        {
            int nextSelectedSlot = selectedSlot - 1;
            if (nextSelectedSlot < 0)
            {
                if (inventoryList.Count < inventorySlots.Length)
                {
                    nextSelectedSlot = inventoryList.Count - 1;
                }
                else
                {
                    nextSelectedSlot = inventorySlots.Length - 1;
                }
            }
            if (inventorySlots[nextSelectedSlot].itemDetails != null)
            {
                inventorySlots[nextSelectedSlot].SetSelectedItem();
            }
        }
    }

    private void ItemSelectRight()
    {
        List<InventoryItem> inventoryList = InventoryManager.Instance.inventoryLists[(int)InventoryLocation.Player];
        if (inventoryList.Count > 0)
        {
            int nextSelectedSlot = selectedSlot + 1;
            if (nextSelectedSlot > inventoryList.Count || nextSelectedSlot == inventoryList.Count || nextSelectedSlot == inventorySlots.Length)
            {
                nextSelectedSlot = 0;
            }

            if (inventorySlots[nextSelectedSlot].itemDetails != null)
            {
                inventorySlots[nextSelectedSlot].SetSelectedItem();
            }
        }
    }
}
