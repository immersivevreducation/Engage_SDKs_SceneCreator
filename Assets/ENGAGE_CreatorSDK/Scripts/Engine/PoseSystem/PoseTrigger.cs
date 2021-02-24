using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace Engage.Avatars.Poses
{
    [RequireComponent(typeof(LVR_SitTrigger))]
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
        public PoseConstraintData[] ConstraintData { get { return m_constraintData; } }
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

            DrawConstraintData();
        }

        private float m_avatarHeight = 1.88f;

        private Dictionary<PoseBodyPart, PoseMapping> m_constraintDictionary;
        public Dictionary<PoseBodyPart, PoseMapping> ConstraintDictionary { get { return m_constraintDictionary; } }

        private void DrawConstraintData()
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
            DrawLines();
            DrawMeshes();
        }

        private void DrawMeshes()
        {
            DrawCube(ConstraintDictionary[PoseBodyPart.PELVIS].Position, ConstraintDictionary[PoseBodyPart.PELVIS].Rotation, Color.blue);
            DrawCube(ConstraintDictionary[PoseBodyPart.HEAD].Position, ConstraintDictionary[PoseBodyPart.HEAD].Rotation, Color.blue);
            DrawSphere(ConstraintDictionary[PoseBodyPart.CHEST].Position, Color.green);

            DrawCube(ConstraintDictionary[PoseBodyPart.RIGHT_FOOT].Position, ConstraintDictionary[PoseBodyPart.RIGHT_FOOT].Rotation, Color.blue);
            DrawCube(ConstraintDictionary[PoseBodyPart.LEFT_FOOT].Position, ConstraintDictionary[PoseBodyPart.LEFT_FOOT].Rotation, Color.blue);
            DrawSphere(ConstraintDictionary[PoseBodyPart.RIGHT_KNEE].Position, Color.green);
            DrawSphere(ConstraintDictionary[PoseBodyPart.LEFT_KNEE].Position, Color.green);

            DrawCube(ConstraintDictionary[PoseBodyPart.RIGHT_HAND].Position, ConstraintDictionary[PoseBodyPart.RIGHT_HAND].Rotation, Color.blue);
            DrawCube(ConstraintDictionary[PoseBodyPart.LEFT_HAND].Position, ConstraintDictionary[PoseBodyPart.LEFT_HAND].Rotation, Color.blue);
            DrawSphere(ConstraintDictionary[PoseBodyPart.RIGHT_ELBOW].Position, Color.green);
            DrawSphere(ConstraintDictionary[PoseBodyPart.LEFT_ELBOW].Position, Color.green);
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
            //rotation = Quaternion.AngleAxis(m_angle, Vector3.up) * rotation;
            //rotation *= Quaternion.Euler(0, m_angle, 0);

            position = (position * m_avatarHeight) + Position;

            if (m_testPoseData.SnapToGround(bodyPart))
            {
                //Debug.Log("Snapping to ground " + bodyPart);
                position = SnapToGround(position);
            }

            DrawSphere(position, Color.red, .02f);

            m_constraintDictionary.Add(bodyPart, new PoseMapping(position, rotation, true));
        }

        public Vector3 SnapToGround(Vector3 pos)
        {
            if (m_groundLayers.value == 0)
                m_groundLayers = LayerMask.GetMask("TeleportCollider");

            RaycastHit hit;
            if (!Physics.Raycast(pos, Vector3.down, out hit, m_groundLayers))
                return pos;

            pos.y = hit.point.y;
            return pos;
        }

        private float m_angle;
        private void SetupRotation()
        {
            m_angle = Vector3.SignedAngle(Vector3.forward, Transform.forward, Vector3.up);
        }

        private void DrawCube(Vector3 pos, Quaternion rot, Color colour)
        {
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(pos, rot, Vector3.one);
            Gizmos.matrix = rotationMatrix;

            Gizmos.color = colour;

            Gizmos.DrawCube(Vector3.zero, m_footCubeSize);

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
