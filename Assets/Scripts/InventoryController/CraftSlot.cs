using UnityEngine;
using UnityEngine.UI;

public class CraftSlot : MonoBehaviour
{
    [SerializeField] private Image craftImage;
    [SerializeField] private TMPro.TextMeshProUGUI craftName;
    [SerializeField] private TMPro.TextMeshProUGUI craftDescription;
    private CraftSO craftSO;

    public void SetCraftSlot(CraftSO item)
    {
        craftSO = item;
        craftImage.sprite = item.craftedItem.itemSprite;
        craftName.text = item.craftedItem.itemName + " x" + item.amountCrafted.ToString();
        craftDescription.text = "";
        for (int i = 0; i < item.requiredItems.Length; i++)
        {
            craftDescription.text += item.requiredItems[i].itemName + " x" + item.requiredAmounts[i] + "\n";
        }
    }

    public void OnClick()
    {
        FindObjectOfType<InventoryController>().CanCraft(craftSO);
    }
}
