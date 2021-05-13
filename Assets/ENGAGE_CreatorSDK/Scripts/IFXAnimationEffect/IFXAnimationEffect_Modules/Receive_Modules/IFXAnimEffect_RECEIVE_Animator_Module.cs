using UnityEngine;
[AddComponentMenu("IFXAnimEffect_RECEIVE/Animator - IFX RECEIVE Module")]
[RequireComponent(typeof(IFXAnimEffect_RECEIVE_MAIN))]
public class IFXAnimEffect_RECEIVE_Animator_Module : IFXAnimEffect_RECEIVE_Module
{
    #if UNITY_EDITOR
    public override string moduleType {get{ return "Animator";} }
    #endif
    [SerializeField]
    Animator to_Animator;

    [SerializeField]    
    string animator_ParamaterNameFloat;
    [SerializeField]
    string animator_ParamaterNameBool;

    private void OnEnable()
    {
        
        if (to_Animator != null && !string.IsNullOrEmpty(animator_ParamaterNameBool))
        {
            this.InputBoolAction += InputAnimatorBool;
        }
        else
        {
            Debug.Log("AnimationEffectReceive: Can't use animator Bool when no animator is set or parameter name is empty");
        }
        
    
    
        if (to_Animator != null && !string.IsNullOrEmpty(animator_ParamaterNameFloat))
        {
            this.InputFloatAction += InputAnimatorFloat;
        }
        else
        {
            Debug.Log("AnimationEffectReceive: Can't use animator float when no animator is set or parameter name is empty");
        }  
            
        
                
            
       
    }

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
    
}
