using UnityEngine;

[RequireComponent(typeof(IFXAnimEffect_SEND_MAIN))]
public class IFXAnimEffect_SEND_Sine_Module : IFXAnimEffect_SEND_Module
{
    
    #if UNITY_EDITOR
    public override string moduleType {get{ return "Sine";} }
    #endif
    delegate float UpdateValuesDelegate();
    UpdateValuesDelegate UpdateValues;

    //////////////////////////////////
    [SerializeField]
    bool sinePositiveOnly=false; 
    [SerializeField]
    float sineFreq = 1;
    [SerializeField]
    float sineAmp = 1;
    [SerializeField]
    IFXAnimationEffectFloatVariable sineFreqInput;
    [SerializeField]
    IFXAnimationEffectFloatVariable sineAmpInput;
    //////////////////////////////////

    private void OnEnable()
    {        
        if (sineFreqInput != null || sineAmpInput != null)
        {
            UpdateValues += GetSineFromInput;
        }
        else
        {
            UpdateValues += GetSine;
        }      
    }
    //This method gets called by SEND_Main to retrive to value from the delegate. Only one method should be returning values.
    public override void SendOutput()
    {
        AnimationEffectVariable.Value = UpdateValues();
    }
    ///////////////////////////////////////////////

    private float GetSine() 
    {
        int sinePositiveOnlyINT=0;
        if (sinePositiveOnly)
        {
            sinePositiveOnlyINT=1;
        }
        
        float output = sineAmp*Mathf.Sin(Time.time*sineFreq)+sinePositiveOnlyINT;
        
        return output;
    }
    private float GetSineFromInput() 
    {
        int sinePositiveOnlyINT=0;
        float amp = sineAmp;
        float freq = sineFreq;
        if (sinePositiveOnly)
        {
            sinePositiveOnlyINT=1;
        }
        if (sineAmpInput !=null)
        {
            amp = sineAmpInput.GetMathOutput();           
        }
        if (sineFreqInput !=null)
        {
            freq = sineFreqInput.GetMathOutput();
        }
        float output = amp*Mathf.Sin(Time.time*freq)+sinePositiveOnlyINT;
        
        return output;
    }
    
}
