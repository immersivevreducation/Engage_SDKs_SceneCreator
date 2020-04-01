using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class URLInputField : MonoBehaviour
{
    private InputField _inputField;
    private Text[] _textDisplays;

    private void OnEnable()
    {
        _textDisplays = GetComponentsInChildren<Text>();
        _inputField = GetComponent<InputField>();
        ToggleTextDisplays(false);
        ToggleInputField(false);
    }
    public void EnableInput()
    {
        ToggleTextDisplays(true);
        ToggleInputField(true);
    }

    public void DisableInput()
    {
        ToggleTextDisplays(false);
        ToggleInputField(false);
    }

    private void ToggleTextDisplays(bool active)
    {
        for (int i = 0; i < _textDisplays.Length; i++)
        {
            if (active)
            {
                _textDisplays[i].gameObject.SetActive(true);
                _textDisplays[i].CrossFadeAlpha(1, 1.5f, true);
            }
            else
            {
                _textDisplays[i].CrossFadeAlpha(0, 1.5f, true);
                _textDisplays[i].gameObject.SetActive(false);
            }
        }
    }

    private void ToggleInputField(bool active)
    {
        _inputField.interactable = active;

        if (active)
        {
            _inputField.image.CrossFadeAlpha(1, 1.5f, true);
        }
        else
        {
            _inputField.image.CrossFadeAlpha(0, 1.5f, true);
        }

    }

    public string GetText()
    {
        return _inputField.text;
    }
}
