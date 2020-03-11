using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkState_ButtonAutoClickTester : MonoBehaviour {

    public bool PressButton = false;
    private Button m_Button;
	void Start () {
		if (GetComponent<Button>() != null)
        {
            Debug.Log("Has Button");
            m_Button = GetComponent<Button>();
        }

        else
        {
            Debug.Log("NoButton");
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (PressButton)
        {
            m_Button.onClick.Invoke();
        }
        PressButton = false;
	}
}
