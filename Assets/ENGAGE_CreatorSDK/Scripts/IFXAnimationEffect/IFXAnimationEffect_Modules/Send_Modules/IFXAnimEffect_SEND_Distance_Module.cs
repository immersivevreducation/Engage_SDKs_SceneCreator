using UnityEngine;
[AddComponentMenu("IFXAnimEffect_SEND/Distance - IFX SEND Module")]
[RequireComponent(typeof(IFXAnimEffect_SEND_MAIN))]
public class IFXAnimEffect_SEND_Distance_Module : IFXAnimEffect_SEND_Module
{
    #if UNITY_EDITOR
    public override string moduleType {get{ return "Distance";} }
    #endif

    delegate float UpdateValuesDelegate();
    UpdateValuesDelegate UpdateValues;

    //////////////////////////////////
    [SerializeField]
    bool invert;
    [SerializeField]
    Transform distanceTarget1;
    [SerializeField]
    Transform distanceTarget2;

    //////////////////////////////////

    private void OnEnable()
    {
        if (distanceTarget1 && distanceTarget2 !=null)
        {
            if (invert)
            {
                UpdateValues += GetDistanceInverted;
            }
            else
            {
                UpdateValues += GetDistance;
            }
            
        }
        else
        {
            Debug.Log("FXAnimEffect_SEND_Distance_Module: Distance target 1 or 2 is empty");
        }
        
        
    }
    //This method gets called by SEND_Main to retrive to value from the delegate. Only one method should be returning values.
    public override void SendOutput()
    {
       AnimationEffectVariable.Value = UpdateValues();
    }
    ///////////////////////////////////////////////

    private float GetDistance() 
    {
        float output = Vector3.Distance(distanceTarget1.position,distanceTarget2.position);
        return output;
    }
    private float GetDistanceInverted() 
    {
        float output = 1 / Vector3.Distance(distanceTarget1.position,distanceTarget2.position);
        return output;
    }
    
}
