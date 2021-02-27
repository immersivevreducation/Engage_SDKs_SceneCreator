using UnityEngine;
using UnityEditor;
using Engage.Avatars.Poses;
using System.Collections.Generic;

[CustomEditor(typeof(PoseTrigger))]
public class PoseTriggerEditor : Editor
{
    private SerializedObject obj;
    private PoseTrigger m_trigger = null;

    private PoseTrigger Trigger { get { return m_trigger; } }

    private Vector3 m_footCubeSize = new Vector3(.2f, .1f, .3f);

    private bool m_editPoseData = false;

    public override bool RequiresConstantRepaint()
    {
        return true;
    }

    public void OnEnable()
    {
        obj = new SerializedObject(target);
        m_trigger = obj.targetObject as PoseTrigger;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Refresh Pose Data"))
        {
            RefreshView();
        }

        DrawOverrideOptions();

        if (!Trigger.EditPoseData)
            return;

        DrawPoseEditOptions();

        //if (GUILayout.Button("Instantiate Dummy"))
        //    Trigger.DropDummy(PoseContainer.Instance.Dummy);
    }

    private void RefreshView()
    {
        Trigger.TestPoseData.ResetPoseData();
        m_trigger.RefreshConstraintData();
        SceneView.RepaintAll();
    }

    private void DrawPoseEditOptions()
    {
        GUILayout.BeginVertical("Edit PoseData Positions", GUI.skin.box);

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();

        Trigger.EditPelvis = EditorGUILayout.Toggle("Pelvis", Trigger.EditPelvis);

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        Trigger.EditHead = EditorGUILayout.Toggle("Head", Trigger.EditHead);
        Trigger.EditChest = EditorGUILayout.Toggle("Chest", Trigger.EditChest);

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        Trigger.EditRightFoot = EditorGUILayout.Toggle("Right Foot", Trigger.EditRightFoot);
        Trigger.EditLeftFoot = EditorGUILayout.Toggle("Left Foot", Trigger.EditLeftFoot);

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        Trigger.EditRightKnee = EditorGUILayout.Toggle("Right Knee", Trigger.EditRightKnee);
        Trigger.EditLeftKnee = EditorGUILayout.Toggle("Left Knee", Trigger.EditLeftKnee);

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        Trigger.EditRightHand = EditorGUILayout.Toggle("Right Hand", Trigger.EditRightHand);
        Trigger.EditLeftHand = EditorGUILayout.Toggle("Left Hand", Trigger.EditLeftHand);

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        Trigger.EditRightElbow = EditorGUILayout.Toggle("Right Elbow", Trigger.EditRightElbow);
        Trigger.EditLeftElbow = EditorGUILayout.Toggle("Left Elbow", Trigger.EditLeftElbow);

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    public void OnSceneGUI()
    {
        if (Trigger.TestPoseData == null)
            return;
        
        DrawHandles();
    }

    #region EditorStuff

    private bool m_drawEditor = true;

    private float m_avatarHeight = 1.88f;

    private bool m_requiresRefresh = false;

    private void DrawHandles()
    {
        if (!Trigger.EditPoseData)
            return;

        if (Trigger.ConstraintDictionary == null)
            return;

        m_requiresRefresh = false;

        if (Trigger.EditPelvis)
            DrawHandle(PoseBodyPart.PELVIS);
        if (Trigger.EditHead)
            DrawHandle(PoseBodyPart.HEAD);
        if (Trigger.EditChest)
            DrawHandle(PoseBodyPart.CHEST);

        if (Trigger.EditRightHand)
            DrawHandle(PoseBodyPart.RIGHT_HAND);
        if (Trigger.EditLeftHand)
            DrawHandle(PoseBodyPart.LEFT_HAND);
        if (Trigger.EditRightElbow)
            DrawHandle(PoseBodyPart.RIGHT_ELBOW);
        if (Trigger.EditLeftElbow)
            DrawHandle(PoseBodyPart.LEFT_ELBOW);

        if (Trigger.EditRightFoot)
            DrawHandle(PoseBodyPart.RIGHT_FOOT);
        if (Trigger.EditLeftFoot)
            DrawHandle(PoseBodyPart.LEFT_FOOT);
        if (Trigger.EditRightKnee)
            DrawHandle(PoseBodyPart.RIGHT_KNEE);
        if (Trigger.EditLeftKnee)
            DrawHandle(PoseBodyPart.LEFT_KNEE);

        if (m_requiresRefresh)
            Trigger.TestPoseData.ResetPoseData();
    }

    #region Overrides

    private void DrawOverrideOptions()
    {
        GUILayout.BeginVertical("Pose Override Positions", GUI.skin.box);

        GUILayout.Space(20);

        DrawOverride(PoseBodyPart.PELVIS);
        DrawOverride(PoseBodyPart.HEAD);
        DrawOverride(PoseBodyPart.CHEST);

        DrawOverride(PoseBodyPart.RIGHT_HAND);
        DrawOverride(PoseBodyPart.LEFT_HAND);
        DrawOverride(PoseBodyPart.RIGHT_ELBOW);
        DrawOverride(PoseBodyPart.LEFT_ELBOW);

        DrawOverride(PoseBodyPart.RIGHT_FOOT);
        DrawOverride(PoseBodyPart.LEFT_FOOT);
        DrawOverride(PoseBodyPart.RIGHT_KNEE);
        DrawOverride(PoseBodyPart.LEFT_KNEE);

        GUILayout.EndVertical();
    }

    private void DrawOverride(PoseBodyPart bodyPart)
    {
        if (Trigger.ConstraintData == null)
            return;

        for (int i = 0; i < Trigger.ConstraintData.Length; i++)
        {
            if (Trigger.ConstraintData[i].BodyPart != bodyPart)
                continue;

            if (Trigger.ConstraintData[i].Anchor == null)
            {
                GUI.color = Color.red;
                GUILayout.BeginHorizontal(GUI.skin.box);
                GUILayout.Label(bodyPart.ToString(), GUILayout.Width(100));
                GUILayout.Label("Transform Missing!");

                if (GUILayout.Button("Clear"))
                {
                    RemoveConstraint(i);
                    return;
                }
            }
            else
            {
                GUI.color = Color.green;
                GUILayout.BeginHorizontal(GUI.skin.box);
                GUILayout.Label(bodyPart.ToString(), GUILayout.Width(100));

                if (GUILayout.Button("Select Anchor"))
                {
                    Selection.objects = new Object[] { Trigger.ConstraintData[i].Anchor.gameObject };
                    EditorGUIUtility.PingObject(Trigger.ConstraintData[i].Anchor.gameObject);
                }
                else if (GUILayout.Button("Clear"))
                {
                    RemoveConstraint(i, Trigger.ConstraintData[i].Anchor);
                    return;
                }
            }

            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            return;
        }

        GUILayout.BeginHorizontal(GUI.skin.box);

        GUILayout.Label(bodyPart.ToString(), GUILayout.Width(100));
        if (GUILayout.Button("Create Constraint"))
            AddConstraint(bodyPart);

        GUILayout.EndHorizontal();
    }

    private void AddConstraint(PoseBodyPart bodyPart)
    {
        Undo.RecordObject(target, "Created " + bodyPart);

        Transform anchor = new GameObject(bodyPart.ToString() + "_Anchor").transform;
        anchor.parent = Trigger.transform;
        if (Trigger.ConstraintDictionary.ContainsKey(bodyPart))
            anchor.position = Trigger.ConstraintDictionary[bodyPart].Position;

        PoseConstraintData data = new PoseConstraintData(anchor, bodyPart);

        PoseConstraintData[] constraints = Trigger.ConstraintData;
        ArrayUtility.Add<PoseConstraintData>(ref constraints, data);
        Trigger.ConstraintData = constraints;
    }

    private void RemoveConstraint(int id, Transform anchor = null)
    {
        Undo.RecordObject(target, "Removed Body Part");

        PoseConstraintData[] constraints = Trigger.ConstraintData;
        ArrayUtility.RemoveAt<PoseConstraintData>(ref constraints, id);
        Trigger.ConstraintData = constraints;

        if (anchor != null)
            GameObject.DestroyImmediate(anchor.gameObject);
    }

    #endregion

    private void DrawHandle(PoseBodyPart bodyPart)
    {
        if (!Trigger.ConstraintDictionary.ContainsKey(bodyPart))
            return;

        PoseMapping map = Trigger.ConstraintDictionary[bodyPart];

        if (!map.FromPoseData)
            return;

        Vector3 pos = Trigger.ConstraintDictionary[bodyPart].Position;// = Handles.PositionHandle(m_constraintDictionary[bodyPart].Position, m_constraintDictionary[bodyPart].Rotation);
        Quaternion rot = Trigger.ConstraintDictionary[bodyPart].Rotation;

        Handles.TransformHandle(ref pos, ref rot);

        if (pos == Trigger.ConstraintDictionary[bodyPart].Position && rot == Trigger.ConstraintDictionary[bodyPart].Rotation)
            return;

        Undo.RecordObject(Trigger.TestPoseData, "Moved " + bodyPart);

        pos -= Trigger.Position;
        pos /= m_avatarHeight;

        float angle = Vector3.SignedAngle(Vector3.forward, Trigger.Transform.forward, Vector3.up);
        pos = Quaternion.AngleAxis(-angle, Vector3.up) * pos;

        //Debug.Log("Updating " + bodyPart + " to POS : [" + pos + "] ROT : [" + rot + "]");

        Trigger.TestPoseData.UpdatePosition(bodyPart, pos);
        Trigger.TestPoseData.UpdateRotation(bodyPart, rot.eulerAngles);

        m_requiresRefresh = true;
    }

    #endregion
}