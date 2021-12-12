using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public Image inventorySlotHighlight;
    public Image inventorySlotImage;
    public TextMeshProUGUI textMeshProUGUI;

    [HideInInspector] public ItemDetails itemDetails;
    [HideInInspector] public int itemQuantity;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemDetails != null)
        {
            Debug.Log($"Clicked {itemDetails.itemDescription}");
        }
        else
        {
            Debug.Log("Clicked empty");
        }
    }
}
