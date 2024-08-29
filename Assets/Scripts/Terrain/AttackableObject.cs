using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackableObject : MonoBehaviour
{
    [SerializeField] public AttackableObjectSO attackableObjectSO;
    private TerrainManager terrainManager;
    void Start()
    {
        terrainManager = FindObjectOfType<TerrainManager>();
        attackableObjectSO = Instantiate(attackableObjectSO);
    }
    public void TakeDamage(float damage, EquipmentType tool, int equipmentLevel)
    {
        float damageMultiplier = 1 + (equipmentLevel - attackableObjectSO.requiredToolLevel);
        // print("Damage Multiplier: " + damageMultiplier);
        if (attackableObjectSO.requiredTool == EquipmentType.None || attackableObjectSO.requiredToolLevel == 1)
        {
            bool destroyed = attackableObjectSO.Attacked(damage, damageMultiplier);
            // terrainManager.PlayerAttackTerrain(resource, attackableObjectSO.dropItem);
            if (destroyed) Death();
            return;
        }
        else if (attackableObjectSO.requiredTool == tool && attackableObjectSO.requiredToolLevel <= equipmentLevel)
        {
            bool destroyed = attackableObjectSO.Attacked(damage, damageMultiplier);
            // terrainManager.PlayerAttackTerrain(resource, attackableObjectSO.dropItem);
            if (destroyed) Death();
            return;
        }
        Debug.Log("You need a " + attackableObjectSO.requiredTool + " level " + attackableObjectSO.requiredToolLevel + " to attack this object.");
    }
    public float GetHealth()
    {
        return attackableObjectSO.health;
    }
    private void Death()
    {
        if (attackableObjectSO.attackableType == AttackableType.Tree)
        {
            terrainManager.TreeDeath(gameObject);
        }
        else if (attackableObjectSO.attackableType == AttackableType.Rock)
        {
            Destroy(gameObject);
        }
        else if (attackableObjectSO.attackableType == AttackableType.Bush)
        {
            Debug.Log("Bush Death");
        }
    }
}

public enum AttackableType
{
    Tree,
    Rock,
    Bush
}


