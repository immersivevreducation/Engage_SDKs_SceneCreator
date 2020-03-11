using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PerfectSeatName : MonoBehaviour
{
    //public GameObject[] objects;

    public List<GameObject> objects;

    public int startNum = 2;
    public string nameToBe = "PerfectSeat";

    public bool startProcess = false;
    public bool renableObjects = false;

    public bool disableObjectAfter = false;
    public bool updateStartNumAfter = false;
    public bool removeFromListAfter = false;

    private int m_objectNum = 0;
    public List<GameObject> m_obsToEnable;
    private GameObject m_tempObj;

    void Update()
    {
        if (startProcess)
        {
            m_objectNum = startNum;

            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].GetComponentInChildren<LVR_SitTrigger>())
                {
                    objects[i].GetComponentInChildren<LVR_SitTrigger>().gameObject.name = nameToBe + m_objectNum;
                }

                if (disableObjectAfter)
                {
                    objects[i].SetActive(false);
                    m_obsToEnable.Add(objects[i]);
                }

                m_objectNum++;
            }

            if (updateStartNumAfter)
                startNum = m_objectNum;

            if (removeFromListAfter)
                objects.Clear();

            m_objectNum = 0;

            startProcess = false;
        }

        if (renableObjects)
        {
            foreach (GameObject go in m_obsToEnable)
            {
                go.SetActive(true);
            }
            m_obsToEnable.Clear();

            renableObjects = false;
        }
    }
}
