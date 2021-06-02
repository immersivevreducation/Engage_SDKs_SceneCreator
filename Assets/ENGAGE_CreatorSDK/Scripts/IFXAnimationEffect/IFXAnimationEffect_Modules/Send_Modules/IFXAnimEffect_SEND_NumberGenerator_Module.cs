using UnityEngine;
[AddComponentMenu("IFXAnimEffect_SEND/NumberGen - IFX SEND Module")]
[RequireComponent(typeof(IFXAnimEffect_SEND_MAIN))]
public class IFXAnimEffect_SEND_NumberGenerator_Module : IFXAnimEffect_SEND_Module
{
    #if UNITY_EDITOR
    public override string moduleType {get{ return "NumberGen";} }
    #endif

    delegate float UpdateValuesDelegate();
    UpdateValuesDelegate UpdateValues;

    [Tooltip("Increase the value of the variable set in floatVariableOut by the number below or another float variable if one is plugged in.")] 
    [SerializeField]
    bool from_IncreaseBY;
    [SerializeField]
    float increment = 1;
    [SerializeField]
    IFXAnimationEffectFloatVariable incrementInput;
    //////////////////////////
    [Header("--------------------------------------------------------------")]
    [SerializeField]
    bool from_Random;  
    [SerializeField]
    float randomMin = 0;
    [SerializeField]
    float randomMax = 1;
    [SerializeField]
    IFXAnimationEffectFloatVariable randomMinInput;
    [SerializeField]
    IFXAnimationEffectFloatVariable randomMaxInput;

    //////////////////////////////////

    private void OnEnable()
    {
        
        if (from_IncreaseBY)
        {
            if (incrementInput == null)
            {
                UpdateValues += GetIncreaseBy;
            }
            else
            {
                UpdateValues += GetIncreaseByInput;
            }

        }
        //////////////////////////
        if (from_Random)
        {
            if (randomMinInput != null || randomMaxInput != null)
            {
                UpdateValues += GetRandomFromInput;
            }
            else
            {
                UpdateValues += GetRandom;
            }

        }
        
    }
    //This method gets called by SEND_Main to retrive to value from the delegate. Only one method should be returning values.
    public override void SendOutput()
    {
       AnimationEffectVariable.Value = UpdateValues();
    }
    ///////////////////////////////////////////////

    private float GetIncreaseBy() 
    {

        float output = AnimationEffectVariable.Value + increment;
        return output;
    }
    private float GetIncreaseByInput() 
    {

        float output = AnimationEffectVariable.Value + incrementInput.GetMathOutput();
        return output;
    }

    private float GetRandom() 
    {
        float output = Random.Range(randomMin, randomMax);
        
        return output;
    }
    private float GetRandomFromInput() 
    {
        float randmin = randomMin;
        float randMax = randomMax;
        if (randomMinInput !=null)
        {
            randmin = randomMinInput.GetMathOutput();           
        }
        if (randomMaxInput !=null)
        {
            randMax = randomMaxInput.GetMathOutput();
        }
        float output = Random.Range(randmin, randMax);
        
        return output;
    }
    
}
