﻿using UnityEngine;
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
        public string cdnWinIFXLoc;
        public string cdnAndroidIFXLoc;
        public string cdniOSIFXLoc;

        public string cdnWinSceneLoc;
        public string cdnAndroidSceneLoc;
        public string cdniOSSceneLoc;
        public string projectWinLoc; 
        public string projectAndroidLoc;
        public string projectiOSLoc;
        public string unityEXELoc;
        /////////////Other tool Settings/////////////
        public string prefabPrefix;
        public string prefabAfix;
        public string currentIFXNum;
        public string thumbnailSavePath;
        public bool debugMode = false;
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

                    cdnWinIFXLoc = result.cdnWinIFXLoc;
                    cdnAndroidIFXLoc = result.cdnAndroidIFXLoc;
                    cdniOSIFXLoc = result.cdniOSIFXLoc;

                    cdnWinSceneLoc = result.cdnWinSceneLoc;
                    cdnAndroidSceneLoc = result.cdnAndroidSceneLoc;
                    cdniOSSceneLoc = result.cdniOSSceneLoc;

                    projectWinLoc = result.projectWinLoc;
                    projectAndroidLoc = result.projectAndroidLoc;
                    projectiOSLoc = result.projectiOSLoc;

                    unityEXELoc = result.unityEXELoc;

                    prefabPrefix = result.prefabPrefix;
                    prefabAfix = result.prefabAfix;

                    thumbnailSavePath = result.thumbnailSavePath;

                    //debugMode = result.debugMode;

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
            if (!string.IsNullOrEmpty(cdnProjectPath))
            {
                this.cdnWinIFXLoc = cdnProjectPath+"/engage_online_root/asset_bundles/effects/unity_2019_2/Windows";
                this.cdnAndroidIFXLoc = cdnProjectPath+"/engage_online_root/asset_bundles/effects/unity_2019_2/Android";
                this.cdniOSIFXLoc = cdnProjectPath+"/engage_online_root/asset_bundles/effects/unity_2019_2/iOS";

                this.cdnWinSceneLoc = cdnProjectPath+"/engage_online_root/asset_bundles/scenes/unity_2019_2/Windows";
                this.cdnAndroidSceneLoc = cdnProjectPath+"/engage_online_root/asset_bundles/scenes/unity_2019_2/Android";
                this.cdniOSSceneLoc = cdnProjectPath+"/engage_online_root/asset_bundles/scenes/unity_2019_2/iOS";
            }
            else
            {
                this.cdnWinIFXLoc = null;
                this.cdnAndroidIFXLoc = null;
                this.cdniOSIFXLoc = null;

                this.cdnWinSceneLoc = null;
                this.cdnAndroidSceneLoc = null;
                this.cdniOSSceneLoc = null;
            }           
            
            

            Debug.Log("Unity Path: "+unityEXELoc);
            Debug.Log("IFX CDN Project Path - Windows: "+cdnWinIFXLoc);
            Debug.Log("IFX CDN Project Path - Android: "+cdnAndroidIFXLoc);
            Debug.Log("IFX CDN Project Path - iOS: "+cdniOSIFXLoc);
            Debug.Log("Scene CDN Project Path - Windows: "+cdnWinSceneLoc);
            Debug.Log("Scene CDN Project Path - Android: "+cdnAndroidSceneLoc);
            Debug.Log("Scene CDN Project Path - iOS: "+cdniOSSceneLoc);
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