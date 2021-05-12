using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


using System.Linq;


public class IFXAnimationEffectsSend : MonoBehaviour
{   
    ///////////////////////////////////////////////////////////////////////////
    delegate float UpdateValuesDelegate();
    UpdateValuesDelegate UpdateValues;
    ////////////////////////////////////////////////////////////////////////////
    AudioSource audioIn;

    Camera playerCamera;

    
    [SerializeField]
    IFXAnimationEffectFloatVariable floatVariableOUT;
     //////////////////////    
    [SerializeField]
    float updateRate = 0f;
    
    [Header("Take Value From:")]
    [Header("ONLY SELECT ONE!")]
    
    //////////////////////
    [SerializeField]
    Transform input_Transform;

    [SerializeField]
    bool from_Positon_X;
    [SerializeField]
    bool from_Positon_Y;
    [SerializeField]
    bool from_Positon_Z;


    [SerializeField]
    bool from_Rotation_X;
    [SerializeField]
    bool from_Rotation_Y;
    [SerializeField]
    bool from_Rotation_Z;


    [SerializeField]
    bool from_Scale_X;
    [SerializeField]
    bool from_Scale_Y;
    [SerializeField]
    bool from_Scale_Z;
    [Header("--------------------------------------------------------------")]

     
    //////////////////////////
     [SerializeField]
    bool from_AudioSpectrum;
    [SerializeField]
    bool from_AudioVolume;
    [SerializeField]
    bool from_AudioPitch;
    [Header("--------------------------------------------------------------")]
    //////////////////////////
    [SerializeField]
    bool from_Sine;
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
    //////////////////////////
    [Header("--------------------------------------------------------------")]
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
    [Header("--------------------------------------------------------------")]
    //////////////////////////
    [SerializeField]
    bool from_Collision;
    [SerializeField]
    Collider collider1;
    [SerializeField]
    Collider collider2;
    [Header("--------------------------------------------------------------")]
    /////////////////////////
    [SerializeField]
    bool from_Distance;
    [SerializeField]
    Transform distanceTarget1;
    [SerializeField]
    Transform distanceTarget2;
    [Header("--------------------------------------------------------------")] 
    //////////////////////////
    //////////////////////    
    [SerializeField]
    bool from_PlayerPositon_X;
    [SerializeField]
    bool from_PlayerPositon_Y;
    [SerializeField]
    bool from_PlayerPositon_Z;

    //////////////////////    
    [SerializeField]
    bool from_PlayerRotation_X;
    [SerializeField]
    bool from_PlayerRotation_Y;
    [SerializeField]
    bool from_PlayerRotation_Z;
    
    ///////////////////////
    [SerializeField]
    bool from_PlayerCollision;
    [SerializeField]
    Collider playerCollisionTarget;
    ///////////////////////
    [SerializeField]
    bool from_PlayerDistance;

    [SerializeField]
    Transform playerDistanceTarget;
    ///////////////////////
    [SerializeField]
    bool from_PlayerHandDistance;

    [SerializeField]
    Transform playerHandDistanceTarget;
    
    ////////////////////////
    ///////////////////////
    [SerializeField]
    bool from_PlayerTouch;
    [SerializeField]
    Collider playerTouchCollider;
    [SerializeField]
    bool requireTriggerPress;
    [SerializeField]
    bool requireGrippedPress;

    
         
    private void OnEnable()
    {
        Initialize();
    }

    private void Initialize()
    {
        
        UpdateValues = null;
        //playerCamera = Engage.ScriptHelper.mainCamera;
        //////////////////////////////
        if (from_Positon_X)
        {
            UpdateValues += GetPostition_X;
        }
        if (from_Positon_Y)
        {
            UpdateValues += GetPostition_Y;
        }
        if (from_Positon_Z)
        {
            UpdateValues += GetPostition_Z;
        }
        /////////////////////
        if (from_Rotation_X)
        {
            UpdateValues += GetRotation_X;
        }
        if (from_Rotation_Y)
        {
            UpdateValues += GetRotation_Y;
        }
        if (from_Rotation_Z)
        {
            UpdateValues += GetRotation_Z;
        }
        //////////////////////
        if (from_Scale_X)
        {
            UpdateValues += GetScale_X;
        }
        if (from_Scale_Y)
        {
            UpdateValues += GetScale_Y;
        }
        if (from_Scale_Z)
        {
            UpdateValues += GetScale_Z;
        }
        ////////////////////////
        if (from_Distance)
        {
            UpdateValues += GetDistance;
        }
        ////////////////////////
        if (from_AudioSpectrum)
        {
            if (audioIn != null)
            {
                UpdateValues += GetAudioSpectrum;
            }
            else
            {
                Debug.Log("AnimationEffect: from AudioSpectrum selected but audioIn input empty");
            }
        }
        if (from_AudioVolume)
        {
            if (audioIn != null)
            {
                UpdateValues += GetAudioVolume;
            }
            else
            {
                Debug.Log("AnimationEffect: from AudioVolume selected but audioIn input empty");
            }
        }
        if (from_AudioPitch)
        {
            if (audioIn != null)
            {
                UpdateValues += GetAudioPitch;
            }
            else
            {
                Debug.Log("AnimationEffect: from AudioPitch selected but audioIn input empty");
            }
        }
        //////////////////////////
        if (from_Sine)
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
        //////////////////////////
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
        //////////////////////////
        if (from_Collision)
        {
            if (collider1 != null && collider2 != null)
            {
                UpdateValues += GetCollision;
            }
            else
            {
                Debug.Log("AnimationEffect: from collision selected but Collider input empty");
            }
        }
        //////////////////////////
        // if (from_PlayerPositon_X)
        // {
        //     if (playerCamera != null)
        //     {
        //         UpdateValues += GetPlayerPostition_X;
        //     }
        // }
        // if (from_PlayerPositon_Y)
        // {
        //     if (playerCamera != null)
        //     {
        //         UpdateValues += GetPlayerPostition_Y;
        //     }
        // }
        // if (from_PlayerPositon_Z)
        // {
        //     if (playerCamera != null)
        //     {
        //         UpdateValues += GetPlayerPostition_Z;
        //     }
        // }
        // //////////////////////////
        // if (from_PlayerRotation_X)
        // {
        //     if (playerCamera != null)
        //     {
        //         UpdateValues += GetPlayerRotation_X;
        //     }
        // }
        // if (from_PlayerRotation_Y)
        // {
        //     if (playerCamera != null)
        //     {
        //         UpdateValues += GetPlayerRotation_Y;
        //     }
        // }
        // if (from_PlayerRotation_Z)
        // {
        //     if (playerCamera != null)
        //     {
        //         UpdateValues += GetPlayerRotation_Z;
        //     }
        // }
        // //////////////////////////
        // if (from_PlayerDistance)
        // {
        //     if (playerCamera != null && playerDistanceTarget != null)
        //     {
        //         UpdateValues += GetPlayerDistance;
        //     }
        //     else
        //     {
        //         Debug.Log("AnimationEffect: from player distance selected but player distance target input empty");
        //     }
        // }
        // if (from_PlayerHandDistance)
        // {
        //     if (playerCamera != null && playerHandDistanceTarget != null)
        //     {
        //         UpdateValues += GetPlayerHandDistance;
        //     }
        //     else
        //     {
        //         Debug.Log("AnimationEffect: from player distance selected but player distance target input empty");
        //     }
        // }
        
        // //////////////////////////
        // if (from_PlayerTouch)
        // {
        //     if (Engage.ScriptHelper.using_vr && ENG_TrackedMotionControllers.instance != null)
        //     {
        //         UpdateValues += GetPlayerTouchedVR;
        //     }
        //     else
        //     {
        //         UpdateValues += GetPlayerTouchedMouse;
        //     }
        // }
        // //////////////////////////
        // if (from_PlayerCollision)
        // {
        //     if (playerCamera != null)
        //     {
        //         UpdateValues += GetPlayerCollision;
        //     }
        // }
    }

    // Update is called once per frame
    float timer = 0;
    void Update()
    {
        //Check if the player has loaded in yet. if not restart. There is probobly a better way to do this than checking every frame.
        //only an issue when the script is used in a scene, since the scene loades before the player.
        if (playerCamera == null )
        {
            Initialize();
            Debug.Log("AnimationEffect Send: Player Not found Restarting");
            return;
        }

        if (updateRate > 0f)
        {
            timer += Time.deltaTime;
            if(timer>updateRate)
            {
                float output = UpdateValues();
                floatVariableOUT.Value = output;
                timer = 0;
            }    
        }
        else
        {
            float output = UpdateValues();
            floatVariableOUT.Value = output;
        }

        
        
    }

    

    private float GetDistance() 
    {

        float output = Vector3.Distance(distanceTarget1.position,distanceTarget2.position);
        return output;
    }
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
    private float GetIncreaseBy() 
    {

        float output = floatVariableOUT.GetMathOutput() + increment;
        return output;
    }
    private float GetIncreaseByInput() 
    {

        float output = floatVariableOUT.GetMathOutput() + incrementInput.GetMathOutput();
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

    private float GetAudioSpectrum() 
    {
        float[] spectrum = new float[8192];
        
        audioIn.GetSpectrumData(spectrum, 1, FFTWindow.Rectangular);
        float spectrumAverage = spectrum.Average();
        float output = spectrumAverage*8192;
        return output;
    }
    private float GetAudioVolume() 
    {
       
        float output = audioIn.volume;
        return output;
    }
    private float GetAudioPitch() 
    {
        
        float output = audioIn.pitch;
        return output;
    }
    
    ////////////////////////////////////////
    private float GetPostition_X() 
    {
        float output = input_Transform.localPosition.x;
        return output;
    }
    private float GetPostition_Y() 
    {
        float output = input_Transform.localPosition.y;
        return output;
    }
    private float GetPostition_Z() 
    {
        float output = input_Transform.localPosition.z;
        return output;
    }

    /////////////////////////////////
    private float GetRotation_X() 
    {
        float output = input_Transform.localEulerAngles.x;
        return output;
    }
    private float GetRotation_Y() 
    {
        float output = input_Transform.localEulerAngles.y;
        return output;
    }
    private float GetRotation_Z() 
    {
        float output = input_Transform.localEulerAngles.z;
        return output;
    }
    //////////////////////////////////
    private float GetScale_X() 
    {
        float output = input_Transform.localScale.x;
        return output;
    }
    private float GetScale_Y() 
    {
        float output = input_Transform.localScale.y;
        return output;
    }
    private float GetScale_Z() 
    {
        float output = input_Transform.localScale.z;
        return output;
    }

    ////////////////////////////////////////
    // private float GetPlayerPostition_X() 
    // {
    //     float output = playerCamera.transform.position.x;
    //     return output;
    // }
    // private float GetPlayerPostition_Y() 
    // {
    //     float output = playerCamera.transform.position.y;
    //     return output;
    // }
    // private float GetPlayerPostition_Z() 
    // {
    //     float output = playerCamera.transform.position.z;
    //     return output;
    // }

    // private float GetPlayerRotation_X() 
    // {
    //     float output = playerCamera.transform.rotation.x;
    //     return output;
    // }
    // private float GetPlayerRotation_Y() 
    // {
    //     float output = playerCamera.transform.rotation.y;
    //     return output;
    // }
    // private float GetPlayerRotation_Z() 
    // {
    //     float output = playerCamera.transform.rotation.z;
    //     return output;
    // }

    // ////////////////////////////////////////
    // private float GetPlayerDistance() 
    // {
    //     float output = Vector3.Distance(playerDistanceTarget.position, playerCamera.transform.position);
    //     return output;
    // }
    // private float GetPlayerHandDistance() 
    // {
    //     int primaryHand = ENG_TrackedMotionControllers.instance.primaryHand;
    //     int secondaryHand = ENG_TrackedMotionControllers.instance.secondaryHand;
    //     Transform  primaryhandtransform;
    //     Transform  Secondaryhandtransform;
    //     float distanceToPrimaryHand = 100;
    //     float distanceToSecondaryHand = 100;

    //     if (ENG_TrackedMotionControllers.instance.controllerTransform[primaryHand] !=null)
    //     {
    //         primaryhandtransform = ENG_TrackedMotionControllers.instance.controllerTransform[primaryHand];
    //         distanceToPrimaryHand = Vector3.Distance(primaryhandtransform.position, playerHandDistanceTarget.position);
    //     }
    //     else
    //     {
    //         Debug.Log("AnimationEffect Send: Primary hand controllers not found, can't get distance");
    //     }

    //     if (ENG_TrackedMotionControllers.instance.controllerTransform[secondaryHand] !=null)
    //     {
    //         Secondaryhandtransform = ENG_TrackedMotionControllers.instance.controllerTransform[secondaryHand];
    //         distanceToSecondaryHand = Vector3.Distance(Secondaryhandtransform.position, playerHandDistanceTarget.position);
    //     }
    //     else
    //     {
    //         Debug.Log("AnimationEffect Send: Secondary hand controllers not found, can't get distance");
    //     }

        
    //     return Mathf.Min(distanceToPrimaryHand,distanceToSecondaryHand);
        
        
        
    // }
    ////////////////////////////////////////
    private float GetCollision() 
    {
        if(collider1.bounds.Intersects(collider2.bounds))
        {
            return 1.0f;
        }
        return 0.0f;
    }
    

    // private float GetPlayerTouchedVR() 
    // {
    //     Transform primaryhandtransform = ENG_TrackedMotionControllers.instance.controllerTransform[ENG_TrackedMotionControllers.instance.primaryHand];
    //     Transform Secondaryhandtransform = ENG_TrackedMotionControllers.instance.controllerTransform[ENG_TrackedMotionControllers.instance.secondaryHand];
    //     if (playerTouchCollider.bounds.Contains(primaryhandtransform.position))
    //     {
           
    //         if(requireTriggerPress)
    //         {
    //             if (ENG_TrackedMotionControllers.instance.triggerPressed[ENG_TrackedMotionControllers.instance.primaryHand])
    //             {
    //                 return 1.0f;
    //             }
    //         }
    //         if (requireGrippedPress)
    //         {
    //             if ( ENG_TrackedMotionControllers.instance.gripped[ENG_TrackedMotionControllers.instance.primaryHand])
    //             {
    //                return 1.0f; 
    //             }
                
    //         }
    //         if (!requireGrippedPress && !requireTriggerPress)
    //         {
    //             return 1.0f;
    //         }
    //     }
    //     if (playerTouchCollider.bounds.Contains(Secondaryhandtransform.position))
    //     {
    //         if(requireTriggerPress)
    //         {
    //             if (ENG_TrackedMotionControllers.instance.triggerPressed[ENG_TrackedMotionControllers.instance.secondaryHand])
    //             {
    //                 return 1.0f;
    //             }
    //         }
    //         if (requireGrippedPress)
    //         {
    //             if ( ENG_TrackedMotionControllers.instance.gripped[ENG_TrackedMotionControllers.instance.secondaryHand])
    //             {
    //                return 1.0f; 
    //             }
                
    //         }
    //         if (!requireGrippedPress && !requireTriggerPress)
    //         {
    //             return 1.0f;
    //         }
    //     }
    //     return 0.0f;
    // }
    // private float GetPlayerTouchedMouse() 
    // {

    //     //Ray interaction_ray = Engage.ScriptHelper.mainCamera.ScreenPointToRay(Engage.ScriptHelper.mainCamera.WorldToScreenPoint(ENG_TrackedMotionControllers.instance.GetWorldGUIPointerTransform().position));
    //     Ray interaction_ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
    //     RaycastHit hit;
    //     if (Input.GetMouseButton(0))
    //     {
    //         Debug.DrawRay(interaction_ray.origin, interaction_ray.direction,  Color.green,  500.0f, true);
    //         if (playerTouchCollider.Raycast(interaction_ray,out hit, 10.0f))
    //         {
    //             return 1.0f;
    //         }
    //     }
    //     return 0.0f;
    // }
    // private float GetPlayerCollision() 
    // {
        
    //     if(playerCollisionTarget.bounds.Intersects(ENG_IGM_PlayerManager.instance.myPlayerObject.playerObject.GetComponent<Collider>().bounds))
    //     {
    //         return 1.0f;
    //     }
    //     return 0.0f;
    // }
}
