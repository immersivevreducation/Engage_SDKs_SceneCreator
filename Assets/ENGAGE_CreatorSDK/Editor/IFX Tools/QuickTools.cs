using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.IO;
using System.Collections.Generic;
using IFXToolSM = IFXTools.IFXToolsStaticMethods;






////////////////////////////////////////////////////////////////////TO DO//////////////////////////////////////////////////////////
//1. refactor
//2. scenes builds recognized as scenes
//3. when syncing projects gather ALL scripts so they can't be any missing dependencies

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace IFXTools{
    public class QuickTools : EditorWindow
    {
////////////////////////////////////////////////////////Instance Variables///////////////////////////////////////////////////////        
        IFXBundleTools bundleTools;
        IFXToolsUserSettings userSettings;

        IFXThumbnailTool thumbnailToolInstance;  
////////////////////////////////////////////////////////Build Variables///////////////////////////////////////////////////////
        bool passedQualityCheck;
        List<string> bundlesBuiltWin = new List<string>();
        List<string> bundlesBuiltAndroid = new List<string>();
////////////////////////////////////////////////////////UI Variables///////////////////////////////////////////////////////
        
        public bool bundlesBuilding{get; set;}

        public bool buildQACheckOverride{get; set;}
        public string gitCommitM{get; set;}
        public bool windowsBuildYesNo{get; set;}
        public bool androidBuildYesNo {get; set;}
        public bool iOSBuildYesNo {get; set;}
        public bool autoGitYesNo {get; set;}

        bool altCenterMethod {get; set;}
        bool hardResetCache {get; set;}
        
        

        string folderName;
        
        string destinationFolder;
        
        
        int selGridInt=0;
        Vector2 scrollPos;
        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        [MenuItem("Assets/IFX Tools")]
        [MenuItem("Creator SDK/Bundle IFX Tools - Beta")]
        static void Init()
        {
            EditorWindow window = EditorWindow.GetWindowWithRect(typeof(QuickTools), new Rect(100, 100, 700, 700));
            //EditorWindow window = GetWindow(typeof(QuickTools));
            //window.minSize = new Vector2(500,700);
           
            window.Show();

        }
        void OnDestroy()
        {
            
            if (thumbnailToolInstance.ifxObject !=null)
            {
                DestroyImmediate(thumbnailToolInstance.ifxObject, true);
            }
            
        }
        
        void OnEnable()
        {
            //set up other class instances
            userSettings = IFXToolsUserSettings.GetUserSettings();
            userSettings.LoadUserSettings();
            thumbnailToolInstance = new IFXThumbnailTool();
            bundleTools = (IFXBundleTools )ScriptableObject.CreateInstance(typeof(IFXBundleTools ));
            bundleTools.Init(userSettings);

            //other
            bundlesBuilding=false;

            //Set defualt checkmarks
            buildQACheckOverride=false;
            windowsBuildYesNo =true;
            androidBuildYesNo =true;
            iOSBuildYesNo =true;
            altCenterMethod=false;
            hardResetCache=false;

            //disable auto git for non content team mode
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
            //changes possible buttons based on ct mode
            string[] selStrings = new string[] {"Build Bundles","Settings"};
            if (userSettings.CTMode())
            {
                selStrings = new string[] {"Build Bundles", "Prefab Tools", "Organising Tools","Creation Tools","Settings"};
            }
            
            
            //the button area on the left stuff
            Rect buttonGroupRect=new Rect(5, 25, this.position.width / 4, this.position.height-100);
            selGridInt = GUI.SelectionGrid(buttonGroupRect, selGridInt, selStrings, 1);//TAb switching controlls

            Rect groupRect=new Rect(buttonGroupRect.width+ 10, 0, this.position.width-buttonGroupRect.width, this.position.height);
            Rect subToolsGroupRect=new Rect(5, 25, buttonGroupRect.width*3, this.position.height-10);
            GUI.BeginGroup(groupRect);
            
            //build bundles                                       
            if (selGridInt==0)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(buttonGroupRect.width*3-10));
                BuildSelectedBundleWindowUI();
                EditorGUILayout.EndVertical();
                
            }
            //Prefab Tools
            if (selGridInt==1)
            {
                
                if (userSettings.CTMode())
                {
                    
                
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(buttonGroupRect.width*3-10));
                    EmptyPrefabWindowUI();
                    EditorGUILayout.EndVertical();
                    
                    EditorGUILayout.LabelField(" ");
                    
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(buttonGroupRect.width*3-10));
                    BatchPrefabWindowUI();
                    EditorGUILayout.EndVertical();
                
                    EditorGUILayout.LabelField(" ");
                    
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(buttonGroupRect.width*3-10));
                    AutoPivotUI();
                    EditorGUILayout.EndVertical();
                }
              
                   
            }
            //Organization Tools
            if (selGridInt==2)
            {              
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(buttonGroupRect.width*3-10));
                if (GUILayout.Button("Youtube Downloader"))
                {
                    EditorWindow window = GetWindow(typeof(IFXToolsYouTubeDownloader));
                    window.Show();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(buttonGroupRect.width*3-10));
                
                NewDependenciesWindowUI();

                EditorGUILayout.EndVertical();
                
                EditorGUILayout.LabelField(" ");                
            }
            //Creation Tools
            if (selGridInt==3)
            {
                
                //create iamges plane form image
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(buttonGroupRect.width * 3 - 10));

                CreateImagePlaneUI();

                EditorGUILayout.EndVertical();
                

                EditorGUILayout.LabelField(" ");

                //Anim clips from fbx
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(buttonGroupRect.width * 3 - 10));

                AnimCNTRLFromClipsWindowUI();

                EditorGUILayout.EndVertical();
               

                //thumbnail tool
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(buttonGroupRect.width * 3 - 10));
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                if (GUILayout.Button("Thumbnail Tool"))
                {
                    EditorWindow window = GetWindow(typeof(IFXThumbnailToolWindow));
                    window.Show();
                }
                //ThumbnailToolUI();
                EditorGUILayout.EndScrollView();  

                EditorGUILayout.EndVertical();
                
                
            }
            //changes settings position on button menu based on ctmode
            if (userSettings.CTMode())
                {
                    if (selGridInt==4)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(buttonGroupRect.width*3-10));
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
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(buttonGroupRect.width*3-10));
                    SettingsWindowUI();
                    userSettings.cTCode = EditorGUILayout.TextField("",userSettings.cTCode);
                    EditorGUILayout.EndVertical();
                    
                }
            }
            GUI.EndGroup();                           
        }
        void BuildSelectedBundleWindowUI()
        {
            if (bundleTools.buildingStatus !=null)
            {
                GUIStyle buildstatusStyle = new GUIStyle();
                buildstatusStyle.fontSize = 45;
                buildstatusStyle.normal.textColor = Color.red;
                GUI.Label(new Rect(20, Screen.height / 2, 300, 50), bundleTools.buildingStatus, buildstatusStyle);
                //EditorGUILayout.LabelField(bundleTools.buildingStatus);//Building Status.
            }
            
            
            QuickToolsHelp.BuildBundlesInstructions(); // displays the instructions for this tool

            //with qa check disabled it will skip the whole qa process. sometimes qeird components or things like that can trip it up so I left the option to skip it.
            buildQACheckOverride = EditorGUILayout.Toggle( "Disable Quality Check", buildQACheckOverride);
            
            //ct mode dsiables the git stuff
            if (userSettings.CTMode())
            {
                autoGitYesNo = EditorGUILayout.Toggle( "Push Bundles To CDN?", autoGitYesNo);
                gitCommitM = EditorGUILayout.TextField("Git Commit message: ", gitCommitM);

                if (GUILayout.Button("Push changes in folder to git"))
                {
                     if (gitCommitM=="")
                    {
                        EditorUtility.DisplayDialog("WARNING!", "To auto push you need to enter a commit message", "OK", "Cancel");                          
                    }
                    else
                    {
                        bundleTools.GitCommitChangesToRepo(IFXToolSM.GetSelectedObjectsAsList(),gitCommitM);
                    }
                    
                }
            }
            
            
            windowsBuildYesNo = EditorGUILayout.Toggle("Build for Windows?", windowsBuildYesNo);
            androidBuildYesNo = EditorGUILayout.Toggle("Build for Android?", androidBuildYesNo);
            iOSBuildYesNo = EditorGUILayout.Toggle("Build for iOS?", iOSBuildYesNo);           

            //CheckList
            QuickToolsHelp.PreBuildChecklist(); // displays the instructions for this tool 

            if (GUILayout.Button("AssetBundle From Selection"))
            {
                if (autoGitYesNo==true && gitCommitM=="")
                {
                    EditorUtility.DisplayDialog("WARNING!", "To auto push you need to enter a commit message", "OK", "Cancel");                          
                }
                
                else if(IFXToolSM.SelectionIsDirectoryBool(Selection.objects) == false)
                {
                    EditorUtility.DisplayDialog("WARNING!", "Please Only Have FOLDERS Selected", "OK", "Cancel");
                }
                else
                {
                    List<Object> selectedBundles = new List<Object>();
                    foreach (var dir in Selection.objects)
                    {
                        //check if the bundles has scene files and if they are the only file type present
                        if(IFXToolSM.DoesSelectedFolderContainOnlyScenes(dir)==false && IFXToolSM.DoesSelectedFolderContainFileType(dir,"*.unity") == true)
                        {
                            EditorUtility.DisplayDialog("WARNING!", "When Building scenes please ensure ONLY scene files are in the selected folder", "OK", "Cancel");
                            return;
                        }
                        //add the bundle so long as the above dosn't trigger
                        selectedBundles.Add(dir);                        
                    }
                    BundleBuildSettings buildSettings = new BundleBuildSettings();
                    buildSettings.selectedBundles = selectedBundles;
                    buildSettings.windowsBuildYesNo = windowsBuildYesNo;
                    buildSettings.androidBuildYesNo = androidBuildYesNo;
                    buildSettings.iOSBuildYesNo = iOSBuildYesNo;
                    buildSettings.buildQACheckOverride = buildQACheckOverride;
                    buildSettings.autoGitYesNo = autoGitYesNo;
                    buildSettings.gitCommitMessage = gitCommitM;

                    // Build the bundles!
                    bundleTools.BuildSelectedBundle(buildSettings);                    
                }  
            }
            if (bundlesBuilding)
            {
                EditorGUILayout.LabelField("Building Bundles...", EditorStyles.boldLabel);
            }
        }
        void SettingsWindowUI()
        {          
            EditorGUILayout.LabelField("");//blank space for formating  
            // If this is toggled delete the entire project folders in the cache when clear button hit
            hardResetCache = EditorGUILayout.Toggle( "Full Reset Cache", hardResetCache);

            //delete the asset folders in the build projects
            if (GUILayout.Button("Clear Build Projects Cache"))
            {
                IFXToolSM.ClearDependenciesCache(hardResetCache,userSettings.projectWinLoc);
            }
            if (GUILayout.Button("Test Button"))
            {
               var alldascripts=  IFXToolSM.GetAllScriptsInProject();
               foreach (var item in alldascripts)
               {
                   Debug.Log(item.ToString());
               }
            }

            EditorGUILayout.LabelField("");//blank space for formating 

            //ct mode only settings such as cdn
            if (userSettings.CTMode())
            {
                userSettings.debugMode = EditorGUILayout.Toggle( "Enable DebugMode", userSettings.debugMode); 

                EditorGUILayout.LabelField("Windows Project Folder: "+userSettings.projectWinLoc);
                EditorGUILayout.LabelField("Android Project Folder: "+userSettings.projectAndroidLoc);
                EditorGUILayout.LabelField("iOS Project Folder: "+userSettings.projectiOSLoc);
                
                EditorGUILayout.LabelField("Set the paths to your CDN Project folder");
                EditorGUILayout.LabelField("CDN Project Folder: "+userSettings.cdnProjectPath);
                EditorGUILayout.LabelField("IFX CDN Win Project Folder: "+userSettings.cdnWinIFXLoc);
                EditorGUILayout.LabelField("IFX CDN Android Project Folder: "+userSettings.cdnAndroidIFXLoc);
                EditorGUILayout.LabelField("IFX CDN iOS Project Folder: "+userSettings.cdniOSIFXLoc);

                EditorGUILayout.LabelField("Scene CDN Win Project Folder: "+userSettings.cdnWinSceneLoc);
                EditorGUILayout.LabelField("Scene CDN Android Project Folder: "+userSettings.cdnAndroidSceneLoc);
                EditorGUILayout.LabelField("Scene CDN iOS Project Folder: "+userSettings.cdniOSSceneLoc);

                if (GUILayout.Button("CDN Project Folder - Browse"))
                {
                    userSettings.cdnProjectPath = EditorUtility.OpenFolderPanel("Select Windows CDN folder", "", "");
                    userSettings.SettingsAutoSetup();                       
                }
                EditorGUILayout.LabelField("OPTIONAL settings below");
                userSettings.prefabPrefix = EditorGUILayout.TextField("Prefix for creating prefabs ",userSettings.prefabPrefix);
                userSettings.prefabAfix = EditorGUILayout.TextField("Afix for creating prefabs ",userSettings.prefabAfix);

                EditorGUILayout.LabelField("");//blank space for formating 
                userSettings.currentIFXNum = EditorGUILayout.TextField("current ifx number:",userSettings.currentIFXNum);

                EditorGUILayout.LabelField("");//blank space for formating 

                userSettings.thumbnailSavePath = EditorGUILayout.TextField("Thumbnail Save Location: ",userSettings.thumbnailSavePath);
                if (GUILayout.Button("Select Thumbnail Save Location"))
                {
                    userSettings.thumbnailSavePath = EditorUtility.OpenFolderPanel("Thumbnail Save Location", "", "");                  
                }  
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
            QuickToolsHelp.NewDependenciesWindowInstructions(); // displays the instructions for this tool
            folderName = EditorGUILayout.TextField("Folder Name: ", folderName);        
            if (GUILayout.Button("Create Folders"))
            {
                IFXToolSM.CreateDependenciesFolder(userSettings.currentIFXNum,folderName);
            }
        }
        void AnimCNTRLFromClipsWindowUI()
        {    
            QuickToolsHelp.AnimClipsFromFBXInstructions(); // displays the instructions for this tool        
            if (GUILayout.Button("Quick Create Anim Controlers"))
            {
                IFXToolSM.CreateAnimClipsFromSelectedFBX();
            }
        }

        

        void EmptyPrefabWindowUI()
        {
            EditorGUILayout.LabelField("Add a top level zeroed out empty", EditorStyles.boldLabel);
            if (GUILayout.Button("Create zeroed prefab"))
            {
                IFXToolSM.InsertSelectedObjectIntoEmpty(Selection.gameObjects, userSettings.prefabPrefix, userSettings.prefabAfix);
            }
        }

        

        void BatchPrefabWindowUI()
        {
            QuickToolsHelp.BatchMakePrefabsInstructions(); // displays the instructions for this tool
            
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
                IFXToolSM.ImagePlaneFromImage();
            }
        }
        void AutoPivotUI()
        {           
            QuickToolsHelp.AutoPivotInstructions(); // displays the instructions for this tool
            altCenterMethod = GUILayout.Toggle(altCenterMethod,"ALT Centering");           
            
            if (GUILayout.Button("Auto Pivot"))
            {
                if (Selection.gameObjects!=null)
                {
                    //string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(Selection.gameObjects[0]);
                    //string assetPath = AssetDatabase.GetAssetPath(Selection.gameObjects[0]);
                    //Debug.Log("Asset path is: "+assetPath);
                    IFXToolSM.BatchCenterIFX(Selection.gameObjects,false);
                }
                else
                {
                    Debug.Log("Nothing selected - please select the top most object of the hierarchy and try again");
                }
            }
        }
    }
        
}

public static class QuickToolsHelp
{
       
    
    public static void BuildBundlesInstructions()
    {
        
        EditorGUILayout.LabelField("TO USE: Select the folder you want made into a bundle");
        EditorGUILayout.LabelField("You can shift select more folders if you want to build more than one bundle.");
        EditorGUILayout.LabelField("Use the check marks to choose the build type");
        EditorGUILayout.LabelField("you can have more than one build type checked to build multiple bundle types at once");
    }
    public static void PreBuildChecklist()
    {
        EditorGUILayout.LabelField("___________________CHECKLIST__________________");
        EditorGUILayout.LabelField("0. -PUSH CHANGES TO PROJECT - It's a good idea to push changes made to the project now if you are using github. - OPTIONAL");
        EditorGUILayout.LabelField("1. -SELECT ONLY FOLDERS - Check that you have only the folders you want bundled selected.");
        EditorGUILayout.LabelField("2. -CHECK TOGGLES- Check build toggles are set correctly.");
        // EditorGUILayout.LabelField("3. -COMMIT MESSAGE- if you want to push directly to staging you must enter a commit message.");
        // EditorGUILayout.LabelField("4. -PULL- if you use auto git it's not a bad idea to do a manual pull first. -OPTIONAL");
        // EditorGUILayout.LabelField("5. -PULL PROJECT- Have you pulled changes to your project recently? -OPTIONAL");
        EditorGUILayout.LabelField("6.- HIT BUILD BUTTON-");   
    }
    public static void AutoPivotInstructions()
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
    }

     public static void NewDependenciesWindowInstructions()
        {                    
            
            EditorGUILayout.LabelField("Set up Client folders", EditorStyles.boldLabel);        
            EditorGUILayout.LabelField("Type client name and click the button to create dependencies folder, ifx and asset label.");
            EditorGUILayout.LabelField("For example type mcdonalds and you would get Dependencies_mcdonalds, ");
            EditorGUILayout.LabelField("ifx3-mcdonalds folders and ifx3-mcdonalds assetbunddle created and applied.");
        }
          
    public static void AnimClipsFromFBXInstructions()
    {
        EditorGUILayout.LabelField("Create Animation Controlers from Selection", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("TO USE: Select the fbx in the project window and hit the button.");
        EditorGUILayout.LabelField("You will get a named anim controller for every anim clip in the fbx.");
        EditorGUILayout.LabelField("each anim controler will already be connected to it's animation clip.");           
    }
    
    public static void BatchMakePrefabsInstructions()
        {                    
            
            EditorGUILayout.LabelField("Batch Make Prefabs ", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("This tool adds all the selected objects in a scene as prefabs so you don't have to do it one by one.");
            EditorGUILayout.LabelField("TO USE: Set the destination folder by selecting the folder and hitting the button or by typing in the box.");
            EditorGUILayout.LabelField("Then select all the objects you want added as prefabs and click the Make Prefabs button.");
        }
}