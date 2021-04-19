using UnityEngine;
using UnityEditor;
using Engage.Avatars.Poses;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(PoseTrigger)), CanEditMultipleObjects]
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

        DrawAllPoseOverrides();
        DrawHeightChange();

        if (Trigger.HasExitPosition)
        {
            if (GUILayout.Button("Zero Exit Position"))
                Trigger.ExitPosition = Vector3.zero;
        }
        else if (GUILayout.Button("Add Exit Position"))
            Trigger.ExitPosition = Vector3.forward;

        if (Trigger.HasOverrides(Trigger.Archetype, true))
        {
            DrawOverrideOptions();
            DrawRestrictions();
            DrawCopyPose();
        }

        if (Trigger.EditPoseData)
        {
            DrawPoseEditOptions();
        }

        //if (GUILayout.Button("Instantiate Dummy"))
        //    Trigger.DropDummy(PoseContainer.Instance.Dummy);

        if (GUILayout.Button("Refresh Pose Data", GUILayout.Height(40)))
        {
            RefreshView();
        }
    }

    [MenuItem("%&z")]
    private void RefreshView()
    {
        if (Trigger == null)
            return;

        if (Trigger.DefaultPoseData == null)
            return;

        Trigger.DefaultPoseData.ResetPoseData();
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
        if (Trigger.DefaultPoseData == null)
            return;

        DrawHandles();
        DrawExitPosition();
        //UpdateMeshes();
    }

    #region EditorStuff

    private bool m_drawEditor = true;

    private bool m_requiresRefresh = false;

    private void DrawExitPosition()
    {
        if (!Trigger.HasExitPosition)
            return;

        Vector3 pos = Handles.PositionHandle(Trigger.ExitPosition, Quaternion.identity);

        if (pos == Trigger.ExitPosition)
            return;

        Trigger.ExitPosition = pos - Trigger.transform.position;
    }

    private void DrawHandles()
    {
        if (!Trigger.EditPoseData)
            return;

        if (Trigger.ConstraintDictionary == null)
            return;

        m_requiresRefresh = false;

        if (Trigger.EditPelvis)
            DrawHandle(PoseBodyPart.PELVIS);
        //if (Trigger.EditHead)
        //    DrawHandle(PoseBodyPart.HEAD);
        //if (Trigger.EditChest)
        //    DrawHandle(PoseBodyPart.CHEST);

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
            Trigger.DefaultPoseData.ResetPoseData();
    }

    #region Overrides

    private void DrawOverrideOptions()
    {
        GUILayout.Space(20);

        DrawPoseName();

        GUILayout.BeginVertical();

        DrawOverride(PoseBodyPart.PELVIS);
        //DrawOverride(PoseBodyPart.HEAD);
        //DrawOverride(PoseBodyPart.CHEST);

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

        if (Trigger.AutoSave)
            GUI.color = Color.red;
        else if (Trigger.NeedsSave)
            GUI.color = Color.yellow;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save Constraints"))
            Trigger.SavePoseConstraints();
        if (GUILayout.Button("Revert Constraints"))
            Trigger.ResetPoseConstraints();

        GUILayout.EndHorizontal();
        GUI.color = Color.white;

        DrawExportOption();

        GUILayout.EndVertical();
    }

    private GUIStyle m_guiStyleTextArea = null;

    private void DrawPoseName()
    {
        PoseOverrides poseOverride;

        if (!Trigger.GetCurrentOverrides(out poseOverride))
            return;

        GUILayout.BeginHorizontal();

        if (m_guiStyleTextArea == null)
        {
            m_guiStyleTextArea = new GUIStyle(GUI.skin.textField);
            m_guiStyleTextArea.alignment = TextAnchor.MiddleCenter;
            m_guiStyleTextArea.fontSize = 16;
            m_guiStyleTextArea.fontStyle = FontStyle.Bold;
        }

        poseOverride.Name = GUILayout.TextField(poseOverride.Name, m_guiStyleTextArea);

        GUILayout.EndHorizontal();
    }

    private string m_newPoseName = "newPose";

    private const string PATH_POSEDATA = "Assets/ENGAGE_CreatorSDK/Scripts/Engine/PoseSystem/PoseData";

    //[SerializeField]
    //private List<PoseData> m_historyQueue;

    private void DrawExportOption()
    {
        GUILayout.BeginVertical("Export Constraints as PoseData:", GUI.skin.box);

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        GUILayout.Label("New Pose Name:", GUILayout.Width(150));
        m_newPoseName = GUILayout.TextField(m_newPoseName, 30);
        GUILayout.EndHorizontal();

        string assetPath = PATH_POSEDATA + "/" + Trigger.Type.FriendlyName() + "/" + Trigger.Archetype.FriendlyName() + "/" + m_newPoseName + ".asset";

        PoseOverrides overrides;

        if (GUILayout.Button("Export Constraints"))
        {
            PoseData newPose = ScriptableObject.CreateInstance<PoseData>();

            newPose.CopyConstraints(Trigger.ConstraintData, Trigger.Transform, Trigger.AvatarHeight);

            if (!Directory.Exists(PATH_POSEDATA + "/" + Trigger.Type.FriendlyName()))
                AssetDatabase.CreateFolder(PATH_POSEDATA, Trigger.Type.FriendlyName());
            if (!Directory.Exists(PATH_POSEDATA + "/" + Trigger.Type.FriendlyName() + "/" + Trigger.Archetype.FriendlyName()))
                AssetDatabase.CreateFolder(PATH_POSEDATA + "/" + Trigger.Type.FriendlyName(), Trigger.Archetype.FriendlyName());

            AssetDatabase.CreateAsset(newPose, assetPath);
            AssetDatabase.SaveAssets();

            EditorUtility.FocusProjectWindow();

            Selection.activeObject = newPose;
            EditorGUIUtility.PingObject(newPose);

            //PoseOverrides overrides;
            if (Trigger.GetCurrentOverrides(out overrides))
            {
                overrides.Name = m_newPoseName;
            }
        }

        PoseData data = AssetDatabase.LoadAssetAtPath<PoseData>(assetPath);

        if (data == null)
        {
            GUILayout.Label("Will create new Pose Data");
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Will Override PoseData");

            if (GUILayout.Button("Select"))
            {
                Selection.objects = new Object[] { data };
                EditorGUIUtility.PingObject(data);
            }
            GUILayout.EndHorizontal();
        }

        if (!Trigger.GetCurrentOverrides(out overrides))
        {
            GUILayout.EndVertical();
            return;
        }    

        if (overrides.HistoryList != null && overrides.HistoryList.Count > 0)
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Pose Data History");

            for(int i = overrides.HistoryList.Count - 1; i >= 0; i--)
            {
                if (overrides.HistoryList[i] == null)
                {
                    overrides.HistoryList.RemoveAt(i);
                    return;
                }

                if (GUILayout.Button(overrides.HistoryList[i].name))
                {
                    Selection.objects = new Object[] { overrides.HistoryList[i] };
                    EditorGUIUtility.PingObject(overrides.HistoryList[i]);
                }
            }

            GUILayout.EndVertical();
        }

        GUILayout.EndVertical();
    }

    private void DrawHeightChange()
    {
        GUILayout.BeginHorizontal(GUI.skin.box);
        GUILayout.Label("Height: " + Trigger.AvatarHeight, GUILayout.Width(100));
        EditorGUI.BeginChangeCheck();
        float avatarHeight = GUILayout.HorizontalSlider(Trigger.AvatarHeight, 1.5f, 2.15f);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this, "Changed Avatar Height");
            Trigger.AvatarHeight = avatarHeight;
            RefreshView();
        }
        GUILayout.EndHorizontal();
    }

    private void DrawRestrictions()
    {
        PoseOverrides poseOverride;
        if (!Trigger.GetCurrentOverrides(out poseOverride))
            return;

        GUILayout.BeginVertical("Restrictions", GUI.skin.box);
        {
            GUILayout.Space(20);
            
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Max Height", GUILayout.Width(80));

                if (poseOverride.HasMaxHeight)
                {
                    GUILayout.Label(poseOverride.MaxHeight.ToString("F2") + "m", GUILayout.Width(80));
                    poseOverride.MaxHeight = GUILayout.HorizontalSlider(poseOverride.MaxHeight, 1.5f, 2.15f);
                    GUILayout.Space(15);
                    if (GUILayout.Button("X", GUILayout.Width(25)))
                        poseOverride.MaxHeight = -1f;
                }
                else
                {
                    GUILayout.Label("Unrestricted", GUILayout.Width(80));
                    if (GUILayout.Button("Set Max Height"))
                        poseOverride.MaxHeight = 2f;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Min Height", GUILayout.Width(80));

                if (poseOverride.HasMinHeight)
                {
                    GUILayout.Label(poseOverride.MinHeight.ToString("F2") + "m", GUILayout.Width(80));
                    poseOverride.MinHeight = GUILayout.HorizontalSlider(poseOverride.MinHeight, 1.5f, 2.15f);
                    GUILayout.Space(15);
                    if (GUILayout.Button("X", GUILayout.Width(25)))
                        poseOverride.MinHeight = -1f;
                }
                else
                {
                    GUILayout.Label("Unrestricted", GUILayout.Width(80));
                    if (GUILayout.Button("Set Min Height"))
                        poseOverride.MinHeight = 1.6f;
                }
            }
            GUILayout.EndHorizontal();
        }
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
                    Trigger.UpdateSave();
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
                    Trigger.UpdateSave();
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
        {
            AddConstraint(bodyPart);
            Trigger.UpdateSave();
        }

        GUILayout.EndHorizontal();
    }

    private bool HasConstraint(PoseBodyPart bodyPart)
    {
        for (int i = 0; i < Trigger.ConstraintData.Length; i++)
            if (bodyPart == Trigger.ConstraintData[i].BodyPart)
                return true;

        return false;
    }

    private void PopulateConstraints(PoseData data = null)
    {
        //if (!HasConstraint(PoseBodyPart.HEAD))
        //    AddConstraint(PoseBodyPart.HEAD, data);
        if (!HasConstraint(PoseBodyPart.PELVIS))
            AddConstraint(PoseBodyPart.PELVIS, data);
        //if (!HasConstraint(PoseBodyPart.CHEST))
        //    AddConstraint(PoseBodyPart.CHEST, data);

        if (!HasConstraint(PoseBodyPart.RIGHT_HAND))
            AddConstraint(PoseBodyPart.RIGHT_HAND, data);
        if (!HasConstraint(PoseBodyPart.LEFT_HAND))
            AddConstraint(PoseBodyPart.LEFT_HAND, data);
        if (!HasConstraint(PoseBodyPart.RIGHT_ELBOW))
            AddConstraint(PoseBodyPart.RIGHT_ELBOW, data);
        if (!HasConstraint(PoseBodyPart.LEFT_ELBOW))
            AddConstraint(PoseBodyPart.LEFT_ELBOW, data);

        if (!HasConstraint(PoseBodyPart.RIGHT_FOOT))
            AddConstraint(PoseBodyPart.RIGHT_FOOT, data);
        if (!HasConstraint(PoseBodyPart.LEFT_FOOT))
            AddConstraint(PoseBodyPart.LEFT_FOOT, data);
        if (!HasConstraint(PoseBodyPart.RIGHT_KNEE))
            AddConstraint(PoseBodyPart.RIGHT_KNEE, data);
        if (!HasConstraint(PoseBodyPart.LEFT_KNEE))
            AddConstraint(PoseBodyPart.LEFT_KNEE, data);

        Trigger.UpdateSave();
    }

    private void ClearConstraints()
    {
        for (int i = Trigger.ConstraintData.Length - 1; i >= 0; i--)
        {
            RemoveConstraint(i, Trigger.ConstraintData[i].Anchor);
        }

        Trigger.UpdateSave();
    }

    private void AddConstraint(PoseBodyPart bodyPart, PoseData pose = null)
    {
        Undo.RecordObject(target, "Created " + bodyPart);

        Transform anchor = Trigger.GetBodyTransform(bodyPart);

        if (anchor == null)
        {
            anchor = new GameObject(bodyPart.ToString() + "_Anchor").transform;
            anchor.parent = Trigger.transform;
            Trigger.SetBodyTransform(bodyPart, anchor);
        }

        if (pose != null)
        {
            float angle = Vector3.SignedAngle(Vector3.forward, Trigger.Transform.forward, Vector3.up);

            PoseDataHandle handle = pose.GetBodyPartHandle(bodyPart);
            Vector3 pos = Quaternion.AngleAxis(angle, Vector3.up) * handle.Position;
            pos = (pos * Trigger.AvatarHeight) + Trigger.Position;

            //Debug.Log("Adding constraint " + Trigger.transform.localEulerAngles);
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up) * Quaternion.Euler(handle.Rotation);
            //Quaternion rot = Trigger.ConstraintDictionary[bodyPart].Rotation;
            anchor.rotation = rot;
            anchor.position = pos;
        }
        else if (Trigger.ConstraintDictionary.ContainsKey(bodyPart))
        {
            anchor.position = Trigger.ConstraintDictionary[bodyPart].Position;

            float angle = Vector3.SignedAngle(Vector3.forward, Trigger.Transform.forward, Vector3.up);

            //Debug.Log("Adding constraint " + Trigger.transform.localEulerAngles);
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up) * Trigger.ConstraintDictionary[bodyPart].Rotation;
            //Quaternion rot = Trigger.ConstraintDictionary[bodyPart].Rotation;
            anchor.rotation = rot;
        }

        Undo.RecordObject(target, "Constrained " + bodyPart);

        PoseConstraintData data = new PoseConstraintData(anchor, bodyPart);

        PoseConstraintData[] constraints = Trigger.ConstraintData;
        ArrayUtility.Add<PoseConstraintData>(ref constraints, data);
        Trigger.ConstraintData = constraints;

        //Trigger.SavePoseConstraints();
    }

    private void RemoveConstraint(int id, Transform anchor = null)
    {
        Undo.RecordObject(target, "Removed Body Part");

        PoseConstraintData[] constraints = Trigger.ConstraintData;
        ArrayUtility.RemoveAt<PoseConstraintData>(ref constraints, id);
        Trigger.ConstraintData = constraints;

        //if (anchor != null)
        //    GameObject.DestroyImmediate(anchor.gameObject);
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

        Undo.RecordObject(Trigger.DefaultPoseData, "Moved " + bodyPart);

        pos -= Trigger.Position;
        pos /= Trigger.AvatarHeight;

        float angle = Vector3.SignedAngle(Vector3.forward, Trigger.Transform.forward, Vector3.up);
        pos = Quaternion.AngleAxis(-angle, Vector3.up) * pos;

        //Debug.Log("Updating " + bodyPart + " to POS : [" + pos + "] ROT : [" + rot + "]");

        Trigger.DefaultPoseData.UpdatePosition(bodyPart, pos);
        Trigger.DefaultPoseData.UpdateRotation(bodyPart, rot.eulerAngles);

        m_requiresRefresh = true;
    }

    #endregion

    #region Copying

    private bool m_draggingPose = false;

    public void DrawCopyPose()
    {
        Undo.RecordObject(target, "CopiedConstraints");

        Event evt = Event.current;
        Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));

        CheckDrag();

        GUI.color = m_draggingPose ? Color.green : Color.white;

        GUI.Box(drop_area, "Drop PoseTrigger or PoseData to Copy Constraints");

        GUI.color = Color.white;

        m_draggingPose = false;

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
                        if (CopyConstraints(dragged_object as PoseData))
                            return;

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

    private void CheckDrag()
    {
        m_draggingPose = false;

        foreach (Object dragged_object in DragAndDrop.objectReferences)
        {
            if (dragged_object is PoseData)
            {
                m_draggingPose = true;
                break;
            }

            GameObject trigger = dragged_object as GameObject;

            if (trigger == null || trigger.GetComponentInChildren<PoseTrigger>(true))
            {
                m_draggingPose = true;
                break;
            }
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

        for (int i = 0; i < pose.ConstraintData.Length; i++)
            constraints[i] = new PoseConstraintData(CopyTransformToLocal(pose.ConstraintData[i].Anchor), pose.ConstraintData[i].BodyPart);

        Trigger.ConstraintData = constraints;

        return true;
    }

    public bool CopyConstraints(PoseData pose)
    {
        if (pose == null)
            return false;

        Debug.Log("CopyingConstraints from pose");

        ClearConstraints();
        PopulateConstraints(pose);

        PoseOverrides overrides;
        if (Trigger.GetCurrentOverrides(out overrides))
        {
            overrides.Name = pose.name;

            if (overrides.HistoryList == null)
                overrides.HistoryList = new List<PoseData>(3);
            else if (overrides.HistoryList.Count >= 3)
                overrides.HistoryList.RemoveAt(0);

            overrides.HistoryList.Add(pose);
        }

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

        return newAnchor;
    }



    #endregion

    #region Pose Data Overrides

    private void DrawAllPoseOverrides()
    {
        GUILayout.BeginVertical("Edit PoseData Positions", GUI.skin.box);

        GUILayout.Space(20);

        DrawPoseOverride(PoseArchetype.SIT_CLOSED_LEG);
        DrawPoseOverride(PoseArchetype.SIT_OPEN_LEG);

        GUILayout.EndVertical();
    }

    private void DrawPoseOverride(PoseArchetype archetype)
    {
        GUILayout.BeginHorizontal(archetype.ToString(), GUI.skin.box);
        
        GUILayout.Space(20);

        List<PoseOverrides> poses;

        if (!Trigger.GetOverrides(archetype, out poses))
        {
            GUILayout.Label("Using Default");
            if (GUILayout.Button("Override"))
            {
                Trigger.AddPoseOverride(archetype, new PoseOverrides());
                //PopulateConstraints();
                //Trigger.SavePoseConstraints();
                //Trigger.SelectOverride(archetype, 0);
                return;
            }

            GUILayout.EndHorizontal();
            return;
        }

        GUILayout.BeginVertical(GUI.skin.box);
        //GUILayout.Space(20);

        GUILayout.BeginHorizontal();
        GUILayout.Label(archetype.ToString());
        if (GUILayout.Button("+", GUILayout.Width(25)))
        {
            Trigger.AddPoseOverride(archetype, new PoseOverrides());
            //PopulateConstraints(); 
            //Trigger.SavePoseConstraints();
        }
        GUILayout.EndHorizontal();

        for (int i = 0; i < poses.Count; i++)
        {
            GUILayout.Space(10);

            if (Trigger.IsOverrideSelected(archetype, i))
                GUI.color = Color.green;

            GUILayout.BeginHorizontal(archetype.ToString(), GUI.skin.box);

            if (GUILayout.Button(poses[i].Name))
            {
                Trigger.SelectOverride(archetype, i);
                RefreshView();
                GUILayout.EndHorizontal();
                break;
            }

            if (GUILayout.Button("-", GUILayout.Width(25)))
            {
                Trigger.RemovePoseOverride(archetype, i);
                GUILayout.EndHorizontal();
                break;
            }

            GUI.color = Color.white;
            GUILayout.EndHorizontal();
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    #endregion
}