using UnityEngine;
using UnityEngine.Events;
[AddComponentMenu("IFXAnimEffect_RECEIVE/Event - IFX RECEIVE Module")]
[RequireComponent(typeof(IFXAnimEffect_RECEIVE_MAIN))]
public class IFXAnimEffect_RECEIVE_Event_Module : IFXAnimEffect_RECEIVE_Module
{
    #if UNITY_EDITOR
    public override string moduleType {get{ return "Event";} }
    #endif
    //////////////////////////////////
    [Header("Invoke event while anim effect trigger is TRUE")]


    [SerializeField]
    bool invokeWhenTrue;

    [SerializeField]
    UnityEvent eventWhenTrue;
    [Header("Invoke event while anim effect trigger is False")]

    [SerializeField]
    bool invokeWhenFalse;
    
    [SerializeField]    
    UnityEvent eventWhenFalse;
    //////////////////////////////////

    private void OnEnable()
    { 
        if (eventWhenTrue != null ||eventWhenFalse != null)
        {
            if (eventWhenTrue != null)
            {
                this.InputBoolAction += TriggerEventOnTrue;
            }
            if (eventWhenFalse != null)
            {
                this.InputBoolAction += TriggerEventOnFalse;
            }            
        }
        else
        {
            Debug.Log("IFXAnimEffect_RECEIVE_Event_Module: Requires at least one event");
        } 
    }

    //Add your custom methods here. They should take either a float or bool input as appropriate and do somthing with that value. use the delegates show above to feed these methods
    ///////////////////////////////////////////////

    void TriggerEventOnTrue(bool input)
    {    
        if (input)
        {            
            eventWhenTrue.Invoke();       
        }
    }
    void TriggerEventOnFalse(bool input)
    {    
        if (!input)
        {
            eventWhenFalse.Invoke();
        }
    }
    
}
