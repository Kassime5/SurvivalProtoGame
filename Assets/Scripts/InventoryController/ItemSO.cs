using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Item", order = 1)]
public class ItemSO : ScriptableObject
{
    [SerializeField] public string itemName;
    [TextArea][SerializeField] public string itemDescription;
    [SerializeField] public Sprite itemSprite;
    [SerializeField] public ItemType itemType;
    [SerializeField] public EquipmentType equipmentType = EquipmentType.None;
    [SerializeField] public int equipmentLevel = 1;
    [SerializeField] public bool canBeUseAsFuel = false;
}
