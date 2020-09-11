using UnityEngine;
using System.Collections;

public class LVR_SitTrigger : MonoBehaviour {
	public Transform floorCollider;
	public float lowerHeadAmount = 0.6f;
    public bool lockedInSeat = false;
    public bool movingEffect = false;
    public Vector3 m_seatPosition = new Vector3(0f, 0.57f, 0.15f);
    public Vector3 m_leftFootPos = new Vector3(-0.1f, 0.11f, 0.23f);
    public Vector3 m_rightFootPos = new Vector3(0.1f, 0.11f, 0.23f);
}
