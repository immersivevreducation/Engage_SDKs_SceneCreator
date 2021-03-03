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
        DrawCopyPose();

        if (!Trigger.EditPoseData)
            return;

        DrawPoseEditOptions();

        //if (GUILayout.Button("Instantiate Dummy"))
        //    Trigger.DropDummy(PoseContainer.Instance.Dummy);
    }

    private void RefreshView()
    {
        if (Trigger == null)
            return;

        if (Trigger.TestPoseData == null)
            return;

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
        //UpdateMeshes();
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

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Fill All"))
            PopulateConstraints();
        if (GUILayout.Button("Clear All"))
            ClearConstraints();

        GUILayout.EndHorizontal();

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

    private bool HasConstraint(PoseBodyPart bodyPart)
    {
        for (int i = 0; i < Trigger.ConstraintData.Length; i++)
            if (bodyPart == Trigger.ConstraintData[i].BodyPart)
                return true;

        return false;
    }

    private void PopulateConstraints()
    {
        if (!HasConstraint(PoseBodyPart.HEAD))
            AddConstraint(PoseBodyPart.HEAD);
        if (!HasConstraint(PoseBodyPart.PELVIS))
            AddConstraint(PoseBodyPart.PELVIS);
        if (!HasConstraint(PoseBodyPart.CHEST))
            AddConstraint(PoseBodyPart.CHEST);

        if (!HasConstraint(PoseBodyPart.RIGHT_HAND))
            AddConstraint(PoseBodyPart.RIGHT_HAND);
        if (!HasConstraint(PoseBodyPart.LEFT_HAND))
            AddConstraint(PoseBodyPart.LEFT_HAND);
        if (!HasConstraint(PoseBodyPart.RIGHT_ELBOW))
            AddConstraint(PoseBodyPart.RIGHT_ELBOW);
        if (!HasConstraint(PoseBodyPart.LEFT_ELBOW))
            AddConstraint(PoseBodyPart.LEFT_ELBOW);

        if (!HasConstraint(PoseBodyPart.RIGHT_FOOT))
            AddConstraint(PoseBodyPart.RIGHT_FOOT);
        if (!HasConstraint(PoseBodyPart.LEFT_FOOT))
            AddConstraint(PoseBodyPart.LEFT_FOOT);
        if (!HasConstraint(PoseBodyPart.RIGHT_KNEE))
            AddConstraint(PoseBodyPart.RIGHT_KNEE);
        if (!HasConstraint(PoseBodyPart.LEFT_KNEE))
            AddConstraint(PoseBodyPart.LEFT_KNEE);
    }

    private void ClearConstraints()
    {
        for (int i = Trigger.ConstraintData.Length - 1; i >= 0; i--)
        {
            RemoveConstraint(i, Trigger.ConstraintData[i].Anchor);
        }
    }

    private void AddConstraint(PoseBodyPart bodyPart)
    {
        Undo.RecordObject(target, "Created " + bodyPart);

        Transform anchor = new GameObject(bodyPart.ToString() + "_Anchor").transform;
        anchor.parent = Trigger.transform;
        if (Trigger.ConstraintDictionary.ContainsKey(bodyPart))
        {
            anchor.position = Trigger.ConstraintDictionary[bodyPart].Position;

            float angle = Vector3.SignedAngle(Vector3.forward, Trigger.Transform.forward, Vector3.up);

            Debug.Log("Adding constraint " + Trigger.transform.localEulerAngles);
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up) * Trigger.ConstraintDictionary[bodyPart].Rotation;
            //Quaternion rot = Trigger.ConstraintDictionary[bodyPart].Rotation;
            anchor.rotation = rot;
        }

        Undo.RecordObject(target, "Constrained " + bodyPart);

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

    #region Copying

    public void DrawCopyPose()
    {
        Undo.RecordObject(target, "CopiedConstraints");

        Event evt = Event.current;
        Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(drop_area, "Drop PoseTrigger to Copy Constraints");

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!drop_area.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (Object dragged_object in DragAndDrop.objectReferences)
                    {
                        GameObject dummy = dragged_object as GameObject;

                        if (dummy == null)
                            continue;

                        if (CopyConstraints(dummy.GetComponentInChildren<PoseTrigger>(true)))
                            return;
                    }
                }
                break;
        }
    }

    public bool CopyConstraints(PoseTrigger pose)
    {
        if (pose == null)
            return false;

        if (pose.ConstraintData == null || pose.ConstraintData.Length == 0)
        {
            Debug.Log("Tring to Copy Empty Constraints, just clear instead.");
            return false;
        }

        PoseConstraintData[] constraints = new PoseConstraintData[pose.ConstraintData.Length];

        for(int i = 0; i < pose.ConstraintData.Length; i++)
            constraints[i] = new PoseConstraintData(CopyTransformToLocal(pose.ConstraintData[i].Anchor), pose.ConstraintData[i].BodyPart);

        Trigger.ConstraintData = constraints;

        return true;
    }

    private Transform CopyTransformToLocal(Transform anchor)
    {
        if (anchor == null)
            return null;

        Transform newAnchor = new GameObject(anchor.name).transform;
        newAnchor.SetParent(Trigger.transform);
        newAnchor.rotation = anchor.rotation;
        newAnchor.localPosition = anchor.localPosition;

        Debug.Log("Copied Anchor " + newAnchor.name);

        return newAnchor;
    }

    #endregion

    #region Meshes

    private bool hndlMeshesCreated = false;
    public Transform lfHandle, rfHandle, sHandle;

    private Vector3 m_footRotOffset = new Vector3(0, 90, 0);
    private Vector3 m_pelvisRotOffset = new Vector3(0, 90, 0);

    //private void OnDisable()
    //{
    //    if (hndlMeshesCreated)
    //        DestroyHandleMeshes();
    //}

    void CreateHandleMeshes()
    {
        if (hndlMeshesCreated)
            return;

        SetupMesh(PoseBodyPart.RIGHT_FOOT, "Prefab_Mesh_FootR", ref rfHandle);
        SetupMesh(PoseBodyPart.LEFT_FOOT, "Prefab_Mesh_FootL", ref lfHandle);
        SetupMesh(PoseBodyPart.PELVIS, "Prefab_Mesh_Seat", ref sHandle);

        lfHandle.gameObject.AddComponent<MeshRenderer>().sharedMaterial =
            rfHandle.gameObject.AddComponent<MeshRenderer>().sharedMaterial =
            sHandle.gameObject.AddComponent<MeshRenderer>().sharedMaterial =
            Resources.Load<Material>("SeatPrevis");

        hndlMeshesCreated = true;
    }

    private void UpdateMeshes()
    {
        CreateHandleMeshes();

        UpdateMesh(PoseBodyPart.RIGHT_FOOT, rfHandle, m_footRotOffset);
        UpdateMesh(PoseBodyPart.LEFT_FOOT, lfHandle, m_footRotOffset);
        UpdateMesh(PoseBodyPart.PELVIS, sHandle, m_pelvisRotOffset);
    }

    private void SetupMesh(PoseBodyPart bodyPart, string resourceName, ref Transform anchor)
    {
        anchor = new GameObject(bodyPart.ToString() + "_MESH").transform;
        anchor.hideFlags = HideFlags.HideAndDontSave;

        if (Trigger.ConstraintDictionary != null && Trigger.ConstraintDictionary.ContainsKey(bodyPart))
        {
            anchor.position = Trigger.ConstraintDictionary[bodyPart].Position;
            anchor.rotation = Trigger.ConstraintDictionary[bodyPart].Rotation;
        }

        anchor.gameObject.AddComponent<MeshFilter>().mesh = Resources.Load<GameObject>(resourceName).GetComponent<MeshFilter>().sharedMesh;
        anchor.SetParent(Trigger.Transform);
    }

    private void UpdateMesh(PoseBodyPart bodyPart, Transform anchor, Vector3? rotOffset = null)
    {
        if (Trigger.ConstraintDictionary == null || !Trigger.ConstraintDictionary.ContainsKey(bodyPart))
            return;

        Quaternion rot = Trigger.ConstraintDictionary[bodyPart].Rotation;

        if (rotOffset != null)
            rot = Quaternion.Euler(rotOffset.Value) * rot;

        if (Trigger.ConstraintDictionary[bodyPart].FromPoseData)
        {
            float angle = Vector3.SignedAngle(Vector3.forward, Trigger.Transform.forward, Vector3.up);
            rot *= Quaternion.AngleAxis(angle, Vector3.up);
        }

        anchor.position = Trigger.ConstraintDictionary[bodyPart].Position;
        anchor.rotation = rot;
    }

    void DestroyHandleMeshes()
    {
        DestroyImmediate(lfHandle.gameObject);
        DestroyImmediate(rfHandle.gameObject);
        DestroyImmediate(sHandle.gameObject);
        hndlMeshesCreated = false;
    }

    #endregion
}