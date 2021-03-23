using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Math = System.Math;
[RequireComponent(typeof(Collider))]
public class IFXAnimationEffectsGrabableObject : MonoBehaviour
{
    /////////////////////////////////////////
    //notes: still to do 
    ///////////////////////////////////////
    
    [SerializeField]
    
    bool testingMode;
    [SerializeField]    
    Transform testingTransform;
    [Header("--------------------------------------------------------------")]
    [SerializeField]
    bool resetOnRelease;
    [SerializeField]
    bool requireTriggerPress;
    [SerializeField]
    bool requireGrippedPress;
    [Header("--------------------------------------------------------------")]
    
    [SerializeField]
    bool radialMotionLimiter;
    [SerializeField]
    Transform centerPivot;
    [SerializeField]
    float radiusLimit;
    [SerializeField]
    bool poleAxis_X;
    [SerializeField]
    bool poleAxis_Y;
    [SerializeField]
    bool poleAxis_Z;
    [Header("--------------------------------------------------------------")]
    [SerializeField]
    bool AxisLimit_X;
    [SerializeField]
    Vector2 AxisLimitRange_X;
    [SerializeField]
    bool AxisLimit_Y;
    [SerializeField]
    Vector2 AxisLimitRange_Y;
    [SerializeField]
    bool AxisLimit_Z;
    [SerializeField]
    Vector2 AxisLimitRange_Z;
    [Header("--------------------------------------------------------------")]
    [SerializeField]
    bool rotatable;
    
    GameObject primaryHandChild;
    GameObject secondaryHandChild;
    
    
    
    Collider colliderOnTThis;
    Vector3 restPosition;
    Vector3 restRotation;
    Transform primaryhandtransform;
    Transform Secondaryhandtransform;
    
    int primaryHand = 2;
    
    int secondaryHand = 1;

    bool currentlyGrabbed;
    bool vrMode;
    Ray interaction_ray;
    RaycastHit hit;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        colliderOnTThis = gameObject.GetComponent(typeof(Collider)) as Collider;
        if (centerPivot !=null)
        {
            radiusLimit = Vector3.Distance(centerPivot.position, transform.position);
        }
        if (resetOnRelease)
        {
            restPosition = transform.localPosition;
            restRotation = transform.localEulerAngles;
        }

        // if (Engage.ScriptHelper.using_vr)
        // {
        //     vrMode= true;
        // }
        // if (testingMode != true)
        // {
        //     primaryHand = ENG_TrackedMotionControllers.instance.primaryHand;
        //     secondaryHand = ENG_TrackedMotionControllers.instance.secondaryHand;

        //     if (ENG_TrackedMotionControllers.instance.controllerTransform[primaryHand] !=null)
        //     {
        //         primaryhandtransform = ENG_TrackedMotionControllers.instance.controllerTransform[primaryHand];
        //     }
        //     else
        //     {
        //         Debug.Log("AnimationEffect Grabable Object: Primary hand controllers not found");
        //     }

        //     if (ENG_TrackedMotionControllers.instance.controllerTransform[secondaryHand] !=null)
        //     {
        //         Secondaryhandtransform = ENG_TrackedMotionControllers.instance.controllerTransform[secondaryHand];
        //     }
        //     else
        //     {
        //         Debug.Log("AnimationEffect Grabable Object:Secondary hand controllers not found");
        //     }
            
        // }
        // else
        // {
            primaryhandtransform = testingTransform;
            //Secondaryhandtransform = testingTransform;
            Debug.Log("AnimationEffect Grabable Object: Using testing transforms");
        //}        
    }

    // Update is called once per frame
    void Update()
    {
        if (vrMode)
        {
            VrGrabObject();
        }
        else
        {
            MouseGrabObject();
        }

        if (resetOnRelease && !currentlyGrabbed)
        {
            transform.localPosition = restPosition;
            transform.localEulerAngles = restRotation;
        }
        //Vector2 vmMousePos = Engage.ScriptHelper.VirtualMousePosition();

    }

    private void MouseGrabObject()
    {
        // if (Input.GetMouseButton(0))
        // {
        //     if (primaryHandChild != null)
        //     {
        //         MoveObjectWithLimiters(primaryHandChild.transform);
        //         return;
        //     }
        //     Debug.Log("IFXAnimationEffect-Grabbable: Clicked");
        //     //interaction_ray = Engage.ScriptHelper.mainCamera.ScreenPointToRay(Engage.ScriptHelper.mainCamera.WorldToScreenPoint(ENG_TrackedMotionControllers.instance.GetWorldGUIPointerTransform().position));
        //     interaction_ray = new Ray(Engage.ScriptHelper.mainCamera.transform.position, Engage.ScriptHelper.mainCamera.transform.forward);
        //     RaycastHit hit;
        //     if (colliderOnTThis.Raycast(interaction_ray,out hit, 5.0f))
        //     {
        //         currentlyGrabbed = true;
        //         if (primaryHandChild == null)
        //         {
        //             primaryHandChild = new GameObject("handChild - Mouse");
        //             primaryHandChild.transform.position = transform.position;
        //             primaryHandChild.transform.rotation = transform.rotation;
        //             primaryHandChild.transform.SetParent(Engage.ScriptHelper.mainCamera.transform, true);
        //         }

        //         Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        //         Debug.Log("IFXAnimationEffect-Grabbable: Did Hit");

        //     }
        // }
        // else if(primaryHandChild!=null)
        // {
        //     currentlyGrabbed = false;
        //     Destroy (primaryHandChild);
        // }
        
    }

    private void VrGrabObject()
    {
        
        if (Grabed(primaryhandtransform, primaryHand))
        {
            if (primaryHandChild != null)
            {
                MoveObjectWithLimiters(primaryHandChild.transform);
                return;
            }
            if (primaryHandChild == null)
            {
                primaryHandChild = new GameObject("handChild - Vr Primary Hand");
                primaryHandChild.transform.position = transform.position;
                primaryHandChild.transform.rotation = transform.rotation;
                primaryHandChild.transform.SetParent(primaryhandtransform, true);
            } 
            MoveObjectWithLimiters(primaryhandtransform);        
        }
        else if(primaryHandChild!=null)
        {
            Destroy (primaryHandChild);
        }

        if (Grabed(Secondaryhandtransform, secondaryHand))
        {
            if (secondaryHandChild != null)
            {
                MoveObjectWithLimiters(secondaryHandChild.transform);
                return;
            }
            if (secondaryHandChild == null)
            {
                secondaryHandChild = new GameObject("handChild - Vr Secondary Hand");
                secondaryHandChild.transform.position = transform.position;
                secondaryHandChild.transform.rotation = transform.rotation;
                secondaryHandChild.transform.SetParent(Secondaryhandtransform, true);
            } 
            MoveObjectWithLimiters(Secondaryhandtransform);        
        }
        else if( secondaryHandChild != null)
        {
            Destroy (secondaryHandChild);
        }
        
    }

    private void MoveObjectWithLimiters(Transform handTransformIN)
    {
        if (radialMotionLimiter)
        {
            float distanceToCenter = Vector3.Distance(centerPivot.position, handTransformIN.position);
            Vector3 fromOriginToObject = centerPivot.position - handTransformIN.position;
            fromOriginToObject *= radiusLimit / distanceToCenter;
            Vector3 Position3D = centerPivot.position - fromOriginToObject;
            if (poleAxis_X)
            {
                Position3D.x = centerPivot.position.x;
            }
            if (poleAxis_Y)
            {
                Position3D.y = centerPivot.position.y;
            }
            if (poleAxis_Z)
            {
                Position3D.z = centerPivot.position.z;
            }
            transform.position = Position3D;
        }


        if (AxisLimit_X)
        {            
            Vector3 LocalHandPos = transform.parent.InverseTransformPoint(handTransformIN.position);
            LocalHandPos.x = Mathf.Clamp(LocalHandPos.x, AxisLimitRange_X.x, AxisLimitRange_X.y);
            transform.localPosition = new Vector3(LocalHandPos.x, transform.localPosition.y, transform.localPosition.z);
        }
        if (AxisLimit_Y)
        {
            Vector3 LocalHandPos = transform.parent.InverseTransformPoint(handTransformIN.position);
            LocalHandPos.y = Mathf.Clamp(LocalHandPos.y, AxisLimitRange_Y.x, AxisLimitRange_Y.y);
            transform.localPosition = new Vector3(transform.localPosition.x, LocalHandPos.y, transform.localPosition.z);
        }
        if (AxisLimit_Z)
        {
            Vector3 LocalHandPos = transform.parent.InverseTransformPoint(handTransformIN.position);
            LocalHandPos.z = Mathf.Clamp(LocalHandPos.z, AxisLimitRange_Z.x, AxisLimitRange_Z.y);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, LocalHandPos.z);
        }

        if (rotatable)
        {
            transform.rotation = handTransformIN.rotation;
        }

        if(!AxisLimit_X && !AxisLimit_Y && !AxisLimit_Z && !radialMotionLimiter)// this needs work!
        {
            transform.position = handTransformIN.position;
        }
    }
    
    private bool Grabed(Transform handTransform, int whichHand) 
    {

        if (handTransform !=null)
        {
            if (colliderOnTThis.bounds.Contains(handTransform.position))
            {
                if (testingMode)
                {
                    currentlyGrabbed =true;
                    return true;
                }

                // if(requireTriggerPress && ENG_TrackedMotionControllers.instance.triggerPressed[whichHand])
                // {
                //     currentlyGrabbed =true;
                //     return true;
                // }
                // if(requireGrippedPress && ENG_TrackedMotionControllers.instance.gripped[whichHand])
                // {
                //     currentlyGrabbed =true;
                //     return true;
                // }
                // else if(!requireTriggerPress && !requireGrippedPress)
                // {
                //     currentlyGrabbed =true;
                //     return true;
                    
                // }
            }
        }
        else
        {
            
            Debug.Log("AnimationEffect Grabable Object: hand controllers not found");
            //Attempt to get instance again
            if (!testingMode)
            {
                // primaryhandtransform = ENG_TrackedMotionControllers.instance.controllerTransform[primaryHand];
                // Secondaryhandtransform = ENG_TrackedMotionControllers.instance.controllerTransform[secondaryHand];
            }
            
        }
        
        currentlyGrabbed =false;
        return false;
        
    }
    
    public bool IsBetween(float testValue, float bound1, float bound2)
    {
        return (testValue >= Math.Min(bound1,bound2) && testValue <= Math.Max(bound1,bound2));
    } 
}
