using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    private InventoryController inventoryController;
    [SerializeField] private CraftSO[] craftables;
    [SerializeField] private GameObject craftList;
    [SerializeField] private GameObject craftSlotPrefab;
    private void Start()
    {
        inventoryController = FindObjectOfType<InventoryController>();
        craftables = Resources.LoadAll<CraftSO>("Crafts");
    }

    public void UpdateCraftList()
    {
        int craftListHeight = 275;
        int craftListCounter = -1;
        foreach (Transform child in craftList.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (CraftSO craft in craftables)
        {
            GameObject craftSlot = Instantiate(craftSlotPrefab, craftList.transform);
            craftSlot.GetComponent<CraftSlot>().SetCraftSlot(craft);
            craftListCounter++;
            if (craftListCounter == 4)
            {
                craftListHeight += 275;
                craftListCounter = 0;
            }
        }
        craftList.GetComponent<RectTransform>().sizeDelta = new Vector2(1750, craftListHeight);
    }
}
