using UnityEngine;
using System;
[AddComponentMenu("IFXAnimEffect_RECEIVE/Shader - IFX RECEIVE Module")]
[RequireComponent(typeof(IFXAnimEffect_RECEIVE_MAIN))]
public class IFXAnimEffect_RECEIVE_Shader_Module : IFXAnimEffect_RECEIVE_Module
{
    #if UNITY_EDITOR
    public override string moduleType {get{ return "Shader";} }
    #endif
    [SerializeField]
    Material materialIn;
    [SerializeField]
    GameObject gameObjectIN;

    [SerializeField]
    string SetColor;
    [SerializeField]
    bool setColor_R;
    [SerializeField]
    bool setColor_G;
    [SerializeField]
    bool setColor_B;
    [SerializeField]
    bool setColor_A;
    [SerializeField]
    string texturePropertyName;

    [SerializeField]
    bool textureOffset_U;
    [SerializeField]
    bool textureOffset_V;
    [SerializeField]
    bool textureScale_U;
    [SerializeField]
    bool textureScale_V;

    [SerializeField]
    bool textureSwapOnTrigger;
    [SerializeField]
    Texture altTexture;
    Texture oldTexture;


    [SerializeField]
    string floatPropertyName;
    [SerializeField]
    string intPropertyName;
    [SerializeField]
    string vectorPropertyName;
    [SerializeField]
    bool vector_X;
    [SerializeField]
    bool vector_Y;
    [SerializeField]
    bool vector_Z;
    [SerializeField]
    bool vector_W;
    

    

    //////////////////////////////////
    //This is an example of an option you could have
    // [SerializeField]
    // bool useWorldSpace;
    // [SerializeField]
    // bool from_Positon_X;

    //RECEIVE can use the same value to effect multiple things. For example the value coming in could be used to move the object on the x axis and the y axis at the same time.
    //You should not have another receive module also effect the same value though, for example two RECEIVES both trying to tranlate on the x axis


    //You can use if statments and the "InputBoolAction" & "InputFloatAction" delegates to choose the appropriate update method or methods you want the value fed to.

    //////////////////////////////////

    private void OnEnable()
    {
        if (gameObjectIN !=null)
        {
            materialIn = gameObjectIN.GetComponent<Renderer>().material;
        }

        if (!string.IsNullOrEmpty(SetColor))
        {
            if (setColor_R)
            {
                this.InputFloatAction += SetMaterialColor_R;
            }
            if (setColor_G)
            {
                this.InputFloatAction += SetMaterialColor_G;
            }
            if (setColor_B)
            {
                this.InputFloatAction += SetMaterialColor_B;
            }
            if (setColor_A)
            {
                this.InputFloatAction += SetMaterialColor_A;
            }
        }






        if (!string.IsNullOrEmpty(texturePropertyName))
        {
            if (materialIn.HasProperty(texturePropertyName))
            {
                if (textureOffset_U)
                {
                    this.InputFloatAction += SetTextureOffset_U;
                }
                if (textureOffset_V)
                {
                    this.InputFloatAction += SetTextureOffset_V;
                }
                if (textureScale_U)
                {
                    this.InputFloatAction += SetTextureScale_U;
                }
                if (textureScale_V)
                {
                    this.InputFloatAction += SetTextureScale_V;
                }
                if (textureSwapOnTrigger)
                {
                    oldTexture=materialIn.GetTexture(texturePropertyName);
                    this.InputBoolAction += TextureSwap;
                }
            }
            else
            {
                Debug.Log("IFXAnimEffect_RECEIVE_Material_Module: Shader Texture Property not found: "+texturePropertyName);
            }   
        }

        if (!string.IsNullOrEmpty(floatPropertyName))
        {
            if (materialIn.HasProperty(floatPropertyName))
            {
                this.InputFloatAction += SetFloatValue;
            }
            else
            {
                Debug.Log("IFXAnimEffect_RECEIVE_Material_Module: Shader Property not found: "+floatPropertyName);
            }            
        }
        if (!string.IsNullOrEmpty(intPropertyName))
        {
            if (materialIn.HasProperty(intPropertyName))
            {
                this.InputFloatAction += SetIntValue;
            }
            else
            {
                Debug.Log("IFXAnimEffect_RECEIVE_Material_Module: Shader Property not found: "+intPropertyName);
            }            
        }
        if (!string.IsNullOrEmpty(vectorPropertyName))
        {
            if (materialIn.HasProperty(vectorPropertyName))
            {
                if (vector_X)
                {
                    this.InputFloatAction += SetVector_X;
                }
                if (vector_Y)
                {
                    this.InputFloatAction += SetVector_Y;
                }
                if (vector_Z)
                {
                    this.InputFloatAction += SetVector_Z;
                }
                if (vector_W)
                {
                    this.InputFloatAction += SetVector_W;
                }                
            }
            else
            {
                Debug.Log("IFXAnimEffect_RECEIVE_Material_Module: Shader Property not found: "+vectorPropertyName);
            }            
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

    public void SetMaterialColor_R(float input)
    {       
        Color colorIN = materialIn.GetColor(SetColor);
        colorIN.r = input;
        materialIn.SetColor(SetColor, colorIN);      
    }
    public void SetMaterialColor_G(float input)
    {       
        Color colorIN = materialIn.GetColor(SetColor);
        colorIN.g = input;
        materialIn.SetColor(SetColor, colorIN);      
    }
    public void SetMaterialColor_B(float input)
    {       
        Color colorIN = materialIn.GetColor(SetColor);
        colorIN.b = input;
        materialIn.SetColor(SetColor, colorIN);      
    }
    public void SetMaterialColor_A(float input)
    {       
        Color colorIN = materialIn.GetColor(SetColor);
        colorIN.a = input;
        materialIn.SetColor(SetColor, colorIN);      
    }
    /////////////////
    public void SetTextureOffset_U(float input)
    {       
       Vector2 currentOffset =materialIn.GetTextureOffset(texturePropertyName);
       currentOffset.x = input;
        materialIn.SetTextureOffset(texturePropertyName, currentOffset);  
    }
    public void SetTextureOffset_V(float input)
    {       
       Vector2 currentOffset =materialIn.GetTextureOffset(texturePropertyName);
       currentOffset.y = input;
        materialIn.SetTextureOffset(texturePropertyName, currentOffset);  
    }
    public void SetTextureScale_U(float input)
    {       
       Vector2 currentOffset =materialIn.GetTextureScale(texturePropertyName);
       currentOffset.x = input;
        materialIn.SetTextureScale(texturePropertyName, currentOffset);  
    }
    public void SetTextureScale_V(float input)
    {       
       Vector2 currentOffset =materialIn.GetTextureScale(texturePropertyName);
       currentOffset.y = input;
        materialIn.SetTextureScale(texturePropertyName, currentOffset);  
    }

    public void SetFloatValue(float input)
    {
        materialIn.SetFloat(floatPropertyName, input);  
    }
    public void SetIntValue(float input)
    {
        materialIn.SetInt(intPropertyName, (int)Math.Round(input));  
    }
    public void SetVector_X(float input)
    {
        Vector4 currentOffset =materialIn.GetVector(vectorPropertyName);
        currentOffset.x = input;
        materialIn.SetVector(vectorPropertyName, currentOffset);
    }
    public void SetVector_Y(float input)
    {
        Vector4 currentOffset =materialIn.GetVector(vectorPropertyName);
        currentOffset.y = input;
        materialIn.SetVector(vectorPropertyName, currentOffset);
    }
    public void SetVector_Z(float input)
    {
        Vector4 currentOffset =materialIn.GetVector(vectorPropertyName);
        currentOffset.z = input;
        materialIn.SetVector(vectorPropertyName, currentOffset);
    }
    public void SetVector_W(float input)
    {
        Vector4 currentOffset =materialIn.GetVector(vectorPropertyName);
        currentOffset.w = input;
        materialIn.SetVector(vectorPropertyName, currentOffset);
    }

    public void TextureSwap(bool input)
    {
        if (input)
        {
            materialIn.SetTexture (texturePropertyName, altTexture);
        }
        else
        {
            materialIn.SetTexture (texturePropertyName, oldTexture);
        }        
    }
    
}
