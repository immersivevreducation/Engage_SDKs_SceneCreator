using UnityEngine;
using TMPro;
[RequireComponent(typeof(TextMeshPro))]
[AddComponentMenu("IFXAnimEffect_RECEIVE/Text - IFX RECEIVE Module")]
[RequireComponent(typeof(IFXAnimEffect_RECEIVE_MAIN))]
public class IFXAnimEffect_RECEIVE_Text_Module : IFXAnimEffect_RECEIVE_Module
{
    // a name for this type of module for use in the editor
    #if UNITY_EDITOR
    public override string moduleType {get{ return "Text";} }
    #endif
    //////////////////////////////////
    [SerializeField]
    TextMeshPro textMeshPro;

    //RECEIVE can use the same value to effect multiple things. For example the value coming in could be used to move the object on the x axis and the y axis at the same time.
    //You should not have another receive module also effect the same value though, for example two RECEIVES both trying to tranlate on the x axis


    //You can use if statments and the "InputBoolAction" & "InputFloatAction" delegates to choose the appropriate update method or methods you want the value fed to.

    //////////////////////////////////

    private void OnEnable()
    {
        if (textMeshPro !=null)
        {
            this.InputFloatAction += FloatToText;
        }
        else
        {
            Debug.Log("IFXAnimationEffect: Receive Text Module Null");
        }
        
        //Add your new method to the approriate delagate here. 
        //the two delgates curently are
        // "this.InputFloatAction" for floats
        //and
        //this.InputBoolAction for bools

        //An example for methods that use a float input
        // if (to_Positon_X)
        // {
        //      this.InputFloatAction += InputToPosition_X;
        // }

        //for methods that use a bool input
        // if (useWorldSpace)
        // {
        //      this.InputBoolAction += InputToWorldTrue;
        // }
                
            
       
    }

    //Add your custom methods here. They should take either a float or bool input as appropriate and do somthing with that value. use the delegates show above to feed these methods
    ///////////////////////////////////////////////

    public void FloatToText(float input)
    {
        textMeshPro.text = input.ToString();
    }

    
}
