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

    /// <summary>Unique object name (make sure this is scene-unique)</summary>
    public string veryUniqueObjectName;

    /// <summary>Enable gravity on rigidbody?</summary>
    public bool gravityEnabled;

    /// <summary>Should this object always be kinematic? (no physics)</summary>
    public bool alwaysKinematic;

    /// <summary>String that can be applied and used for interactive games</summary>
    public string optionalStringForGames;

    /// <summary>Default is to use Bones tag</summary>
    public bool dontChangeTag;

    /// <summary>Override for Ridigbody's Mass</summary>
    public float rigidBodyMass = 1.0f;

    /// <summary>Override for Ridigbody's Drag</summary>
    public float rigidBodyDrag = 0.0f;

    /// <summary>Override for Ridigbody's Angular Drag</summary>
    public float rigidBodyAngularDrag = 0.05f;



    #region Network Object manual interface 

    /// <summary>
    /// Update transform value manually. This should be done rarely (via button / etc) by one user if possible.
    /// Object will be dropped if it is held when function is called.
    /// </summary>
    public void UpdatePositionManualViaReference(Transform positionReferenceTransform){}

    public void UpdateRotationManualViaReference(Transform rotationReferenceTransform){}

    public void UpdateScaleManualViaReference(Transform scaleReferenceTransform){}

    public void UpdateAllTransformValuesManualViaReference(Transform referenceTransform){}

    public void SetKinematicDefault(bool alwaysKinematic){}

    public void SetGravityEnabledDefault(bool gravityEnabled){}

    public void SetIsGrabbable(bool grabbable){}

    #endregion
}
