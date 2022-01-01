using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private int _itemCode;
    private SpriteRenderer spriteRenderer;

    public int ItemCode
    {
        get => _itemCode;
        set => _itemCode = value;
    }

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (ItemCode != 0)
        {
            Init(ItemCode);
        }
    }

    public void Init(int itemCode)
    {
        ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(itemCode);
        spriteRenderer.sprite = itemDetails.itemSprite;
        ItemCode = itemDetails.itemCode;
        if (itemDetails.itemType == ItemType.ReapableScenary)
        {
            //gameObject.AddComponent<ItemNudge>();
        }
    }
}
