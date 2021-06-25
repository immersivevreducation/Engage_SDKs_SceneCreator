#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

public class ReplaceWithPrefab : EditorWindow
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private bool applyPosition = true;
    [SerializeField] private bool applyRotation = true;
    [SerializeField] private bool applyScale = true;

    [SerializeField] private bool offsetOptions;
    [SerializeField] private Vector3 offsetPosition;
    [SerializeField] private Vector3 offsetRotation;
    [SerializeField] private Vector3 offsetScale;

    [MenuItem("Tools/Replace With Prefab")]
    static void CreateReplaceWithPrefab()
    {
        EditorWindow.GetWindow<ReplaceWithPrefab>();
    }

    private void OnGUI()
    {
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        applyPosition = EditorGUILayout.Toggle("Apply Position", applyPosition);
        applyRotation = EditorGUILayout.Toggle("Apply Rotation", applyRotation);
        applyScale = EditorGUILayout.Toggle("Apply Scale", applyScale);

        offsetOptions = EditorGUILayout.BeginFoldoutHeaderGroup(offsetOptions, "Offset Transform");

        if (offsetOptions)
        {
            offsetPosition = EditorGUILayout.Vector3Field("Offset Position", offsetPosition);
            offsetRotation = EditorGUILayout.Vector3Field("Offset Rotation", offsetRotation);
            offsetScale = EditorGUILayout.Vector3Field("Offset Scale", offsetScale);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();


        if (GUILayout.Button("Replace"))
        {
            var selection = Selection.gameObjects;

            for (var i = selection.Length - 1; i >= 0; --i)
            {
                var selected = selection[i];
                var prefabType = PrefabUtility.GetPrefabAssetType(prefab);
                GameObject newObject;

                if (prefabType != PrefabAssetType.NotAPrefab)
                {
                    newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    newObject.name = prefab.name + " (" + i + ")";
                }
                else
                {
                    newObject = Instantiate(prefab);
                    newObject.name = prefab.name + " (" + i + ")";
                }

                if (newObject == null)
                {
                    Debug.LogError("Error instantiating prefab");
                    break;
                }

                Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
                newObject.transform.SetParent(selected.transform.parent);

                newObject.transform.localPosition = (applyPosition ? selected.transform.localPosition : Vector3.zero) + offsetPosition;
                newObject.transform.localRotation = Quaternion.Euler((applyRotation ? selected.transform.localRotation.eulerAngles : Vector3.zero) + offsetRotation);
                newObject.transform.localScale = (applyScale ? selected.transform.localScale : Vector3.one) + offsetScale;

                newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
                Undo.DestroyObjectImmediate(selected);
            }
        }

        GUI.enabled = false;
        EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
    }
}
#endif