using UnityEngine;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif
using Math = System.Math;
// Uncomment the line below and enter how you want this module to show in the component menu
[AddComponentMenu("IFXAnimEffect_SEND/PerformanceProfiler - IFX SEND Module")]
[RequireComponent(typeof(IFXAnimEffect_SEND_MAIN))]
public class IFXAnimEffect_SEND_PerformanceProfile_Module : IFXAnimEffect_SEND_Module
{
    // Used In The Editor to show what type of module this is 
    #if UNITY_EDITOR
    public override string moduleType {get{ return "PerformanceProfile";} }
    #endif
    delegate float UpdateValuesDelegate();
    UpdateValuesDelegate UpdateValues;

    //////////////////////////////////
    [SerializeField]
    bool averageFps;
    [SerializeField]
    bool currentFps;
    [SerializeField]
    bool allocatedRam;
    [SerializeField]
    bool reservedRam;
    //////////////////////////////////
    [SerializeField]
    [Header("Get System Info -----------------------------")]
    bool totalMemory;
    [SerializeField]
    bool totalGraphicsMemory;
    [SerializeField]
    bool coreCount;

    
    [Tooltip("0=null, 1=win, 2=Android, 3=Mac, 4=Iphone")]
    [SerializeField]
    bool SystemType;
    ////////////////////////
    float updateInterval = 5F;
    double lastInterval;
    int frames;
    float fps;

    //SEND MODULES SHOULD ONLY SEND ONE VALUE!
    //You can use if statments and the UpdateValues delegate to choose the appropriate update method though

    //////////////////////////////////

    private void OnEnable()
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
        //Add method to UpdateValues delagate here. For example
        if (averageFps)
        {
            UpdateValues += GetAverageFPS;
        }
        if (currentFps)
        {
            UpdateValues += GetCurrentFPS;
        }
        if (allocatedRam)
        {
            UpdateValues += GetAllocatedRam;
        }
        if (reservedRam)
        {
            UpdateValues += GetAllocatedRam;
        }
        if (totalMemory)
        {
            UpdateValues += GetTotalSystemRam;
        }
        if (totalGraphicsMemory)
        {
            UpdateValues += GetTotalGraphicsRam;
        }
        if (coreCount)
        {
            UpdateValues += GetCoreCount;
        }
        if (SystemType)
        {
            UpdateValues += GetSystemType;
        }
        
        
    }
    //This method gets called by SEND_Main to retrive to value from the delegate. Only one method should be returning values.
    public override void SendOutput()
    {
       AnimationEffectVariable.Value = UpdateValues();
    }
    ///////////////////////////////////////////////

    
    private float GetAverageFPS() 
    {
        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval)
        {
            fps = (float)(frames / (timeNow - lastInterval));
            frames = 0;
            lastInterval = timeNow;
        }
        
        return (float)Math.Round(fps);
    }
    private float GetCurrentFPS() 
    {
        float output = (float)Math.Round((1f / Time.unscaledDeltaTime));
        return output;
    }
    private float GetAllocatedRam() 
    {
        float output = (float)Math.Round(Profiler.GetTotalAllocatedMemoryLong()/ 1048576f);;
        return output;
    }
     private float GetReservedRam() 
    {
        float output = (float)Math.Round(Profiler.GetTotalReservedMemoryLong() / 1048576f);
        return output;
    }
    private float GetTotalSystemRam() 
    {
        float output = SystemInfo.systemMemorySize;
        return output;
    }
    private float GetTotalGraphicsRam() 
    {
        float output = SystemInfo.graphicsMemorySize;
        return output;
    }
    private float GetCoreCount() 
    {
        float output = SystemInfo.processorCount;
        return output;
    }

    private float GetSystemType() 
    {

        float output=0;
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            output=1;
        }
        if (Application.platform == RuntimePlatform.Android)
        {
            output=2;
        }
        if (Application.platform == RuntimePlatform.OSXPlayer)
        {
            output=3;
        }
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            output=4;
        }
        return output;
    }

    
}
