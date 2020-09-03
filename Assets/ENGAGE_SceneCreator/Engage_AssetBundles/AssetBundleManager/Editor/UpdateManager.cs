using UnityEngine;
using UnityEditor;

namespace AssetBundles
{
    public class UpdateManager : EditorWindow
    {
        bool checkComplete = false;

        [MenuItem("SDK/Check for updates")]
        public static void ShowUpdateWindow()
        {
            GetWindow<UpdateManager>(false, "Update manager", true);
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Check for updates"))
            {
                checkComplete = true;
            }
            EditorGUILayout.Space();
            //UnityEditor.PackageManager.Client.Add("updated package");    
            if (checkComplete)
                GUILayout.Label("Up to date!");
        }
    }
}