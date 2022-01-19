using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : SingletonMonoBehaviour<InventoryManager>, ISaveable
{
    private string _uniqueID;
    private GameObjectSave _gameObjectSave;
    private Dictionary<int, ItemDetails> itemDetailsDictionary;
    private int[] selectedInventoryItem;
    private InventoryBar inventoryBar;
    [SerializeField] private ItemList itemList = null;
    [SerializeField] private PlayerInventorySettings playerInventorySettings;

    public List<InventoryItem>[] inventoryLists;
    [HideInInspector] public int[] inventoryListCapacity;

    public string UniqueID
    {
        get => _uniqueID;
        set => _uniqueID = value;
    }
    public GameObjectSave GameObjectSave
    {
        get => _gameObjectSave;
        set => _gameObjectSave = value;
    }

    protected override void Awake()
    {
        base.Awake();
        CreateInventoryLists();
        CreateItemDetailsDictionary();

        selectedInventoryItem = new int[(int)InventoryLocation.count];
        for (int i = 0; i < selectedInventoryItem.Length; i++)
        {
            selectedInventoryItem[i] = -1;
        }

        UniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void OnEnable()
    {
        Register();
    }

    private void OnDisable()
    {
        Deregister();
    }

    private void Start()
    {
        inventoryBar = FindObjectOfType<InventoryBar>();
    }

    private void CreateInventoryLists()
    {
        inventoryLists = new List<InventoryItem>[(int)InventoryLocation.count];

        for (int i = 0; i < (int)InventoryLocation.count; i++)
        {
            inventoryLists[i] = new List<InventoryItem>();
        }

        inventoryListCapacity = new int[(int)InventoryLocation.count];
        inventoryListCapacity[(int)InventoryLocation.Player] = playerInventorySettings.initialInvetoryCapacity;
    }

    public ItemDetails GetItemDetails(int itemCode)
    {
        ItemDetails itemDetails;

        if (itemDetailsDictionary.TryGetValue(itemCode, out itemDetails))
        {
            return itemDetails;
        }

        return null;
    }

    public void AddItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];
        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);
        if (itemPosition != -1)
        {
            AddItemAtPosition(inventoryList, itemCode, itemPosition);
        }
        else
        {
            AddItemAtPosition(inventoryList, itemCode);
        }

        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    public void AddItem(InventoryLocation inventoryLocation, Item item, GameObject itemGameObject)
    {
        AddItem(inventoryLocation, item.ItemCode);
        Destroy(itemGameObject);
    }

    public void SwapInventoryItems(InventoryLocation inventoryLocation, int fromSlot, int toSlot)
    {
        if (fromSlot < inventoryLists[(int)inventoryLocation].Count && toSlot < inventoryLists[(int)inventoryLocation].Count && fromSlot != toSlot && fromSlot >= 0 && toSlot >= 0)
        {
            InventoryItem fromItem = inventoryLists[(int)inventoryLocation][fromSlot];
            InventoryItem toItem = inventoryLists[(int)inventoryLocation][toSlot];

            inventoryLists[(int)inventoryLocation][fromSlot] = toItem;
            inventoryLists[(int)inventoryLocation][toSlot] = fromItem;

            EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
        }
    }

    public string GetItemTypeDescription(ItemType itemType)
    {
        string itemTypeDescription;
        switch (itemType)
        {
            case ItemType.BreakingTool:
                itemTypeDescription = Settings.BreakingTool;
                break;
            case ItemType.BuildingTool:
                itemTypeDescription = Settings.BuildingTool;
                break;
            case ItemType.ChoppingTool:
                itemTypeDescription = Settings.ChoppingTool;
                break;
            case ItemType.CollectingTool:
                itemTypeDescription = Settings.CollectingTool;
                break;
            case ItemType.WateringTool:
                itemTypeDescription = Settings.WateringTool;
                break;
            case ItemType.DiggingTool:
                itemTypeDescription = Settings.DiggingTool;
                break;
            case ItemType.Weapon:
                itemTypeDescription = Settings.Weapon;
                break;
            default:
                itemTypeDescription = itemType.ToString();
                break;
        }

        return itemTypeDescription;
    }

    public void SetSelectedInventoryItem(InventoryLocation inventoryLocation, int itemCode)
    {
        selectedInventoryItem[(int)inventoryLocation] = itemCode;
    }

    public void ClearSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        selectedInventoryItem[(int)inventoryLocation] = -1;
    }

    private void CreateItemDetailsDictionary()
    {
        itemDetailsDictionary = new Dictionary<int, ItemDetails>();

        foreach (ItemDetails itemDetails in itemList.itemDetailsList)
        {
            itemDetailsDictionary.Add(itemDetails.itemCode, itemDetails);
        }
    }

    public int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];
        for (int i = 0; i < inventoryList.Count; i++)
        {
            if (inventoryList[i].itemCode == itemCode)
            {
                return i;
            }
        }

        return -1;
    }

    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int itemPosition)
    {
        InventoryItem inventoryItem = new InventoryItem();
        int quantity = inventoryList[itemPosition].itemQuantity + 1;
        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = quantity;
        inventoryList[itemPosition] = inventoryItem;
    }

    private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode)
    {
        InventoryItem inventoryItem = new InventoryItem();
        inventoryItem.itemCode = itemCode;
        inventoryItem.itemQuantity = 1;
        inventoryList.Add(inventoryItem);
    }

    internal void RemoveItem(InventoryLocation inventoryLocation, int itemCode)
    {
        List<InventoryItem> inventoryList = inventoryLists[(int)inventoryLocation];

        int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

        if (itemPosition != -1)
        {
            RemoveItemAtPosition(inventoryList, itemCode, itemPosition);
        }

        EventHandler.CallInventoryUpdatedEvent(inventoryLocation, inventoryLists[(int)inventoryLocation]);
    }

    private void RemoveItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int itemPosition)
    {
        InventoryItem inventoryItem = new InventoryItem();
        int quantity = inventoryList[itemPosition].itemQuantity - 1;

        if (quantity > 0)
        {
            inventoryItem.itemCode = itemCode;
            inventoryItem.itemQuantity = quantity;
            inventoryList[itemPosition] = inventoryItem;
        }
        else
        {
            inventoryList.RemoveAt(itemPosition);
        }
    }

    private int GetSelectedInventoryItem(InventoryLocation inventoryLocation)
    {
        return selectedInventoryItem[(int)inventoryLocation];
    }

    public ItemDetails GetSelectedInventoryItemDetails(InventoryLocation inventoryLocation)
    {
        int itemCode = GetSelectedInventoryItem(inventoryLocation);

        if (itemCode != -1)
        {
            return GetItemDetails(itemCode);
        }

        return null;
    }

    public void Register()
    {
        SaveLoadManager.Instance.saveableObjectList.Add(this);
    }

    public void Deregister()
    {
        SaveLoadManager.Instance.saveableObjectList.Remove(this);
    }

    public void StoreScene(string sceneName)
    {
        // N/A
    }

    public void RestoreScene(string sceneName)
    {
        // N/A
    }

    public GameObjectSave SaveGame()
    {
        SceneSave sceneSave = new SceneSave();
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        sceneSave.inventoryLists = inventoryLists;
        sceneSave.intArrayDictionary = new Dictionary<string, int[]>();
        sceneSave.intArrayDictionary.Add("inventoryListCapacity", inventoryListCapacity);

        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;
    }

    public void LoadGame(GameSave gameSave)
    {
        if (gameSave.gameObjectData.TryGetValue(UniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            if (GameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                if (sceneSave.inventoryLists != null)
                {
                    inventoryLists = sceneSave.inventoryLists;
                    for (int i = 0; i < (int)InventoryLocation.count; i++)
                    {
                        EventHandler.CallInventoryUpdatedEvent((InventoryLocation)i, inventoryLists[i]);
                    }

                    Player.Instance.ClearCarriedItem();
                    inventoryBar.ClearHighlightOnInventorySlots();
                }

                if (sceneSave.intArrayDictionary != null && sceneSave.intArrayDictionary.TryGetValue("inventoryListCapacity", out int[] storedInventoryListCapacity))
                {
                    inventoryListCapacity = storedInventoryListCapacity;
                }
            }
        }
    }
}
