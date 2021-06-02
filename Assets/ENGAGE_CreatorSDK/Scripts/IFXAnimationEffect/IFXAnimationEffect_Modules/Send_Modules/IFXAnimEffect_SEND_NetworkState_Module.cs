using UnityEngine;
using Math = System.Math;

//[RequireComponent(typeof(LVR_Location_NetworkState))]
[RequireComponent(typeof(IFXAnimEffect_SEND_MAIN))]
[AddComponentMenu("IFXAnimEffect_SEND/NetworkState - IFX SEND Module")]
public class IFXAnimEffect_SEND_NetworkState_Module : IFXAnimEffect_SEND_Module
{
    // Used In The Editor to show what type of module this is 
    #if UNITY_EDITOR
    public override string moduleType {get{ return "NetworkState";} }
    #endif
    delegate float UpdateValuesDelegate();
    UpdateValuesDelegate UpdateValues;

    //////////////////////////////////
    [SerializeField]
    bool updateOnlyOnChange =true;
    //This is an example of an option you could have
    [SerializeField]
    LVR_Location_NetworkStateManager networkStateManager;
    [SerializeField]
    LVR_Location_NetworkState module_NetworkState;

    //SEND MODULES SHOULD ONLY SEND ONE VALUE!
    //You can use if statments and the UpdateValues delegate to choose the appropriate update method though

    //////////////////////////////////

    private void OnEnable()
    {
        if (module_NetworkState ==null)
        {
            module_NetworkState = GetComponent<LVR_Location_NetworkState>();
        }
        
        if (networkStateManager!=null)
        {
            UpdateValues += NetworkStateRead;
        }
        
    }
    //This method gets called by SEND_Main to retrive to value from the delegate. Only one method should be returning values.
    public override void SendOutput()
    {
        float NetworksCurrentValue = NetworkStateRead();
        if (updateOnlyOnChange)
        {
            if (AnimationEffectVariable.Value !=NetworksCurrentValue)
            {
                AnimationEffectVariable.Value = NetworksCurrentValue;
            }
        }
        else
        {
            AnimationEffectVariable.Value = NetworksCurrentValue;
        }
       
    }
    ///////////////////////////////////////////////
    private float ConvertIntToFloat(float input)
    {
        return (float)input/1000;
        //return (float)Math.Round(input);
    }
    
    private float NetworkStateRead() 
    {
        //Debug.Log("AnimationEffect: Currentstate "+ ConvertIntToFloat(module_NetworkState.currentState));
        //int retrivedStateValue = module_NetworkState.currentState;
        return ConvertIntToFloat(module_NetworkState.currentState);
    }
    
}
