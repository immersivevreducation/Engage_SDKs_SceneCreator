using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostOrNonHostGameObjectSwitch : MonoBehaviour
{
    public GameObject[] host_OnlyObjects;
    public GameObject[] non_HostObjects;

    float timeDelay = 0.5f;
    float timeCounter = 0;

#if UNITY_ENGAGE
    private void Update()
    {
        if (timeCounter < timeDelay)
        {
            timeCounter += Time.deltaTime;
            return;
        }

        timeCounter = 0;

        if (Engage_NetworkController.instance.isSessionHost)
        {
            foreach (GameObject obj in host_OnlyObjects)
                if (obj != null)
                    if(!obj.activeSelf)
                        obj.SetActive(true);
            foreach (GameObject obj in non_HostObjects)
                if (obj != null)
                    if (obj.activeSelf)
                        obj.SetActive(false);
        }
        else
        {
            foreach (GameObject obj in non_HostObjects)
                if (obj != null)
                    if (!obj.activeSelf)
                        obj.SetActive(true);
            foreach (GameObject obj in host_OnlyObjects)
                if (obj != null)
                    if (obj.activeSelf)
                        obj.SetActive(false);
        }
    }
#endif
}
