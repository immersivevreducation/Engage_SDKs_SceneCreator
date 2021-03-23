using UnityEngine;
using UnityEditor;
using System.Net;
using System.IO;
using System;
using System.Xml;
using System.Security.Cryptography;

namespace AssetBundles
{
    [InitializeOnLoad]
    public class UpdateManager : EditorWindow
    {
        CreatorSDKUpdateHandler updateHandler;
        static bool isBatchMode;
        static bool updateInProgress = false;
        static bool automaticUpdatesEnabled = false;
        static bool packageUpToDate = true;
        static bool checkOnly = false;
        static bool autoupdate = true;
        static bool oldAutoUpdate;
        static bool initialPackageChecked = false;

        float defaultLabelWidth;
        readonly float guiLabelWidth = 160f;
        static string packageStatus = "";
        static readonly string _filepath = "CreatorSDKUpdate.zip";
        static readonly string _localManifestPath = "manifest.xml";
        static readonly string _xpathConfig = "packageData/autoupdate";
        static readonly string _xpathVersion = "packageData/checksum";
        static readonly string _packageUrl = "https://github.com/immersivevreducation/Engage_CreatorSDK/blob/internal_Sdk/CreatorSDKUpdate.zip?raw=true";
        static readonly string _manifestURL = "https://github.com/immersivevreducation/Engage_CreatorSDK/blob/internal_Sdk/Assets/ENGAGE_CreatorSDK/SDKUpdateVersion.txt?raw=true";
        

        private void OnEnable() 
        {
            updateHandler = new CreatorSDKUpdateHandler();
        }

        [MenuItem("Creator SDK/Check for updates")]
        public static void ShowUpdateWindow()
        {
            //packageStatus = "Checking for update";
            //checkOnly = true;
            //ImportPackage();
           UpdateManager updatewindow = GetWindow<UpdateManager>(false, "Update manager", true);
           updatewindow.minSize = new Vector2(200,400);
        }
        

        static UpdateManager()
        {
            EditorApplication.update += Initialize;
        }

        static void Initialize()
        {
            EditorApplication.update -= Initialize;
            if (!initialPackageChecked && !Application.isBatchMode)
            {
                if (!PlayerPrefs.HasKey("SDKAutoUpdate"))
                {
                    autoupdate = true;
                    PlayerPrefs.SetString("SDKAutoUpdate", "True");
                }
                else
                {
                    if (PlayerPrefs.GetString("SDKAutoUpdate") == "False")
                    {
                        autoupdate = false;
                    }
                }

                oldAutoUpdate = autoupdate;

                if (autoupdate)
                {
                    packageStatus = "Checking for update";
                    checkOnly = true;
                    CheckUpdateVersion();
                }
            }
        }

        
        private void OnGUI()
        {
            
            GUILayout.Label("ENGAGE Creator SDK");
            EditorGUILayout.Space();

            autoupdate = GUILayout.Toggle(autoupdate, "Check for SDK Updates automatically");

            if(autoupdate != oldAutoUpdate)
            {
                PlayerPrefs.SetString("SDKAutoUpdate", autoupdate.ToString());
                Debug.Log("Set auto-update to " + autoupdate);
                oldAutoUpdate = autoupdate;
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (GUILayout.Button("Check for Update") && !updateInProgress)
            {
                
                packageStatus = "Checking for update";
                checkOnly = true;
                CheckUpdateVersion();
            }

            EditorGUILayout.Space();

            if (!packageUpToDate)
            {
                if (GUILayout.Button("Update to latest version") && !updateInProgress)
                {
                    packageStatus = "Updating to latest version";
                    checkOnly = false;
                    DownloadUpdate();
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            GUILayout.Label(packageStatus);
        }


        private static void DownloadUpdate()
        {
            WebClient wc = new WebClient();
            Uri _uri = new Uri(_packageUrl);
            wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
            try
            {
                updateInProgress = true;
                wc.DownloadFileAsync(_uri, Application.dataPath.Replace("/Assets", "")+"/"+_filepath);
            }
            catch
            {
                throw new FileNotFoundException();
            }
        }
        private static void CheckUpdateVersion()
        {
            WebClient version_WC = new WebClient();
            Uri _uri = new Uri(_manifestURL);
            version_WC.DownloadFileCompleted += version_WC_DownloadFileCompleted;
            try
            {
                updateInProgress = true;
                version_WC.DownloadFileAsync(_uri, Application.dataPath.Replace("/Assets", "")+"/SDKUpdateVersion.txt");
            }
            catch
            {
                throw new FileNotFoundException();
            }
        }

        private static void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            updateInProgress = false;
            try
            {
                if (File.Exists(Application.dataPath.Replace("/Assets", "")+"/"+_filepath))
                {
                    CreatorSDKUpdateHandler.SDKUpdate(Application.dataPath.Replace("/Assets", ""), Application.dataPath.Replace("/Assets", "")+@"\SdkUpdate");
                    Debug.Log("Updating");
                    packageUpToDate=true;
                    // UpdateManager updatewindow = GetWindow<UpdateManager>(false, "Update manager", true);
                    // updatewindow.Close();
                }
            }
            catch
            {
                packageStatus = "Error Updating to latest version";
                throw e.Error;
            }
        }
        private static void version_WC_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            updateInProgress = false;
            try
            {
                string path1 = Application.dataPath+"/ENGAGE_CreatorSDK"+"/SDKUpdateVersion.txt";
                string path2 = Application.dataPath.Replace("/Assets", "")+"/SDKUpdateVersion.txt";
                Debug.Log("Checking for update");                
                if (File.Exists(path1))
                {
                    Debug.Log("Comparing SDK Versions...");
                    int currentSDKVersion =  Int32.Parse(System.IO.File.ReadAllText(path1));
                    int NewSDKVersion =  Int32.Parse(System.IO.File.ReadAllText(path2));

                    if (currentSDKVersion != NewSDKVersion)
                    {
                        Debug.Log("Update Found: Current = "+currentSDKVersion +" New = "+NewSDKVersion);
                        packageUpToDate=false;
                        packageStatus = "New Creator SDK Update Available!\n\nPlease click \"Update to Latest Version\" to stay up-to-date";
                        if (File.Exists(path2))
                        {
                            File.Delete(path2);
                        }
                    }
                    else
                    {
                        packageUpToDate=true;
                        packageStatus ="The SDK is up to date";
                        Debug.Log("No Update Found");
                    }
                }
                else
                {
                    Debug.Log("Local SDK version file not found");
                    packageUpToDate=false;
                    packageStatus = "New Creator SDK Update Available!\n\nPlease click \"Update to Latest Version\" to stay up-to-date";
                }
            }
            catch
            {
                packageStatus = "Error Updating to latest version";
                throw e.Error;
            }
        }
    }
}