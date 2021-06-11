using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Math = System.Math;
using System;
[AddComponentMenu("IFXAnimEffect_SEND/SEND MAIN")]
public class IFXAnimEffect_SEND_MAIN : MonoBehaviour
{
    delegate void UpdateValuesDelegate();
    UpdateValuesDelegate ModulesSendOuput;
    //Action<float> ModulesSendOuput;

    // [SerializeField]
    // public IFXAnimationEffectFloatVariable AnimationEffectVariable;
     //////////////////////
    //[SerializeField]
    public List<IFXAnimEffect_SEND_Module> SEND_Modules =new List<IFXAnimEffect_SEND_Module>();    
    [SerializeField]
    float updateRate = 0f;

    private void Start()
    {
        foreach (IFXAnimEffect_SEND_Module module in SEND_Modules)
        {
            if (module !=null)
            {
                ModulesSendOuput +=module.SendOutput;
            }           
        }
        // if (SEND_Module == null)
        // {
        //     Debug.Log("IFXAnimationEffect_SEND: SEND_Module not set, a Send Module is required.");
        // }
        // if (AnimationEffectVariable == null)
        // {
        //     Debug.Log("IFXAnimationEffect_SEND: AnimationEffectVariable not set, a AnimationEffectVariable is required.");
        // }
    }
    float timer = 0;
    void Update()
    {
        //Check if the player has loaded in yet. if not restart. There is probobly a better way to do this than checking every frame.
        //only an issue when the script is used in a scene, since the scene loades before the player.
        // if (playerCamera == null )
        // {
        //     //Initialize();
        //     Debug.Log("AnimationEffect Send: Player Not found Restarting");
        //     return;
        // }

        if (updateRate > 0f)
        {
            timer += Time.deltaTime;
            if(timer>updateRate)
            {
                this.ModulesSendOuput();
                //AnimationEffectVariable.Value = output;
                timer = 0;
            }    
        }
        else
        {
            this.ModulesSendOuput();
            //AnimationEffectVariable.Value = output;
        }
    }
}
[SerializeField]
public abstract class  IFXAnimEffect_SEND_Module : MonoBehaviour
{
    // delegate float UpdateValuesDelegate();
    // UpdateValuesDelegate UpdateValues;
    #if UNITY_EDITOR
    public virtual string moduleType { get; }
    public string custom_Name="";
    [HideInInspector]
    public bool isExpanded;
    #endif

    [SerializeField]
    public IFXAnimationEffectFloatVariable AnimationEffectVariable;
    public abstract void SendOutput();
    // {
    //     AnimationEffectVariable.Value = UpdateValues();
    // }
    
}

