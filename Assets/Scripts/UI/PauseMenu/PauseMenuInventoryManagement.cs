using System.Collections.Generic;
using UnityEngine;

public class PauseMenuInventoryManagement : MonoBehaviour
{
    [SerializeField] private PauseMenuInventorySlot[] inventorySlots = null;
    [SerializeField] Sprite transparent16x16 = null;
    [SerializeField] PlayerInventorySettings playerInventorySettings = null;

    [HideInInspector] public GameObject inventoryTextBoxGameObject;
    public GameObject draggedItem;

    private void OnEnable()
    {
        EventHandler.InventoryUpdatedEvent += PopulatePlayerInventory;
        if (InventoryManager.Instance != null)
        {
            PopulatePlayerInventory(InventoryLocation.Player, InventoryManager.Instance.inventoryLists[(int)InventoryLocation.Player]);
        }
    }

    private void OnDisable()
    {
        EventHandler.InventoryUpdatedEvent -= PopulatePlayerInventory;
        DestroyInventoryTextBox();
    }

    public void DestroyInventoryTextBox()
    {
        if (inventoryTextBoxGameObject != null)
        {
            Destroy(inventoryTextBoxGameObject);
        }
    }

    public void DestroyCurrentlyDraggedItems()
    {
        for (int i = 0; i < InventoryManager.Instance.inventoryLists[(int)InventoryLocation.Player].Count; i++)
        {
            if (inventorySlots[i].draggedItem != null)
            {
                Destroy(inventorySlots[i].draggedItem);
            }
        }
    }

    private void PopulatePlayerInventory(InventoryLocation inventoryLocation, List<InventoryItem> inventoryItems)
    {
        if (inventoryLocation == InventoryLocation.Player)
        {
            InitialiseInventoryManagemntSlots();

            for (int i = 0; i < InventoryManager.Instance.inventoryLists[(int)InventoryLocation.Player].Count; i++)
            {
                inventorySlots[i].itemDetails = InventoryManager.Instance.GetItemDetails(inventoryItems[i].itemCode);
                inventorySlots[i].itemQuantity = inventoryItems[i].itemQuantity;
                if (inventorySlots[i].itemDetails != null)
                {
                    inventorySlots[i].slotImage.sprite = inventorySlots[i].itemDetails.itemSprite;
                    inventorySlots[i].textMeshPro.text = inventorySlots[i].itemQuantity.ToString();
                }
            }
        }
    }

    private void InitialiseInventoryManagemntSlots()
    {
        for (int i = 0; i < playerInventorySettings.maxInventoryCapacity; i++)
        {
            inventorySlots[i].grayedOutImageGameObject.SetActive(false);
            inventorySlots[i].itemDetails = null;
            inventorySlots[i].itemQuantity = 0;
            inventorySlots[i].slotImage.sprite = transparent16x16;
            inventorySlots[i].textMeshPro.text = "";
        }

        for (int i = InventoryManager.Instance.inventoryListCapacity[(int)InventoryLocation.Player]; i < playerInventorySettings.maxInventoryCapacity; i++)
        {
            inventorySlots[i].grayedOutImageGameObject.SetActive(true);
        }
    }
}
