using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "ScriptableObjects/FurnaceRecipe", order = 1)]
public class FurnaceSO : ScriptableObject
{
    [SerializeField] public ItemSO inputItem;
    [SerializeField] public ItemSO outputItem;
    [SerializeField] public int outputAmount;
    [SerializeField] public float smeltTime;
}
