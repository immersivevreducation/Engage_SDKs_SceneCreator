using UnityEngine;
[AddComponentMenu("IFXAnimEffect_SEND/Transform - IFX SEND Module")]
[RequireComponent(typeof(IFXAnimEffect_SEND_MAIN))]
public class IFXAnimEffect_SEND_Transforms_Module : IFXAnimEffect_SEND_Module
{
    #if UNITY_EDITOR
    public override string moduleType {get{ return "Transform";} }
    #endif
    //public override string moduleType ="Transform";
    
    [SerializeField]
    Transform input_Transform;

    delegate float UpdateValuesDelegate();
    UpdateValuesDelegate UpdateValues;

    //////////////////////////////////
    [SerializeField]
    bool from_Positon_X;
    [SerializeField]
    bool from_Positon_Y;
    [SerializeField]
    bool from_Positon_Z;


    [SerializeField]
    bool from_Rotation_X;
    [SerializeField]
    bool from_Rotation_Y;
    [SerializeField]
    bool from_Rotation_Z;


    [SerializeField]
    bool from_Scale_X;
    [SerializeField]
    bool from_Scale_Y;
    [SerializeField]
    bool from_Scale_Z;

    //////////////////////////////////

    private void OnEnable()
    {
        if (input_Transform == null)
        {
            Debug.Log("IFXAnimationEffect_SEND_Transforms_Module: Input_Transform not set, an input_Transform is required.");
        }
        /////////////////////////////////////////////////////
         if (from_Positon_X)
        {
            UpdateValues += GetPostition_X;
        }
        if (from_Positon_Y)
        {
            UpdateValues += GetPostition_Y;
        }
        if (from_Positon_Z)
        {
            UpdateValues += GetPostition_Z;
        }
        /////////////////////
        if (from_Rotation_X)
        {
            UpdateValues += GetRotation_X;
        }
        if (from_Rotation_Y)
        {
            UpdateValues += GetRotation_Y;
        }
        if (from_Rotation_Z)
        {
            UpdateValues += GetRotation_Z;
        }
        //////////////////////
        if (from_Scale_X)
        {
            UpdateValues += GetScale_X;
        }
        if (from_Scale_Y)
        {
            UpdateValues += GetScale_Y;
        }
        if (from_Scale_Z)
        {
            UpdateValues += GetScale_Z;
        }
    }

    public override void SendOutput()
    {
       AnimationEffectVariable.Value = UpdateValues();
    }
    ///////////////////////////////////////////////

    private float GetPostition_X() 
    {
        float output = input_Transform.localPosition.x;
        return output;
    }
    private float GetPostition_Y() 
    {
        float output = input_Transform.localPosition.y;
        return output;
    }
    private float GetPostition_Z() 
    {
        float output = input_Transform.localPosition.z;
        return output;
    }

    /////////////////////////////////
    private float GetRotation_X() 
    {
        float output = input_Transform.localEulerAngles.x;
        return output;
    }
    private float GetRotation_Y() 
    {
        float output = input_Transform.localEulerAngles.y;
        return output;
    }
    private float GetRotation_Z() 
    {
        float output = input_Transform.localEulerAngles.z;
        return output;
    }
    //////////////////////////////////
    private float GetScale_X() 
    {
        float output = input_Transform.localScale.x;
        return output;
    }
    private float GetScale_Y() 
    {
        float output = input_Transform.localScale.y;
        return output;
    }
    private float GetScale_Z() 
    {
        float output = input_Transform.localScale.z;
        return output;
    }
    
}
