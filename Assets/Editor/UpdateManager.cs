using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System;

namespace AssetBundles
{
    public class UpdateManager : EditorWindow
    {
        bool checkComplete = false;
        UnityEditor.PackageManager.Requests.AddRequest sdkUpdateRequest;
        string packageID = "com.ivre.engage_scenecreator_sdk";
        string _url = "SceneCreatorSDK.unitypackage";

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
                ImportPackage();
            }
            EditorGUILayout.Space();

            if (checkComplete)
            {
            }

        }

        private void ImportPackage()
        {
            WebClient wc = new WebClient();
            Uri _uri = new Uri("https://github.com/immersivevreducation/Engage_SDKs_SceneCreator/blob/master/engage_scenecreator_sdk.unitypackage?raw=true");
            wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
            wc.DownloadFileAsync(_uri, "SceneCreatorSDK");
        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (File.Exists("SceneCreatorSDK.unitypackage"))
                FileUtil.DeleteFileOrDirectory("SceneCreatorSDK.unitypackage");
            FileUtil.MoveFileOrDirectory("SceneCreatorSDK", "SceneCreatorSDK.unitypackage");
            Debug.Log("Package download completed");
            if (File.Exists(_url))
            {
                Debug.Log("Package exists");
                AssetDatabase.ImportPackage(_url, false);
            }
        }
    }
}