using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace Engage.Avatars.Poses
{
    public class PoseTrigger : MonoBehaviour
    {
        [SerializeField]
        private PoseType m_type;
        [SerializeField]
        private Transform m_centerTransform;

        [SerializeField, ReadOnly]
        private LayerMask m_groundLayers = 0;

        [SerializeField, HideInInspector]
        private PoseConstraintData[] m_constraintData;

        [SerializeField, ReadOnly]
        private LVR_SitTrigger m_sitTrigger;

        [SerializeField]
        private Collider m_collider;

        #region Accessors

        public PoseType Type { get { return m_type; } }
        public Transform Transform { get { return m_centerTransform ?? (m_centerTransform = transform); } }
        public Vector3 Position { get { return Transform.position; } }
        public Quaternion Rotation { get { return Transform.rotation; } }
        public PoseConstraintData[] ConstraintData
        {
            get { return m_constraintData; }
            set { m_constraintData = value; }
        }

        [SerializeField, HideInInspector]
        private Transform m_headTransform, m_chestTransform, m_pelvisTransform,
            m_rightHandTransform, m_leftHandTransform, m_rightElbowTransform, m_leftElbowTransform,
            m_rightFootTransform, m_leftFootTransform, m_rightKneeTransform, m_leftKneeTransform = null;

        #endregion

        #region Pose Data Overrides

        [Header("Pose Overrides")]

        [SerializeField]
        private List<PoseOverridesGroup> m_overrides;

        private PoseArchetype m_currentArchetype;
        private int m_currentID = 0;

        public PoseArchetype Archetype { get { return m_currentArchetype; } }
        public int ArchetypeID { get { return m_currentID; } }

        public bool HasOverrides(PoseArchetype archetype, bool notEmpty = false)
        {
            if (m_overrides == null || m_overrides.Count == 0)
                InitialiseOverridesByType();

            for (int i = 0; i < m_overrides.Count; i++)
                if (m_overrides[i].Archetype == archetype && (!notEmpty || m_overrides[i].OverrideList.Count > 0))
                    return true;

            return false;
        }

        public bool GetOverrides(PoseArchetype archetype, out List<PoseOverrides> overrides)
        {
            if (m_overrides == null || m_overrides.Count == 0)
                InitialiseOverridesByType();

            overrides = null;

            for (int i = 0; i < m_overrides.Count; i++)
            {
                if (m_overrides[i].Archetype == archetype)
                {
                    overrides = m_overrides[i].OverrideList;
                    break;
                }
            }

            return overrides != null;
        }


        public bool GetCurrentOverrides(out PoseOverrides poseOverride)
        {
            List<PoseOverrides> overrides;
            poseOverride = null;

            if (!GetOverrides(m_currentArchetype, out overrides))
                return false;

            if (overrides.Count <= m_currentID)
                return false;

            poseOverride = overrides[m_currentID];
            return true;
        }

        public bool IsOverrideSelected(PoseArchetype archetype, int id)
        {
            return m_currentArchetype == archetype && m_currentID == id;
        }

        public void AddPoseOverride(PoseArchetype archetype, PoseOverrides data = null)
        {
            if (m_overrides == null || m_overrides.Count == 0)
                InitialiseOverridesByType();

            for (int i = 0; i < m_overrides.Count; i++)
            {
                if (archetype == m_overrides[i].Archetype)
                {
                    m_overrides[i].AddOverrides(data);
                    m_currentArchetype = archetype;
                    m_currentID = i;
                    return;
                }
            }
        }

        public void RemovePoseOverride(PoseArchetype archetype, int id)
        {
            if (m_overrides == null || m_overrides.Count == 0)
                InitialiseOverridesByType();

            for (int i = 0; i < m_overrides.Count; i++)
            {
                if (archetype == m_overrides[i].Archetype)
                {
                    m_overrides[i].RemoveOverridesAt(id);
                    return;
                }
            }
        }

        public void SetPoseOverride(PoseArchetype archetype, int id, PoseOverrides data = null)
        {
            if (m_overrides == null)
                InitialiseOverridesByType();

            for (int i = 0; i < m_overrides.Count; i++)
            {
                if (archetype == m_overrides[i].Archetype)
                {
                    m_overrides[i].SetPoseOverrides(data, id);
                    return;
                }
            }
        }

        public void InitialiseOverridesByType()
        {
            switch (Type)
            {
                case PoseType.SITTING:
                    m_overrides = new List<PoseOverridesGroup>(2);
                    m_overrides.Add(new PoseOverridesGroup(PoseType.SITTING, PoseArchetype.SIT_CLOSED_LEG));
                    m_overrides.Add(new PoseOverridesGroup(PoseType.SITTING, PoseArchetype.SIT_OPEN_LEG));
                    break;
                default:
                    m_overrides = new List<PoseOverridesGroup>(2);
                    m_overrides.Add(new PoseOverridesGroup(PoseType.SITTING, PoseArchetype.SIT_CLOSED_LEG));
                    m_overrides.Add(new PoseOverridesGroup(PoseType.SITTING, PoseArchetype.SIT_OPEN_LEG));
                    break;
            }
        }

        public void SelectOverride(PoseArchetype archetype, int id)
        {
            if (!HasOverrides(archetype))
                return;

            m_currentArchetype = archetype;
            m_currentID = id;

            ResetPoseConstraints();
        }

        public void ResetPoseConstraints()
        {
            List<PoseOverrides> overrides;
            if (!GetOverrides(m_currentArchetype, out overrides))
            {
                Debug.LogError("CantGetOverrides for type " + m_currentArchetype);
                return;
            }

            PoseConstraintData[] newOverrides = new PoseConstraintData[overrides[m_currentID].Overrides.Count];
            int i = 0;

            foreach(KeyValuePair<PoseBodyPart, PoseMapping> pair in overrides[m_currentID].Overrides)
            {
                Transform anchor = GetOrCreateTransform(pair.Key);
                anchor.localPosition = pair.Value.Position;
                anchor.localRotation = pair.Value.Rotation;

                newOverrides[i] = new PoseConstraintData(anchor, pair.Key);
                i++;
            }

            ConstraintData = newOverrides;
            m_needsSave = false;
        }

        public void SavePoseConstraints()
        {
            List<PoseOverrides> overrides;

            if (!GetOverrides(m_currentArchetype, out overrides))
                return;

            PoseOverrides oldOverrides = overrides[m_currentID];
            overrides[m_currentID] = new PoseOverrides(m_constraintData, oldOverrides.MinHeight, oldOverrides.MaxHeight);

#if UNITY_EDITOR
            RefreshConstraintData();
#endif

            m_needsSave = false;
        }

        private Transform GetOrCreateTransform(PoseBodyPart bodyPart)
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Created " + bodyPart);
#endif

            Transform anchor = GetBodyTransform(bodyPart);

            if (anchor == null)
            {
                anchor = new GameObject(bodyPart.ToString() + "_Anchor").transform;
                anchor.parent = transform;
                SetBodyTransform(bodyPart, anchor);
            }

            return anchor;
        }

        public Transform GetBodyTransform(PoseBodyPart bodyPart)
        {
            switch(bodyPart)
            {
                case PoseBodyPart.HEAD: return m_headTransform;
                case PoseBodyPart.CHEST: return m_chestTransform;
                case PoseBodyPart.PELVIS: return m_pelvisTransform;
                case PoseBodyPart.RIGHT_HAND: return m_rightHandTransform;
                case PoseBodyPart.LEFT_HAND: return m_leftHandTransform;
                case PoseBodyPart.RIGHT_ELBOW: return m_rightElbowTransform;
                case PoseBodyPart.LEFT_ELBOW: return m_leftElbowTransform;
                case PoseBodyPart.RIGHT_FOOT: return m_rightFootTransform;
                case PoseBodyPart.LEFT_FOOT: return m_leftFootTransform;
                case PoseBodyPart.RIGHT_KNEE: return m_rightKneeTransform;
                case PoseBodyPart.LEFT_KNEE: return m_leftKneeTransform;
                default: return null;
            }
        }

        public void SetBodyTransform(PoseBodyPart bodyPart, Transform anchor)
        {
            switch (bodyPart)
            {
                case PoseBodyPart.HEAD: m_headTransform = anchor; return;
                case PoseBodyPart.CHEST: m_chestTransform = anchor; return;
                case PoseBodyPart.PELVIS: m_pelvisTransform = anchor; return;
                case PoseBodyPart.RIGHT_HAND: m_rightHandTransform = anchor; return;
                case PoseBodyPart.LEFT_HAND: m_leftHandTransform = anchor; return;
                case PoseBodyPart.RIGHT_ELBOW: m_rightElbowTransform = anchor; return;
                case PoseBodyPart.LEFT_ELBOW: m_leftElbowTransform = anchor; return;
                case PoseBodyPart.RIGHT_FOOT: m_rightFootTransform = anchor; return;
                case PoseBodyPart.LEFT_FOOT: m_leftFootTransform = anchor; return;
                case PoseBodyPart.RIGHT_KNEE: m_rightKneeTransform = anchor; return;
                case PoseBodyPart.LEFT_KNEE: m_leftKneeTransform = anchor; return;
            }
        }

        #endregion

        #region Sit Trigger

        public void InitialiseFromSitTrigger(LVR_SitTrigger sitTrigger)
        {
            m_sitTrigger = sitTrigger;

            m_centerTransform = m_sitTrigger.PelvisTarget;

            m_type = PoseType.SITTING;
        }

        #endregion

        #region EditorStuff

        
        [SerializeField]
        private PoseData m_closedLegDefault = null;
        [SerializeField]
        private PoseData m_openLegDefault = null;

        public PoseData DefaultPoseData
        { 
            get 
            {
                switch (m_currentArchetype)
                {
                    case PoseArchetype.SIT_CLOSED_LEG:
                        return m_closedLegDefault;
                    case PoseArchetype.SIT_OPEN_LEG:
                        return m_openLegDefault;
                    default:
                        return m_closedLegDefault;
                }
            }
        }

        [Header("Editing")]

        [SerializeField]
        private bool m_editPoseData = false;
        public bool EditPoseData { get { return m_editPoseData; } }

        [SerializeField]
        private bool m_drawSkeleton = true;
        private Vector3 m_footCubeSize = new Vector3(.15f, .125f, .075f);

        [SerializeField]
        private bool m_autoSave = false;
        public bool AutoSave { get { return m_autoSave; } }

        private bool m_needsSave = false;
        public bool NeedsSave { get { return m_needsSave; } set { m_needsSave = value; } }

        [HideInInspector]
        public bool EditPelvis, EditHead, EditChest,
            EditRightHand, EditLeftHand, EditRightElbow,
            EditLeftElbow, EditRightFoot, EditLeftFoot,
            EditRightKnee, EditLeftKnee = false;

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (!m_drawSkeleton)
                return;

            RefreshConstraintData();

            if (DefaultPoseData == null)
                return;

            DrawLines();
            DrawMeshes();
        }

        private float m_avatarHeight = 1.88f;
        public float AvatarHeight { get { return m_avatarHeight; } set { m_avatarHeight = value; } }

        private Dictionary<PoseBodyPart, PoseMapping> m_constraintDictionary;
        public Dictionary<PoseBodyPart, PoseMapping> ConstraintDictionary { get { return m_constraintDictionary; } }

        public void RefreshConstraintData()
        {
            if (m_constraintDictionary == null)
                m_constraintDictionary = new Dictionary<PoseBodyPart, PoseMapping>(11);
            else
                m_constraintDictionary.Clear();

            if (ConstraintData != null)
            {
                for (int i = 0; i < ConstraintData.Length; i++)
                    AddConstraint(ConstraintData[i]);
            }

            if (DefaultPoseData == null)
                return;

            SetupRotation();

            ApplyBodyPartAI(PoseBodyPart.PELVIS);
            //ApplyBodyPartAI(PoseBodyPart.HEAD);
            //ApplyBodyPartAI(PoseBodyPart.CHEST);

            ApplyBodyPartAI(PoseBodyPart.LEFT_FOOT);
            ApplyBodyPartAI(PoseBodyPart.LEFT_KNEE);
            ApplyBodyPartAI(PoseBodyPart.RIGHT_FOOT);
            ApplyBodyPartAI(PoseBodyPart.RIGHT_KNEE);

            ApplyBodyPartAI(PoseBodyPart.LEFT_HAND);
            ApplyBodyPartAI(PoseBodyPart.RIGHT_HAND);
            ApplyBodyPartAI(PoseBodyPart.LEFT_ELBOW);
            ApplyBodyPartAI(PoseBodyPart.RIGHT_ELBOW);

            if (m_constraintDictionary.ContainsKey(PoseBodyPart.HEAD))
                m_constraintDictionary.Remove(PoseBodyPart.HEAD);
            if (m_constraintDictionary.ContainsKey(PoseBodyPart.CHEST))
                m_constraintDictionary.Remove(PoseBodyPart.CHEST);

            m_constraintDictionary.Add(PoseBodyPart.CHEST, new PoseMapping(GetChestPos(m_avatarHeight)));
            m_constraintDictionary.Add(PoseBodyPart.HEAD, new PoseMapping(GetHeadPos(m_avatarHeight)));

            //DrawHandles();
        }

        private Vector3 m_rotOffsetFoot = new Vector3(0, -32f, 0);

        public Vector3 GetHeadPos(float height)
        {
            Vector3 pos = m_constraintDictionary[PoseBodyPart.PELVIS].Position;
            Vector3 headOffset = Vector3.up * (height * .4f);

            headOffset = m_constraintDictionary[PoseBodyPart.PELVIS].Rotation * headOffset;

            Debug.DrawLine(pos, pos + headOffset);

            return pos + headOffset;
        }

        public Vector3 GetChestPos(float height)
        {
            Vector3 pos = m_constraintDictionary[PoseBodyPart.PELVIS].Position;
            Vector3 headOffset = Vector3.up * (height * .25f);

            headOffset = m_constraintDictionary[PoseBodyPart.PELVIS].Rotation * headOffset;

            Debug.DrawLine(pos, pos + headOffset);

            return pos + headOffset;
        }

        private void DrawMeshes()
        {
            DrawPelvis();

            DrawCube(ConstraintDictionary[PoseBodyPart.HEAD].Position, ConstraintDictionary[PoseBodyPart.HEAD].Rotation, ConstraintDictionary[PoseBodyPart.HEAD].FromPoseData ? Color.blue : Color.red);
            DrawSphere(ConstraintDictionary[PoseBodyPart.CHEST].Position, ConstraintDictionary[PoseBodyPart.CHEST].FromPoseData ? Color.green : Color.yellow);

            DrawLegMeshes();
            DrawArmMeshes();
        }

        private void DrawLegMeshes()
        {
            DrawCube(PoseBodyPart.RIGHT_FOOT, m_rotOffsetFoot + new Vector3(-90, 0, 0), new Vector3(-.04f, 0f, 0f), new Vector3(.05f, .1f, .05f));
            DrawCube(PoseBodyPart.LEFT_FOOT, m_rotOffsetFoot + new Vector3(-90, 0, 0), new Vector3(-.04f, 0f, 0f), new Vector3(.05f, .1f, .05f));

            DrawCube(PoseBodyPart.RIGHT_FOOT, m_rotOffsetFoot + new Vector3(0, 0, 0), new Vector3(-.1f, 0f, .075f), new Vector3(.2f, .1f, .08f));
            DrawCube(PoseBodyPart.LEFT_FOOT, m_rotOffsetFoot + new Vector3(0, 0, 0), new Vector3(-.1f, 0f, .075f), new Vector3(.2f, .1f, .08f));

            DrawSphere(ConstraintDictionary[PoseBodyPart.RIGHT_KNEE].Position, ConstraintDictionary[PoseBodyPart.RIGHT_KNEE].FromPoseData ? Color.green : Color.yellow);
            DrawSphere(ConstraintDictionary[PoseBodyPart.LEFT_KNEE].Position, ConstraintDictionary[PoseBodyPart.LEFT_KNEE].FromPoseData ? Color.green : Color.yellow);
        }

        private void DrawPelvis()
        {
            DrawCube(PoseBodyPart.PELVIS, size: new Vector3(.2f, .10f, .05f), rotOffset: new Vector3(0, 0, 0));
            DrawCube(PoseBodyPart.PELVIS, posOffset: new Vector3(0f, .1f, 0f), size: new Vector3(.05f, .1f, .05f), rotOffset: new Vector3(0, 0, 0));
            DrawCube(PoseBodyPart.PELVIS, posOffset: new Vector3(.075f, -.03f, -.05f), size: new Vector3(.03f, .03f, .1f), rotOffset: new Vector3(0, 0, 0));
            DrawCube(PoseBodyPart.PELVIS, posOffset: new Vector3(-.075f, -.03f, -.05f), size: new Vector3(.03f, .03f, .1f), rotOffset: new Vector3(0, 0, 0));
        }

        private void DrawArmMeshes()
        {
            DrawCube(PoseBodyPart.RIGHT_HAND, rotOffset: new Vector3(-45, -90, 0), size: new Vector3(.15f, .05f, .1f));
            DrawCube(PoseBodyPart.RIGHT_HAND, rotOffset: new Vector3(-45, -90, 0), posOffset: new Vector3(-.03f, 0f, .06f), size: new Vector3(.05f, .04f, .06f));

            DrawCube(PoseBodyPart.LEFT_HAND, rotOffset: new Vector3(45, -90, 0), size: new Vector3(.15f, .05f, .1f));
            DrawCube(PoseBodyPart.LEFT_HAND, rotOffset: new Vector3(45, -90, 0), posOffset: new Vector3(-.03f, 0f, -.06f), size: new Vector3(.05f, .04f, .06f));

            DrawSphere(ConstraintDictionary[PoseBodyPart.RIGHT_ELBOW].Position, ConstraintDictionary[PoseBodyPart.RIGHT_ELBOW].FromPoseData ? Color.green : Color.yellow);
            DrawSphere(ConstraintDictionary[PoseBodyPart.LEFT_ELBOW].Position, ConstraintDictionary[PoseBodyPart.RIGHT_ELBOW].FromPoseData ? Color.green : Color.yellow);
        }

        private void DrawCube(PoseBodyPart bodyPart, Vector3? rotOffset = null, Vector3? posOffset = null, Vector3? size = null)
        {
            DrawCube(ConstraintDictionary[bodyPart].Position,// + (posOffset == null ? Vector3.zero : posOffset.Value),
                ConstraintDictionary[bodyPart].Rotation,
                ConstraintDictionary[bodyPart].FromPoseData ? Color.blue : Color.red,
                rotOffset, posOffset, size,
                ConstraintDictionary[bodyPart].FromPoseData);
        }

        public void UpdateSave()
        {
            if (AutoSave)
                SavePoseConstraints();
            else
                m_needsSave = true;
        }

        private void AddConstraint(PoseConstraintData constraint)
        {
            if (m_constraintDictionary.ContainsKey(constraint.BodyPart))
                return;

            if (constraint.Anchor == null)
                return;

            m_constraintDictionary.Add(constraint.BodyPart, new PoseMapping(constraint));            
        }

        private void ApplyBodyPartAI(PoseBodyPart bodyPart)
        {
            if (m_constraintDictionary.ContainsKey(bodyPart))
                return;

            if (DefaultPoseData == null)
                return;

            if (!DefaultPoseData.HasBodyPart(bodyPart))
                return;

            PoseMapping map;
            if (!DefaultPoseData.GetBodyPartMap(bodyPart, out map))
                return;

            Vector3 position = map.Position;
            Quaternion rotation = map.Rotation;

            position = Quaternion.AngleAxis(m_angle, Vector3.up) * position;

            position = (position * m_avatarHeight) + Position;

            if (DefaultPoseData.SnapToGround(bodyPart))
            {
                position = SnapToGround(position);
            }

            m_constraintDictionary.Add(bodyPart, new PoseMapping(position, rotation, true));
        }

        private float m_snapDistUp = .8f;
        private float m_snapDistDown = .7f;

        public Vector3 SnapToGround(Vector3 pos)
        {
            if (m_groundLayers.value == 0)
                m_groundLayers = LayerMask.GetMask("TeleportCollider", "Default");

            RaycastHit hit;
            if (!Physics.Raycast(pos, Vector3.down, out hit, m_snapDistDown, m_groundLayers))
            {
                if (!Physics.Raycast(pos, Vector3.up, out hit, m_snapDistUp, m_groundLayers))
                    return pos;
            }

            pos.y = hit.point.y + .1f;
            return pos;
        }

        private float m_angle;
        private void SetupRotation()
        {
            m_angle = Vector3.SignedAngle(Vector3.forward, Transform.forward, Vector3.up);
        }

        private void DrawCube(Vector3 pos, Quaternion rot, Color colour, Vector3? rotOffset = null, Vector3? posOffset = null, Vector3? size = null, bool fromPoseData = true)
        {
            if (rotOffset != null)
            {
                rot *= Quaternion.Euler(rotOffset.Value);
            }

            if (fromPoseData)
            {
                SetupRotation();
                rot = Quaternion.AngleAxis(m_angle, Vector3.up) * rot;
            }

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(pos, rot, Vector3.one);
            Gizmos.matrix = rotationMatrix;

            colour.a = .85f;
            Gizmos.color = colour;

            Gizmos.DrawCube(posOffset == null ? Vector3.zero : posOffset.Value, size == null ? m_footCubeSize : size.Value);

            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.white;
        }

        private void DrawSphere(Vector3 pos, Color colour, float size = .06f)
        {
            Gizmos.color = colour;

            Gizmos.DrawSphere(pos, size);

            Gizmos.color = Color.white;
        }

        private void DrawLines()
        {
            Gizmos.color = Color.blue;

            //Debug.Log("DictionaryLength = " + m_constraintDictionary.Count);

            Gizmos.DrawLine(m_constraintDictionary[PoseBodyPart.RIGHT_FOOT].Position, m_constraintDictionary[PoseBodyPart.RIGHT_KNEE].Position);
            Gizmos.DrawLine(m_constraintDictionary[PoseBodyPart.RIGHT_KNEE].Position, m_constraintDictionary[PoseBodyPart.PELVIS].Position);

            Gizmos.color = Color.cyan;

            Gizmos.DrawLine(m_constraintDictionary[PoseBodyPart.LEFT_FOOT].Position, m_constraintDictionary[PoseBodyPart.LEFT_KNEE].Position);
            Gizmos.DrawLine(m_constraintDictionary[PoseBodyPart.LEFT_KNEE].Position, m_constraintDictionary[PoseBodyPart.PELVIS].Position);

            Gizmos.color = Color.red;

            Gizmos.DrawLine(m_constraintDictionary[PoseBodyPart.PELVIS].Position, m_constraintDictionary[PoseBodyPart.CHEST].Position);
            Gizmos.DrawLine(m_constraintDictionary[PoseBodyPart.CHEST].Position, m_constraintDictionary[PoseBodyPart.HEAD].Position);

            Gizmos.color = Color.green;

            Gizmos.DrawLine(m_constraintDictionary[PoseBodyPart.CHEST].Position, m_constraintDictionary[PoseBodyPart.RIGHT_ELBOW].Position);
            Gizmos.DrawLine(m_constraintDictionary[PoseBodyPart.RIGHT_ELBOW].Position, m_constraintDictionary[PoseBodyPart.RIGHT_HAND].Position);

            Gizmos.color = Color.yellow;

            Gizmos.DrawLine(m_constraintDictionary[PoseBodyPart.CHEST].Position, m_constraintDictionary[PoseBodyPart.LEFT_ELBOW].Position);
            Gizmos.DrawLine(m_constraintDictionary[PoseBodyPart.LEFT_ELBOW].Position, m_constraintDictionary[PoseBodyPart.LEFT_HAND].Position);

            Gizmos.color = Color.white;
        }

#endif

        #endregion
    }
}
