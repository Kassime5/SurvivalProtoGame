using System;
using System.Collections.Generic;
using GinjaGaming.FinalCharacterController;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour, IMenu
{
    Dictionary<string, int> keyMapping = new Dictionary<string, int>
    {
        { "/Keyboard/1", 0 },
        { "/Keyboard/2", 1 },
        { "/Keyboard/3", 2 },
        { "/Keyboard/4", 3 },
        { "/Keyboard/5", 4 },
        { "/Keyboard/6", 5 },
        { "/Keyboard/7", 6 },
        { "/Keyboard/8", 7 },
        { "/Keyboard/9", 8 },
        { "/Keyboard/0", 9 }
    };
    [SerializeField] public ItemSlot[] itemSlots;
    [SerializeField] public GameObject inventoryPanel;
    [SerializeField] private GameObject craftingPanel;
    [SerializeField] private GameObject furnacePanel;
    [SerializeField] private GameObject itemTooltip;
    [SerializeField] private TMPro.TextMeshProUGUI itemTooltipText;
    [SerializeField] private ItemSlot itemTooltipSlot;
    private PlayerInput playerInput;
    private bool isInventoryOpen = false;
    public static InventoryController instance { get; private set; }
    private NotificationManager notificationManager;
    private CraftingManager craftingManager;
    private PlayerRayCastInfo playerRayCastInfo;
    private Vector2 offset;
    private string currentMode = "Inventory";
    private int hotBarSlotIndex = 0;

    # region Static Methods
    private void Awake()
    {
        instance = this;
        playerInput = new PlayerInput();
        playerInput.Player.Inventory.performed += ctx => MenuButtonPressed();
        playerInput.Player.HotBar.performed += HotBarSlotSelected;
        notificationManager = FindObjectOfType<NotificationManager>();
        craftingManager = FindObjectOfType<CraftingManager>();
        playerRayCastInfo = FindObjectOfType<PlayerRayCastInfo>();
    }

    private void Start()
    {
        inventoryPanel.SetActive(false);
        furnacePanel.SetActive(false);
        itemTooltip.SetActive(false);
        // itemSOs = Resources.LoadAll<ItemSO>("Items");
        SelectHotBarSlot(0);
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }
    # endregion
    private void HotBarSlotSelected(InputAction.CallbackContext context)
    {
        string controlPath = context.control.path;
        if (keyMapping.TryGetValue(controlPath, out int selectedKey))
        {
            if (selectedKey >= 0 && selectedKey < itemSlots.Length)
            {
                SelectHotBarSlot(selectedKey);
                return;
            }
        }
        print("Invalid hotbar slot selected");
    }

    private void SelectHotBarSlot(int slotIndex)
    {
        hotBarSlotIndex = slotIndex;
        DeselectSlots();
        itemSlots[slotIndex].itemSelected.SetActive(true);
        playerRayCastInfo.EquipItem(itemSlots[slotIndex].itemSOInSlot);
    }

    private void MenuButtonPressed()
    {
        MenuManager.instance.OpenMenu(gameObject, "Inventory");
    }

    public bool ToggleMenu(params object[] args)
    {
        string nextMode = args.Length > 0 && args[0] is string ? (string)args[0] : "";
        // DeselectSlots();

        if (!isInventoryOpen)
        {
            isInventoryOpen = true;
        }
        else
        {
            FurnaceManager.instance.OpenFurnace(null);
            isInventoryOpen = false;
            inventoryPanel.SetActive(false);
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;
            return false;
        }

        currentMode = nextMode;
        inventoryPanel.SetActive(true);
        // Cursor.lockState = isInventoryOpen ? CursorLockMode.None : CursorLockMode.Locked;
        // Cursor.visible = isInventoryOpen;

        if (currentMode == "Inventory")
        {
            craftingPanel.SetActive(true);
            furnacePanel.SetActive(false);
            craftingManager.UpdateCraftList();
        }
        else if (currentMode == "Furnace")
        {
            craftingPanel.SetActive(false);
            furnacePanel.SetActive(true);
            var furnaceObject = (GameObject)args[1];
            FurnaceManager.instance.OpenFurnace(furnaceObject.GetComponent<Furnace>());
        }
        else
        {
            print(nextMode + " is not a valid mode");
        }
        return isInventoryOpen;

        // itemTooltip.SetActive(isInventoryOpen);
    }
    void Update()
    {
        if (isInventoryOpen)
        {
            if (itemTooltip.activeSelf)
            {
                UpdateItemTooltipPosition();
            }
            UpdateItemTooltipSlotPosition();
        }
    }
    private void UpdateItemTooltipSlotPosition()
    {
        if (itemTooltipSlot.itemSOInSlot == null)
        {
            itemTooltipSlot.gameObject.SetActive(false);
            return;
        }
        itemTooltipSlot.gameObject.SetActive(true);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)inventoryPanel.transform, Input.mousePosition, null, out localPoint);
        localPoint += offset;
        itemTooltipSlot.transform.localPosition = localPoint;
    }

    private void UpdateItemTooltipPosition()
    {
        if (!itemTooltip.activeSelf)
        {
            return;
        }
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)inventoryPanel.transform, Input.mousePosition, null, out localPoint);
        localPoint += offset;
        itemTooltip.transform.localPosition = localPoint;
    }
    public void ShowItemTooltip(string text)
    {
        itemTooltipText.text = text;
        itemTooltip.SetActive(true);
        offset = new Vector2(itemTooltip.transform.GetComponent<RectTransform>().rect.width / 2 + 35, -itemTooltip.transform.GetComponent<RectTransform>().rect.height / 2 - 35);
        UpdateItemTooltipPosition();

    }
    public void HideItemTooltip()
    {
        itemTooltip.SetActive(false);
    }
    public int AddItem(ItemSO item, int itemQuantity)
    {
        if (itemQuantity == 0)
        {
            return 0;
        }
        int slotIndex = FindItemSlot(item.itemName);
        if (slotIndex != -1)
        {
            int leftOverItems = itemSlots[slotIndex].AddItem(item, itemQuantity);
            if (leftOverItems > 0)
            {
                leftOverItems = AddItem(item, leftOverItems);
            }
            else
            {
                notificationManager.PickingUpItemNotification(item.itemName, item.itemSprite, itemQuantity - leftOverItems);
            }
            return leftOverItems;
        }
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].isFull == false && itemSlots[i].itemSOInSlot == item || itemSlots[i].itemQuantity == 0)
            {
                int leftOverItems = itemSlots[i].AddItem(item, itemQuantity);

                if (leftOverItems > 0)
                {
                    leftOverItems = AddItem(item, leftOverItems);
                }
                else
                {
                    notificationManager.PickingUpItemNotification(item.itemName, item.itemSprite, itemQuantity - leftOverItems);
                }
                if (itemSlots[i].itemSlotType == ItemSlotType.Hotbar && i == hotBarSlotIndex) SelectHotBarSlot(i);
                return leftOverItems;
            }
        }
        return itemQuantity;
    }

    private int FindItemSlot(string itemName)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i].itemSOInSlot == null)
            {
                continue;
            }
            if (itemSlots[i].itemSOInSlot.itemName == itemName && !itemSlots[i].isFull)
            {
                return i;
            }
        }
        return -1;
    }

    public bool isEquipment(ItemType itemType)
    {
        if (itemType == ItemType.Equipment)
        {
            return true;
        }
        return false;
    }

    internal void DeselectSlots()
    {
        foreach (ItemSlot itemSlot in itemSlots)
        {
            itemSlot.itemSelected.SetActive(false);
        }
    }

    public void CanCraft(CraftSO craft)
    {
        bool canCraft = true;
        for (int i = 0; i < craft.requiredItems.Length; i++)
        {
            if (!HaveItems(craft.requiredItems[i], craft.requiredAmounts[i]))
            {
                canCraft = false;
                break;
            }
        }
        if (canCraft)
        {
            CraftItem(craft);
        }
        else
        {
            notificationManager.CraftFailedNotification(craft.craftedItem.itemSprite);
        }
    }

    public bool HaveItems(ItemSO item, int itemQuantity)
    {
        if (itemQuantity == 0)
        {
            return false;
        }
        int slotIndex = FindItemSlot(item.itemName);
        if (slotIndex != -1)
        {
            if (itemSlots[slotIndex].itemQuantity >= itemQuantity)
            {
                return true;
            }
        }
        return false;
    }
    public void CraftItem(CraftSO craft)
    {
        foreach (ItemSO requiredItem in craft.requiredItems)
        {
            if (RemoveItem(requiredItem, craft.requiredAmounts[Array.IndexOf(craft.requiredItems, requiredItem)]))
            {
                continue;
            }
            else
            {
                Debug.LogError("Failed to remove item " + requiredItem.itemName);
                return;
            }
        }
        AddItem(craft.craftedItem, craft.amountCrafted);
    }

    public bool RemoveItem(ItemSO itemSO, int itemQuantity)
    {
        if (itemQuantity == 0)
        {
            return false;
        }
        int slotIndex = FindItemSlot(itemSO.itemName);
        if (slotIndex != -1)
        {
            if (itemSlots[slotIndex].itemQuantity >= itemQuantity)
            {
                itemSlots[slotIndex].itemQuantity -= itemQuantity;
                if (itemSlots[slotIndex].itemQuantity == 0)
                {
                    itemSlots[slotIndex].ClearSlot();
                }
                else
                {
                    itemSlots[slotIndex].RefreshSlot();
                }
                return true;
            }
        }
        return false;
    }

    internal void SwapItems(ItemSlot itemSlot, ItemSlot targetSlot)
    {
        ItemSO itemToSwap = itemSlot.itemSOInSlot;
        int itemQuantityToSwap = itemSlot.itemQuantity;
        ItemSO targetItem = targetSlot.itemSOInSlot;
        int targetItemQuantity = targetSlot.itemQuantity;

        // If the target slot is empty, move the item to the target slot
        if (targetItem == null)
        {
            targetSlot.AddItem(itemToSwap, itemQuantityToSwap);
            itemSlot.ClearSlot();
        }
        else
        {
            // Swap items between slots
            itemSlot.ClearSlot();
            targetSlot.ClearSlot();
            itemSlot.AddItem(targetItem, targetItemQuantity);
            targetSlot.AddItem(itemToSwap, itemQuantityToSwap);
            SelectHotBarSlot(hotBarSlotIndex);
        }
        itemTooltipText.text = targetSlot.itemSOInSlot.itemName;
        itemTooltip.SetActive(true);
    }

    // int StackItems(ItemSlot itemSlot, ItemSlot targetSlot)
    // {
    //     int leftOverItems = targetSlot.AddItem(itemSlot.itemSOInSlot, itemSlot.itemQuantity);
    //     if (leftOverItems > 0)
    //     {
    //         itemSlot.itemQuantity = leftOverItems;
    //         itemSlot.RefreshSlot();
    //         return leftOverItems;
    //     }
    //     else
    //     {
    //         itemSlot.ClearSlot();
    //         return 0;
    //     }
    // }

    public void OnLeftClickSlot(ItemSlot slot)
    {
        if (slot == itemTooltipSlot)
        {
            return;
        }
        // If there is no item in the tooltip slot
        if (itemTooltipSlot.itemSOInSlot == null)
        {
            // Item is transferred to the tooltip slot
            itemTooltipSlot.AddItem(slot.itemSOInSlot, slot.itemQuantity);
            FurnaceManager.instance.HandleInput(slot, slot.itemSOInSlot, -slot.itemQuantity);
            slot.ClearSlot();
            return;
        }
        else if (slot.itemSOInSlot == null)
        {
            slot.AddItem(itemTooltipSlot.itemSOInSlot, itemTooltipSlot.itemQuantity);
            FurnaceManager.instance.HandleInput(slot, itemTooltipSlot.itemSOInSlot, itemTooltipSlot.itemQuantity);
            itemTooltipSlot.ClearSlot();
            return;
        }
        else
        {
            // If the item in the tooltip slot is the same as the item in the slot
            if (itemTooltipSlot.itemSOInSlot == slot.itemSOInSlot)
            {
                // Try to stack the items
                if (slot.AddItem(itemTooltipSlot.itemSOInSlot, itemTooltipSlot.itemQuantity) == 0)
                {
                    // All items are stacked
                    FurnaceManager.instance.HandleInput(slot, slot.itemSOInSlot, itemTooltipSlot.itemQuantity);
                    itemTooltipSlot.ClearSlot();
                    return;
                }
            }
            // The items are swapped
            SwapItems(slot, itemTooltipSlot);
            FurnaceManager.instance.HandleInput(slot, slot.itemSOInSlot, slot.itemQuantity);
            return;
        }
    }

    internal void OnRightClickSlot(ItemSlot slot)
    {
        if (slot == itemTooltipSlot)
        {
            return;
        }

        // If no item in the tooltip slot
        // Right click to split the stack
        if (itemTooltipSlot.itemSOInSlot == null)
        {
            int splitAmount = (slot.itemQuantity + 1) / 2;
            if (splitAmount == 0)
            {
                return;
            }
            itemTooltipSlot.AddItem(slot.itemSOInSlot, splitAmount);
            slot.itemQuantity -= splitAmount;
            slot.RefreshSlot();
            FurnaceManager.instance.HandleInput(slot, slot.itemSOInSlot, -splitAmount);
            return;
        }

        // If the item in the tooltip slot is the same as the item in the slot
        // Or if the slot is empty
        if (itemTooltipSlot.itemSOInSlot == slot.itemSOInSlot || slot.itemSOInSlot == null)
        {
            // Put one item down
            slot.AddItem(itemTooltipSlot.itemSOInSlot, 1);
            FurnaceManager.instance.HandleInput(slot, itemTooltipSlot.itemSOInSlot, 1);
            itemTooltipSlot.itemQuantity -= 1;
            if (itemTooltipSlot.itemQuantity == 0)
            {
                itemTooltipSlot.ClearSlot();
            }
            slot.RefreshSlot();
            itemTooltipSlot.RefreshSlot();
            return;
        }
        slot.RefreshSlot();
        itemTooltipSlot.RefreshSlot();
    }
}

public enum ItemType
{
    None,
    Consumable,
    Resource,
    Equipment,
    Placable,
}

public enum EquipmentType
{
    None,
    BuildingHammer,
    Axe,
    Pickaxe,
    Shovel,
}
