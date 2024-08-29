using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Craft", order = 1)]
public class CraftSO : ScriptableObject
{
    [SerializeField] public ItemSO craftedItem;
    [SerializeField] public ItemSO[] requiredItems;
    [SerializeField] public int[] requiredAmounts;
    [SerializeField] public int amountCrafted;
}
