using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(MeshRenderer))]
public class URLBasedImageDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Behaviour Settings")]
    public bool loadOnlyOnStart;
    public string url;

    [Header("Object References")]
    public GameObject uiObject;
    public URLInputField inputField;
    public Transform _displayTransform;
    private float _textureWidth;
    private float _textureHeight;
    private Texture _texture;
    private MeshRenderer[] _renderers;
    private Material _displayMaterial;


    #region Unity Events
    private void OnEnable()
    {
        _renderers = GetComponentsInChildren<MeshRenderer>();
        inputField.DisableInput();
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("On pointer enter");
        inputField.EnableInput();
    }

    private void Start()
    {
        if (loadOnlyOnStart)
        {
            DownloadImage(url);
            uiObject.SetActive(false);
        }
    }
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("OnPointerExit");
        DownloadImage(inputField.GetText());
        inputField.DisableInput();
    }
    #endregion

    public void DownloadImage(string imgUrl)
    {
        StartCoroutine(CR_GetTextureRequest(imgUrl, (texture) =>
        {
            _texture = texture;
            DisplayImageOnTexture();
        }));
    }

    private IEnumerator CR_GetTextureRequest(string imgUrl, System.Action<Texture> callback)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imgUrl))
        {

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.LogError("URLBasedDownloader @ " + DateTime.Now + " :: " + imgUrl + " :: Error ::" + www.error);
            }
            else
            {
                if (www.isDone)
                {
                    Texture texture = DownloadHandlerTexture.GetContent(www);
                    _textureWidth = texture.width;
                    _textureHeight = texture.height;
#if UNITY_ANDROID
                    texture = texture.Compress(false);
#endif
                    callback(texture);
                }
            }


        }

    }

    private void DisplayImageOnTexture()
    {
        _displayMaterial = CreateNewMaterial();
        _displayMaterial.mainTexture = _texture;

        SetRendererMaterials();
        ResizeDisplayPlanes();
    }

    private Material CreateNewMaterial()
    {
        if (_displayMaterial != null)
        {
            Destroy(_displayMaterial);
        }

        return new Material(Shader.Find("Diffuse"));
    }

    private void ResizeDisplayPlanes()
    {
        _displayTransform.localScale = Vector3.one;


        float height = _textureHeight > _textureWidth ? _textureHeight / _textureWidth : 1;
        float width = _textureWidth > _textureHeight ? _textureWidth / _textureHeight : 1;

        _displayTransform.localScale = new Vector3(width, height, 1);
    }

    private void SetRendererMaterials()
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].material = _displayMaterial;
        }
    }
}
