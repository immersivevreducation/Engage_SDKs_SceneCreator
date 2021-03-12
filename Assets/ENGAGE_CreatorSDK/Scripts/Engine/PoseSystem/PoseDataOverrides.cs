using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engage.Avatars.Poses
{
    [System.Serializable]
    public class PoseOverridesGroup
    {
        [SerializeField]
        private PoseType m_type;
        [SerializeField]
        private PoseArchetype m_archetype;

        public PoseType Type { get { return m_type; } }
        public PoseArchetype Archetype { get { return m_archetype; } }

        [SerializeField]
        private List<PoseOverrides> m_poseOverrides;

        public List<PoseOverrides> OverrideList { get { return m_poseOverrides; } }

        public PoseOverridesGroup(PoseType type, PoseArchetype archetype)
        {
            m_type = type;
            m_archetype = archetype;
            m_poseOverrides = new List<PoseOverrides>(1);
        }

        public PoseOverridesGroup(PoseType type, PoseArchetype archetype, PoseConstraintData[] constraints)
        {
            m_type = type;
            m_archetype = archetype;
            m_poseOverrides = new List<PoseOverrides>(1);
        }

        public void AddOverrides(PoseOverrides overrides)
        {
            if (m_poseOverrides == null)
                m_poseOverrides = new List<PoseOverrides>(1);

            m_poseOverrides.Add(overrides);
        }

        public void RemoveOverridesAt(int id)
        {
            if (m_poseOverrides == null)
                m_poseOverrides = new List<PoseOverrides>(1);

            m_poseOverrides.RemoveAt(id);
        }

        public void SetPoseOverrides(PoseOverrides overrides, int id)
        {
            if (m_poseOverrides == null)
                m_poseOverrides = new List<PoseOverrides>(1);

            m_poseOverrides[id] = overrides;
        }
    }

    [System.Serializable]
    public class PoseOverrides
    {
        public Dictionary<PoseBodyPart, PoseMapping> Overrides 
        { 
            get 
            {
                Dictionary<PoseBodyPart, PoseMapping> overrides = new Dictionary<PoseBodyPart, PoseMapping>();

                if (m_overrideHead) overrides.Add(PoseBodyPart.HEAD, m_headOverride);
                if (m_overrideChest) overrides.Add(PoseBodyPart.CHEST, m_chestOverride);
                if (m_overridePelvis) overrides.Add(PoseBodyPart.PELVIS, m_pelvisOverride);

                if (m_overrideRightHand) overrides.Add(PoseBodyPart.RIGHT_HAND, m_rightHandOverride);
                if (m_overrideLeftHand) overrides.Add(PoseBodyPart.LEFT_HAND, m_leftHandOverride);
                if (m_overrideRightElbow) overrides.Add(PoseBodyPart.RIGHT_ELBOW, m_rightElbowOverride);
                if (m_overrideLeftElbow) overrides.Add(PoseBodyPart.LEFT_ELBOW, m_leftElbowOverride);

                if (m_overrideRightFoot) overrides.Add(PoseBodyPart.RIGHT_FOOT, m_rightFootOverride);
                if (m_overrideLeftFoot) overrides.Add(PoseBodyPart.LEFT_FOOT, m_leftFootOverride);
                if (m_overrideRightKnee) overrides.Add(PoseBodyPart.RIGHT_KNEE, m_rightKneeOverride);
                if (m_overrideLeftKnee) overrides.Add(PoseBodyPart.LEFT_KNEE, m_leftKneeOverride);

                return overrides;
            } 
        }

        [SerializeField]
        private PoseMapping m_headOverride;
        [SerializeField]
        private PoseMapping m_chestOverride;
        [SerializeField]
        private PoseMapping m_pelvisOverride;

        [SerializeField]
        private PoseMapping m_rightHandOverride;
        [SerializeField]
        private PoseMapping m_leftHandOverride;
        [SerializeField]
        private PoseMapping m_rightElbowOverride;
        [SerializeField]
        private PoseMapping m_leftElbowOverride;

        [SerializeField]
        private PoseMapping m_rightFootOverride;
        [SerializeField]
        private PoseMapping m_leftFootOverride;
        [SerializeField]
        private PoseMapping m_rightKneeOverride;
        [SerializeField]
        private PoseMapping m_leftKneeOverride;

        private bool m_overrideHead, m_overrideChest, m_overridePelvis,
            m_overrideRightHand, m_overrideLeftHand, m_overrideRightElbow, m_overrideLeftElbow,
            m_overrideRightFoot, m_overrideLeftFoot, m_overrideRightKnee, m_overrideLeftKnee;

        public bool HasOverride(PoseBodyPart bodyPart)
        {
            switch (bodyPart)
            {
                case PoseBodyPart.HEAD:
                    return m_overrideHead;
                case PoseBodyPart.CHEST:
                    return m_overrideChest;
                case PoseBodyPart.PELVIS:
                    return m_overridePelvis;
                case PoseBodyPart.RIGHT_HAND:
                    return m_overrideRightHand;
                case PoseBodyPart.LEFT_HAND:
                   return m_overrideLeftHand;
                case PoseBodyPart.RIGHT_ELBOW:
                    return m_overrideRightElbow;
                case PoseBodyPart.LEFT_ELBOW:
                    return m_overrideLeftElbow;
                case PoseBodyPart.RIGHT_FOOT:
                    return m_overrideRightFoot;
                case PoseBodyPart.LEFT_FOOT:
                    return m_overrideLeftFoot;
                case PoseBodyPart.RIGHT_KNEE:
                    return m_overrideRightKnee;
                case PoseBodyPart.LEFT_KNEE:
                    return m_overrideLeftKnee;
                default:
                    return false;
            }
        }

        public PoseMapping GetOverride(PoseBodyPart bodyPart)
        {
            switch (bodyPart)
            {
                case PoseBodyPart.HEAD:
                    return m_headOverride;
                case PoseBodyPart.CHEST:
                    return m_chestOverride;
                case PoseBodyPart.PELVIS:
                    return m_pelvisOverride;
                case PoseBodyPart.RIGHT_HAND:
                    return m_rightHandOverride;
                case PoseBodyPart.LEFT_HAND:
                    return m_leftHandOverride;
                case PoseBodyPart.RIGHT_ELBOW:
                    return m_rightElbowOverride;
                case PoseBodyPart.LEFT_ELBOW:
                    return m_leftElbowOverride;
                case PoseBodyPart.RIGHT_FOOT:
                    return m_rightFootOverride;
                case PoseBodyPart.LEFT_FOOT:
                    return m_leftFootOverride;
                case PoseBodyPart.RIGHT_KNEE:
                    return m_rightKneeOverride;
                case PoseBodyPart.LEFT_KNEE:
                    return m_leftKneeOverride;
                default:
                    return new PoseMapping();
            }
        }

        public PoseOverrides() { }

        public PoseOverrides(PoseConstraintData[] constraints, Transform parent = null)
        {
            ClearOverrides();

            for(int i = 0; i < constraints.Length; i++)
            {
                switch (constraints[i].BodyPart)
                {
                    case PoseBodyPart.HEAD:
                        m_headOverride = new PoseMapping(constraints[i].Anchor.localPosition, constraints[i].Anchor.localRotation);
                        m_overrideHead = true;
                        break;
                    case PoseBodyPart.CHEST:
                        m_chestOverride = new PoseMapping(constraints[i].Anchor.localPosition, constraints[i].Anchor.localRotation);
                        m_overrideChest = true;
                        break;
                    case PoseBodyPart.PELVIS:
                        m_pelvisOverride = new PoseMapping(constraints[i].Anchor.localPosition, constraints[i].Anchor.localRotation);
                        m_overridePelvis = true;
                        break;
                    case PoseBodyPart.RIGHT_HAND:
                        m_rightHandOverride = new PoseMapping(constraints[i].Anchor.localPosition, constraints[i].Anchor.localRotation);
                        m_overrideRightHand = true;
                        break;
                    case PoseBodyPart.LEFT_HAND:
                        m_leftHandOverride = new PoseMapping(constraints[i].Anchor.localPosition, constraints[i].Anchor.localRotation);
                        m_overrideLeftHand = true;
                        break;
                    case PoseBodyPart.RIGHT_ELBOW:
                        m_rightElbowOverride = new PoseMapping(constraints[i].Anchor.localPosition, constraints[i].Anchor.localRotation);
                        m_overrideRightElbow = true;
                        break;
                    case PoseBodyPart.LEFT_ELBOW:
                        m_leftElbowOverride = new PoseMapping(constraints[i].Anchor.localPosition, constraints[i].Anchor.localRotation);
                        m_overrideLeftElbow = true;
                        break;
                    case PoseBodyPart.RIGHT_FOOT:
                        m_rightFootOverride = new PoseMapping(constraints[i].Anchor.localPosition, constraints[i].Anchor.localRotation);
                        m_overrideRightFoot = true;
                        break;
                    case PoseBodyPart.LEFT_FOOT:
                        m_leftFootOverride = new PoseMapping(constraints[i].Anchor.localPosition, constraints[i].Anchor.localRotation);
                        m_overrideLeftFoot = true;
                        break;
                    case PoseBodyPart.RIGHT_KNEE:
                        m_rightKneeOverride = new PoseMapping(constraints[i].Anchor.localPosition, constraints[i].Anchor.localRotation);
                        m_overrideRightKnee = true;
                        break;
                    case PoseBodyPart.LEFT_KNEE:
                        m_leftKneeOverride = new PoseMapping(constraints[i].Anchor.localPosition, constraints[i].Anchor.localRotation);
                        m_overrideLeftKnee = true;
                        break;
                }
            }
        }

        private void ClearOverrides()
        {
            m_overrideHead = m_overrideChest = m_overridePelvis =
            m_overrideRightHand = m_overrideLeftHand = m_overrideRightElbow = m_overrideLeftElbow =
            m_overrideRightFoot = m_overrideLeftFoot = m_overrideRightKnee = m_overrideLeftKnee = false;
        }
    }
}