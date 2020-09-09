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
        bool updateComplete = false;
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
            GUILayout.Label("SceneCreatorSDK package may not be up to date with latest version.");
            EditorGUILayout.Space();
            if (GUILayout.Button("Check for updates"))
            {
                checkComplete = true;
                ImportPackage();
            }
            EditorGUILayout.Space();

            if (checkComplete)
            {
                if (updateComplete)
                {
                    GUILayout.Label("SceneCreator updated to latest version!");
                }
                else
                {
                    GUILayout.Label("Downloading package from server, this may take several moments...");
                }
            }

        }

        private void ImportPackage()
        {
            WebClient wc = new WebClient();
            Uri _uri = new Uri("https://github.com/immersivevreducation/Engage_SDKs_SceneCreator/blob/master/engage_scenecreator_sdk.unitypackage?raw=true");
            wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
            try
            {
                wc.DownloadFileAsync(_uri, "SceneCreatorSDK");
            }
            catch
            {
                throw new FileNotFoundException();
            }
        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
            {
                if (File.Exists("SceneCreatorSDK.unitypackage"))
                    FileUtil.DeleteFileOrDirectory("SceneCreatorSDK.unitypackage");
                FileUtil.MoveFileOrDirectory("SceneCreatorSDK", "SceneCreatorSDK.unitypackage");

                Debug.Log("Package download completed");
                if (File.Exists(_url))
                {
                    Debug.Log("Package exists");
                    AssetDatabase.ImportPackage(_url, false);
                    updateComplete = true;
                }
            }
            catch
            {
                throw new FileNotFoundException();
            }
        }
    }
}