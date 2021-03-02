using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Engage.Avatars.Poses;

[CustomEditor(typeof(LVR_SitTrigger)), CanEditMultipleObjects]
public class SitTriggerInspector : Editor
{
    private bool editMode;

    private bool hndlMeshesCreated;

    private LVR_SitTrigger tgt;
    private PoseTrigger pose;

    public Transform lfHandle, rfHandle, sHandle;

    private void OnEnable()
    {
        tgt = (LVR_SitTrigger)target;

        if (tgt.HasAdvancedPose)
            pose = tgt.AdvancedPose;
    }
    private void OnDisable()
    {
        if (hndlMeshesCreated)
            DestroyHandleMeshes();
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        editMode = EditorGUILayout.Toggle("Edit Mode", editMode);

        if (pose == null)
        {
            if (GUILayout.Button("Add Advanced Pose"))
            {
                pose = tgt.CreateAdvancedPose();
            }
        }
    }
    protected virtual void OnSceneGUI()
    {
        if (editMode && pose == null)
        {
            if (hndlMeshesCreated == false)
            {
                CreateHandleMeshes();
            }
            EditorGUI.BeginChangeCheck();
            sHandle.position = 
                Handles.PositionHandle(tgt.floorCollider.position + 
                tgt.floorCollider.TransformDirection(tgt.m_seatPosition), tgt.floorCollider.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(tgt, "Change Seat Position");
                tgt.m_seatPosition = 
                    tgt.floorCollider.InverseTransformDirection(sHandle.position - tgt.floorCollider.position);
            }
            EditorGUI.BeginChangeCheck();
            lfHandle.position = 
                Handles.PositionHandle(tgt.floorCollider.position + 
                tgt.floorCollider.TransformDirection(tgt.m_leftFootPos), tgt.floorCollider.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(tgt, "Change Left Foot Position");
                tgt.m_leftFootPos = 
                    tgt.floorCollider.InverseTransformDirection(lfHandle.position - tgt.floorCollider.position);
            }
            EditorGUI.BeginChangeCheck();
            rfHandle.position = 
                Handles.PositionHandle(tgt.floorCollider.position + 
                tgt.floorCollider.TransformDirection(tgt.m_rightFootPos), tgt.floorCollider.rotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(tgt, "Change Right Foot Position");
                tgt.m_rightFootPos = 
                    tgt.floorCollider.InverseTransformDirection(rfHandle.position - tgt.floorCollider.position);
            }

        }
        else
        {
            if (hndlMeshesCreated)
            {
                DestroyHandleMeshes();
            }
        }
    }

    void CreateHandleMeshes()
    {
        lfHandle = new GameObject("Left Foot Handle").transform;
        lfHandle.hideFlags = HideFlags.HideAndDontSave;
        lfHandle.position = tgt.floorCollider.position + tgt.floorCollider.TransformDirection(tgt.m_leftFootPos);
        lfHandle.gameObject.AddComponent<MeshFilter>().mesh = Resources.Load<GameObject>("Prefab_Mesh_FootL").GetComponent<MeshFilter>().sharedMesh;

        rfHandle = new GameObject("Right Foot Handle").transform;
        rfHandle.hideFlags = HideFlags.HideAndDontSave;
        rfHandle.position = tgt.floorCollider.position + tgt.floorCollider.TransformDirection(tgt.m_rightFootPos);
        rfHandle.gameObject.AddComponent<MeshFilter>().mesh = Resources.Load<GameObject>("Prefab_Mesh_FootR").GetComponent<MeshFilter>().sharedMesh;

        sHandle = new GameObject("Seat Handle").transform;
        sHandle.hideFlags = HideFlags.HideAndDontSave;
        sHandle.position = tgt.floorCollider.position + tgt.floorCollider.TransformDirection(tgt.m_seatPosition);
        sHandle.gameObject.AddComponent<MeshFilter>().mesh = Resources.Load<GameObject>("Prefab_Mesh_Seat").GetComponent<MeshFilter>().sharedMesh;

        lfHandle.eulerAngles = rfHandle.transform.eulerAngles = sHandle.transform.eulerAngles = tgt.floorCollider.eulerAngles;
        lfHandle.SetParent(tgt.floorCollider);
        rfHandle.SetParent(tgt.floorCollider);
        sHandle.SetParent(tgt.floorCollider);

        lfHandle.gameObject.AddComponent<MeshRenderer>().sharedMaterial =
            rfHandle.gameObject.AddComponent<MeshRenderer>().sharedMaterial =
            sHandle.gameObject.AddComponent<MeshRenderer>().sharedMaterial =
            Resources.Load<Material>("SeatPrevis");
        hndlMeshesCreated = true;
    }

    void DestroyHandleMeshes()
    {
        DestroyImmediate(lfHandle.gameObject);
        DestroyImmediate(rfHandle.gameObject);
        DestroyImmediate(sHandle.gameObject);
        hndlMeshesCreated = false;
    }
}
