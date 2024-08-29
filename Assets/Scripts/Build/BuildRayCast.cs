using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildRayCast : MonoBehaviour
{
    [SerializeField] private float rayDistance = 3f;
    [SerializeField] private float rayPosition = 0f;
    public bool isBuilding = false;
    [SerializeField] private GameObject buildingPrefab;
    [SerializeField] private LayerMask layerMask; // Layer mask to exclude the prefab layer
    [SerializeField] private Material transparentMaterial;

    private GameObject currentBuildingInstance;
    public static BuildRayCast instance;

    void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (isBuilding)
        {
            RayCastPlayerVision();
        }
        else
        {
            if (currentBuildingInstance != null)
            {
                Destroy(currentBuildingInstance);
            }
        }
    }

    private void RayCastPlayerVision()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        Vector3 targetPosition;

        if (Physics.Raycast(ray, out hit, rayDistance, ~layerMask))
        {
            targetPosition = hit.point;
            targetPosition.y += buildingPrefab.transform.localScale.y / 2;
        }
        else
        {
            targetPosition = ray.GetPoint(rayDistance);
        }

        if (currentBuildingInstance == null)
        {
            currentBuildingInstance = Instantiate(buildingPrefab, targetPosition, Quaternion.identity);
            currentBuildingInstance.layer = LayerMask.NameToLayer("Ignore Raycast");
            currentBuildingInstance.GetComponentInChildren<MeshRenderer>().material = transparentMaterial;
        }
        else
        {
            currentBuildingInstance.transform.position = targetPosition;
        }
    }

    public void StartBuilding(GameObject buildingPrefab)
    {
        this.buildingPrefab = buildingPrefab;
        isBuilding = true;
    }

    public void StopBuilding()
    {
        this.buildingPrefab = null;
        Destroy(currentBuildingInstance);
        isBuilding = false;
    }

    internal Vector3 GetCurrentBuildingPosition()
    {
        return currentBuildingInstance.transform.position;
    }
}