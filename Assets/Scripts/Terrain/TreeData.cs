using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeData
{
    public TreeInstance TreeInstance { get; set; }
    public TreePrototype TreePrototype { get; set; }
    public GameObject TreeObject { get; set; }
    public Vector3 TreeWorldPosition { get; set; }
    public Quaternion TreeRotation { get; set; }
}
