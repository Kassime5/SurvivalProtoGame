using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField] private string objectName;
    public string ObjectName { get => objectName; set => objectName = value; }
    public bool CanBeInteractedWithE;
}
