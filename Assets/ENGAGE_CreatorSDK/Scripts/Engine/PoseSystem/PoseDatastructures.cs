using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engage.Avatars.Poses
{
    [System.Serializable]
    public struct PoseDataHandle
    {
        public bool Enabled;
        public bool OverrideTracking;
        public Vector3 Position;
        public Vector3 Rotation;
        public bool SnapToGround;
        public PoseBodyPart Parent;
    }

    [System.Flags]
    public enum PoseType
    {
        NONE = 0,
        SITTING = 1,
        LYING = 2,
        LEANING = 4,

    }

    public enum PoseArchetype
    {
        CUSTOM = 0,

        SIT_CLOSED_LEG = 1,
        SIT_OPEN_LEG,
        SIT_CROSSED_LEG,
        
        LIE_RECUMBANT = 10,

        LEAN_ONE_LEG,
        LEAN_BOTH_LEGS,


    }

    [System.Serializable]
    public struct PoseMapping
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public bool FromPoseData;

        public PoseMapping(Vector3 pos, Quaternion rot) { Position = pos; Rotation = rot; FromPoseData = false; }
        public PoseMapping(Vector3 pos, Quaternion rot, bool fromPoseData) { Position = pos; Rotation = rot; FromPoseData = fromPoseData; }
        public PoseMapping(Vector3 pos) { Position = pos; Rotation = Quaternion.identity; FromPoseData = false; }
        public PoseMapping(PoseConstraintData data) { Position = data.Anchor.position; Rotation = data.Anchor.rotation; FromPoseData = false; }

        public void CopyConstraint(PoseConstraintData data) { Position = data.Anchor.position; Rotation = data.Anchor.rotation; }
    }

    public enum PoseBodyPart
    {
        PELVIS = 0,
        RIGHT_FOOT,
        LEFT_FOOT,
        HEAD,
        RIGHT_HAND,
        LEFT_HAND,
        RIGHT_ELBOW,
        LEFT_ELBOW,
        RIGHT_KNEE,
        LEFT_KNEE,
        CHEST,
    }

    [System.Serializable]
    public struct PoseConstraintData
    {
        public Transform Anchor;
        public PoseBodyPart BodyPart;

        public PoseConstraintData(Transform anchor, PoseBodyPart bodyPart)
        {
            Anchor = anchor;
            BodyPart = bodyPart;
        }

        public bool HasMovedFromMap(PoseMapping map) { return map.Position != Anchor.position || map.Rotation != Anchor.rotation; }
    }

    public static class PoseUtils
    {
        public static string FriendlyName(this PoseType type)
        {
            switch (type)
            {
                case PoseType.SITTING:
                    return "Sit";
                default:
                    return "Sit";
            }
        }

        public static string FriendlyName(this PoseArchetype type)
        {
            switch (type)
            {
                case PoseArchetype.SIT_CLOSED_LEG:
                    return "CloseLeg";
                case PoseArchetype.SIT_OPEN_LEG:
                    return "OpenLeg";
                default:
                    return "Default";
            }
        }
    }
}