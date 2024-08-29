using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Build", menuName = "ScriptableObjects/Build", order = 1)]
public class BuildSO : ScriptableObject
{
    public string buildName;
    [SerializeField] public ItemSO[] requiredItems;
    [SerializeField] public int[] requiredAmounts;
    [SerializeField] public GameObject buildPrefab;
    // Maybe a time to build mechanic
}
