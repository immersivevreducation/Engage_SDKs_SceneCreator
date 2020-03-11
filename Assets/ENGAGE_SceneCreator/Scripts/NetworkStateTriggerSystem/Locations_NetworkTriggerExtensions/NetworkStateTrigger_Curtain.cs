using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LVR_Location_NetworkState))]
public class NetworkStateTrigger_Curtain : MonoBehaviour {

    private bool closeCurtain = false;

    /// <summary>
    /// Curtain Variables
    /// </summary>
    public Transform curtainStartMarkerL;
    public Transform curtainEndMarkerL;
    public Transform curtainStartMarkerR;
    public Transform curtainEndMarkerR;
    public GameObject leftCurtain;
    public GameObject rightCurtain;
    private Vector3 currentPositionLeftCurtain;
    private Vector3 currentPositionRightCurtain;
    public float curtainSpeed = 1.0f;
    private bool curtainOpened = false;
    private float journeyLength;
    private float distCovered;

    private LVR_Location_NetworkState m_state;

    /*
     * 0 = Open
     * 1 = Close
     */
    public int networkedState = 0;

    void Start () {
        m_state = GetComponent<LVR_Location_NetworkState>();
        journeyLength = Vector3.Distance(curtainStartMarkerL.position, curtainEndMarkerL.position);
    }

    public void CloseCurtains(int close)
    {
        m_state.UpdateState(close);
        Debug.Log("Button Pressed");
        Debug.Log("M_State: " + m_state.currentState);
    }

    void FixedUpdate()
    {
        CurtainFixedUpdate();
    }

    void CurtainFixedUpdate()
    {
        if (Time.timeSinceLevelLoad > 1)
        {
            if (m_state.currentState == 0)
            {
                closeCurtain = false;
            }

            else if (m_state.currentState == 1)
            {
                closeCurtain = true;
            }

            if (!closeCurtain) // Open curtains
            {
                if (!curtainOpened)
                {
                    distCovered = 0f;
                    curtainOpened = true;
                }
                if (distCovered < 6)
                {
                    distCovered = distCovered + 0.05f;
                }

                float fracJourney = (distCovered / journeyLength) * Time.deltaTime;

                currentPositionLeftCurtain = leftCurtain.transform.position;
                currentPositionRightCurtain = rightCurtain.transform.position;

                leftCurtain.transform.position = Vector3.Lerp(currentPositionLeftCurtain, curtainStartMarkerL.position, fracJourney);
                rightCurtain.transform.position = Vector3.Lerp(currentPositionRightCurtain, curtainStartMarkerR.position, fracJourney);
            }

            if (closeCurtain) // Close curtains
            {
                if (curtainOpened)
                {
                    distCovered = 0f;
                    curtainOpened = false;
                }
                if (distCovered < 6)
                {
                    distCovered = distCovered + 0.05f;
                }

                float fracJourney = (distCovered / journeyLength) * Time.deltaTime;

                currentPositionLeftCurtain = leftCurtain.transform.position;
                currentPositionRightCurtain = rightCurtain.transform.position;

                leftCurtain.transform.position = Vector3.Lerp(currentPositionLeftCurtain, curtainEndMarkerL.position, fracJourney);
                rightCurtain.transform.position = Vector3.Lerp(currentPositionRightCurtain, curtainEndMarkerR.position, fracJourney);
            }
        }
        else
        {
            if (closeCurtain)
            {
                curtainOpened = false;
                leftCurtain.transform.position = curtainEndMarkerL.position;
                rightCurtain.transform.position = curtainEndMarkerR.position;
            }
            else
            {
                curtainOpened = true;
                leftCurtain.transform.position = curtainStartMarkerL.position;
                rightCurtain.transform.position = curtainStartMarkerR.position;
            }
        }
    }
}
