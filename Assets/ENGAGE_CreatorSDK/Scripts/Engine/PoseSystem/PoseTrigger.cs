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
        [SerializeField]
        private PoseConstraintData[] m_constraintData;
        [SerializeField, ReadOnly]
        private LVR_SitTrigger m_sitTrigger;

        [Header("Overrides (Leave Empty to use Defaults)")]

        [SerializeField]
        private IKDefaults m_ikOverride = null;
        [SerializeField]
        private PoseData[] m_additionalPoses = null;

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
        public IKDefaults IKOverride { get { return m_ikOverride; } }
        public PoseData[] AdditionalPoses { get { return m_additionalPoses; } }

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

        [SerializeField, ReadOnly]
        private LayerMask m_groundLayers = 0;

        [SerializeField]
        private PoseData m_testPoseData = null;
        public PoseData TestPoseData { get { return m_testPoseData; } }
        [SerializeField]
        private bool m_editPoseData = false;
        public bool EditPoseData { get { return m_editPoseData; } }

        [SerializeField]
        private bool m_drawEditor = true;
        private Vector3 m_footCubeSize = new Vector3(.15f, .125f, .075f);

        [HideInInspector]
        public bool EditPelvis, EditHead, EditChest,
            EditRightHand, EditLeftHand, EditRightElbow,
            EditLeftElbow, EditRightFoot, EditLeftFoot,
            EditRightKnee, EditLeftKnee = false;

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (!m_drawEditor)
                return;

            RefreshConstraintData();

            if (m_testPoseData == null)
                return;

            DrawLines();
            DrawMeshes();
        }

        private float m_avatarHeight = 1.88f;

        private Dictionary<PoseBodyPart, PoseMapping> m_constraintDictionary;
        public Dictionary<PoseBodyPart, PoseMapping> ConstraintDictionary { get { return m_constraintDictionary; } }

        public void RefreshConstraintData()
        {
            //DrawFrustrum(m_centerTransform);

            //if (ConstraintData == null || ConstraintData.Length == 0)
            //    return;

            if (m_constraintDictionary == null)
                m_constraintDictionary = new Dictionary<PoseBodyPart, PoseMapping>(11);
            else
                m_constraintDictionary.Clear();

            if (ConstraintData != null)
            {
                for (int i = 0; i < ConstraintData.Length; i++)
                    AddConstraint(ConstraintData[i]);
            }

            if (m_testPoseData == null)
                return;

            SetupRotation();

            ApplyBodyPartAI(PoseBodyPart.PELVIS);
            ApplyBodyPartAI(PoseBodyPart.HEAD);
            ApplyBodyPartAI(PoseBodyPart.CHEST);

            ApplyBodyPartAI(PoseBodyPart.LEFT_FOOT);
            ApplyBodyPartAI(PoseBodyPart.LEFT_KNEE);
            ApplyBodyPartAI(PoseBodyPart.RIGHT_FOOT);
            ApplyBodyPartAI(PoseBodyPart.RIGHT_KNEE);

            ApplyBodyPartAI(PoseBodyPart.LEFT_HAND);
            ApplyBodyPartAI(PoseBodyPart.RIGHT_HAND);
            ApplyBodyPartAI(PoseBodyPart.LEFT_ELBOW);
            ApplyBodyPartAI(PoseBodyPart.RIGHT_ELBOW);

            //DrawHandles();

        }

        private Vector3 m_rotOffsetFoot = new Vector3(0, -32f, 0);

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
            DrawCube(PoseBodyPart.PELVIS, size: new Vector3(.2f, .10f, .05f), rotOffset: new Vector3(0, -90, 0));
            DrawCube(PoseBodyPart.PELVIS, posOffset: new Vector3(0f, .1f, 0f), size: new Vector3(.05f, .1f, .05f), rotOffset: new Vector3(0, -90, 0));
            DrawCube(PoseBodyPart.PELVIS, posOffset: new Vector3(.075f, -.03f, -.05f), size: new Vector3(.03f, .03f, .1f), rotOffset: new Vector3(0, -90, 0));
            DrawCube(PoseBodyPart.PELVIS, posOffset: new Vector3(-.075f, -.03f, -.05f), size: new Vector3(.03f, .03f, .1f), rotOffset: new Vector3(0, -90, 0));
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

            if (m_testPoseData == null)
                return;

            if (!m_testPoseData.HasBodyPart(bodyPart))
                return;

            PoseMapping map;
            if (!m_testPoseData.GetBodyPartMap(bodyPart, out map))
                return;

            Vector3 position = map.Position;
            Quaternion rotation = map.Rotation;

            position = Quaternion.AngleAxis(m_angle, Vector3.up) * position;

            position = (position * m_avatarHeight) + Position;

            if (m_testPoseData.SnapToGround(bodyPart))
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
