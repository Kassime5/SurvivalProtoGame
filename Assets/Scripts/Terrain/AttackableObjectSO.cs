using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "AttackableObject", menuName = "ScriptableObjects/AttackableObject")]
public class AttackableObjectSO : ScriptableObject
{
    public float health = 100f;
    public float maxHealth = 100f;
    public AttackableType attackableType;
    public EquipmentType requiredTool;
    public int requiredToolLevel;
    public ItemSO[] dropItems;
    public int[] totalResources;
    private bool destroyed = false;

    void Start()
    {
        if (dropItems.Length != totalResources.Length)
        {
            Debug.LogError("Drop Items and Total Resources must be the same length.");
        }
    }

    public bool Attacked(float damage, float damageMultiplier = 1)
    {
        if (destroyed) return destroyed;
        float calculatedDamage = damage * damageMultiplier;
        health -= calculatedDamage;
        if (health <= 0)
        {
            health = 0;
            destroyed = true;
        }
        DropItems(calculatedDamage);
        // int resourceAmount = (int)(totalResources / (maxHealth / damage));
        return destroyed;
    }

    private void DropItems(float damage)
    {
        TerrainManager terrainManager = FindObjectOfType<TerrainManager>();
        // If the object got destroyed, drop all the remaining resources
        if (health <= 0)
        {
            for (int i = 0; i < dropItems.Length; i++)
            {
                terrainManager.PlayerAttackTerrain(totalResources[i], dropItems[i]);
            }
        }
        else
        {
            for (int i = 0; i < dropItems.Length; i++)
            {
                if (totalResources[i] > 0)
                {
                    int resourceAmount = (int)Math.Round(totalResources[i] / (maxHealth / damage));
                    totalResources[i] -= resourceAmount;
                    terrainManager.PlayerAttackTerrain(resourceAmount, dropItems[i]);
                }
            }
        }
    }
}
