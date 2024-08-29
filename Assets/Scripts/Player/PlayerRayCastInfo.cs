using System;
using TMPro;
using UnityEngine;

public class PlayerRayCastInfo : MonoBehaviour
{
    [SerializeField] private float rayDistance = 3f;
    [SerializeField] private float rayPosition = 0f;
    [SerializeField] private float baseDamage = 20f;
    private GameObject lastHitObject;
    [SerializeField] private TextMeshProUGUI text;
    private EquipmentType equipmentType;
    private PlayerInput playerInput;
    private float damage = 50f;
    private int equipmentLevel = 1;

    # region Static Methods
    void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Interact.performed += ctx => TryInteract();
    }
    void Update()
    {
        RayCastPlayerVision();
    }


    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }
    #endregion

    private void TryInteract()
    {
        if (lastHitObject != null && lastHitObject.GetComponent<InteractableObject>() != null)
        {
            if (lastHitObject.GetComponent<InteractableObject>().CanBeInteractedWithE)
            {
                IInteractable interactable = lastHitObject.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }
    void RayCastPlayerVision()
    {
        RaycastHit hit;
        Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y + rayPosition, transform.position.z);
        Vector3 rayDirection = transform.TransformDirection(Vector3.forward);
        float sphereRadius = 0.5f; // Define the radius of the sphere

        Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red);
        UpdateText("");
        lastHitObject = null;
        // Check if the player is inside an object at the start of the raycast
        Collider[] colliders = Physics.OverlapSphere(rayOrigin, sphereRadius);
        if (colliders.Length > 0)
        {
            foreach (Collider collider in colliders)
            {
                if (collider.transform.gameObject != Terrain.activeTerrain.gameObject && collider.transform.name != "FirstPersonController")
                {
                    // Debug.Log("Inside object: " + collider.transform.name);
                    lastHitObject = collider.transform.gameObject;
                    ObjectHit(lastHitObject);
                    return;
                }
            }
        }
        if (Physics.SphereCast(rayOrigin, sphereRadius, rayDirection, out hit, rayDistance))
        {
            if (hit.transform.gameObject != Terrain.activeTerrain.gameObject && hit.transform.name != "FirstPersonController")
            {
                // Debug.Log("Hit: " + hit.transform.name);
                lastHitObject = hit.transform.gameObject;
                ObjectHit(lastHitObject);
            }
        }
    }

    void ObjectHit(GameObject hitObject)
    {
        // Debug.Log("Hit: " + hitObject.name);
        if (hitObject.GetComponent<InteractableObject>() != null)
        {
            if (hitObject.GetComponent<AttackableObject>() != null)
            {
                UpdateText(hitObject.GetComponent<InteractableObject>().ObjectName + " - Health: " + hitObject.GetComponent<AttackableObject>().GetHealth());
            }
            else
            {
                UpdateText(hitObject.GetComponent<InteractableObject>().ObjectName);
            }
        }
    }

    void UpdateText(string text)
    {
        this.text.text = text;
    }

    public void AnimationAttack()
    {
        if (lastHitObject != null && lastHitObject.GetComponent<AttackableObject>() != null)
        {
            // print(damage + " " + equipmentType + " " + equipmentLevel);
            lastHitObject.GetComponent<AttackableObject>().TakeDamage(damage, equipmentType, equipmentLevel);
        }
    }
    public void EquipItem(ItemSO item = null)
    {
        if (item == null)
        {
            equipmentLevel = 1;
            damage = baseDamage;
            equipmentType = EquipmentType.None;
            return;
        }
        if (item.equipmentType == EquipmentType.None)
        {
            equipmentLevel = 1;
            damage = baseDamage;
            equipmentType = EquipmentType.None;
            return;
        }
        equipmentLevel = item.equipmentLevel;
        equipmentType = item.equipmentType;
        damage = 10f * equipmentLevel;
    }
}
