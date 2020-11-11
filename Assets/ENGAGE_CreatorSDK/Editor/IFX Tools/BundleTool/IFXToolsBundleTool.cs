using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Process = System.Diagnostics.Process;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;
 /////////////TO-DO////
 // self create mirrored unity projects one for android one for ios (can be made inside windows unity project)
 // get basic bundle tools working in creators sdk (no git pushing just build the bundles localy)
 //use auto git to push changes to repo? select dependencies of bundle being psuhed and add them to commit and push?

 namespace IFXTools{

     public interface IBundleToolsParentWindow
     {
        bool buildQACheckOverride {get; set;}
        string gitCommitM {get; set;}
        bool windowsBuildYesNo {get; set;}
        bool androidBuildYesNo {get; set;}
        bool autoGitYesNo {get; set;}

        void CloseWindow();
     }
    public class IFXBundleTools : EditorWindow
    {
              
        IFXToolsQualityCheckTool qaTool;
        IFXToolsUserSettings userSettings;
        IBundleToolsParentWindow parentWindow;
        public UnityEngine.Object selectedBundle {get; set;}
        public IFXBundleTools()
        {
  
        }
        public void Init(IBundleToolsParentWindow parentWindowIN,IFXToolsUserSettings userSettingsIN)
        {
            parentWindow = parentWindowIN;
            userSettings = userSettingsIN; 
        }
        public bool buildQACheckOverride {get; set;}
        public string gitCommitM {get; set;}
        public bool windowsBuildYesNo {get; set;}
        public bool androidBuildYesNo {get; set;}
        public bool iOSBuildYesNo {get; set;}
        public bool autoGitYesNo {get; set;}
        void OnEnable()
        {
            
        }
        
        bool passedQualityCheck;
        
        
        List<string> bundlesBuiltWin = new List<string>();
        List<string> bundlesBuiltAndroid = new List<string>();
        List<string> bundlesBuiltiOS = new List<string>();
       
        public void BuildSelectedBundle(UnityEngine.Object selectedBundleIN,
        bool windowsYes,
        bool androidYes,
        bool iOSYes,
        bool autoGitYes,
        bool buildQACheckOverrideIN=false,
        string gitCommitMessageIN = "")
        {
            selectedBundle = selectedBundleIN;

            buildQACheckOverride = buildQACheckOverrideIN;
            gitCommitM = gitCommitMessageIN;
            windowsBuildYesNo = windowsYes;
            androidBuildYesNo = androidYes;
            iOSBuildYesNo = iOSYes;
            autoGitYesNo = autoGitYes;
            
            passedQualityCheck = true;
            if (autoGitYes)
            {
                GitPull();
                bundlesBuiltWin.Clear();
                bundlesBuiltAndroid.Clear();
                bundlesBuiltiOS.Clear();        
            }
            /////////////////////////////////////////////////////////////
            
            if (selectedBundle !=null)
            {
                //CheckBundleDirectoryiesExist(); //Left for debuging but no longer needed in established projects                
                
                //This part isn't auto "add"ing the files to a git comit. this part just makes the varable to hold all the paths
                if (autoGitYes)
                {
                    if (androidYes)
                    {
                        bundlesBuiltAndroid.Add(userSettings.cdnAndroidLoc + "/" + selectedBundle.name + ".engagebundle");
                        bundlesBuiltAndroid.Add(userSettings.cdnAndroidLoc + "/" + selectedBundle.name + ".manifest");
                    }
                    if (iOSYes)
                    {
                        bundlesBuiltiOS.Add(userSettings.cdniOSLoc + "/" + selectedBundle.name + ".engagebundle");
                        bundlesBuiltiOS.Add(userSettings.cdniOSLoc + "/" + selectedBundle.name + ".manifest");
                    }
                    if (windowsYes)
                    {
                        bundlesBuiltWin.Add(userSettings.cdnWinLoc + "/" + selectedBundle.name + ".engagebundle");
                        bundlesBuiltWin.Add(userSettings.cdnWinLoc + "/" + selectedBundle.name + ".manifest");
                    }       
                }
                ClearAllAssetLabelsInProject(); // might bother people... Make into a manual button if needed                   
                SetAssetLabelToFolderName(selectedBundle);
                //Checks for bad components
                if (!buildQACheckOverride)
                {
                    qaTool = (IFXToolsQualityCheckTool )ScriptableObject.CreateInstance(typeof(IFXToolsQualityCheckTool ));
                    qaTool.Init(this);
                    bool qaCheck = qaTool.BundleQualityCheck(selectedBundle); //pass QA true or false
                    if (!qaCheck)
                    {
                        passedQualityCheck = false;
                    }
                }
            

                if (passedQualityCheck | buildQACheckOverride)
                {
                    buildQACheckOverride = false;
                    // if true build android 
                    if (androidYes)
                    {
                        DeleteFolderContents(userSettings.projectAndroidLoc + "/AssetBundles/Android"); //maybe move this to when bundles go to server so bundles can be built near the same time.
                        CreateAndroidBatchFile(autoGitYesNo);
                        //RoboCopyDependenciesFiles("Android");
                        //RunBatchFile(Application.dataPath + "/Editor/IFX Tools/BundleTool/AndroidSync.bat");
                        RunBatchFile(Application.dataPath + "/Editor/IFX Tools/BundleTool/AndroidBuild.bat");
                        // Git stuff handled in batch file!
                        
                    }
                    if (iOSYes)
                    {
                        DeleteFolderContents(userSettings.projectiOSLoc + "/AssetBundles/iOS"); //maybe move this to when bundles go to server so bundles can be built near the same time.
                        CreateiOSBatchFile(autoGitYesNo);
                        //RoboCopyDependenciesFiles("iOS");
                        //RunBatchFile(Application.dataPath + "/Editor/IFX Tools/BundleTool/iOSSync.bat");
                        RunBatchFile(Application.dataPath + "/Editor/IFX Tools/BundleTool/iOSBuild.bat");
                        // Git stuff handled in batch file!
                        
                    }
                    // if true build windows 
                    if (windowsYes)
                    {
                        DeleteFolderContents(userSettings.projectWinLoc + "/AssetBundles/Windows"); //maybe move this to when bundles go to server so bundles can be built near the same time.
                        //Build the bundle
                        AssetBundles.BuildScript.BuildAssetBundles();
                        
                        //Copy bundles to cdn
                        if (userSettings.cdnWinLoc != "" && userSettings.CTMode())
                        {
                            CopyFolderContents(userSettings.projectWinLoc + "/AssetBundles/Windows", userSettings.cdnWinLoc);
                        }
                        
                        //Do Git stuff
                        if (autoGitYesNo)
                        {
                            GitPull();
                            GitPushWin();
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Notheing Selected - Select the folder you want build first");
            }

            

            void SetAssetLabelToFolderName(UnityEngine.Object item)
            {
                
                
                //This part changes the asset labelsS
                var itemPath = AssetDatabase.GetAssetPath(item);
                var itemDirectory = Path.GetDirectoryName(itemPath);
                var itemFolderName = Path.GetFileName(itemPath);//was itemDirectory, this is what it should be when intergrated into DB tool

                AssetImporter assetImporterForSelection = AssetImporter.GetAtPath(itemPath);//was itemDirectory, this is what it should be when intergrated into DB tool
                                                                                            //Debug.Log(assetImporterForSelection.assetBundleName);
                                                                                            //Debug.Log("this is the item dir:" + itemFolderName);
                assetImporterForSelection.assetBundleName = itemFolderName;

                //Debug.Log(assetImporterForSelection.assetBundleName);
            }
        }
        public void CreateTempCommandBatchFile(List<string> commands,string batchfileName)
        {
            string batchFilePath = Application.dataPath + "/Editor/IFX Tools/BundleTool/"+batchfileName+".bat";
            //Write some text to the test.txt file
            StreamWriter writer = new StreamWriter(batchFilePath, false);
            foreach (var command in commands)
            {
                writer.WriteLine(command);
            }
            writer.Close();
        }
        void RunBatchFile(string path)
        {
            //need some better way to set this for diffrent ifx project numbers
            FileInfo info = new FileInfo(path);
            System.Diagnostics.Process.Start(info.FullName);
            //Debug.Log(info);
        }
        public void CreateAndroidBatchFile(bool autoGitYesNo)
        {
            // string androidBuildPath = Application.dataPath + "/Editor/IFX Tools/BundleTool/AndroidBuild.bat";
            // //Write some text to the test.txt file
            // StreamWriter writer = new StreamWriter(androidBuildPath, false);
            List<string> commands = new List<string>();
            foreach (var command in RoboCopyDependenciesFiles("Android"))
            {
                commands.Add(command);
            }
            //commands.Add("robocopy "+userSettings.projectWinLoc+"/Assets "+userSettings.projectAndroidLoc+"/Assets /MIR");
            //delete the old bundles out
            commands.Add("del /s /q "+userSettings.projectWinLoc +"/IFXBuildToolProjects/Android/AssetBundles");
            commands.Add("\""+userSettings.unityEXELoc+"\" -quit -batchmode -buildTarget \"Android\" -projectPath \""+userSettings.projectAndroidLoc+"\" -executeMethod AssetBundles.BuildScript.BuildAssetBundles");
            
            commands.Add("robocopy "+userSettings.projectWinLoc+"/IFXBuildToolProjects/Android/AssetBundles/Android "+userSettings.projectWinLoc+"/AssetBundles/Android");
            if (userSettings.cdnAndroidLoc != "" && userSettings.CTMode())
            {
                commands.Add("robocopy "+userSettings.projectAndroidLoc+"/AssetBundles/Android "+userSettings.cdnAndroidLoc +" /MIR");
            }
            
            if (autoGitYesNo && userSettings.CTMode())
            {
                //Start of git bit
                string listOfBundles = string.Join(" ", bundlesBuiltAndroid);
                //commands = new string[8]{ "cd "+userSettings.cdnWinLoc, "cd ..", "cd ..", "cd ..", "cd ..","cd ..", "git add "+listOfBundles,"git commit -m "+gitCommitM};
                commands.Add("cd "+userSettings.cdnAndroidLoc);
                commands.Add("cd ..");
                commands.Add("cd ..");
                commands.Add("cd ..");
                commands.Add("cd ..");
                commands.Add("cd ..");
                commands.Add("git pull");
                commands.Add("git add "+listOfBundles);
                commands.Add("git commit -m "+"\""+gitCommitM+"_ANDROID"+"\"");
                commands.Add("git push");
            }
            CreateTempCommandBatchFile(commands,"AndroidBuild");
        }
        public void CreateiOSBatchFile(bool autoGitYesNo,List<string> commandsIN = null)
        {
            // string iOSBuildPath = Application.dataPath + "/Editor/IFX Tools/BundleTool/iOSBuild.bat";
            // //Write some text to the test.txt file
            // StreamWriter writer = new StreamWriter(iOSBuildPath, false);
            List<string> commands = new List<string>();
            
            foreach (var command in RoboCopyDependenciesFiles("iOS"))
            {
                commands.Add(command);
            }
            //delete the old bundles out
            commands.Add("del /s /q "+userSettings.projectWinLoc +"/IFXBuildToolProjects/iOS/AssetBundles");
            //commands.Add("robocopy "+userSettings.projectWinLoc+"/Assets "+userSettings.projectiOSLoc+"/Assets /MIR");
            commands.Add("\""+userSettings.unityEXELoc+"\" -quit -batchmode -buildTarget \"iOS\" -projectPath \""+userSettings.projectiOSLoc+"\" -executeMethod AssetBundles.BuildScript.BuildAssetBundles");
            
            commands.Add("robocopy "+userSettings.projectWinLoc +"/IFXBuildToolProjects/iOS/AssetBundles/iOS "+userSettings.projectWinLoc+"/AssetBundles/iOS ");
            if (userSettings.cdnAndroidLoc != "" && userSettings.CTMode())
            {
                commands.Add("robocopy "+userSettings.projectiOSLoc+"/AssetBundles/iOS "+userSettings.cdniOSLoc);
            }
            
            if (autoGitYesNo && userSettings.CTMode())
            {
                //Start of git bit
                string listOfBundles = string.Join(" ", bundlesBuiltiOS);
                //commands = new string[8]{ "cd "+userSettings.cdnWinLoc, "cd ..", "cd ..", "cd ..", "cd ..","cd ..", "git add "+listOfBundles,"git commit -m "+gitCommitM};
                commands.Add("cd "+userSettings.cdniOSLoc);
                commands.Add("cd ..");
                commands.Add("cd ..");
                commands.Add("cd ..");
                commands.Add("cd ..");
                commands.Add("cd ..");
                commands.Add("git pull");
                commands.Add("git add "+listOfBundles);
                commands.Add("git commit -m "+"\""+gitCommitM+"_iOS"+"\"");
                commands.Add("git push");
            }

            CreateTempCommandBatchFile(commands,"iOSBuild");
        }
        void CopyFolderContents(string source,string destination)
        {
            //Copy all files found in source folder to destination folder
            System.IO.DirectoryInfo sourceFolder = new DirectoryInfo(source);
            foreach (FileInfo file in sourceFolder.GetFiles())
            {
                string destFile = System.IO.Path.Combine(destination, file.Name);
                Debug.Log(destFile);
                System.IO.File.Copy(file.FullName, destFile, true);
            }
        }

        void DeleteFolderContents(string Folder)
        {   
            if (Directory.Exists(Folder))
            {
                System.IO.DirectoryInfo dir = new DirectoryInfo(Folder);
                foreach (FileInfo file in dir.GetFiles())
                {
                    file.Delete(); 
                }
            }
            else
            {
                Debug.Log("DeleteFolderContents: Can't delete contents as no folder exists at path");
            }
            
        }
        void ClearAssetLabelsDirectorys(string[] directory)
        {
            foreach (var assetfolder in directory)
            {
                Debug.Log(assetfolder);
                AssetImporter assetImporter = AssetImporter.GetAtPath(assetfolder);
                //Debug.Log(assetImporter.assetBundleName);
                assetImporter.assetBundleName = "";
            }
        }
        public void ClearAssetLabelGameObject(GameObject item)
        {
            string prefabPath = AssetDatabase.GetAssetPath(item);
            AssetImporter assetImporter = AssetImporter.GetAtPath(prefabPath);
            assetImporter.assetBundleName = "";
        }
        public static void ClearAllAssetLabelsInProject()
        {

            var names = AssetDatabase.GetAllAssetBundleNames();
            foreach (var name in names)
            {
                AssetDatabase.RemoveAssetBundleName(name,true);
            }
            
            // string[] LabeledAssets = AssetDatabase.FindAssets("l:", new[] {"Assets"});
            // List<string> assetPaths = new List<string>();
            // foreach (var asset in LabeledAssets)
            // {
            //     var path = AssetDatabase.GUIDToAssetPath(asset);
            //     assetPaths.Add(path);
            //     Debug.Log(path);
            // }
            // ClearAssetLabelsDirectorys(assetPaths.ToArray());
        }
        
        void GitPushWin()
        {
            // Declaration of the array
            string[] commands;
            string listOfBundles = string.Join(" ", bundlesBuiltWin);
            Debug.Log(listOfBundles);
            // Initialization of array
            commands = new string[9]{ "cd "+userSettings.cdnWinLoc, "cd ..", "cd ..", "cd ..", "cd ..","cd ..", "git add "+listOfBundles,"git commit -m "+"\""+gitCommitM+"\"", "git push"};
            RunCMD(commands);
        }
        public void GitPull()
        {
            // Declaration of the array
            string[] commands;

            // Initialization of array
            commands = new string[7]{ "cd "+userSettings.cdnWinLoc, "cd ..", "cd ..", "cd ..", "cd ..","cd ..", "git pull"};
            RunCMD(commands);
            
        }
        public void SetupUnityProjects(string buildType)
        {
            userSettings.SettingsAutoSetup();
            //needs to set user settings android project location at some point
            string[] commands;            
            
            // Initialization of array
            commands = new string[]
            {
            "mkdir "+userSettings.projectWinLoc.Replace("/","\\")+"\\IFXBuildToolProjects\\"+buildType+"\\AssetBundles\\"+buildType, 
            "robocopy "+userSettings.projectWinLoc+"/Assets/Editor "+userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType+"/Assets/Editor"+" /MIR /XD "+userSettings.projectWinLoc+"/IFXBuildToolProjects",
            "robocopy "+userSettings.projectWinLoc+"/Assets/--ENGAGE-IFXProjectPlugin "+userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType+"/Assets/--ENGAGE-IFXProjectPlugin"+" /MIR /XD "+userSettings.projectWinLoc+"/IFXBuildToolProjects",
            
            "robocopy "+userSettings.projectWinLoc+"/ProjectSettings "+userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType+"/ProjectSettings"+ "/MIR /XD "+userSettings.projectWinLoc+"/IFXBuildToolProjects",
            "\""+userSettings.unityEXELoc+"\""+" -quit -batchmode -buildTarget \""+buildType+"\" -projectPath "+userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType          
            };
            //"robocopy "+userSettings.projectWinLoc+"/AssetBundles "+userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType+"/AssetBundles /MIR"
            //userSettings.unityEXELoc+" -quit -batchmode -buildTarget \""+buildType+"\" -projectPath "+userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType
            //commands = new string[]{ "robocopy "+userSettings.projectWinLoc+" "+userSettings.projectWinLoc+"/IFXBuildToolProjects/" +buildType+ " /MIR /XD "+userSettings.projectWinLoc+"/IFXBuildToolProjects",
            //userSettings.unityEXELoc+" -quit -batchmode -buildTarget \""+buildType+"\" -projectPath "+userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType};

            if (buildType == "Android")
            {
                userSettings.projectAndroidLoc = userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType;
                userSettings.SaveUserSettings();
            }
            if (buildType == "iOS")
            {
                userSettings.projectiOSLoc = userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType;
                userSettings.SaveUserSettings();
            }
            
            // foreach (var item in commands)
            // {
            //     Debug.Log(item);
            // }
            //commands =commands.Replace("/","\\");
            BatchRunCMDS(commands,buildType);
        }
        public void BatchRunCMDS(string[] input,string fileNameforBatch)
        {
            string TempCMDBatchPath = Application.dataPath + "/Editor/IFX Tools/BundleTool/"+fileNameforBatch+"_Temp.bat";
            //Write some text to the test.txt file
            StreamWriter writer = new StreamWriter(TempCMDBatchPath, false);
            foreach (string cmd in input)
            {
                //string cmdIN = cmd.Replace("/","\\");
                writer.WriteLine(cmd);
            }
            
            
            writer.Close();
            RunBatchFile(TempCMDBatchPath);
            //File.Delete(TempCMDBatchPath);
        }
        void RunCMD(string[] arguments)
        {
            Process cmd = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.RedirectStandardInput = true;
            info.UseShellExecute = false;
            cmd.StartInfo = info;
            cmd.Start();
            using (StreamWriter sw = cmd.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    foreach (string arg in arguments)
                    {
                        
                        string argIN = arg.Replace("/","\\");
                        sw.WriteLine(argIN);
                        Debug.Log("CMD LINE input command: "+arg);
                    }
                }
            }
            cmd.StandardInput.Close();
            cmd.WaitForExit();
        }
        public  bool DirectoryBool(UnityEngine.Object selectedObject) //fix
        {
            var path = "";
            bool dirFound=false;
            if (selectedObject == null)
            {
                    path = "Assets";
            }
            else
            {
                    path = AssetDatabase.GetAssetPath(selectedObject.GetInstanceID());
            }
            if (path.Length > 0)
            {
                if (Directory.Exists(path))
                {
                    dirFound= true;
                    //Debug.Log("Folder");
                }
                else
                {
                    dirFound= false;
                }
            }
            return dirFound;
        }

        public bool DirectoryBoolMulti(UnityEngine.Object[] selectedObject) //fix
        {
            bool dirFound=true;
            bool dir=false;
            foreach (UnityEngine.Object item in selectedObject)
            {
                dir=DirectoryBool(item);
                if (dir==false)
                {
                    
                    dirFound=false;
                    break;
                }
            }
            return dirFound;
        }
        public List<string> RoboCopyDependenciesFiles(string buildType)
        {
            string[] dependencies =  GetFolderDependencies(Selection.objects).ToArray();
            List<string> commands = new List<string>();
            foreach (var itemPath in dependencies)
            {
                var itemDirectory = Path.GetDirectoryName(itemPath);
                commands.Add("robocopy "+userSettings.projectWinLoc+"/"+itemDirectory+" "+userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType+"/"+itemDirectory+" /MIR");

                
            }
            foreach (var item in Selection.objects)
            {
                var directoryName = Path.GetDirectoryName(AssetDatabase.GetAssetPath(item));
                Debug.Log(directoryName);
                //var bundlesParentFolder = System.IO.Directory.GetParent(directoryName);
                //Debug.Log(bundlesParentFolder.Name);
                commands.Add("robocopy "+userSettings.projectWinLoc+"/"+directoryName +" "+ userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType+"/" +directoryName +"  *.meta /MIR");
                //commands.Add("robocopy "+userSettings.projectWinLoc+"/"+bundlesParentFolder +" "+ userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType+"/" +bundlesParentFolder +"  *.meta /MIR");
            }
            
            return commands;
            //commands.Add("PAUSE");
            // string fileName = buildType+"Sync";
            // CreateTempCommandBatchFile(commands,fileName);
            // RunBatchFile(Application.dataPath + "/Editor/IFX Tools/BundleTool/"+fileName+".bat");
            //RunCMD(commands.ToArray());
        }

        private List<string> GetFolderDependencies(UnityEngine.Object[] goIN) // this is a test of the export package idea can probobly delete
        {
            List<string> result = new List<string>();
            foreach (var go in goIN)
            {
                string assetPath =AssetDatabase.GetAssetPath(go);
                
                AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
                if (assetImporter.assetBundleName != go.name)
                {
                    assetImporter.assetBundleName = go.name;
                }
                

                
                string[] assetsInCurBundle = AssetDatabase.GetAssetPathsFromAssetBundle(assetImporter.assetBundleName);
                foreach (string item in assetsInCurBundle)
                {
                    string[] dpeendencies = AssetDatabase.GetDependencies(item,true);
                    foreach (var item2 in dpeendencies)
                    {
                        result.Add(item2);
                        //Debug.Log(item2);
                    }
                    
                    
                    
                }
                
            }
            return result;
        }

        public void ClearDependenciesCache()
        {
            Directory.Delete(userSettings.projectWinLoc+"/IFXBuildToolProjects/",true);
            EditorUtility.DisplayDialog("Cache Cleared",
            "Cache cleared, rebuilding projects", "OK");   
        }
    }
 }