using UnityEngine;
using System.Linq;
[AddComponentMenu("IFXAnimEffect_SEND/Audio - IFX SEND Module")]
[RequireComponent(typeof(IFXAnimEffect_SEND_MAIN))]
public class IFXAnimEffect_SEND_Audio_Module : IFXAnimEffect_SEND_Module
{

    #if UNITY_EDITOR
    public override string moduleType {get{ return "Audio";} }
    #endif
    delegate float UpdateValuesDelegate();
    UpdateValuesDelegate UpdateValues;

    AudioSource audioIn;

    //////////////////////////////////
    [SerializeField]
    bool from_AudioSpectrum;
    [SerializeField]
    bool from_AudioVolume;
    [SerializeField]
    bool from_AudioPitch;
    //////////////////////////////////

    private void OnEnable()
    {
        if (audioIn != null)
        {
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
        }
        
        
    }
    //This method gets called by SEND_Main to retrive to value from the delegate. Only one method should be returning values.
    public override void SendOutput()
    {
       AnimationEffectVariable.Value = UpdateValues();
    }
    ///////////////////////////////////////////////

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
    
}
