using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Math = System.Math;
using System;


public class IFXAnimEffect_RECEIVE_MAIN : MonoBehaviour
{
    Action<float> ModulesInputFloatAction;
    Action<bool> ModulesInputBoolAction;


    [SerializeField]
    IFXAnimationEffectFloatVariable AnimationEffectVariable;
     //////////////////////
    //[SerializeField]
    public List<IFXAnimEffect_RECEIVE_Module> RECEIVE_Modules = new List<IFXAnimEffect_RECEIVE_Module>();
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


    //////////////////////////

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

    /////////////////////////////
    private void OnEnable()
    {
        foreach (IFXAnimEffect_RECEIVE_Module module in RECEIVE_Modules)
        {
            if (module !=null)
            {
                if (module.InputFloatAction !=null)
                {
                    ModulesInputFloatAction += module.ReceiveInputFloat;
                }
                if (module.InputBoolAction !=null)
                {
                    ModulesInputBoolAction += module.ReceiveInputBool;
                }
            }
            
            
        }
        //Debug.Log(InputBoolAction);
    }
    private void Update()
    {
        //InputTrigger(false);                              
        float valueIN = AnimationEffectVariable.GetMathOutput() * weight;
        
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
        
        if (ModulesInputFloatAction !=null)
        {
           if (InputLimiter(currentValue))
            {
                ModulesInputFloatAction(currentValue);
            }
        }
        
        if (ModulesInputBoolAction !=null)
        {
            ModulesInputBoolAction(InputTrigger(currentValue));
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
    public bool IsBetween(float testValue, float bound1, float bound2)
    {
        return (testValue >= Math.Min(bound1,bound2) && testValue <= Math.Max(bound1,bound2));
    }
}
public abstract class  IFXAnimEffect_RECEIVE_Module : MonoBehaviour
{
    #if UNITY_EDITOR
    public virtual string moduleType { get; }
    public string custom_Name="";
    [HideInInspector]
    public bool isExpanded;
    #endif
    public Action<float> InputFloatAction;
    public Action<bool> InputBoolAction;
    public void ReceiveInputFloat(float input)
    {
        InputFloatAction(input);
    }
    public void ReceiveInputBool(bool input)
    {
        InputBoolAction(input);
    } 
}

