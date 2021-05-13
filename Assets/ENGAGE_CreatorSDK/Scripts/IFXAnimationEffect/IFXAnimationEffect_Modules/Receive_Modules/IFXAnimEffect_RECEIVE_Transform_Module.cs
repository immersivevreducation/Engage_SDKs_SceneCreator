using UnityEngine;
[AddComponentMenu("IFXAnimEffect_RECEIVE/Transform - IFX RECEIVE Module")]
[RequireComponent(typeof(IFXAnimEffect_RECEIVE_MAIN))]
public class IFXAnimEffect_RECEIVE_Transform_Module : IFXAnimEffect_RECEIVE_Module
{

    //Action<float> this.InputFloatAction;
    //Action<bool> InputBoolAction;
    #if UNITY_EDITOR
    public override string moduleType {get{ return "Transform";} }
    #endif
    [SerializeField]
    Transform to_Transform;
    [SerializeField]
    bool use_World_Space;
    [SerializeField]
    bool to_Positon_X;
    [SerializeField]
    bool to_Positon_Y;
     [SerializeField]
    bool to_Positon_Z;
     [SerializeField]
    bool to_Positon_X_Additive;
    [SerializeField]
    bool to_Positon_Y_Additive;
   
    [SerializeField]
    bool to_Positon_Z_Additive;
    [SerializeField]
    bool to_Rotation_X;
    [SerializeField]
    bool to_Rotation_Y;
    [SerializeField]
    bool to_Rotation_Z;
    [SerializeField]
    bool to_Rotation_X_Additive;
    [SerializeField]
    bool to_Rotation_Y_Additive;
    [SerializeField]
    bool to_Rotation_Z_Additive;


    [SerializeField]
    bool to_Scale_X;
    [SerializeField]
    bool to_Scale_Y;
    [SerializeField]
    bool to_Scale_Z;

    //////////////////////////////////

    private void OnEnable()
    {
        if (to_Transform == null)
        {
            Debug.Log("IFXAnimationEffect_RECEIVE_Transforms_Module: to_Transform not set, a Transform is required.");
        }
        if (to_Positon_X)
        {
            if (to_Transform !=null)
            {
                if (use_World_Space)
                {
                    this.InputFloatAction += InputToPositionWorld_X;
                }
                else
                {
                    this.InputFloatAction += InputToPosition_X;
                }
                
            }            
            else
            {
                Debug.Log("AnimationEffectReceive: To Position X selected but to_Transform input is empty");
            }
            
        }
        if (to_Positon_X_Additive)
        {
            if (to_Transform !=null)
            {
                if (use_World_Space)
                {
                    this.InputFloatAction += InputToPositionWorld_X_Additive;
                }
                else
                {
                    this.InputFloatAction += InputToPosition_X_Additive;
                }   
            }        
            else
            {
                Debug.Log("AnimationEffectReceive: To Position X Additive selected but to_Transform input is empty");
            }
            
        }
        if (to_Positon_Y)
        {
            if (to_Transform !=null)
            {
                if (use_World_Space)
                {
                    this.InputFloatAction += InputToPositionWorld_Y;
                }
                else
                {
                    this.InputFloatAction += InputToPosition_Y;
                }   
            }
            else
            {
                Debug.Log("AnimationEffectReceive: To Position Y  selected but to_Transform input is empty");
            }  
            
        }
        if (to_Positon_Y_Additive)
        {
            if (to_Transform !=null)
            {
                if (use_World_Space)
                {
                    this.InputFloatAction += InputToPositionWorld_Y_Additive;
                }
                else
                {
                    this.InputFloatAction += InputToPosition_Y_Additive;
                }   
            }               
            else
            {
                Debug.Log("AnimationEffectReceive: To Position Y Additive selected but to_Transform input is empty");
            }
            
        }
        if (to_Positon_Z)
        {
            if (to_Transform !=null)
            {
                if (use_World_Space)
                {
                    this.InputFloatAction += InputToPositionWorld_Z;
                }
                else
                {
                    this.InputFloatAction += InputToPosition_Z;
                }   
            }               
            else
            {
                Debug.Log("AnimationEffectReceive: To Position Z selected but to_Transform input is empty");
            }
            
        }
        if (to_Positon_Z_Additive)
        {
            if (to_Transform !=null)
            {
                if (use_World_Space)
                {
                    this.InputFloatAction += InputToPositionWorld_Z_Additive;
                }
                else
                {
                    this.InputFloatAction += InputToPosition_Z_Additive;
                }   
            }            
            else
            {
                Debug.Log("AnimationEffectReceive: To Position Z Additive selected but to_Transform input is empty");
            }   
        }
        //////////////////
        if (to_Rotation_X)
        {
            if (to_Transform !=null)
            {
                // if (use_World_Space)
                // {
                //     this.InputFloatAction += InputToRotationWorld_X;
                // }
                // else
                // {
                    this.InputFloatAction += InputToRotation_X;
                //}
                
            }             
            else
            {
                Debug.Log("AnimationEffectReceive: To Rotation X selected but to_Transform input is empty");
            }
            
        }
        if (to_Rotation_Y)
        {
            if (to_Transform !=null)
            {
                // if (use_World_Space)
                // {
                //     this.InputFloatAction += InputToRotationWorld_Y;
                // }
                // else
                // {
                    this.InputFloatAction += InputToRotation_Y;
                //}
                
            }           
            else
            {
                Debug.Log("AnimationEffectReceive: To Rotation Y selected but to_Transform input is empty");
            }
            
        }
        if (to_Rotation_Z)
        {
            if (to_Transform !=null)
            {
                // if (use_World_Space)
                // {
                //     this.InputFloatAction += InputToRotationWorld_Z;
                // }
                // else
                // {
                    this.InputFloatAction += InputToRotation_Z;
                //}
                
            }           
            else
            {
                Debug.Log("AnimationEffectReceive: To Rotation Z selected but to_Transform input is empty");
            }
        }

        if (to_Rotation_X_Additive)
        {
            if (to_Transform !=null)
            {
                if (use_World_Space)
                {
                    this.InputFloatAction += InputToRotationWorld_X;
                }
                else
                {
                    this.InputFloatAction += InputToRotation_X_Additive;
                }
            }            
            else
            {
                Debug.Log("AnimationEffectReceive: To Rotation X additive selected but to_Transform input is empty");
            }
            
        }
        if (to_Rotation_Y_Additive)
        {
            if (to_Transform !=null)
            {
                if (use_World_Space)
                {
                    this.InputFloatAction += InputToRotationWorld_Y;
                }
                else
                {
                    this.InputFloatAction += InputToRotation_Y_Additive;
                }
            }            
            else
            {
                Debug.Log("AnimationEffectReceive: To Rotation Y additive selected but to_Transform input is empty");
            }
            
        }
        if (to_Rotation_Z_Additive)
        {
            if (to_Transform !=null)
            {
                if (use_World_Space)
                {
                    this.InputFloatAction += InputToRotationWorld_Z;
                }
                else
                {
                    this.InputFloatAction += InputToRotation_Z_Additive;
                }
            }            
            else
            {
                Debug.Log("AnimationEffectReceive: To Rotation Z additive selected but to_Transform input is empty");
            }
        }
        //////////////////////
        if (to_Scale_X)
        {
            if (to_Transform !=null)
            {
                this.InputFloatAction += InputToScale_X;
            }            
            else
            {
                Debug.Log("AnimationEffectReceive: To Scale X selected but to_Transform input is empty");
            }
            
        }
        if (to_Scale_Y)
        {
            if (to_Transform !=null)
            {
                this.InputFloatAction += InputToScale_Y;
            }            
            else
            {
                Debug.Log("AnimationEffectReceive: To Scale Y selected but to_Transform input is empty");
            }
            
        }
        if (to_Scale_Z)
        {
             if (to_Transform !=null)
            {
                this.InputFloatAction += InputToScale_Z;
            }            
            else
            {
                Debug.Log("AnimationEffectReceive: To Scale Z selected but to_Transform input is empty");
            }
            
        }
    }

  
    ///////////////////////////////////////////////

    public void InputToPositionWorld_X(float input)
    {
        Vector3 currentPos = to_Transform.position;
        currentPos.x = input;       
        to_Transform.transform.position = currentPos;        
    }
    public void InputToPositionWorld_Y(float input)
    {
        Vector3 currentPos = to_Transform.position;
        currentPos.y = input;       
        to_Transform.transform.position = currentPos;        
    }
    public void InputToPositionWorld_Z(float input)
    {
        Vector3 currentPos = to_Transform.position;
        currentPos.z = input;       
        to_Transform.transform.position = currentPos;        
    }
    public void InputToPositionWorld_X_Additive(float input)
    {
        to_Transform.Translate(input,0,0, Space.World);        
    }
    public void InputToPositionWorld_Y_Additive(float input)
    {
        to_Transform.Translate(0,input,0, Space.World);        
    }
    public void InputToPositionWorld_Z_Additive(float input)
    {
        to_Transform.Translate(0,0,input, Space.World);        
    }

    public void InputToRotationWorld_X(float input)
    { 
       transform.Rotate(Vector3.right, input, Space.World);        
    }
    public void InputToRotationWorld_Y(float input)
    {
        
    transform.Rotate(Vector3.up, input, Space.World);
    }
    public void InputToRotationWorld_Z(float input)
    {
        
        transform.Rotate(Vector3.forward, input, Space.World);        
    }
    
    public void InputToPosition_X(float input)
    {
        Vector3 currentPos = to_Transform.localPosition;
        currentPos.x = input;       
        to_Transform.transform.localPosition = currentPos;        
    }
    
    public void InputToPosition_Y(float input)
    {
        Vector3 currentPos = to_Transform.localPosition;       
        currentPos.y = input;        
        to_Transform.transform.localPosition = currentPos;        
    }
    

    public void InputToPosition_Z(float input)
    {       
        Vector3 currentPos = to_Transform.localPosition;        
        currentPos.z = input;                
        to_Transform.transform.localPosition = currentPos;          
    }
    public void InputToPosition_X_Additive(float input)
    {
        to_Transform.Translate(input,0,0);        
    }
    public void InputToPosition_Y_Additive(float input)
    {
        to_Transform.Translate(0,input,0);        
    }
    public void InputToPosition_Z_Additive(float input)
    {
        to_Transform.Translate(0,0,input);        
    }
    /////////////////////////////////////////////////////////////
    public void InputToRotation_X(float input)
    {
        
        Vector3 currentRot = to_Transform.localEulerAngles;
        currentRot.x = input;    
        to_Transform.transform.localEulerAngles = currentRot;        
    }
    public void InputToRotation_Y(float input)
    {
        Vector3 currentRot = to_Transform.localEulerAngles;
        currentRot.y = input;
        to_Transform.transform.localEulerAngles = currentRot;      
    }

    public void InputToRotation_Z(float input)
    {       
        Vector3 currentRot = to_Transform.localEulerAngles;
        currentRot.z = input;
        to_Transform.transform.localEulerAngles = currentRot;        
    }
    public void InputToRotation_X_Additive(float input)
    {
        to_Transform.Rotate(input,0,0,Space.Self);
        // Vector3 currentRot = to_Transform.localEulerAngles;
        // currentRot.x += input;
        // to_Transform.transform.localEulerAngles = currentRot;        
    }
    public void InputToRotation_Y_Additive(float input)
    {
        to_Transform.Rotate(0,input,0,Space.Self);     
    }

    public void InputToRotation_Z_Additive(float input)
    {       
       to_Transform.Rotate(0,0,input,Space.Self);        
    }
    /////////////////////////////////////////////////////////////
    public void InputToScale_X(float input)
    {
        Vector3 currentScale = to_Transform.localScale;
        currentScale.x = input;        
        to_Transform.transform.localScale = currentScale;        
    }
    public void InputToScale_Y(float input)
    {
        Vector3 currentScale = to_Transform.localScale;
        currentScale.y = input;        
        to_Transform.transform.localScale = currentScale;      
    }

    public void InputToScale_Z(float input)
    {       
        Vector3 currentScale = to_Transform.localScale;
        currentScale.z = input;
        to_Transform.transform.localScale = currentScale;        
    }
    
}
