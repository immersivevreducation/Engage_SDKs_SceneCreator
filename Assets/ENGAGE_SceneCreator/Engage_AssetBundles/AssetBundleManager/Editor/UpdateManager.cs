using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Threading.Tasks;

namespace AssetBundles
{
    public class UpdateManager : EditorWindow
    {
        bool checkComplete = false;
        UnityEditor.PackageManager.Requests.AddRequest sdkUpdateRequest;
        string packageID = "com.ivre.engage_scenecreator_sdk";
        string url = "https://github.com/immersivevreducation/Engage_SDKs_SceneCreator/raw/master/Engage_SceneCreatorSDK.unitypackage";

        [MenuItem("SDK/Check for updates")]
        public static void ShowUpdateWindow()
        {
            GetWindow<UpdateManager>(false, "Update manager", true);
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Check for updates"))
            {
                sdkUpdateRequest = UnityEditor.PackageManager.Client.Add(packageID);
                checkComplete = true;
            }
            EditorGUILayout.Space();

            if (checkComplete)
                if (sdkUpdateRequest.IsCompleted)
                    Debug.Log("SDK update complete with result of: " + sdkUpdateRequest.Error.message);
        }
    }
}