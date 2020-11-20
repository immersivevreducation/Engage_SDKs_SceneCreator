using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
/////////////////////////////////////
//Check for asset labels on prefabs both in and out of selected bundle folder.
//mesh combine before build = less draw calls?
////////////////////////////////////
namespace IFXTools {
    public class IFXToolsQualityCheckTool : EditorWindow
    {
        IFXBundleTools bundleTool;
        public IFXToolsQualityCheckTool()
        {
            
        }
        public void Init(IFXBundleTools BundleToolIN)
        {
            bundleTool = BundleToolIN; 
        }
        public List<QACheckListItem> checkList= new List<QACheckListItem>();
        public List<UnityEngine.Object> bundleGettingChecked;
        
        //UI Varaiables
        Vector2 scrollPos;
        private void OnGUI() 
        {
            IFXToolsQualityCheckToolUI();
            this.Repaint();
        }
        void IFXToolsQualityCheckToolUI()
        {
            Rect groupRect=new Rect(5, 25, Screen.width-20, Screen.height-10);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(groupRect.width), GUILayout.Height(groupRect.height-175));
            //GUI.BeginGroup(groupRect);
                                                    
            foreach (QACheckListItem item in checkList)
            {
                
                GUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Name: "+item.rootGameObject.name, EditorStyles.boldLabel);
                foreach (string error in item.errors)
                {
                   GUILayout.Label("Errors: "+error);
                }
               
                if (GUILayout.Button("open prefab"))
                {
                    AssetDatabase.OpenAsset(item.rootGameObject);
                    
                                                     
                    this.Close();
                    //Debug.Log("Error window closed");
                }

                if (GUILayout.Button("Auto Fix prefab"))
                {
                    FixPrefabs(item.rootGameObject);                    
                    //Debug.Log("Log in test");
                }
                
                
                GUILayout.EndVertical();
                GUILayout.Label(" ");
            }
            
///////     /////////          /////////        ////////           /////////////           ////////////            ///////////
            GUILayout.Label(" ");
            GUILayout.Label(" ");
                     
        
            //GUI.EndGroup();
            EditorGUILayout.EndScrollView(); 

            if (GUI.Button(new Rect(10, groupRect.height - 175, groupRect.width, 50), "Recheck Bundle"))
            {
                BundleQualityCheck(bundleGettingChecked);
                //this.Close();
            }        

            if (GUI.Button(new Rect(10, groupRect.height - 125, groupRect.width, 50), "Ignore Remaining Issues"))
            {
                bundleTool.BuildSelectedBundle(
                bundleGettingChecked,
                bundleTool.windowsBuildYesNo,
                bundleTool.androidBuildYesNo,
                bundleTool.iOSBuildYesNo,
                bundleTool.autoGitYesNo,
                true,
                bundleTool.gitCommitM);
                this.Close();
            }

            if (GUI.Button(new Rect(10, groupRect.height - 75, groupRect.width, 50), "Cancel"))
            {
                this.Close();
            }
        
    
        }
        
        
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////// -----Functions-----///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        
        public  void OpenQAErrorWindow()
        {
            //set the checklist of the chechlist window to match the one instanced in quicktools
            //qualityCheckListWindow.checkList = ifxErrorList;
            this.minSize=new Vector2(500,200);
            this.Show();
        }
        public static List<string> PrefabQualityCheck(GameObject gameObjectToCheck)
        {
            
            List<string> errorsFound = new List<string>();
            
            
            if (gameObjectToCheck.transform.position!=Vector3.zero | gameObjectToCheck.transform.rotation != Quaternion.identity | gameObjectToCheck.transform.localScale != new Vector3(1,1,1))
            {
                Debug.Log("Prefab Root not zeroed: ");
                
                errorsFound.Add("Prefab Root not zeroed");
            }
            
            if(QuickTools.ObjectPivotCenteredCheck(gameObjectToCheck) == false)
            {
                Debug.Log("Object appears off center: ");
                
                errorsFound.Add("Object Appears OffCenter");
                
            }
            if(gameObjectToCheck.GetComponentInChildren(typeof(Camera),true) != null)
            {
                Debug.Log("Camera component found within ");
                
                errorsFound.Add("Camera component found within: ");
                
            
            }

            if(gameObjectToCheck.GetComponentInChildren(typeof(Collider),true) != null)
            {
                Debug.Log("Collider component found within ");
                
                errorsFound.Add("Collider component found within: ");
                
                
            }

            if(gameObjectToCheck.GetComponentInChildren(typeof(Light),true) != null)
            {
                Debug.Log("Light component found within ");
                
                errorsFound.Add("Light component found within: ");
                
            }

            if(gameObjectToCheck.GetComponentInChildren(typeof(LODGroup),true) != null)
            {
                Debug.Log("LODGroup component found within ");
                
                errorsFound.Add("LODGroup component found within: ");                
            }

            if(gameObjectToCheck.GetComponentInChildren(typeof(AudioSource),true) != null && gameObjectToCheck.GetComponentInChildren(typeof(AudioDefaultScale),true) == null)
            {
                Debug.Log("LODGroup component found within ");
                
                errorsFound.Add("AudioSource component requires \"AudioDefaultScale\" script: ");                
            }
                        
            return errorsFound;
            }

        
        public bool BundleQualityCheck(List<UnityEngine.Object> inputFolder)// Also clears asset labels from prefabs while it's at it.
        {
            bundleGettingChecked = new List<UnityEngine.Object>();
            bool QualityCheck = true;
            checkList.Clear();
            foreach (UnityEngine.Object item in inputFolder)
            {
                Debug.Log(item.name);
                bundleGettingChecked.Add(item);
                string path = AssetDatabase.GetAssetPath(item);
                
                
                DirectoryInfo dirInfo = new DirectoryInfo(path);
                FileInfo[] fileInf = dirInfo.GetFiles("*.prefab");

                //loop through directory loading the game object and checking if it has the component you want
                if (fileInf!=null)
                {
                    
                    foreach (FileInfo file in fileInf)
                    {
                        
                        
                        string fullPath = file.FullName.Replace(@"\","/");
                        string assetPath = "Assets" + fullPath.Replace(Application.dataPath, "");
                        GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;

                        
                        

                        if(prefab!= null)
                        {
                            List<string> errors = PrefabQualityCheck(prefab);
                            if (errors.Count>0)
                            {
                                checkList.Add(new QACheckListItem(prefab,errors));
                                QualityCheck = false;
                                OpenQAErrorWindow();
                            } 
                        }
                    }
                }
                else
                {
                    Debug.Log("Failed to find prefabs in bundle");
                }
            }
                
                
                
            return QualityCheck;
            
        }
        
        static void FixPrefabs(GameObject inputGO)
        {
            GameObject prefabRoot = inputGO;
            string assetPath="";
        
            assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefabRoot);
            Debug.Log("Asset path is: "+assetPath);
            if (assetPath!="")
            {
                prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);
            }

            Camera[] cameras = prefabRoot.GetComponentsInChildren<Camera>();
            Collider[] colliders = prefabRoot.GetComponentsInChildren<Collider>();
            Light[] lights = prefabRoot.GetComponentsInChildren<Light>();
            LODGroup[] lodGroups = prefabRoot.GetComponentsInChildren<LODGroup>();

            foreach (Camera item in cameras)
            {
                DestroyImmediate(item);
            }
            foreach (Collider item in colliders)
            {
                DestroyImmediate(item);
            }
            foreach (Light item in lights)
            {
                DestroyImmediate(item);
            }
            foreach (LODGroup item in lodGroups)
            {
                DestroyImmediate(item);
            }

            PrefabZeroTransforms(prefabRoot);

            

            if (assetPath!="")
            {
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
            }
            // center ifx opens prefab so it's better to keep it after the rest to avoid conflict then prefab is opened for above changes
            if (!QuickTools.ObjectPivotCenteredCheck(inputGO))
            {
                QuickTools.CenterIFX(inputGO);
            }
        }

        static void PrefabZeroTransforms(GameObject inputGO)
        {
            inputGO.transform.position = Vector3.zero;
            inputGO.transform.rotation = Quaternion.identity;
            inputGO.transform.localScale = new Vector3(1,1,1);
        }
}
    
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////// -----Check list class-----///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////// -----Check list Item class-----///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    public class QACheckListItem
    {
        public GameObject rootGameObject;
        public List<string> errors;
        public QACheckListItem(GameObject goIN,List<string> errorsIN)
        {
            rootGameObject = goIN;
            errors = errorsIN;
        }
    }                   
}
    
        
