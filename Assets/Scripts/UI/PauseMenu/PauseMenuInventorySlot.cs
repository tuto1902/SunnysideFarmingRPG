using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenuInventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image slotImage;
    public TextMeshProUGUI textMeshPro;
    public GameObject grayedOutImageGameObject;

    [SerializeField] private PauseMenuInventoryManagement pauseMenuInventoryManagement = null;
    [SerializeField] private GameObject inventoryTextBoxPrefab = null;
    [SerializeField] private int slotNumber;

    [HideInInspector] public ItemDetails itemDetails;
    [HideInInspector] public int itemQuantity;

    public GameObject draggedItem;

    // private Vector3 startingPosition;
    private Canvas parentCanvas;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemDetails != null)
        {
            draggedItem = Instantiate(pauseMenuInventoryManagement.draggedItem, pauseMenuInventoryManagement.transform);
            Image draggedItemImage = draggedItem.GetComponentInChildren<Image>();
            draggedItemImage.sprite = slotImage.sprite;
            
            if (GamepadCursor.Instance.CurrentControlScheme == Settings.gamepadScheme)
            {
                GamepadCursor.Instance.CanFadeOut = false;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggedItem != null)
        {
            if (GamepadCursor.Instance.CurrentControlScheme == Settings.gamepadScheme)
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

            if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.GetComponent<PauseMenuInventorySlot>() != null)
            {
                int toSlotNumber = eventData.pointerCurrentRaycast.gameObject.GetComponent<PauseMenuInventorySlot>().slotNumber;
                
                InventoryManager.Instance.SwapInventoryItems(InventoryLocation.Player, slotNumber, toSlotNumber);

                pauseMenuInventoryManagement.DestroyInventoryTextBox();
            }

            if (GamepadCursor.Instance.CurrentControlScheme == Settings.gamepadScheme)
            {
                GamepadCursor.Instance.CanFadeOut = true;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemQuantity != 0)
        {
            pauseMenuInventoryManagement.inventoryTextBoxGameObject = Instantiate(inventoryTextBoxPrefab, transform.position, Quaternion.identity);
            pauseMenuInventoryManagement.inventoryTextBoxGameObject.transform.SetParent(parentCanvas.transform, false);

            InventoryTextBox inventoryTextBox = pauseMenuInventoryManagement.inventoryTextBoxGameObject.GetComponent<InventoryTextBox>();

            string itemTypeDescription = InventoryManager.Instance.GetItemTypeDescription(itemDetails.itemType);

            inventoryTextBox.SetTextBoxes(itemDetails.itemDescription, itemTypeDescription, "", itemDetails.itemLongDescription, "", "");

            if (slotNumber < 12)
            {
                pauseMenuInventoryManagement.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                pauseMenuInventoryManagement.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 70, transform.position.z);
            }
            else
            {
                pauseMenuInventoryManagement.inventoryTextBoxGameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
                pauseMenuInventoryManagement.inventoryTextBoxGameObject.transform.position = new Vector3(transform.position.x, transform.position.y - 70, transform.position.z);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pauseMenuInventoryManagement.DestroyInventoryTextBox();
    }
}
