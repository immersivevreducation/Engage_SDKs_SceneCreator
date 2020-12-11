using UnityEngine;
using UnityEditor;
using System.IO;

namespace IFXTools
{
    public class IFXToolsUserSettings
    {
        private static readonly IFXToolsUserSettings _instance = new IFXToolsUserSettings();
        private IFXToolsUserSettings()
        {
            
        }
        public static IFXToolsUserSettings GetUserSettings()
        {
            return _instance;
        }

        //Editable variables
        public string cdnProjectPath;
        public string cdnWinLoc;
        public string cdnAndroidLoc;
        public string cdniOSLoc;
        public string projectWinLoc; 
        public string projectAndroidLoc;
        public string projectiOSLoc;
        public string unityEXELoc;
        /////////////Other tool Settings/////////////
        public string prefabPrefix;
        public string prefabAfix;
        public string currentIFXNum;
        public string thumbnailSavePath;
        public bool debugMode;
        public string cTCode;
        
        string settingsFilePath = Application.dataPath.Replace("/Assets", "") + "/UserSettings.json";
        public void LoadUserSettings()
        {
            if (File.Exists(settingsFilePath))
            {
                var textFile = File.ReadAllText(settingsFilePath);
                IFXToolsUserSettings result = JsonUtility.FromJson<IFXToolsUserSettings>(textFile);
                if (result !=null)
                {
                    cdnProjectPath= result.cdnProjectPath;
                    cdnWinLoc = result.cdnWinLoc;
                    cdnAndroidLoc = result.cdnAndroidLoc;
                    cdniOSLoc = result.cdniOSLoc;

                    projectWinLoc = result.projectWinLoc;
                    projectAndroidLoc = result.projectAndroidLoc;
                    projectiOSLoc = result.projectiOSLoc;

                    unityEXELoc = result.unityEXELoc;

                    prefabPrefix = result.prefabPrefix;
                    prefabAfix = result.prefabAfix;

                    thumbnailSavePath = result.thumbnailSavePath;

                    debugMode = result.debugMode;

                    cTCode = result.cTCode;
                    
                }
                else
                {
                    Debug.Log("No settings found at : "+settingsFilePath);
                }
            }
            else
            {
                SettingsAutoSetup();
                SaveUserSettings();
            }                   
        }
        public void SaveUserSettings()
        {
            string json = JsonUtility.ToJson(this);
            File.WriteAllText(settingsFilePath, json);
            Debug.Log("Saving: " + json);
        }

        

        public void SettingsAutoSetup()
        {
            this.unityEXELoc= EditorApplication.applicationPath;

            this.projectWinLoc = Application.dataPath;
            this.projectWinLoc = this.projectWinLoc.Replace("/Assets", "");

            this.projectAndroidLoc = this.projectWinLoc+"/IFXBuildToolProjects/Android";
            this.projectiOSLoc = this.projectWinLoc+"/IFXBuildToolProjects/iOS";           
            
            this.cdnWinLoc = cdnProjectPath+"/engage_online_root/asset_bundles/effects/unity_2019_2/Windows";
            this.cdnAndroidLoc = cdnProjectPath+"/engage_online_root/asset_bundles/effects/unity_2019_2/Android";
            this.cdniOSLoc = cdnProjectPath+"/engage_online_root/asset_bundles/effects/unity_2019_2/iOS";

            Debug.Log("Unity Path: "+unityEXELoc);
            Debug.Log("CDN Project Path - Windows: "+cdnWinLoc);
            Debug.Log("CDN Project Path - Android: "+cdnAndroidLoc);
            Debug.Log("CDN Project Path - iOS: "+cdniOSLoc);
            Debug.Log("Project Path: "+projectWinLoc);
            Debug.Log("Android Project Path: "+projectAndroidLoc);
            Debug.Log("iOS Project Path: "+projectiOSLoc);
        }
        public bool CTMode()
        {
            bool activateCT = false;
            if (this.cTCode == "content_team")
            {
                activateCT = true;
            }
            return activateCT;
        }               
    }
}