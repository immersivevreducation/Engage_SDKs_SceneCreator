using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Individual script attached to any Renderers or Raw Images that are to copy
/// the main video screen's material from WebHelper.
/// </summary>
public class VideoMaterialInstance : MonoBehaviour {
    Renderer renderer;
    RawImage rawImage;
    /*
    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        rawImage = GetComponent<RawImage>();
    }
    void Update(){
        if (renderer)
            renderer.material = WebHelper.screenMaterial;
        if (rawImage) {
            rawImage.material = WebHelper.screenMaterial;
            rawImage.texture = WebHelper.screenMaterial.mainTexture;
        }

        
	}*/
}
