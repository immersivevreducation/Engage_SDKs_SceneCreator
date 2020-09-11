using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engage_CreateNetworkObjectFromSceneObject : MonoBehaviour
{
    [Header("",order = 0)]
    [Header("Create Networked Object for ENGAGE Scene", order = 1)]
    [Header("Do not use Mesh Colliders", order = 2)]
    [Header("Make this parent object", order = 3)]
    [Header("This objects scale will always be (1,1,1)", order = 4)]
    [Header("",order = 5)]

    [Header("Unique name for Objects, this must match GO Name", order = 6)]
    public string veryUniqueObjectName;
    public bool gravityEnabled;
    public bool alwaysKinematic;

    [Header("Advanced Functionality", order = 7)]
    public string optionalStringForGames;
    public bool dontChangeTag;
}
