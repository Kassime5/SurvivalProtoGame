using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler//, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public InventoryController inventoryController;
    [SerializeField] public GameObject itemSelected;
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemQuantityText;
    [SerializeField] private Sprite emptySlotImage;
    [SerializeField] private int maxItems = 64;
    public ItemSO itemSOInSlot;
    public int itemQuantity;
    public bool isFull;
    [SerializeField] public ItemSlotType itemSlotType;
    [SerializeField] private GameObject itemDragDropTooltip;
    private FurnaceManager furnaceManager;

    void Awake()
    {
        ClearSlot();
        furnaceManager = FurnaceManager.instance;
    }

    void Update()
    {
        if (itemSlotType == ItemSlotType.FurnaceInput || itemSlotType == ItemSlotType.FurnaceFuel)
        {
            if (itemSOInSlot != null)
            {

            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        int clickCount = eventData.clickCount;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (clickCount == 1)
            {
                OnLeftClick();
            }
            else if (clickCount == 2)
            {
                OnDoubleClick();
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    private void OnDoubleClick()
    {
        OnLeftClick();
    }

    private void OnLeftClick()
    {
        // inventoryController.DeselectSlots();
        inventoryController.OnLeftClickSlot(this);
    }

    private void OnRightClick()
    {
        inventoryController.OnRightClickSlot(this);
    }
    public int AddItem(ItemSO item, int itemQuantity)
    {
        if (isFull)
        {
            return itemQuantity;
        }
        if (itemQuantity <= 0)
        {
            return 0;
        }
        int extraItems = 0;
        itemSOInSlot = item;
        itemImage.sprite = item.itemSprite;
        this.itemQuantity += itemQuantity;
        if (this.itemQuantity >= maxItems)
        {
            extraItems = this.itemQuantity - maxItems;
            this.itemQuantity = maxItems;
            itemQuantityText.text = this.itemQuantity.ToString();
            itemQuantityText.enabled = true;
            isFull = true;
        }
        else if (inventoryController.isEquipment(itemSOInSlot.itemType))
        {
            isFull = true;
            itemQuantityText.text = this.itemQuantity.ToString();
            itemQuantityText.enabled = true;
        }
        else
        {
            itemQuantityText.text = this.itemQuantity.ToString();
            itemQuantityText.enabled = true;
            isFull = false;
        }
        // if (itemSlotType == ItemSlotType.FurnaceInput || itemSlotType == ItemSlotType.FurnaceFuel)
        // {
        //     furnaceManager.HandleInput(this, item, itemQuantity);
        // }
        return extraItems;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemSOInSlot == null)
        {
            return;
        }
        int maxCharsPerLine = 25;
        string formattedItemName = itemSOInSlot.itemName;
        int currentIndex = 0;
        // Format the item name to fit within the tooltip
        while (currentIndex < formattedItemName.Length)
        {
            if (currentIndex + maxCharsPerLine < formattedItemName.Length) // Ensure we're not at the end
            {
                int splitIndex = currentIndex + maxCharsPerLine;
                int spaceBefore = -1;
                int spaceAfter = -1;

                // Look for spaces before and after the split index within a 3-character range
                for (int i = 0; i <= 3; i++)
                {
                    if (splitIndex - i > currentIndex && formattedItemName[splitIndex - i] == ' ')
                    {
                        spaceBefore = splitIndex - i;
                        break;
                    }
                    if (splitIndex + i < formattedItemName.Length && formattedItemName[splitIndex + i] == ' ')
                    {
                        spaceAfter = splitIndex + i;
                        break;
                    }
                }

                if (spaceBefore != -1) // Space found before
                {
                    formattedItemName = formattedItemName.Insert(spaceBefore + 1, "\n");
                    currentIndex = spaceBefore + 1;
                }
                else if (spaceAfter != -1) // Space found after
                {
                    formattedItemName = formattedItemName.Insert(spaceAfter + 1, "\n");
                    currentIndex = spaceAfter + 1;
                }
                else // No space found, split with a hyphen
                {
                    formattedItemName = formattedItemName.Insert(splitIndex, "-\n");
                    currentIndex = splitIndex + 2; // Skip the hyphen and newline
                }
            }
            else
            {
                break; // End of the string
            }
        }

        inventoryController.ShowItemTooltip(formattedItemName);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.HideItemTooltip();
    }
    public void ClearSlot()
    {
        itemQuantity = 0;
        itemImage.sprite = emptySlotImage;
        itemQuantityText.text = "";
        itemQuantityText.enabled = false;
        isFull = false;
        itemSOInSlot = null;
        itemSelected.SetActive(false);
    }
    internal void RefreshSlot()
    {
        if (itemSOInSlot == null || itemQuantity <= 0)
        {
            ClearSlot();
        }
        else
        {
            itemImage.sprite = itemSOInSlot.itemSprite;
            itemQuantityText.text = itemQuantity.ToString();
            itemQuantityText.enabled = true;
        }
    }
}

public enum ItemSlotType
{
    Inventory,
    Hotbar,
    FurnaceInput,
    FurnaceFuel,
    FurnaceOutput
}