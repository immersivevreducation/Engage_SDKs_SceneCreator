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

        [MenuItem("SDK/Check for updates")]
        public static void ShowUpdateWindow()
        {
            GetWindow<UpdateManager>(false, "Update manager", true);
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Check for updates"))
            {
                sdkUpdateRequest = UnityEditor.PackageManager.Client.Add("https://github.com/immersivevreducation/Engage_SDKs_SceneCreator/raw/master/Engage_SceneCreatorSDK.unitypackage");
                checkComplete = true;
            }
            EditorGUILayout.Space();

            if (sdkUpdateRequest.IsCompleted && checkComplete)
                Debug.Log("SDK update complete with result of: " + sdkUpdateRequest.Result);
        }
    }
}