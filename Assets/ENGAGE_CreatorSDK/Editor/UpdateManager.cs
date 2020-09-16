using UnityEngine;
using UnityEditor;
using System.Net;
using System.IO;
using System;
using System.Xml;

namespace AssetBundles
{
    public class UpdateManager : EditorWindow
    {
        bool checkComplete = false;
        bool updateComplete = false;
        bool updateInProgress = false;
        bool automaticUpdatesEnabled = false;
        bool packageUpToDate = false;

        float defaultLabelWidth;
        readonly float guiLabelWidth = 160f;
        readonly string _filepath = "CreatorSDK.unitypackage";
        readonly string _packageUrl = "https://github.com/immersivevreducation/Engage_CreatorSDK/blob/master/CreatorSDK.unitypackage?raw=true";
        readonly string _manifestUrl = "https://github.com/immersivevreducation/Engage_CreatorSDK/raw/master/Assets/ENGAGE_CreatorSDK/manifest.json";
        readonly string _tempManifestPath = "manifest.xml";
        readonly string _localManifestPath = "Assets\\ENGAGE_CreatorSDK\\manifest.xml";

        [MenuItem("Creator SDK/Check for updates")]
        public static void ShowUpdateWindow()
        {
            GetWindow<UpdateManager>(false, "Update manager", true);
        }

        private void OnGUI()
        {
            GUILayout.Label("Creator SDK package may not be up to date with latest version.");
            EditorGUILayout.Space();
            if (GUILayout.Button("Check for updates") && !updateInProgress)
            {
                //updateInProgress = true;
                //checkComplete = true;
                //ImportPackage();
                bool i = PackageIsUpToDate();
            }
            EditorGUILayout.Space();
            defaultLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = guiLabelWidth;
            automaticUpdatesEnabled = EditorGUILayout.Toggle("Enabled automatic updates", false);
            EditorGUIUtility.labelWidth = defaultLabelWidth;

            if (checkComplete)
            {
                if (updateComplete)
                {
                    GUILayout.Label("Creator SDK updated to latest version!");
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
            Uri _uri = new Uri(_packageUrl);
            wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
            try
            {
                wc.DownloadFileAsync(_uri, "CreatorSDK");
            }
            catch
            {
                throw new FileNotFoundException();
            }
        }

        private bool PackageIsUpToDate()
        {
            WebClient wc = new WebClient();
            Uri _uri = new Uri(_manifestUrl);
            wc.DownloadFileCompleted += Wc_DownloadManifestCompleted;
            try
            {
                wc.DownloadFileAsync(_uri, "manifest");
            }
            catch
            {
                throw new FileNotFoundException();
            }
            return true;
        }

        private void Wc_DownloadManifestCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            string _manifestData;
            try
            {
                if (File.Exists(_tempManifestPath))
                {
                    _manifestData = File.ReadAllText(_tempManifestPath);
                    GetVersionNumberFromXML(_manifestData);
                }
            }
            catch
            {
                throw e.Error;
            }

            packageUpToDate = true;
        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            updateInProgress = false;
            try
            {
                if (File.Exists(_filepath))
                    FileUtil.DeleteFileOrDirectory(_filepath);
                FileUtil.MoveFileOrDirectory("CreatorSDK", _filepath);

                if (File.Exists(_filepath))
                {
                    AssetDatabase.ImportPackage(_filepath, false);
                    updateComplete = true;
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
            catch
            {
                throw e.Error;
            }
        }

        private float GetVersionNumberFromXML(string _s)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(_s);
            string xpath = "packageData/version";
            var node = xDoc.SelectSingleNode(xpath);

            return float.Parse(node.InnerText);
        }
    }
}