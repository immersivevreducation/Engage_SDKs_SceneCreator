using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Math = System.Math;
using System;

public class IFXAnimationEffectsReceive : MonoBehaviour
{
    [Header("Value Input options")]
    [SerializeField]
    IFXAnimationEffectFloatVariable floatVariableIN;
    [SerializeField]
    float currentValue;
    [Tooltip("Multiplies the effect of the input value. 1 = 1:1 ratio. Minus Numbers reverse the effect")] 
    [SerializeField]
    float weight = 1.0f;

    [Tooltip("Makes value transition between current value and latest value recieved. A value of 0 disables smoothing")]
    [SerializeField]
    float inputSmoothing;
    

    [Header("Limit Input value:")]
    [Tooltip("If set only values that are more/less than this value will be used")] 
    [SerializeField]
    float valueLimiter = 0.0f;
    [SerializeField]
    bool moreThan;
    [SerializeField]
    bool lessThan;
    [Tooltip("If set only values between these two values will be used")]
    [SerializeField]
    bool useValueRangeLimit;
    [SerializeField]
    Vector2 valueRange;
    
    


    // //delegate void InputValuesDelegate(float value);
    // InputValuesDelegate InputValues;

    // //delegate void InputTriggerDelegate(bool value);
    // InputTriggerDelegate InputTrigger;
    //delegate void InputValuesDelegate(float value);
    Action<float> InputValuesAction;

    //delegate void InputTriggerDelegate(bool value);
    Action<bool> InputTriggerAction;

    
    ///////////////////////////////////////////////////////////
    
   

    ///////////////////////////////////////////////////////////
    
    
    ///////////////////////////////////////////////////////////
    [Header("Apply value to:")]
    [Header("--------------------------------------------------------------")]
    [SerializeField]
    Transform to_Transform;
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
    
    ///////////////////////////
    [Header("--------------------------------------------------------------")]
    [SerializeField]
    Material materialIn;

    [SerializeField]
    bool to_uvOffset_X;
    [SerializeField]
    bool to_uvOffset_Y;

    [SerializeField]
    bool to_MaterialColor_R;
    [SerializeField]
    bool to_MaterialColor_G;
    [SerializeField]
    bool to_MaterialColor_B;
    [SerializeField]
    bool to_MaterialColor_A;

    [SerializeField]
    bool to_MaterialEmissive_R;
    [SerializeField]
    bool to_MaterialEmissive_G;
    [SerializeField]
    bool to_MaterialEmissive_B;


     
    //////////////////////////
    [Header("--------------------------------------------------------------")]
    [SerializeField]
    AudioSource audioIn;
    [SerializeField]
    bool to_AudioTime;
    [SerializeField]
    bool to_AudioVolume;
    [SerializeField]
    bool to_AudioPitch;
    [Header("--------------------------------------------------------------")]

     //////////////////////////
     [SerializeField]
    Animator to_Animator;
    [SerializeField]
    bool to_AnimatorFloat;
    [SerializeField]
    
    string animator_ParamaterNameFloat;
    //[SerializeField]
    //IFXAnimationEffectFloatVariable animator_ParamaterValueFloat; 
    ////////////////////////////
    
    [Header("Trigger Settings:")]
    [SerializeField]
    float triggerThreshold=0.0f;
    [SerializeField]
    bool moreThan_Trigger;
    [SerializeField]
    bool lessThan_Trigger;
    [SerializeField]
    bool whenNotEqualTo_Trigger;
    [SerializeField]
    float notEqualToValue;

    [SerializeField]
    bool useTriggerValueRangeLimit;
    [SerializeField]
    Vector2 triggerValueRange;
    
    [Header("Triggerables:")]
    [Header("--------------------------------------------------------------")]
    
    [SerializeField]
    bool to_AnimatorBool;
    [SerializeField]
    string animator_ParamaterNameBool;
    
    
    
    [Header("--------------------------------------------------------------")]
    [SerializeField]
    UnityEvent to_Event;
    [Header("--------------------------------------------------------------")]
    [SerializeField]
    List<Behaviour> to_EnableDisableComponents;
    [Header("--------------------------------------------------------------")]
    [SerializeField]
    List<GameObject> to_EnableDisableGameObjects;
    
    // [SerializeField]
    // float enableDisableThreshhold;
    // [SerializeField]
    // bool moreThan;
    // [SerializeField]
    // bool lessThan;
    
    ///////////////////////////////////////////////////////////
    
   
    ///////////////////////////////////////////////////////////

    private void OnEnable() 
    {
        

        //////////////////
        if (to_Positon_X)
        {
            if (to_Transform !=null)
            {
                InputValuesAction += InputToPosition_X;
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
                InputValuesAction += InputToPosition_X_Additive;
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
                InputValuesAction += InputToPosition_Y;
            }            
            else
            {
                Debug.Log("AnimationEffectReceive: To Position Y selected but to_Transform input is empty");
            }
            
        }
        if (to_Positon_Y_Additive)
        {
            if (to_Transform !=null)
            {
                InputValuesAction += InputToPosition_Y_Additive;
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
                InputValuesAction += InputToPosition_Z;
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
                InputValuesAction += InputToPosition_Z_Additive;
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
                InputValuesAction += InputToRotation_X;
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
                InputValuesAction += InputToRotation_Y;
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
                InputValuesAction += InputToRotation_Z;
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
                InputValuesAction += InputToRotation_X_Additive;
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
                InputValuesAction += InputToRotation_Y_Additive;
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
                InputValuesAction += InputToRotation_Z_Additive;
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
                InputValuesAction += InputToScale_X;
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
                InputValuesAction += InputToScale_Y;
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
                InputValuesAction += InputToScale_Z;
            }            
            else
            {
                Debug.Log("AnimationEffectReceive: To Scale Z selected but to_Transform input is empty");
            }
            
        }
         //////////////////////
        if (to_uvOffset_X)
        {
            InputValuesAction += InputMaterialUVOffset_X;
        }
        if (to_uvOffset_Y)
        {
            InputValuesAction += InputMaterialUVOffset_Y;
        }

        //////////////////
        if (to_MaterialColor_R)
        {
            if (materialIn !=null)
            {
                InputValuesAction += InputMaterialColor_R;
            }            
            else
            {
                Debug.Log("AnimationEffectReceive: to Material R selected but Material input empty");
            }
        }
        if (to_MaterialColor_G)
        {
            if (materialIn !=null)
            {
                InputValuesAction += InputMaterialColor_G;
            }            
            else
            {
                Debug.Log("AnimationEffectReceive: to Material G selected but Material input empty");
            }
        }
        if (to_MaterialColor_B)
        {
            if (materialIn !=null)
            {
                InputValuesAction += InputMaterialColor_B;
            }            
            else
            {
                Debug.Log("AnimationEffectReceive: to Material B selected but Material input empty");
            }
            
        }
        if (to_MaterialColor_A)
        {
            if (materialIn !=null)
            {
                InputValuesAction += InputMaterialColor_ALPHA;
            }            
            else
            {
                Debug.Log("AnimationEffectReceive: to Material Alpha selected but Material input empty");
            }
            
        }

        //////////////////
        if (to_MaterialEmissive_R)
        {
            if (materialIn !=null)
            {
                InputValuesAction += InputMaterialEmission_R;
            }            
            else
            {
                Debug.Log("AnimationEffectReceive: to Material emission R selected but Material input empty");
            }
        }
        if (to_MaterialEmissive_G)
        {
            if (materialIn !=null)
            {
                InputValuesAction += InputMaterialEmission_G;
            }            
            else
            {
                Debug.Log("AnimationEffectReceive: to Material emission G selected but Material input empty");
            }
            
        }
        if (to_MaterialEmissive_B)
        {
            if (materialIn !=null)
            {
                InputValuesAction += InputMaterialEmission_B;
            }            
            else
            {
                Debug.Log("AnimationEffectReceive: to Material emission B selected but Material input empty");
            }
        }
        
    
        ////////////////////////
       
        if (to_AudioVolume)
        {
            if (audioIn !=null)
            {
                InputValuesAction += InputAudioVolume;
            }            
            else
            {
                Debug.Log("AnimationEffectReceive: to audio volume selected but audio input empty");
            }
        }
        if (to_AudioPitch)
        {
            if (audioIn !=null)
            {
                InputValuesAction += InputAudioPitch;
            }            
            else
            {
                Debug.Log("AnimationEffectReceive: to audio pitch selected but audio input empty");
            }
        }
        if (to_AudioTime)
        {
            if (audioIn !=null)
            {
                InputValuesAction += InputAudioTime;
            }            
            else
            {
                Debug.Log("AnimationEffectReceive: to audio time selected but audio input empty");
            }            
        }
        //////////////////////////////////////////
        //Trigger Stuff
        //////////////////////////////////////////
        if (to_AnimatorBool)
        {
            if (to_Animator != null && !string.IsNullOrEmpty(animator_ParamaterNameBool))
            {
                InputTriggerAction += InputAnimatorBool;
            }
            else
            {
                Debug.Log("AnimationEffectReceive: Can't use animator Bool when no animator is set or parameter name is empty");
            }
            
        }
        if ( to_AnimatorFloat)
        {
            if (to_Animator != null && !string.IsNullOrEmpty(animator_ParamaterNameFloat))
            {
                InputValuesAction += InputAnimatorFloat;
            }
            else
            {
                Debug.Log("AnimationEffectReceive: Can't use animator float when no animator is set or parameter name is empty");
            }  
            
        }
        //////////////////////////////////////////
        if (to_Event != null)
        {
            InputTriggerAction += InputTriggerEvent;
        }
 
        ////////////////////////
       
        if (to_EnableDisableComponents.Count > 0)
        {
            InputTriggerAction += InputEnableDisableComponents;
        }

        if (to_EnableDisableGameObjects.Count > 0)
        {
            InputTriggerAction += InputEnableDisableGameObjects;
        }
       
       currentValue = floatVariableIN.GetMathOutput();
    }
    private void Update()
    {
        //InputTrigger(false);                              
        float valueIN = floatVariableIN.GetMathOutput();
        
        if (inputSmoothing >0f)
        {        
           if (currentValue != valueIN)
           {
                float t = 1f * Time.deltaTime;
                currentValue = Mathf.Lerp(currentValue, valueIN, t / inputSmoothing);                  
           }              
        }
        else
        {
            currentValue = valueIN;
        }
        
        if (InputValuesAction !=null)
        {
           if (InputLimiter(currentValue))
            {
                InputValuesAction(currentValue);
            }
        }
        
        if (InputTriggerAction !=null)
        {
            InputTriggerAction(InputTrigger(currentValue));
        }

        //InputTrigger(InputLimiterAndTrigger(currentValue, triggerThreshold, triggerValueRange, moreThan_Trigger, lessThan_Trigger));                        
        
    }
    bool InputLimiter(float input)
    {
        
        if (moreThan)
        {
            if (input > valueLimiter)
            {                
                return true;
            }
            else
            {
                return false;           
            }
        }
        if (lessThan)
        {
            if (input < valueLimiter)
            {
                
                return true;
            }
            else
            {
                return false;           
            }
        }
        
        if (useValueRangeLimit)
        {
            if (IsBetween(input, valueRange.x, valueRange.y))
            {
                return true;
            }
            else
            {
                return false;           
            }
        }

        else
        {
            return true;           
        }
    }
    bool InputTrigger(float input)
    {
        
        if (moreThan_Trigger)
        {
            if (input > triggerThreshold)
            {                
                return true;
            }
            else
            {
                return false;           
            }
        }
        if (lessThan_Trigger)
        {
            if (input < triggerThreshold)
            {
                
                return true;
            }
            else
            {
                return false;           
            }
        }
        if (whenNotEqualTo_Trigger)
        {
            if (input != notEqualToValue)
            {
                return true;
            }
            else
            {
                return false;           
            }
            
        }
        if (useTriggerValueRangeLimit)
        {
            if (IsBetween(input, triggerValueRange.x, triggerValueRange.y))
            {
                return true;
            }
            else
            {
                return false;           
            }
        }

        else
        {
            return false;           
        }
    }
    /////////////////////////////////////////////////////
    public void InputToPosition_X(float input)
    {
        Vector3 currentPos = to_Transform.localPosition;
        currentPos.x = input*weight;       
        to_Transform.transform.localPosition = currentPos;        
    }
    
    public void InputToPosition_Y(float input)
    {
        Vector3 currentPos = to_Transform.localPosition;       
        currentPos.y = input*weight;        
        to_Transform.transform.localPosition = currentPos;        
    }
    

    public void InputToPosition_Z(float input)
    {       
        Vector3 currentPos = to_Transform.localPosition;        
        currentPos.z = input*weight;                
        to_Transform.transform.localPosition = currentPos;          
    }
    public void InputToPosition_X_Additive(float input)
    {
        to_Transform.Translate(input*weight,0,0);        
    }
    public void InputToPosition_Y_Additive(float input)
    {
        to_Transform.Translate(0,input*weight,0);        
    }
    public void InputToPosition_Z_Additive(float input)
    {
        to_Transform.Translate(0,0,input*weight);        
    }
    /////////////////////////////////////////////////////////////
    public void InputToRotation_X(float input)
    {
        
        Vector3 currentRot = to_Transform.localEulerAngles;
        currentRot.x = input*weight;    
        to_Transform.transform.localEulerAngles = currentRot;        
    }
    public void InputToRotation_Y(float input)
    {
        Vector3 currentRot = to_Transform.localEulerAngles;
        currentRot.y = input*weight;
        to_Transform.transform.localEulerAngles = currentRot;      
    }

    public void InputToRotation_Z(float input)
    {       
        Vector3 currentRot = to_Transform.localEulerAngles;
        currentRot.z = input*weight;
        to_Transform.transform.localEulerAngles = currentRot;        
    }
    public void InputToRotation_X_Additive(float input)
    {
        to_Transform.Rotate(input*weight,0,0,Space.Self);
        // Vector3 currentRot = to_Transform.localEulerAngles;
        // currentRot.x += input*weight;
        // to_Transform.transform.localEulerAngles = currentRot;        
    }
    public void InputToRotation_Y_Additive(float input)
    {
        to_Transform.Rotate(0,input*weight,0,Space.Self);     
    }

    public void InputToRotation_Z_Additive(float input)
    {       
       to_Transform.Rotate(0,0,input*weight,Space.Self);        
    }
    /////////////////////////////////////////////////////////////
    public void InputToScale_X(float input)
    {
        Vector3 currentScale = to_Transform.localScale;
        currentScale.x = input*weight;        
        to_Transform.transform.localScale = currentScale;        
    }
    public void InputToScale_Y(float input)
    {
        Vector3 currentScale = to_Transform.localScale;
        currentScale.y = input*weight;        
        to_Transform.transform.localScale = currentScale;      
    }

    public void InputToScale_Z(float input)
    {       
        Vector3 currentScale = to_Transform.localScale;
        currentScale.z = input*weight;
        to_Transform.transform.localScale = currentScale;        
    }
    /////////////////////////////////////////////////////////////


    public void InputAudioPitch(float input)
    {       
        audioIn.pitch = input*weight;      
    }
    public void InputAudioVolume(float input)
    {       
        audioIn.volume = input*weight;      
    }
    public void InputAudioTime(float input)
    {       
        audioIn.time = input*weight;      
    }
    /////////////////////////////////////////////////////////////

    public void InputMaterialColor_R(float input)
    {       
        Color colorIN = materialIn.color;
        colorIN.r = input*weight;
        materialIn.color =colorIN;      
    }
    public void InputMaterialColor_G(float input)
    {       
        Color colorIN = materialIn.color;
        colorIN.g = input*weight;
        materialIn.color =colorIN;      
    }
    public void InputMaterialColor_B(float input)
    {       
        Color colorIN = materialIn.color;
        colorIN.b = input*weight;
        materialIn.color =colorIN;      
    }
    public void InputMaterialColor_ALPHA(float input)
    {       
        Color colorIN = materialIn.color;
        colorIN.a = input*weight;
        materialIn.color =colorIN;      
    }
        public void InputMaterialEmission_R(float input)
    {       
        Color colorIN = materialIn.GetColor("_EmissionColor");
        colorIN.r = input*weight;
        materialIn.SetColor("_EmissionColor", colorIN);      
    }
    public void InputMaterialEmission_G(float input)
    {       
        Color colorIN = materialIn.GetColor("_EmissionColor");
        colorIN.g = input*weight;
        materialIn.SetColor("_EmissionColor", colorIN);     
    }
    public void InputMaterialEmission_B(float input)
    {       
        Color colorIN = materialIn.GetColor("_EmissionColor");
        colorIN.b = input*weight;
        materialIn.SetColor("_EmissionColor", colorIN);     
    }
    public void InputMaterialUVOffset_X(float input)
    {       
        Vector2 uvOffset= materialIn.mainTextureOffset;
        uvOffset.x = input*weight;
        materialIn.mainTextureOffset = uvOffset;
    }
    public void InputMaterialUVOffset_Y(float input)
    {       
        Vector2 uvOffset= materialIn.mainTextureOffset;
        uvOffset.y = input*weight; 
        materialIn.mainTextureOffset = uvOffset;
    }
    //////////////////////////////////////////////////////
    public void InputEnableDisableComponents(bool input)
    {    if (input)
        {            
            for (int i = 0; i < to_EnableDisableComponents.Count; i++)
            {
                to_EnableDisableComponents[i].enabled =true;
            }         
        }

        else
        {           
            for (int i = 0; i < to_EnableDisableComponents.Count; i++)
            {
                to_EnableDisableComponents[i].enabled =false;
            }            
        }                 
    }

    public void InputEnableDisableGameObjects(bool input)
    {    
        if (input)
        {            
            for (int i = 0; i < to_EnableDisableGameObjects.Count; i++)
            {
                to_EnableDisableGameObjects[i].SetActive(true);
            }         
        }

        else
        {           
            for (int i = 0; i < to_EnableDisableGameObjects.Count; i++)
                {
                    to_EnableDisableGameObjects[i].SetActive(false);
                }            
        }         
    }

    /////////////////////////////////////////////////////////////

    public void InputTriggerEvent(bool input)
    {    
        if (input)
        {            
            to_Event.Invoke();       
        }
    }

    /////////////////////////////////////////////////////////////

    public void InputAnimatorBool(bool input)
    {         
        if (input)
        {            
            to_Animator.SetBool(animator_ParamaterNameBool,true);         
        }

        else
        {           
            to_Animator.SetBool(animator_ParamaterNameBool,false);            
        }                   
    }


    public void InputAnimatorFloat(float input)
    {
        to_Animator.SetFloat(animator_ParamaterNameFloat, input);
    }



///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public bool IsBetween(float testValue, float bound1, float bound2)
    {
        return (testValue >= Math.Min(bound1,bound2) && testValue <= Math.Max(bound1,bound2));
    } 
}



//this is an error to remind me to //seporate animator onoff from animatior feed float variable
