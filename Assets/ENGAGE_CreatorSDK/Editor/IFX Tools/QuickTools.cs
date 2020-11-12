using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.IO;
using System.Collections.Generic;






////////////////////////////////////////////////////////////////////TO DO//////////////////////////////////////////////////////////
//1. check for bug making portrait aspect ration images with image plane creater



///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace IFXTools{
    public class QuickTools : EditorWindow, IBundleToolsParentWindow
    {
        
        IFXToolsQualityCheckTool qualityCheckListWindow;
        
        IFXBundleTools bundleTools;
        //List<IFXQualityCheckListItem> ifxErrorList = new List<IFXQualityCheckListItem>();
        IFXToolsUserSettings userSettings;

        IFXThumbnailTool thumbnailToolInstance;  

        bool passedQualityCheck;
        
        string folderName;

        public bool buildQACheckOverride{get; set;}
        public string gitCommitM{get; set;}
        public bool windowsBuildYesNo{get; set;}
        public bool androidBuildYesNo {get; set;}
        public bool iOSBuildYesNo {get; set;}
        public bool autoGitYesNo {get; set;}

        bool altCenterMethod {get; set;}
        
        string destinationFolder;
        List<string> bundlesBuiltWin = new List<string>();
        List<string> bundlesBuiltAndroid = new List<string>();
        
        int selGridInt=0;
        Vector2 scrollPos;
        
        ////////////////////////////////////////////////////////SET THE IFX NUM HERE///////////////////////////////////////////////////////
        string ifxNum ="ifx3";
        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [MenuItem("Assets/QuickTools")]
        [MenuItem("Creator SDK/Bundle QuickTools")]
        static void Init()
        {
            
            EditorWindow window = GetWindow(typeof(QuickTools));
            //window.minSize=new Vector2(500,600);

            window.Show();

        }
        void OnDestroy()
        {
            //Debug.Log("Destroyed...");
            if (thumbnailToolInstance.ifxObject !=null)
            {
                DestroyImmediate(thumbnailToolInstance.ifxObject, true);
            }
            
        }
        
        void OnEnable()
        {
            userSettings = IFXToolsUserSettings.GetUserSettings(); //fix
            userSettings.LoadUserSettings();
            thumbnailToolInstance = new IFXThumbnailTool();
            //MyCustomWindow window = (IFXBundleTools )ScriptableObject.CreateInstance(typeof(IFXBundleTools ));
            bundleTools = (IFXBundleTools )ScriptableObject.CreateInstance(typeof(IFXBundleTools )); //fix
            bundleTools.Init(this,userSettings);
            //Set defualt checkmarks
            buildQACheckOverride=false;
            windowsBuildYesNo =true;
            androidBuildYesNo =true;
            altCenterMethod=false;
            if (userSettings.CTMode())
            {
                autoGitYesNo =true;
            }
            else
            {
                autoGitYesNo =false;
            }
            
            
        }
        void OnGUI()
        {
            MainUI();            
        }

    /////////////////////////////////////// -----UI-----///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void MainUI()
        {
            string[] selStrings = new string[] {"Build Bundles","Settings"};
            if (userSettings.CTMode())
            {
                selStrings = new string[] {"Build Bundles", "Prefab Tools", "Organising Tools","Creation Tools","Settings"};
            }
            
            
            
            Rect buttonGroupRect=new Rect(5, 25, Screen.width / 4, Screen.height-100);
            selGridInt = GUI.SelectionGrid(buttonGroupRect, selGridInt, selStrings, 1);//TAb switching controlls
            Rect groupRect=new Rect(buttonGroupRect.width+ 10, 0, Screen.width-buttonGroupRect.width, Screen.height);
            Rect subToolsGroupRect=new Rect(5, 25, buttonGroupRect.width*3, Screen.height-10);
            GUI.BeginGroup(groupRect);
            
                                                    
            if (selGridInt==0)
            {
                EditorGUILayout.BeginVertical(EditorStyles.colorField, GUILayout.Width(buttonGroupRect.width*3-10));
                BuildSelectedBundleWindowUI();
                EditorGUILayout.EndVertical();
                
            }
            //Prefab Tools
            if (selGridInt==1)
            {
                
                if (userSettings.CTMode())
                {
                    
                
                    EditorGUILayout.BeginVertical(EditorStyles.colorField, GUILayout.Width(buttonGroupRect.width*3-10));
                    EmptyPrefabWindowUI();
                    EditorGUILayout.EndVertical();
                    
                    EditorGUILayout.LabelField(" ");
                    
                    EditorGUILayout.BeginVertical(EditorStyles.colorField, GUILayout.Width(buttonGroupRect.width*3-10));
                    BatchPrefabWindowUI();
                    EditorGUILayout.EndVertical();
                
                    EditorGUILayout.LabelField(" ");
                    
                    EditorGUILayout.BeginVertical(EditorStyles.colorField, GUILayout.Width(buttonGroupRect.width*3-10));
                    AutoPivotUI();
                    EditorGUILayout.EndVertical();
                }
              
                   
            }
            //Organization Tools
            if (selGridInt==2)
            {              
                
                EditorGUILayout.BeginVertical(EditorStyles.colorField, GUILayout.Width(buttonGroupRect.width*3-10));

                NewDependenciesWindowUI();

                EditorGUILayout.EndVertical();
                
                EditorGUILayout.LabelField(" ");                
            }
            //Creation Tools
            if (selGridInt==3)
            {
                
                
                EditorGUILayout.BeginVertical(EditorStyles.colorField, GUILayout.Width(buttonGroupRect.width * 3 - 10));

                CreateImagePlaneUI();

                EditorGUILayout.EndVertical();
                

                EditorGUILayout.LabelField(" ");
                
                EditorGUILayout.BeginVertical(EditorStyles.colorField, GUILayout.Width(buttonGroupRect.width * 3 - 10));

                AnimCNTRLFromClipsWindowUI();

                EditorGUILayout.EndVertical();
               

                
                EditorGUILayout.BeginVertical(EditorStyles.colorField, GUILayout.Width(buttonGroupRect.width * 3 - 10));
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                ThumbnailToolUI();
                EditorGUILayout.EndScrollView();  

                EditorGUILayout.EndVertical();
                
                
            }
            if (userSettings.CTMode())
                {
                    if (selGridInt==4)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.colorField, GUILayout.Width(buttonGroupRect.width*3-10));
                        SettingsWindowUI();
                        userSettings.cTCode = EditorGUILayout.TextField("",userSettings.cTCode);
                        EditorGUILayout.EndVertical();
                        
                    }
            }
            else
            {
                //Settings Window
                if (selGridInt==1)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.colorField, GUILayout.Width(buttonGroupRect.width*3-10));
                    SettingsWindowUI();
                    userSettings.cTCode = EditorGUILayout.TextField("",userSettings.cTCode);
                    EditorGUILayout.EndVertical();
                    
                }
            }
            
            
            GUI.EndGroup();

                           
        }

        public void CloseWindow()
        {
            Debug.Log("tried to clsoe ");
            this.Close();
        }
        private void ThumbnailToolUI()
        {
            EditorGUILayout.LabelField("IFX Thumbnail Creation Tool");
            GUILayout.Label(thumbnailToolInstance.previewImage, GUILayout.Width(500), GUILayout.Height(281));
            var thumbnailPreviewRect = GUILayoutUtility.GetLastRect();
            if (GUILayout.Button("Load Thumbnail Scene"))
                {
                    if (EditorUtility.DisplayDialog("WARNING!", "Unsaved work in  the current scene will be lost", "Load IFX Thumbnail Scene", "Cancel"))
                    {
                        EditorSceneManager.OpenScene("Assets/Scenes/IFX_Thumbnail_Scene.unity");
                    }
                
                }
            if (GUILayout.Button("Load Object for camera"))
            {
                if (thumbnailToolInstance.ifxObject != Selection.activeGameObject)
                {
                    DestroyImmediate(thumbnailToolInstance.ifxObject, true);
                }
                if (Selection.activeObject is GameObject)
                {                    
                    GameObject obj = Selection.activeObject as GameObject;
                    thumbnailToolInstance.ifxObject = (GameObject)Instantiate(obj, new Vector3(0, 0, 0), Quaternion.identity);
                    thumbnailToolInstance.ThumbnailSetup(thumbnailToolInstance.ifxObject);                    
                }
                else
                {
                    Debug.Log("Select a GameObject object first");
                }
            }         
            if (GUILayout.Button("Reset Camera"))
            {
                thumbnailToolInstance.ResetCameraSettings();
            }
            if (GUILayout.Button("Auto Camera - This is a WIP"))
            {
                thumbnailToolInstance.AutoCamera(Selection.activeGameObject, thumbnailToolInstance.cameraObject.transform);
            }
            EditorGUILayout.LabelField(" ");
            if (GUILayout.Button("Save Thumbnail"))
            {
                thumbnailToolInstance.SaveThumbnail(Application.dataPath + "/IFX_Thumbnail_Assets/Saved_Thumbnails/");
            }
            //if the camera still exists, Update the preview
            if (thumbnailToolInstance.cameraObject)
            {
                thumbnailToolInstance.ThumbnailToolControlsUI();
                thumbnailToolInstance.UpdatePreviewImage();
            }            
        }

        void SettingsWindowUI()
        {
            
            
            
            
            // userSettings.cdnAndroidLoc = EditorGUILayout.TextField("Android Folder of CDN: ",userSettings.cdnAndroidLoc);
            // if (GUILayout.Button("Android CDN Folder - Browse"))
            // {
            //     userSettings.cdnAndroidLoc = EditorUtility.OpenFolderPanel("Select Android CDN folder", "", "");
                
            // }
            // userSettings.cdniOSLoc = EditorGUILayout.TextField("iOS Folder of CDN: ",userSettings.cdniOSLoc);
            // if (GUILayout.Button("iOS CDN Folder - Browse"))
            // {
            //     userSettings.cdniOSLoc = EditorUtility.OpenFolderPanel("Select iOS CDN folder", "", "");
                
            // }
            //EditorGUILayout.LabelField("Set the paths to your IFX Project folders");
            // userSettings.projectWinLoc = EditorGUILayout.TextField("Windows Project Folder: ",userSettings.projectWinLoc);
            // if (GUILayout.Button("Windows Project Folder - Browse"))
            // {
            //     userSettings.projectWinLoc = EditorUtility.OpenFolderPanel("Select Windows Project folder", "", "");
                
            // }
            // userSettings.projectAndroidLoc = EditorGUILayout.TextField("Android Project Folder: ",userSettings.projectAndroidLoc);
            // if (GUILayout.Button("Android Project Folder - Browse"))
            // {
            //     userSettings.projectAndroidLoc = EditorUtility.OpenFolderPanel("Select Android Project folder", "", "");
                
            // }
            // EditorGUILayout.LabelField("Set the path to your Unity Instal");
            // EditorGUILayout.LabelField("This needs to be the appropriate Unity version for the projects above");
            // userSettings.unityEXELoc = EditorGUILayout.TextField("Unity.exe:",userSettings.unityEXELoc);

            // if (GUILayout.Button("Auto Fill Settings"))
            // {
            //     userSettings.unityEXELoc=EditorApplication.applicationPath;

            //     userSettings.projectWinLoc = Application.dataPath;
            //     userSettings.projectWinLoc = userSettings.projectWinLoc.Replace("/Assets", "");

            //     userSettings.projectAndroidLoc = userSettings.projectWinLoc+"/IFXBuildToolProjects/Android";
            //     userSettings.projectiOSLoc = userSettings.projectWinLoc+"/IFXBuildToolProjects/iOS";
                
            // }
            EditorGUILayout.LabelField("");//blank space for formating    
            if (GUILayout.Button("Inital Setup - Build Projects"))
            {
                userSettings.SettingsAutoSetup();
                userSettings.SaveUserSettings();
                bundleTools.SetupUnityProjects("Android");
                bundleTools.SetupUnityProjects("iOS");              
            }
            if (GUILayout.Button("Clear Dependencies Cashe"))
            {
                bundleTools.ClearDependenciesCache();
                
                //bundleTools.SetupUnityProjects("Android");
                //bundleTools.SetupUnityProjects("iOS");              
            }

            EditorGUILayout.LabelField("");//blank space for formating 

            if (userSettings.CTMode())
            {
                EditorGUILayout.LabelField("Set the paths to your CDN Project folder");
                userSettings.cdnWinLoc = EditorGUILayout.TextField("CDN Project Folder: ",userSettings.cdnWinLoc);
                if (GUILayout.Button("CDN Project Folder - Browse"))
                {
                    string cdnProjectFolder = EditorUtility.OpenFolderPanel("Select Windows CDN folder", "", "");
                    userSettings.cdnWinLoc = cdnProjectFolder+"/engage_online_root/asset_bundles/effects/unity_2019_2/Windows";
                    userSettings.cdnAndroidLoc = userSettings.cdnWinLoc+"/engage_online_root/asset_bundles/effects/unity_2019_2/Android";
                    userSettings.cdniOSLoc = userSettings.cdnWinLoc+"/engage_online_root/asset_bundles/effects/unity_2019_2/iOS";
                    
                    
                }
                EditorGUILayout.LabelField("OPTIONAL settings below");
                userSettings.prefabPrefix = EditorGUILayout.TextField("Prefix for creating prefabs ",userSettings.prefabPrefix);
                userSettings.prefabAfix = EditorGUILayout.TextField("Afix for creating prefabs ",userSettings.prefabAfix);

                EditorGUILayout.LabelField("");//blank space for formating 
                userSettings.currentIFXNum = EditorGUILayout.TextField("current ifx number:",userSettings.currentIFXNum);  
            } 

            EditorGUILayout.LabelField("");//blank space for formating      
                    
            if (GUILayout.Button("Save Settings"))
            {
                userSettings.SaveUserSettings();
                             
            }
            
            
                                
        }

        void NewDependenciesWindowUI()
        {                    
            // Create all the folders and stuff for a new dependencie
            EditorGUILayout.LabelField("Set up Client folders", EditorStyles.boldLabel);        
            EditorGUILayout.LabelField("Type client name and click the button to create dependencies folder, ifx and asset label.");
            EditorGUILayout.LabelField("For example type mcdonalds and you would get Dependencies_mcdonalds, ");
            EditorGUILayout.LabelField("ifx3-mcdonalds folders and ifx3-mcdonalds assetbunddle created and applied.");
            folderName = EditorGUILayout.TextField("Folder Name: ", folderName);
        
            if (GUILayout.Button("Create Folders"))
            {
                CreateDependenciesFolder();
            }
            
        }
        void AnimCNTRLFromClipsWindowUI()
        {            
            EditorGUILayout.LabelField("Create Animation Controlers from Selection", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("TO USE: Select the fbx in the project window and hit the button.");
            EditorGUILayout.LabelField("You will get a named anim controller for every anim clip in the fbx.");
            EditorGUILayout.LabelField("each anim controler will already be connected to it's animation clip.");
            if (GUILayout.Button("Quick Create Anim Controlers"))
            {
            string assetPath =  AssetDatabase.GetAssetPath((GameObject)Selection.activeObject);
                
            ModelImporter MI = (ModelImporter)AssetImporter.GetAtPath(assetPath);
            ModelImporterClipAnimation[] clips = MI.clipAnimations;
                
            var itemPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            var itemDirectory = Path.GetDirectoryName(itemPath);
            var findClipsAtPath = AssetDatabase.LoadAllAssetRepresentationsAtPath(itemPath);
                
            foreach (var aClips in findClipsAtPath) 
                {
                    var animationClip = aClips as AnimationClip;
                
                    if (animationClip != null) 
                    {
                            var createController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPathWithClip(itemDirectory+"/" +animationClip.name+"_ANIM_CONTROLLER.controller",animationClip);
                            Debug.Log("Found animation clip");
                    }
                }   
                
            
                
            
            }
            
        
            
        }

        void BuildSelectedBundleWindowUI()
        {
            
            EditorGUILayout.LabelField("ATTENTION! You need to set up your user settings before this can work!");
            EditorGUILayout.LabelField("TO USE: Select the folder you want made into a bundle, for example ifx3-mcdonalds.");
            EditorGUILayout.LabelField("You can shift select more folders if you want to build more than one bundle.");
            EditorGUILayout.LabelField("Use the check marks below to choose the build type");
            EditorGUILayout.LabelField("Leave both checked to build windows and android at the same time");

            buildQACheckOverride = EditorGUILayout.Toggle( "Disable Quality Check", buildQACheckOverride);
            if (userSettings.CTMode())
            {
                autoGitYesNo = EditorGUILayout.Toggle( "Push Bundles To Staging?", autoGitYesNo);
                gitCommitM = EditorGUILayout.TextField("Git Commit message: ", gitCommitM);
            }
            
            
            windowsBuildYesNo = EditorGUILayout.Toggle("Build for Windows?", windowsBuildYesNo);
            androidBuildYesNo = EditorGUILayout.Toggle("Build for Android?", androidBuildYesNo);
            iOSBuildYesNo = EditorGUILayout.Toggle("Build for iOS?", iOSBuildYesNo);           
            
            

            //CheckList
            EditorGUILayout.LabelField("___________________CHECKLIST__________________");
            EditorGUILayout.LabelField("0. -PUSH CHANGES TO PROJECT- Push your changes to the project to the appropriate IFX Project.  -OPTIONAL");
            EditorGUILayout.LabelField("1. -SELECT ONLY FOLDERS- Check that you have only the folders you want bundled selected.");
            EditorGUILayout.LabelField("2. -CHECK TOGGLES- Check build toggles are set right.");
            EditorGUILayout.LabelField("3. -COMMIT MESSAGE- if you use auto git you must enter a commit message.");
            EditorGUILayout.LabelField("4. -PULL- if you use auto git it's not a bad idea to do a manual pull first. -OPTIONAL");
            EditorGUILayout.LabelField("5. -PULL PROJECT- Have you pulled changes to your project recently? -OPTIONAL");
            EditorGUILayout.LabelField("6.- HIT BUILD BUTTON-");      

            
            if (GUILayout.Button("AssetBundle From Selection"))
            {
                if (autoGitYesNo==true && gitCommitM=="")
                {
                    EditorUtility.DisplayDialog("WARNING!", "To use auto git you need to enter a commit message", "OK", "Cancel");                          
                }
                else if(bundleTools.DirectoryBoolMulti(Selection.objects) == false) //fix
                {
                    List<string> itemList = new List<string>();
                    
                    foreach (var item in Selection.objects)
                    {
                        if (bundleTools.DirectoryBool(item)==false)
                        {
                            itemList.Add(item.name);
                        } 
                    }
                    string selObjects = string.Join(", ", itemList);
                    
                    string warningMSG="Please select ONLY Folders to be built. Your selection contains These Items that arn't folders:                      "+selObjects;
                    EditorUtility.DisplayDialog("WARNING!",warningMSG, "OK", "Cancel");                    
                }
                else
                {
                    //BuildSelectedBundle(windowsBuildYesNo,androidBuildYesNo);//build windows true, build android true
                    foreach (var dir in Selection.objects)
                    {
                        bundleTools.BuildSelectedBundle(dir, windowsBuildYesNo,androidBuildYesNo,iOSBuildYesNo,autoGitYesNo,buildQACheckOverride,gitCommitM);
                    }
                    
                }  
            }  
        }

        void EmptyPrefabWindowUI()
        {
            EditorGUILayout.LabelField("Add a top level zeroed out empty", EditorStyles.boldLabel);
            if (GUILayout.Button("Create zeroed prefab"))
            {   
                    foreach (GameObject item in Selection.objects)
                    {
                        var topEmpty = new GameObject();
                        topEmpty.name = userSettings.prefabPrefix+item.name+userSettings.prefabAfix;
                        item.transform.parent = topEmpty.transform;
                        topEmpty.transform.position= new Vector3(0,0,0);
                        topEmpty.transform.eulerAngles= new Vector3(0,0,0); 
                    }  
            }

        }

        void BatchPrefabWindowUI()
        {
            EditorGUILayout.LabelField("Batch Make Prefabs ", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("This tool adds all the selected objects in a scene as prefabs so you don't have to do it one by one.");
            EditorGUILayout.LabelField("TO USE: Set the destination folder by selecting the folder and hitting the button or by typing in the box.");
            EditorGUILayout.LabelField("Then select all the objects you want added as prefabs and click the Make Prefabs button.");
            destinationFolder = EditorGUILayout.TextField("Destination Folder:",destinationFolder);
            if (GUILayout.Button("Set Destination Folder"))
            {    
                destinationFolder = AssetDatabase.GetAssetPath(Selection.activeObject);  
            }    
            if (GUILayout.Button("Make Prefabs"))
            {
                // Loop through every GameObject in the array above
                foreach (GameObject gameObject in Selection.objects)
                {
                    // Set the path as within the Assets folder,
                    // and name it as the GameObject's name with the .Prefab format
                    string localPath = destinationFolder+"/" + gameObject.name + ".prefab";
                    // Make sure the file name is unique, in case an existing Prefab has the same name.
                    localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                    // Create the new Prefab.
                    PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, localPath, InteractionMode.UserAction);
                }
            }
        }

        void CreateImagePlaneUI()
        {
            EditorGUILayout.LabelField("Create Image Plane From image", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("This feature still needs error handeling to be written");
            if (GUILayout.Button("Image plane from Image"))
            {
                ImagePlaneFromImage();
            }
        }
        void AutoPivotUI()
        {            
            EditorGUILayout.LabelField("Center IFX", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("This tool assumes a top level empty at 0,0,0");
            EditorGUILayout.LabelField("You can easily add one using the \"quick prefab\" tool.");
            EditorGUILayout.LabelField(" ");
            EditorGUILayout.LabelField("\"ALT Centering\" Slower but more accurate.");
            EditorGUILayout.LabelField("It figures out the center based on mesh verticies.");
            EditorGUILayout.LabelField(" ");
            EditorGUILayout.LabelField("NOTE: this tool has no undo.");
            EditorGUILayout.LabelField("Makes immediate changes to prefabs. No need to override.");
            EditorGUILayout.LabelField("Works on selections in project view or scene view.");
            altCenterMethod = GUILayout.Toggle(altCenterMethod,"ALT Centering");           
            
            if (GUILayout.Button("Auto Pivot"))
            {
                if (Selection.gameObjects!=null)
                {
                    //string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(Selection.gameObjects[0]);
                    //string assetPath = AssetDatabase.GetAssetPath(Selection.gameObjects[0]);
                    //Debug.Log("Asset path is: "+assetPath);
                    BatchCenterIFX(Selection.gameObjects,false);
                }
                else
                {
                    Debug.Log("Nothing selected - please select the top most object of the hierarchy and try again");
                }
            }
        }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////// -----Bundles-----///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////// -----Settings-----///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////// -----Centering Tool-----///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool ObjectPivotCenteredCheck(GameObject sourceGO, bool useAleternateCenter=false)
        {
            bool objectIsCentered=true;
            float diffrenceTollerence=0.5f;
            Vector3 sceneCenter = new Vector3(0,0,0);
            Vector3 center = GetAverageCenter(sourceGO,useAleternateCenter);
            
            float diff = center.sqrMagnitude;
            //Debug.Log(diff);
            if (diff>=diffrenceTollerence)
            {
                objectIsCentered =false;
                //Debug.Log("Object is this far from center: "+diff);
            }
            return objectIsCentered;
        }
        static Vector3 MeshCenter(GameObject inputGO)
        {
            MeshFilter meshFilter = inputGO.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                Mesh mesh = meshFilter.sharedMesh;
                Vector3[] vertices = mesh.vertices;
                Bounds tempBounds =  new Bounds(inputGO.transform.TransformPoint(mesh.bounds.center),new Vector3(0,0,0));
                for (var i = 0; i < vertices.Length; i++)
                {   
                    tempBounds.Encapsulate(inputGO.transform.TransformPoint(vertices[i]));
                }
                //Debug.Log("TempBounds center: "+tempBounds.center);
                //inputGO.transform.localScale = inScale;
                Vector3 result = tempBounds.center;
                return tempBounds.center;
            }
            else
            {
                return Vector3.zero;
            }
            
        }
        static Vector3 GetAverageCenter (GameObject inputGO, bool useAleternateCenter = false)
        {
            
            //index error here
            Bounds tempBounds;
            Vector3 centerAverage;
            if (useAleternateCenter)
            {
                
                MeshFilter[] mFInChildren = inputGO.GetComponentsInChildren<MeshFilter>();
                
                if (mFInChildren.Length >0)
                {                   
                    tempBounds =  new Bounds(MeshCenter(mFInChildren[0].gameObject),Vector3.zero);
                    for (var i = 0; i < mFInChildren.Length; i++)
                    {
                        GameObject child = mFInChildren[i].gameObject;
                        Debug.Log("children: "+child.name);

                        Vector3 meshCenter=MeshCenter(child.gameObject);
                        
                        tempBounds.Encapsulate(meshCenter);
                    }
                    centerAverage = tempBounds.center;
                    centerAverage = Vector3.zero;
                }              
                else
                {
                    Debug.Log("no Meshfilter Components found use other centering method");
                    centerAverage = Vector3.zero;
                }
            }
            else
            {
                List<Vector3> boundsCenterList = new List<Vector3>();
                Renderer[] renderComponentsInChildren = inputGO.GetComponentsInChildren<Renderer>();
                
                if (renderComponentsInChildren.Length >0)
                {
                   
                    foreach (Renderer renderComponent in renderComponentsInChildren)
                    {
                        //Debug.Log("Render Components test");
                        boundsCenterList.Add(renderComponent.bounds.center);
                    }
                    //Debug.Log("Render Components it aint null bruh");
                    tempBounds =  new Bounds(boundsCenterList[0],Vector3.zero);
                    foreach (Vector3 bound in boundsCenterList)
                    {
                        tempBounds.Encapsulate(bound);
                    }
                    centerAverage = tempBounds.center;  
                }
                else
                {
                    Debug.Log("no Render Components found use other centering method");
                    centerAverage = Vector3.zero;
                }                    
            } 
            return centerAverage;
        }
        public static void CenterIFX(GameObject inputGO, bool useAleternateCenter = false)
        {
            //if it has an asset path AKA it's a prefab then load that prefab for editing otherwise just use the scene GO
            GameObject prefabRoot = inputGO;
            string assetPath="";
            assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(inputGO);
            //Debug.Log("Asset path is: "+assetPath);
            if (assetPath!="")
            {
                prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);
            }
            Vector3 averageCenter = GetAverageCenter(prefabRoot,useAleternateCenter);
            //Need to load all the direct children transforms into a list before reparenting or the foreach count gets confused 
            List<Transform> childrenTransformsList = new List<Transform>();
            //create a GO that is moved to the averaged center of geo. Paaretn everything to it then move it to 0,0,0
            GameObject offsetMoverGO = new GameObject();
            offsetMoverGO.name="ReCentering Mover";
            offsetMoverGO.transform.parent = prefabRoot.transform;
            offsetMoverGO.transform.position = averageCenter;
            //Debug.Log("average center is: "+averageCenter);
            //Debug.Log("mover pos is: "+offsetMoverGO.transform.position);
            //Debug.Log(prefabRoot.transform.childCount);
            foreach (Transform child in prefabRoot.transform)
            {
                Debug.Log(child.name);
                childrenTransformsList.Add(child.transform);
            }
            foreach (Transform child in childrenTransformsList)
            {
                child.transform.SetParent(offsetMoverGO.transform);
            }
            //move the offset back to zero
            offsetMoverGO.transform.position =new Vector3(0,0,0);
            //parent all the objects back to their original parent then delete the offset mover
            foreach (Transform child in childrenTransformsList)
            {
                child.transform.SetParent(prefabRoot.transform);
            }
            DestroyImmediate(offsetMoverGO);
            //if it was used on a prefab save it over the old prefab
            if (assetPath!="")
            {
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
            }        
        }
        void BatchCenterIFX(GameObject[] inputGO, bool useAleternateCenter = false)
        {
            foreach (GameObject GO in inputGO)
            {
                Debug.Log("got to batch center:");
                CenterIFX(GO,useAleternateCenter);
            }
        }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////// -----Folder Making  Tool-----///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void CreateDependenciesFolder()
        {
            Debug.Log("Creating:" + folderName);
            string createDependenciesFolder = AssetDatabase.CreateFolder("Assets", "Dependencies_" + folderName);
            string createBundleFolder = AssetDatabase.CreateFolder("Assets/---IFXBundles--Windows_Android_Both", "ifx"+userSettings.currentIFXNum + "-" + folderName.ToLower());
            // AssetImporter assetImporter = AssetImporter.GetAtPath("Assets/---IFXBundles--Windows_Android_Both/" + userSettings.currentIFXNum + "-" + folderName.ToLower());
            // assetImporter.assetBundleName = userSettings.currentIFXNum + folderName.ToLower();
            //assetImporter.SaveAndReimport();
        }

        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////// -----Image Plane Tool-----///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void ImagePlaneFromImage()
        {
            UnityEngine.Object[] activeGOs = Selection.GetFiltered(typeof(Texture2D),SelectionMode.Editable | SelectionMode.TopLevel);
            float ratio1=1f;
            float ratio2=1f;
            if (activeGOs.Length >0)
            {
                foreach (Texture2D image in activeGOs)
                {
                    var itemPath = AssetDatabase.GetAssetPath(image);
                    var itemDirectory = Path.GetDirectoryName(itemPath);
                    TextureImporter textureImporter = (TextureImporter)TextureImporter.GetAtPath(itemPath);
                    textureImporter.npotScale = TextureImporterNPOTScale.None;
                    textureImporter.SaveAndReimport();
                    GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    ratio1 = image.height;
                    ratio2 = image.width;
                    //scale to the ratios
                    plane.transform.localScale = new Vector3(ratio2/ratio1, 1, ratio1/ratio1);
                    //Set the name
                    plane.name = image.name;
                    //Remove Colliders
                    DestroyImmediate(plane.GetComponent<Collider>());
                    //Material Stuff
                    Renderer rend = plane.GetComponent<Renderer>();
                    Material material = new Material(Shader.Find("Standard"));
                    material.SetFloat("_Mode", 1.0f);
                    material = MaterialCutoutmode(material);
                    material.name = image.name + "_MAT";
                    material.SetTexture("_MainTex", image);
                    rend.material = material;
                    AssetDatabase.CreateAsset(material, itemDirectory + "/" + image.name + "_MAT.mat");
                    //Rotate
                    plane.transform.eulerAngles = new Vector3(90, 0, 0);
                    //Duplicate the plane
                    GameObject planeOtherSide = Instantiate(plane, new Vector3(0, 0, 0), Quaternion.identity);
                    planeOtherSide.name = plane.name + "_OtherSide";
                    planeOtherSide.transform.eulerAngles = new Vector3(90, 0, 180);
                    //Paretnt to empty
                    GameObject topEmpty = new GameObject();
                    topEmpty.name = plane.name+"_ScaleThis";
                    plane.transform.parent = topEmpty.transform;
                    planeOtherSide.transform.parent = topEmpty.transform;
                }
            }
            else
            {
                Debug.Log("No image selected");
                EditorUtility.DisplayDialog("WARNING!", "Select an image in the project view first", "OK", "Cancel");
            } 
        }

        private static Material MaterialCutoutmode(Material material)
        {
            material.SetOverrideTag("RenderType", "TransparentCutout");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.EnableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
            return material;
        }
    }
        
}