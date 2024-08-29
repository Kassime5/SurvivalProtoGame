using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class TerrainManager : MonoBehaviour
{
    public Terrain terrain;
    private List<TreeData> treeData = new List<TreeData>();
    [SerializeField] private GameObject[] treePrefabs;
    [SerializeField] GameObject treeCollider;
    [SerializeField] private InventoryController inventoryController;
    [SerializeField] private AttackableObjectSO treeSO;
    [SerializeField] private AttackableObjectSO rockSO;
    void Start()
    {
        // Saves the terrain data and creates a copy of it to prevent changes in the original terrain
        TerrainData copyTerrainData = Instantiate(terrain.terrainData);
        terrain.terrainData = copyTerrainData;
        LoadTreeData();
        TranformTreesIntoGameObject();
        LoadRocks();
    }
    void LoadTreeData()
    {
        var treeInstances = terrain.terrainData.treeInstances;
        treeData = new List<TreeData>();

        for (int i = 0; i < treeInstances.Length; i++)
        {
            TreeInstance treeInstance = treeInstances[i];
            TreePrototype treePrototype = terrain.terrainData.treePrototypes[treeInstance.prototypeIndex];
            Vector3 treeWorldPosition = Vector3.Scale(treeInstance.position, terrain.terrainData.size) + terrain.transform.position;
            Quaternion treeRotation = Quaternion.AngleAxis(treeInstance.rotation * Mathf.Rad2Deg, Vector3.up);

            TreeData tree = new TreeData()
            {
                TreeInstance = treeInstance,
                TreePrototype = treePrototype,
                TreeWorldPosition = treeWorldPosition,
                TreeRotation = treeRotation,
            };

            treeData.Add(tree);
        }
    }
    void TranformTreesIntoGameObject()
    {
        var treeInstances = terrain.terrainData.treeInstances.ToList();
        foreach (TreeData tree in treeData)
        {
            var prefab = treePrefabs[tree.TreeInstance.prototypeIndex];
            var treeObject = Instantiate(prefab, tree.TreeWorldPosition, tree.TreeRotation, terrain.transform);
            treeObject.transform.parent = transform;
            treeObject.tag = "Tree";
            tree.TreeObject = treeObject;
            treeInstances.Remove(tree.TreeInstance);
            if (treeObject.GetComponentInChildren<LODGroup>() == null)
            {
                treeObject.AddComponent<MeshCollider>();
                treeObject.AddComponent<InteractableObject>().ObjectName = "Tree";
                treeObject.AddComponent<AttackableObject>().attackableObjectSO = treeSO;
                // treeObject.AddComponent<Outline>();
            }
            else
            {
                treeObject.transform.GetChild(0).gameObject.AddComponent<MeshCollider>();
                treeObject.transform.GetChild(0).gameObject.AddComponent<InteractableObject>().ObjectName = "Tree";
                treeObject.transform.GetChild(0).gameObject.AddComponent<AttackableObject>().attackableObjectSO = treeSO;
                // treeObject.transform.GetChild(0).gameObject.AddComponent<Outline>();
            }
        }
        terrain.terrainData.treeInstances = treeInstances.ToArray();
    }
    public void TreeDeath(GameObject treeObject)
    {
        treeObject.GetComponentInChildren<MeshCollider>().enabled = false;
        GameObject colliderInstance = Instantiate(treeCollider);
        colliderInstance.transform.position = treeObject.transform.position;
        colliderInstance.transform.parent = treeObject.transform;
        colliderInstance.layer = LayerMask.NameToLayer("FallingTree");
        var rb = treeObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        StartCoroutine(EnablePhysicsAfterDelay(rb, 0.5f));
    }
    void LoadRocks()
    {
        var rockInstances = GameObject.FindGameObjectsWithTag("Rock");
        for (int i = 0; i < rockInstances.Length; i++)
        {
            rockInstances[i].AddComponent<InteractableObject>().ObjectName = "Rock";
            rockInstances[i].AddComponent<AttackableObject>().attackableObjectSO = rockSO;
        }
    }
    IEnumerator<WaitForSeconds> EnablePhysicsAfterDelay(Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    public void PlayerAttackTerrain(int resourceAmount, ItemSO itemSO)
    {
        inventoryController.AddItem(itemSO, resourceAmount);
    }
}
