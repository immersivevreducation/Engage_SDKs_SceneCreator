using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("IFXAnimEffect_RECEIVE/EnableDisable - IFX RECEIVE Module")]
[RequireComponent(typeof(IFXAnimEffect_RECEIVE_MAIN))]
public class IFXAnimEffect_RECEIVE_EnableDisable_Module : IFXAnimEffect_RECEIVE_Module
{

    #if UNITY_EDITOR
    public override string moduleType {get{ return "EnableDisable";} }
    #endif

    [SerializeField]
    List<Behaviour> to_EnableDisableComponents;
    [Header("--------------------------------------------------------------")]
    [SerializeField]
    List<GameObject> to_EnableDisableGameObjects;

    //////////////////////////////////

    private void OnEnable()
    {
        if (to_EnableDisableComponents.Count > 0)
        {
            this.InputBoolAction += InputEnableDisableComponents;
        }

        if (to_EnableDisableGameObjects.Count > 0)
        {
            this.InputBoolAction += InputEnableDisableGameObjects;
        }
    }

   
    ///////////////////////////////////////////////

    public void InputEnableDisableComponents(bool input)
    {    
        if (!input)
        {            
            for (int i = 0; i < to_EnableDisableComponents.Count; i++)
            {
                to_EnableDisableComponents[i].enabled =true;
            }         
        }

        else
        {           
            for (int i = 0; i < to_EnableDisableComponents.Count; i++)
            {
                to_EnableDisableComponents[i].enabled =false;
            }            
        }                 
    }

    public void InputEnableDisableGameObjects(bool input)
    {    
        if (!input)
        {            
            for (int i = 0; i < to_EnableDisableGameObjects.Count; i++)
            {
                to_EnableDisableGameObjects[i].SetActive(true);
            }         
        }

        else
        {           
            for (int i = 0; i < to_EnableDisableGameObjects.Count; i++)
                {
                    to_EnableDisableGameObjects[i].SetActive(false);
                }            
        }         
    }
    /////////////////////////////////////////////////
    
}
