using System;
using UnityEngine;

public class Furnace : MonoBehaviour, IInteractable
{
    private FurnaceManager furnaceManager;
    private InventoryController inventoryController;
    private FurnaceSO furnaceRecipe;
    private float smeltTimer;
    private bool isSmelting;
    public ItemSO inputItem;
    public int inputAmount = 0;
    public ItemSO outputItem;
    public int outputAmount = 0;
    public ItemSO fuelItem;
    public int fuelAmount = 0;
    // Progress bar

    private void Start()
    {
        furnaceManager = FurnaceManager.instance;
        inventoryController = InventoryController.instance;
    }

    public void Interact()
    {
        // Bring up furnace UI
        MenuManager.instance.OpenMenu(inventoryController.gameObject, "Furnace", gameObject);
        furnaceManager.UpdateProgressBar(0, 0, this);
    }

    public void AddToSlot(ItemSlotType slotName, ItemSO item, int amount)
    {
        if (slotName == ItemSlotType.FurnaceInput)
        {
            if (inputItem == item)
            {
                inputAmount += amount;
                return;
            }
            inputItem = item;
            inputAmount = amount;
        }
        else if (slotName == ItemSlotType.FurnaceFuel)
        {
            if (fuelItem == item)
            {
                fuelAmount += amount;
                return;
            }
            fuelItem = item;
            fuelAmount = amount;
        }
        else if (slotName == ItemSlotType.FurnaceOutput)
        {
            if (outputItem == item)
            {
                outputAmount += amount;
                return;
            }
            outputItem = item;
            outputAmount = amount;
        }
    }

    private void Update()
    {
        // print("Furnace content" + inputItem + " x" + inputAmount + " + " + fuelItem + " x" + fuelAmount + " -> " + outputItem + " x" + outputAmount);
        furnaceRecipe = furnaceManager.GetRecipe(inputItem);
        if (furnaceRecipe == null)
        {
            // print("No recipe found");
            return;
        }
        if (inputAmount > 0 && fuelAmount > 0)
        {
            if (outputItem != furnaceRecipe.outputItem && outputItem != null) return;
            isSmelting = true;
        }
        else
        {
            isSmelting = false;
        }

        if (isSmelting)
        {
            // print("Smelting");
            smeltTimer += Time.deltaTime;
            furnaceManager.UpdateProgressBar(furnaceRecipe.smeltTime, (int)smeltTimer, this);
            if (smeltTimer >= furnaceRecipe.smeltTime)
            {
                smeltTimer = 0;
                isSmelting = false;
                Smelt();
            }
        }
    }


    private void Smelt()
    {
        inputAmount -= 1;
        fuelAmount -= 1;
        outputItem = furnaceRecipe.outputItem;
        outputAmount += furnaceRecipe.outputAmount;
        furnaceManager.RefreshUI(this);
        if (inputAmount == 0 || fuelAmount == 0)
        {
            isSmelting = false;
        }
    }

    internal void RemoveFromSlot(ItemSlotType slotName, ItemSO item, int amount)
    {
        if (slotName == ItemSlotType.FurnaceInput)
        {
            inputAmount += amount;
            if (inputAmount <= 0)
            {
                inputItem = null;
                inputAmount = 0;
            }
        }
        else if (slotName == ItemSlotType.FurnaceFuel)
        {
            fuelAmount += amount;
            if (fuelAmount <= 0)
            {
                fuelItem = null;
                fuelAmount = 0;
            }
        }
        else if (slotName == ItemSlotType.FurnaceOutput)
        {
            outputAmount += amount;
            if (outputAmount <= 0)
            {
                outputItem = null;
                outputAmount = 0;
            }
        }
    }

    internal void ClearSlot(ItemSlotType itemSlotType)
    {
        if (itemSlotType == ItemSlotType.FurnaceInput)
        {
            inputItem = null;
            inputAmount = 0;
        }
        else if (itemSlotType == ItemSlotType.FurnaceFuel)
        {
            fuelItem = null;
            fuelAmount = 0;
        }
        else if (itemSlotType == ItemSlotType.FurnaceOutput)
        {
            outputItem = null;
            outputAmount = 0;
        }
    }
}
