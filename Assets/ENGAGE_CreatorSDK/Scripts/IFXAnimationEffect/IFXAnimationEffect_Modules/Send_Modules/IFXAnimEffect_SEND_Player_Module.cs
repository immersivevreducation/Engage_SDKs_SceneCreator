using UnityEngine;
[AddComponentMenu("IFXAnimEffect_SEND/Player - IFX SEND Module")]
[RequireComponent(typeof(IFXAnimEffect_SEND_MAIN))]
public class IFXAnimEffect_SEND_Player_Module : IFXAnimEffect_SEND_Module
{
    #if UNITY_EDITOR
    public override string moduleType {get{ return "Player";} }
    #endif
    Camera playerCamera;

    delegate float UpdateValuesDelegate();
    UpdateValuesDelegate UpdateValues;

    [SerializeField]
    bool from_PlayerPositon_X;
    [SerializeField]
    bool from_PlayerPositon_Y;
    [SerializeField]
    bool from_PlayerPositon_Z;

    //////////////////////    
    [SerializeField]
    bool from_PlayerRotation_X;
    [SerializeField]
    bool from_PlayerRotation_Y;
    [SerializeField]
    bool from_PlayerRotation_Z;
    
    ///////////////////////
    [SerializeField]
    bool from_PlayerCollision;
    [SerializeField]
    Collider playerCollisionTarget;
    ///////////////////////
    [SerializeField]
    bool from_PlayerDistance;

    [SerializeField]
    Transform playerDistanceTarget;
    ///////////////////////
    [SerializeField]
    bool from_PlayerHandDistance;

    [SerializeField]
    Transform playerHandDistanceTarget;
    
    ////////////////////////
    ///////////////////////
    [SerializeField]
    bool from_PlayerTouch;
    [SerializeField]
    Collider playerTouchCollider;
    [SerializeField]
    bool requireTriggerPress;
    [SerializeField]
    bool requireGrippedPress;

    private void OnEnable()
    {
        // if (Engage.ScriptHelper.mainCamera !=null)
        // {
        //     playerCamera = Engage.ScriptHelper.mainCamera;
        //     if (from_PlayerPositon_X)
        //     {
        //         if (playerCamera != null)
        //         {
        //             UpdateValues += GetPlayerPostition_X;
        //         }
        //     }
        //     if (from_PlayerPositon_Y)
        //     {
        //         if (playerCamera != null)
        //         {
        //             UpdateValues += GetPlayerPostition_Y;
        //         }
        //     }
        //     if (from_PlayerPositon_Z)
        //     {
        //         if (playerCamera != null)
        //         {
        //             UpdateValues += GetPlayerPostition_Z;
        //         }
        //     }
        //     //////////////////////////
        //     if (from_PlayerRotation_X)
        //     {
        //         if (playerCamera != null)
        //         {
        //             UpdateValues += GetPlayerRotation_X;
        //         }
        //     }
        //     if (from_PlayerRotation_Y)
        //     {
        //         if (playerCamera != null)
        //         {
        //             UpdateValues += GetPlayerRotation_Y;
        //         }
        //     }
        //     if (from_PlayerRotation_Z)
        //     {
        //         if (playerCamera != null)
        //         {
        //             UpdateValues += GetPlayerRotation_Z;
        //         }
        //     }
        //     //////////////////////////
        //     if (from_PlayerDistance)
        //     {
        //         if (playerCamera != null && playerDistanceTarget != null)
        //         {
        //             UpdateValues += GetPlayerDistance;
        //         }
        //         else
        //         {
        //             Debug.Log("AnimationEffect: from player distance selected but player distance target input empty");
        //         }
        //     }
        //     if (from_PlayerHandDistance)
        //     {
        //         if (playerCamera != null && playerHandDistanceTarget != null)
        //         {
        //             UpdateValues += GetPlayerHandDistance;
        //         }
        //         else
        //         {
        //             Debug.Log("AnimationEffect: from player distance selected but player distance target input empty");
        //         }
        //     }
            
        //     //////////////////////////
        //     if (from_PlayerTouch)
        //     {
        //         if (Engage.ScriptHelper.using_vr && ENG_TrackedMotionControllers.instance != null)
        //         {
        //             UpdateValues += GetPlayerTouchedVR;
        //         }
        //         else
        //         {
        //             UpdateValues += GetPlayerTouchedMouse;
        //         }
        //     }
        //     //////////////////////////
        //     if (from_PlayerCollision)
        //     {
        //         if (playerCamera != null)
        //         {
        //             UpdateValues += GetPlayerCollision;
        //         }
        //     }
        // }
        // else
        // {
        //      UpdateValues += Restart;
        // }
        
        
    }
    //This method gets called by SEND_Main to retrive to value from the delegate. Only one method should be returning values.
    // public override void SendOutput()
    // {
    //    AnimationEffectVariable.Value = UpdateValues();
    // }
    ///////////////////////////////////////////////
    // private float Restart()
    // {
    //     Debug.Log("AnimationEffect: Player not found... Restarting");
    //     if (Engage.ScriptHelper.mainCamera !=null)
    //     {
    //         UpdateValues -= Restart;
    //         playerCamera = Engage.ScriptHelper.mainCamera;
    //         if (from_PlayerPositon_X)
    //         {
    //             if (playerCamera != null)
    //             {
                    
    //             }
    //         }
    //         if (from_PlayerPositon_Y)
    //         {
    //             if (playerCamera != null)
    //             {
    //                 UpdateValues += GetPlayerPostition_Y;
    //             }
    //         }
    //         if (from_PlayerPositon_Z)
    //         {
    //             if (playerCamera != null)
    //             {
    //                 UpdateValues += GetPlayerPostition_Z;
    //             }
    //         }
    //         //////////////////////////
    //         if (from_PlayerRotation_X)
    //         {
    //             if (playerCamera != null)
    //             {
    //                 UpdateValues += GetPlayerRotation_X;
    //             }
    //         }
    //         if (from_PlayerRotation_Y)
    //         {
    //             if (playerCamera != null)
    //             {
    //                 UpdateValues += GetPlayerRotation_Y;
    //             }
    //         }
    //         if (from_PlayerRotation_Z)
    //         {
    //             if (playerCamera != null)
    //             {
    //                 UpdateValues += GetPlayerRotation_Z;
    //             }
    //         }
    //         //////////////////////////
    //         if (from_PlayerDistance)
    //         {
    //             if (playerCamera != null && playerDistanceTarget != null)
    //             {
    //                 UpdateValues += GetPlayerDistance;
    //             }
    //             else
    //             {
    //                 Debug.Log("AnimationEffect: from player distance selected but player distance target input empty");
    //             }
    //         }
    //         if (from_PlayerHandDistance)
    //         {
    //             if (playerCamera != null && playerHandDistanceTarget != null)
    //             {
    //                 UpdateValues += GetPlayerHandDistance;
    //             }
    //             else
    //             {
    //                 Debug.Log("AnimationEffect: from player distance selected but player distance target input empty");
    //             }
    //         }
            
    //         //////////////////////////
    //         if (from_PlayerTouch)
    //         {
    //             if (Engage.ScriptHelper.using_vr && ENG_TrackedMotionControllers.instance != null)
    //             {
    //                 UpdateValues += GetPlayerTouchedVR;
    //             }
    //             else
    //             {
    //                 UpdateValues += GetPlayerTouchedMouse;
    //             }
    //         }
    //         //////////////////////////
    //         if (from_PlayerCollision)
    //         {
    //             if (playerCamera != null)
    //             {
    //                 UpdateValues += GetPlayerCollision;
    //             }
    //         }
    //     }
    //     return 0;
    // }
    // private float GetPlayerPostition_X() 
    // {
    //     if (playerCamera != null)
    //     {
    //         playerCamera = Engage.ScriptHelper.mainCamera;
    //     }
    //     float output = playerCamera.transform.position.x;
    //     return output;
    // }
    // private float GetPlayerPostition_Y() 
    // {
    //     if (playerCamera != null)
    //     {
    //         playerCamera = Engage.ScriptHelper.mainCamera;
    //     }
    //     float output = playerCamera.transform.position.y;
    //     return output;
    // }
    // private float GetPlayerPostition_Z() 
    // {
    //     if (playerCamera != null)
    //     {
    //         playerCamera = Engage.ScriptHelper.mainCamera;
    //     }
    //     float output = playerCamera.transform.position.z;
    //     return output;
    // }

    // private float GetPlayerRotation_X() 
    // {
    //     if (playerCamera != null)
    //     {
    //         playerCamera = Engage.ScriptHelper.mainCamera;
    //     }
    //     float output = playerCamera.transform.rotation.x;
    //     return output;
    // }
    // private float GetPlayerRotation_Y() 
    // {
    //     if (playerCamera != null)
    //     {
    //         playerCamera = Engage.ScriptHelper.mainCamera;
    //     }
    //     float output = playerCamera.transform.rotation.y;
    //     return output;
    // }
    // private float GetPlayerRotation_Z() 
    // {
    //     if (playerCamera != null)
    //     {
    //         playerCamera = Engage.ScriptHelper.mainCamera;
    //     }
    //     float output = playerCamera.transform.rotation.z;
    //     return output;
    // }

    // ////////////////////////////////////////
    // private float GetPlayerDistance() 
    // {
    //     if (playerCamera != null)
    //     {
    //         playerCamera = Engage.ScriptHelper.mainCamera;
    //     }
    //     float output = Vector3.Distance(playerDistanceTarget.position, playerCamera.transform.position);
    //     return output;
    // }
    // private float GetPlayerHandDistance() 
    // {
    //     int primaryHand = ENG_TrackedMotionControllers.instance.primaryHand;
    //     int secondaryHand = ENG_TrackedMotionControllers.instance.secondaryHand;
    //     Transform  primaryhandtransform;
    //     Transform  Secondaryhandtransform;
    //     float distanceToPrimaryHand = 100;
    //     float distanceToSecondaryHand = 100;

    //     if (ENG_TrackedMotionControllers.instance.controllerTransform[primaryHand] !=null)
    //     {
    //         primaryhandtransform = ENG_TrackedMotionControllers.instance.controllerTransform[primaryHand];
    //         distanceToPrimaryHand = Vector3.Distance(primaryhandtransform.position, playerHandDistanceTarget.position);
    //     }
    //     else
    //     {
    //         Debug.Log("AnimationEffect Player Module: Primary hand controllers not found, can't get distance");
    //     }

    //     if (ENG_TrackedMotionControllers.instance.controllerTransform[secondaryHand] !=null)
    //     {
    //         Secondaryhandtransform = ENG_TrackedMotionControllers.instance.controllerTransform[secondaryHand];
    //         distanceToSecondaryHand = Vector3.Distance(Secondaryhandtransform.position, playerHandDistanceTarget.position);
    //     }
    //     else
    //     {
    //         Debug.Log("AnimationEffect Player Module: Secondary hand controllers not found, can't get distance");
    //     }

        
    //     return Mathf.Min(distanceToPrimaryHand,distanceToSecondaryHand);
        
        
        
    // }
    // ////////////////////////////////////////    

    // private float GetPlayerTouchedVR() 
    // {
    //     Transform primaryhandtransform = ENG_TrackedMotionControllers.instance.controllerTransform[ENG_TrackedMotionControllers.instance.primaryHand];
    //     Transform Secondaryhandtransform = ENG_TrackedMotionControllers.instance.controllerTransform[ENG_TrackedMotionControllers.instance.secondaryHand];
    //     if (playerTouchCollider.bounds.Contains(primaryhandtransform.position))
    //     {
           
    //         if(requireTriggerPress)
    //         {
    //             if (ENG_TrackedMotionControllers.instance.triggerPressed[ENG_TrackedMotionControllers.instance.primaryHand])
    //             {
    //                 return 1.0f;
    //             }
    //         }
    //         if (requireGrippedPress)
    //         {
    //             if ( ENG_TrackedMotionControllers.instance.gripped[ENG_TrackedMotionControllers.instance.primaryHand])
    //             {
    //                return 1.0f; 
    //             }
                
    //         }
    //         if (!requireGrippedPress && !requireTriggerPress)
    //         {
    //             return 1.0f;
    //         }
    //     }
    //     if (playerTouchCollider.bounds.Contains(Secondaryhandtransform.position))
    //     {
    //         if(requireTriggerPress)
    //         {
    //             if (ENG_TrackedMotionControllers.instance.triggerPressed[ENG_TrackedMotionControllers.instance.secondaryHand])
    //             {
    //                 return 1.0f;
    //             }
    //         }
    //         if (requireGrippedPress)
    //         {
    //             if ( ENG_TrackedMotionControllers.instance.gripped[ENG_TrackedMotionControllers.instance.secondaryHand])
    //             {
    //                return 1.0f; 
    //             }
                
    //         }
    //         if (!requireGrippedPress && !requireTriggerPress)
    //         {
    //             return 1.0f;
    //         }
    //     }
    //     return 0.0f;
    // }
    // private float GetPlayerTouchedMouse() 
    // {
    //     if (playerCamera != null)
    //     {
    //         playerCamera = Engage.ScriptHelper.mainCamera;
    //     }

    //     //Ray interaction_ray = Engage.ScriptHelper.mainCamera.ScreenPointToRay(Engage.ScriptHelper.mainCamera.WorldToScreenPoint(ENG_TrackedMotionControllers.instance.GetWorldGUIPointerTransform().position));
    //     Ray interaction_ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
    //     RaycastHit hit;
    //     if (Input.GetMouseButton(0))
    //     {
    //         Debug.DrawRay(interaction_ray.origin, interaction_ray.direction,  Color.green,  500.0f, true);
    //         if (playerTouchCollider.Raycast(interaction_ray,out hit, 10.0f))
    //         {
    //             return 1.0f;
    //         }
    //     }
    //     return 0.0f;
    // }
    // private float GetPlayerCollision() 
    // {
        
    //     if(playerCollisionTarget.bounds.Intersects(ENG_IGM_PlayerManager.instance.myPlayerObject.playerObject.GetComponent<Collider>().bounds))
    //     {
    //         return 1.0f;
    //     }
    //     return 0.0f;
    // }
    
}
