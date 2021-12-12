using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Item item = other.gameObject.GetComponent<Item>();

        if (item != null)
        {
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);
            if (itemDetails.canBePickedUp)
            {
                InventoryManager.Instance.AddItem(InventoryLocation.Player, item, other.gameObject);
            }
        }
    }
}
