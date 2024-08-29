using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class FurnaceManager : MonoBehaviour
{
    public static FurnaceManager instance;
    public List<FurnaceSO> furnaceSOs = new List<FurnaceSO>();
    private Furnace currentFurnace;
    [SerializeField] private ItemSlot inputSlot;
    [SerializeField] private ItemSlot fuelSlot;
    [SerializeField] private ItemSlot outputSlot;
    [SerializeField] private Slider progressBar;

    private void Awake()
    {
        instance = this;
        furnaceSOs = Resources.LoadAll<FurnaceSO>("FurnaceRecipes").ToList();
    }

    private void Start()
    {
        foreach (FurnaceSO Recipe in furnaceSOs)
        {
            // print(Recipe.inputItem.itemName + " -> " + Recipe.outputItem.itemName + " x" + Recipe.outputAmount + " in " + Recipe.smeltTime + " seconds");
        }
    }
    public void OpenFurnace(Furnace furnace)
    {
        currentFurnace = furnace;
        inputSlot.ClearSlot();
        fuelSlot.ClearSlot();
        outputSlot.ClearSlot();
        if (furnace == null)
        {
            return;
        }

        // print("Opening furnace" + currentFurnace);

        if (currentFurnace.inputItem != null) inputSlot.AddItem(currentFurnace.inputItem, currentFurnace.inputAmount);
        if (currentFurnace.fuelItem != null) fuelSlot.AddItem(currentFurnace.fuelItem, currentFurnace.fuelAmount);
        if (currentFurnace.outputItem != null) outputSlot.AddItem(currentFurnace.outputItem, currentFurnace.outputAmount);
    }

    public void RefreshUI(Furnace furnace)
    {
        if (furnace == currentFurnace)
        {
            OpenFurnace(furnace);
        }
    }

    public void HandleInput(ItemSlot slot, ItemSO item, int amount)
    {
        // Called whenever the input slot changes
        if (currentFurnace == null)
        {
            return;
        }
        if (slot.itemSlotType != ItemSlotType.FurnaceInput && slot.itemSlotType != ItemSlotType.FurnaceFuel)
        {
            // print(slot.itemSlotType);
            return;
        }
        if (item == null)
        {
            currentFurnace.ClearSlot(slot.itemSlotType);
        }
        else
        {
            if (amount > 0)
            {
                currentFurnace.AddToSlot(slot.itemSlotType, item, amount);
            }
            else
            {
                currentFurnace.RemoveFromSlot(slot.itemSlotType, item, amount);
            }
        }
    }

    public FurnaceSO GetRecipe(ItemSO inputItem)
    {
        foreach (FurnaceSO Recipe in furnaceSOs)
        {
            if (Recipe.inputItem == inputItem)
            {
                return Recipe;
            }
        }
        return null;
    }

    public void UpdateProgressBar(float max, int value, Furnace furnace)
    {
        if (furnace != currentFurnace)
        {
            return;
        }
        progressBar.maxValue = max;
        progressBar.value = value;
    }
}
