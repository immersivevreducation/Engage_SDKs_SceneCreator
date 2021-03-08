using UnityEngine;
using Math = System.Math;

[CreateAssetMenu(fileName = "Data", menuName = "IFXAnimationEffectVariable/Float", order = 1)]
public class IFXAnimationEffectFloatVariable : ScriptableObject
{
    public float Value;
     [SerializeField]
    bool UseMath = false;
    [SerializeField]
    
    bool useValueRangeLimiter;
    [SerializeField]
    Vector2 valueRangeLimiter;
    [SerializeField]
    [Header("Value Override:")]
    IFXAnimationEffectFloatVariable valueIN;
    [Header("Value Modifyer:")]
    [SerializeField]
    IFXAnimationEffectFloatVariable modifyer;
    ///////////////////////////////////////////
    [SerializeField]
    float manual_Input;
    [SerializeField]
    bool plus;
    [SerializeField]
    bool minus;

    [SerializeField]
    bool multiply;
    [SerializeField]
    bool divide;
       
    public float GetMathOutput()
    {
        float value = Value;
        if (UseMath==false)
        {
            return value;
        }

        if (valueIN !=null)
        {
            value = valueIN.GetMathOutput();
        }

        if (modifyer !=null)
        {
            if (plus)
            {
                value = value + modifyer.GetMathOutput();
            }
            if (minus)
            {
                value = value - modifyer.GetMathOutput();
            }
            if (multiply)
            {
                value = value * modifyer.GetMathOutput();
            }
            if (divide)
            {
                value = value / modifyer.GetMathOutput();
            }
            
            if (useValueRangeLimiter)
            {
                return InputLimiter(value);
            }


            return value;
        }
        if (plus)
        {
            value = value + manual_Input;
        }
        if (minus)
        {
            value = value - manual_Input;
        }
        if (multiply)
        {
            value = value * manual_Input;
        }
        if (divide)
        {
            value = value / manual_Input;
        }

        if (useValueRangeLimiter)
        {
            return InputLimiter(value);
        }
        return value;
    }

    public void SetValue(float setValue)
    {
        Value = setValue;
    }

    //////////////////////////////////////////
    float InputLimiter(float input)
    {
        //if ((input >= Math.Min(valueRangeLimiter.x, valueRangeLimiter.y) && input <= Math.Max(valueRangeLimiter.x,valueRangeLimiter.y)))
        if ((input > Math.Max(valueRangeLimiter.x,valueRangeLimiter.y)))
        {
            return Math.Max(valueRangeLimiter.x,valueRangeLimiter.y);
        }
        if(input < Math.Min(valueRangeLimiter.x, valueRangeLimiter.y))
        {
            return Math.Min(valueRangeLimiter.x, valueRangeLimiter.y);           
        }
        return input;
        
    }
}
