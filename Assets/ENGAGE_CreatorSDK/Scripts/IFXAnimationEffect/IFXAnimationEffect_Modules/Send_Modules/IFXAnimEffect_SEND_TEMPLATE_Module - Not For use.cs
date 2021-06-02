using UnityEngine;
// Uncomment the line below and enter how you want this module to show in the component menu
//[AddComponentMenu("IFXAnimEffect_SEND/Template - IFX SEND Module")]
[RequireComponent(typeof(IFXAnimEffect_SEND_MAIN))]
public class IFXAnimEffect_SEND_Template_Module : IFXAnimEffect_SEND_Module
{
    // Used In The Editor to show what type of module this is 
    #if UNITY_EDITOR
    public override string moduleType {get{ return "Template";} }
    #endif
    delegate float UpdateValuesDelegate();
    UpdateValuesDelegate UpdateValues;

    //////////////////////////////////
    //This is an example of an option you could have
    // [SerializeField]
    // Transform input_Transform;
    // [SerializeField]
    // bool from_Positon_Y;

    //SEND MODULES SHOULD ONLY SEND ONE VALUE!
    //You can use if statments and the UpdateValues delegate to choose the appropriate update method though

    //////////////////////////////////

    private void OnEnable()
    {
        //Add method to UpdateValues delagate here. For example
        // if (from_Positon_Y)
        // {
        //     UpdateValues += GetPostition_Y;
        // }
        
    }
    //This method gets called by SEND_Main to retrive to value from the delegate. Only one method should be returning values.
    public override void SendOutput()
    {
       AnimationEffectVariable.Value = UpdateValues();
    }
    ///////////////////////////////////////////////

    //Add your methods here for example
    // private float GetPostition_Y() 
    // {
    //     float output = input_Transform.localPosition.y;
    //     return output;
    // }
    
}
