using UnityEngine;
using System.Collections;
using Engage.Avatars.Poses;

public class LVR_SitTrigger : MonoBehaviour {
	public Transform floorCollider;
	public float lowerHeadAmount = 0.6f;
    public bool lockedInSeat = false;
    public bool movingEffect = false;
    public Vector3 m_seatPosition = new Vector3(0f, 0.57f, 0.15f);
    public Vector3 m_leftFootPos = new Vector3(-0.1f, 0.11f, 0.23f);
    public Vector3 m_rightFootPos = new Vector3(0.1f, 0.11f, 0.23f);

    [SerializeField]
    private Transform _pelvisTarget;

    public Transform PelvisTarget
    {
        get
        {
            if (_pelvisTarget == null)
            {
                _pelvisTarget = new GameObject("Root").transform;
                _pelvisTarget.SetParent(floorCollider.transform); _pelvisTarget.localRotation = Quaternion.identity;
            }
            //Many seats are oddly scaled, so we use transformDirection to ensure real values are used.
            _pelvisTarget.position = floorCollider.position + floorCollider.TransformDirection(m_seatPosition);
            return _pelvisTarget;
        }
    }

    [SerializeField]
    private PoseTrigger m_advancedPose;
    public PoseTrigger AdvancedPose { get { return m_advancedPose; } }
    public bool HasAdvancedPose { get { return m_advancedPose != null; } }

    public PoseTrigger CreateAdvancedPose()
    {
        if (!HasAdvancedPose)
            m_advancedPose = gameObject.AddComponent<PoseTrigger>();

        m_advancedPose.InitialiseFromSitTrigger(this);

        return m_advancedPose;
    }
}
