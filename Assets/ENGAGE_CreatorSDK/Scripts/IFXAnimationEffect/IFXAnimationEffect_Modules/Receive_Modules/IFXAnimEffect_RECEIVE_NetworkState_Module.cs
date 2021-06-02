using UnityEngine;
using Math = System.Math;

//[RequireComponent(typeof(LVR_Location_NetworkState))]
[RequireComponent(typeof(IFXAnimEffect_RECEIVE_MAIN))]
[AddComponentMenu("IFXAnimEffect_RECEIVE/NetworkState - IFX RECEIVE Module")]
public class IFXAnimEffect_RECEIVE_NetworkState_Module : IFXAnimEffect_RECEIVE_Module
{
    // a name for this type of module for use in the editor
    #if UNITY_EDITOR
    public override string moduleType {get{ return "NetworkState";} }
    #endif
    [SerializeField]
    bool updateOnlyOnChange;
    
    [SerializeField]
    LVR_Location_NetworkStateManager networkStateManager;
    [SerializeField]
    LVR_Location_NetworkState module_NetworkState;

    private void OnEnable()
    {
        if (module_NetworkState ==null)
        {
            module_NetworkState = GetComponent<LVR_Location_NetworkState>();
        }
        
        if (networkStateManager!=null)
        {
            this.InputFloatAction += NetworkStateUpdate;
        }
        
    }

    private int ConvertFloatToInt(float input)
    {
        return (int)Math.Round(input*1000);
        //return (int)Math.Round(input);
    }

    private  void NetworkStateUpdate(float input) 
    {
        int convertedValue = ConvertFloatToInt(input);
        //Debug.Log(convertedValue);
        if (updateOnlyOnChange)
        {
            if (module_NetworkState.currentState == convertedValue)
            {
                //Debug.Log("IFXAnimationEffect_RECEIVE_Networkstate: Value unchanged");
                return;
            }
            else
            {
                //Debug.Log("IFXAnimationEffect_RECEIVE_Networkstate: "+convertedValue);
                
                module_NetworkState.UpdateState(convertedValue);
               
            }
        }
        else
        {
            //Debug.Log("IFXAnimationEffect_RECEIVE_Networkstate: "+input);
            //int test = 10;
            module_NetworkState.UpdateState(convertedValue);
        }
         
        
    }
    
}
