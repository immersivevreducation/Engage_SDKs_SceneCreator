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

        
     }
    public class IFXBundleTools : EditorWindow
    {
              
        IFXToolsQualityCheckTool qaTool;
        IFXToolsUserSettings userSettings;
        IBundleToolsParentWindow parentWindow;
        public List<Object> selectedBundles {get; set;}
        public IFXBundleTools()
        {
  
        }
        public void Init(IBundleToolsParentWindow parentWindowIN,IFXToolsUserSettings userSettingsIN)
        {
            parentWindow = parentWindowIN;
            userSettings = userSettingsIN;
            //CreateEditorDirectoryies();
        }
        public bool buildQACheckOverride {get; set;}
        public string gitCommitM {get; set;}
        public bool windowsBuildYesNo {get; set;}
        public bool androidBuildYesNo {get; set;}
        public bool iOSBuildYesNo {get; set;}
        public bool autoGitYesNo {get; set;}
               
        bool passedQualityCheck;
        
        
        List<string> bundlesBuiltWin = new List<string>();
        List<string> bundlesBuiltAndroid = new List<string>();
        List<string> bundlesBuiltiOS = new List<string>();
       
        public void BuildSelectedBundle(List<Object> selectedBundleIN,
        bool windowsYes,
        bool androidYes,
        bool iOSYes,
        bool autoGitYes,
        bool buildQACheckOverrideIN=false,
        string gitCommitMessageIN = "")
        {
            selectedBundles = selectedBundleIN;

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
            /////////////////////////////////////////////////////////////^Set Up ^///////////////////////////
            
            if (selectedBundles !=null)
            {                            
                ClearAllAssetLabelsInProject();
                //re add asset labels based on folders names to selected folders
                for (int i = 0; i < selectedBundles.Count; i++)
                {
                    SetAssetLabelToFolderName(selectedBundles[i]);
                }                  
                
                //Checks for bad components
                if (!buildQACheckOverride)
                {
                    
                    qaTool = (IFXToolsQualityCheckTool)ScriptableObject.CreateInstance(typeof(IFXToolsQualityCheckTool));
                    qaTool.Init(this);
                    bool qaCheck = qaTool.BundleQualityCheck(selectedBundles); //pass QA true or false
                    if (!qaCheck)
                    {
                        passedQualityCheck = false;
                    }
                    
                }
            ///////////////////////////////////////////////////////Passed QA Check - Build bundles////////////////////
                if (passedQualityCheck | buildQACheckOverride)
                {
                    buildQACheckOverride = false;

                    //This part isn't auto "add"ing the files to a git comit. this part just makes the varable to hold all the paths
                if (autoGitYes)
                    {
                        SetUpGitPathsForCreatedFiles(windowsYes, androidYes, iOSYes);
                    }
                    // if true build android 
                    if (androidYes)
                    {
                        DeleteFolderContents(userSettings.projectAndroidLoc + "/AssetBundles/Android"); //clears out old bundles
                        string androidBuildPath = CreateBatchCMDSFile("Android",SyncUnityProjects("Android"),CreateAndroidBatchFile(autoGitYesNo));                      
                        RunBatchFile(androidBuildPath);                       
                    }
                    // if true build iOS 
                    if (iOSYes)
                    {
                        DeleteFolderContents(userSettings.projectiOSLoc + "/AssetBundles/iOS"); //clears out old bundles
                        string iOSBuildPath = CreateBatchCMDSFile("iOS",SyncUnityProjects("iOS"),CreateiOSBatchFile(autoGitYesNo));
                        RunBatchFile(iOSBuildPath);
                        // Git stuff handled in batch file!
                        
                    }
                    // if true build windows 
                    if (windowsYes)
                    {
                        DeleteFolderContents(userSettings.projectWinLoc + "/AssetBundles/Windows"); //clears out old bundles
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
            ///////////////////////////////////////////Local Methods/////////////////////////////////////////////////
            void SetAssetLabelToFolderName(UnityEngine.Object item)
            {
                //This part changes the asset labelsS
                var itemPath = AssetDatabase.GetAssetPath(item);
                var itemDirectory = Path.GetDirectoryName(itemPath);
                var itemFolderName = Path.GetFileName(itemPath);
                AssetImporter assetImporterForSelection = AssetImporter.GetAtPath(itemPath);                                                                                            
                assetImporterForSelection.assetBundleName = itemFolderName;
            }

            void SetUpGitPathsForCreatedFiles(bool winYes, bool andYes, bool iYes)
            {
                for (int i = 0; i < selectedBundles.Count; i++)
                {
                    if (androidYes)
                    {
                        bundlesBuiltAndroid.Add(userSettings.cdnAndroidLoc + "/" + selectedBundles[i].name + ".engagebundle");
                        bundlesBuiltAndroid.Add(userSettings.cdnAndroidLoc + "/" + selectedBundles[i].name + ".manifest");
                    }
                    if (iOSYes)
                    {
                        bundlesBuiltiOS.Add(userSettings.cdniOSLoc + "/" + selectedBundles[i].name + ".engagebundle");
                        bundlesBuiltiOS.Add(userSettings.cdniOSLoc + "/" + selectedBundles[i].name + ".manifest");
                    }
                    if (windowsYes)
                    {
                        bundlesBuiltWin.Add(userSettings.cdnWinLoc + "/" + selectedBundles[i].name + ".engagebundle");
                        bundlesBuiltWin.Add(userSettings.cdnWinLoc + "/" + selectedBundles[i].name + ".manifest");
                    }
                }
            }
        }
        
        void RunBatchFile(string path)
        {
            FileInfo info = new FileInfo(path);
            System.Diagnostics.Process.Start(info.FullName);
            //Debug.Log(info);
        }
        public List<string> CreateAndroidBatchFile(bool autoGitYesNo)
        {
            List<string> commands = new List<string>();
            foreach (var command in RoboCopyDependenciesFiles("Android"))
            {
                commands.Add(command);
            }           
            commands.Add("\""+userSettings.unityEXELoc+"\" -quit -batchmode -buildTarget \"Android\" -projectPath \""+userSettings.projectAndroidLoc+"\" -executeMethod AssetBundles.BuildScript.BuildAssetBundles");
            
            commands.Add("robocopy "+"\""+userSettings.projectWinLoc +"/IFXBuildToolProjects/Android/AssetBundles/Android"+"\""+" "+"\""+userSettings.projectWinLoc+"/AssetBundles/Android"+"\"");
            if (userSettings.cdnAndroidLoc != "" && userSettings.CTMode())
            {
                commands.Add("robocopy "+"\""+userSettings.projectAndroidLoc+"/AssetBundles/Android"+"\""+" "+"\""+userSettings.cdnAndroidLoc+"\"");
            }
            
            if (autoGitYesNo && userSettings.CTMode())
            {
                //Start of git bit
                string listOfBundles = string.Join(" ", bundlesBuiltAndroid);
                //commands = new string[8]{ "cd "+userSettings.cdnWinLoc, "cd ..", "cd ..", "cd ..", "cd ..","cd ..", "git add "+listOfBundles,"git commit -m "+gitCommitM};
                commands.Add("cd /D "+"\""+userSettings.cdnProjectPath+"\""); 
                commands.Add("git stash");
                commands.Add("git pull");
                commands.Add("git stash pop");
                commands.Add("git add "+listOfBundles);
                commands.Add("git commit -m "+"\""+gitCommitM+"_Android"+"\"");
                commands.Add("git push");
            }
            return commands;
            
        }
        public List<string> CreateiOSBatchFile(bool autoGitYesNo,List<string> commandsIN = null)
        {
            // string iOSBuildPath = Application.dataPath + "/Editor/IFX Tools/BundleTool/iOSBuild.bat";
            // //Write some text to the test.txt file
            // StreamWriter writer = new StreamWriter(iOSBuildPath, false);
            List<string> commands = new List<string>();
            
            foreach (var command in RoboCopyDependenciesFiles("iOS"))
            {
                commands.Add(command);
            }
            
            
            //commands.Add("robocopy "+userSettings.projectWinLoc+"/Assets "+userSettings.projectiOSLoc+"/Assets /MIR");
            commands.Add("\""+userSettings.unityEXELoc+"\" -quit -batchmode -buildTarget \"iOS\" -projectPath \""+userSettings.projectiOSLoc+"\" -executeMethod AssetBundles.BuildScript.BuildAssetBundles");
            
            commands.Add("robocopy "+"\""+userSettings.projectWinLoc +"/IFXBuildToolProjects/iOS/AssetBundles/iOS"+"\""+" "+"\""+userSettings.projectWinLoc+"/AssetBundles/iOS"+"\"");
            if (userSettings.cdnAndroidLoc != "" && userSettings.CTMode())
            {
                commands.Add("robocopy "+"\""+userSettings.projectiOSLoc+"/AssetBundles/iOS"+"\""+" "+"\""+userSettings.cdniOSLoc+"\"");
            }
            
            if (autoGitYesNo && userSettings.CTMode())
            {
                //Start of git bit
                string listOfBundles = string.Join(" ", bundlesBuiltiOS);
                //commands = new string[8]{ "cd "+userSettings.cdnWinLoc, "cd ..", "cd ..", "cd ..", "cd ..","cd ..", "git add "+listOfBundles,"git commit -m "+gitCommitM};                
                commands.Add("cd /D "+"\""+userSettings.cdnProjectPath+"\""); 
                commands.Add("git stash");
                commands.Add("git pull");
                commands.Add("git stash pop");
                commands.Add("git add "+listOfBundles);
                commands.Add("git commit -m "+"\""+gitCommitM+"_iOS"+"\"");
                commands.Add("git push");
            }
            return commands;
            
        }
        void CopyFolderContents(string source,string destination)
        {
            
            if (Directory.Exists(source))
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
            else
            {
                Debug.Log("Could not copy folder contents, Folder doesn't exist: "+ source);
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
        
        public static void ClearAllAssetLabelsInProject()
        {
            var names = AssetDatabase.GetAllAssetBundleNames();
            foreach (var name in names)
            {
                AssetDatabase.RemoveAssetBundleName(name,true);
            }
        }
        
        void GitPushWin()
        {
            // Declaration of the array
            List<string> commands= new List<string>();
            string listOfBundles = string.Join(" ", bundlesBuiltWin);
            //Debug.Log(listOfBundles);
            // Initialization of array
            commands.Add("cd /D "+"\""+userSettings.cdnProjectPath+"\"");            

            commands.Add("git add "+listOfBundles);
            commands.Add("git commit -m "+"\""+gitCommitM+"\"");
            commands.Add("git push");
            
            RunCMD(commands);
        }
        public void GitPull()
        {
            // Declaration of the array
            List<string> commands= new List<string>();

            // Initialization of array
            commands.Add("cd /D "+"\""+userSettings.cdnProjectPath+"\"");          

            //commands.Add("git stash");
            commands.Add("git pull");
            //commands.Add("git stash pop");
            
            RunCMD(commands);
            
        }
        public List<string> SyncUnityProjects(string buildType)
        {
            //userSettings.SettingsAutoSetup();
            //needs to set user settings android project location at some point
            List<string> commands = new List<string>();            
            
            commands.Add("mkdir "+userSettings.projectWinLoc.Replace("/","\\")+"\\IFXBuildToolProjects\\"+buildType+"\\AssetBundles\\"+buildType);
            commands.Add("robocopy "+"\""+userSettings.projectWinLoc+"/Assets/ENGAGE_CreatorSDK"+"\""+ " " +"\""+userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType+"/Assets/ENGAGE_CreatorSDK"+"\""+" "+" /MIR /XD "+"\""+userSettings.projectWinLoc+"/IFXBuildToolProjects"+"\"");
            commands.Add("robocopy "+"\""+userSettings.projectWinLoc+"/ProjectSettings"+"\""+" "+"\""+userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType+"/ProjectSettings"+"\""+ " /MIR /XD "+"\""+userSettings.projectWinLoc+"/IFXBuildToolProjects"+"\""); 
            
                       
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

            return commands;
            
        }
        public void GitCommitChangesToRepo(List<Object> selectedBundleIN,string gitCommitM)
        {
            List<string> commands = new List<string>();
            List<string> filesToAdd = new List<string>();
            filesToAdd.AddRange(GetFolderDependencies(selectedBundleIN));

            commands.Add("cd /D "+userSettings.projectWinLoc);
            commands.Add("git stash");
            commands.Add("git pull");
            commands.Add("git stash pop");
            commands.Add("git reset");
            foreach (var item in filesToAdd)
            {
                commands.Add("git add "+"\""+userSettings.projectWinLoc+"/"+item+"\"");
                commands.Add("git add "+"\""+userSettings.projectWinLoc+"/"+item+".meta"+"\"");
            }
            commands.Add("git status");
            commands.Add("git commit -m "+"\""+gitCommitM+"\"");
            commands.Add("git push");
            commands.Add("PAUSE");


            foreach (var item in commands)
            {
                Debug.Log(item);
            }
            string gitCommitBatch = CreateBatchCMDSFile("GitCommitToRepo",commands);
            RunBatchFile(gitCommitBatch);
        }
        public string CreateBatchCMDSFile(string fileNameforBatch,List<string> input,List<string> input2=null,List<string> input3=null)
        {

            List<string> commandsList = input;
            if (input2 != null)
            {
                commandsList.AddRange(input2);
            }
            if (input3 != null)
            {
                commandsList.AddRange(input3);
            }

            string TempCMDBatchPath = Application.dataPath + "/ENGAGE_CreatorSDK/Editor/IFX Tools/BundleTool/"+fileNameforBatch+"_Temp.bat";
            //Write some text to the test.txt file
            StreamWriter writer = new StreamWriter(TempCMDBatchPath, false);
            foreach (string cmd in commandsList)
            {
                //string cmdIN = cmd.Replace("/","\\");
                writer.WriteLine(cmd);
            }
            
            
            writer.Close();
            return TempCMDBatchPath;
        }
        void RunCMD(List<string> arguments)
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
                        
                        //string argIN = arg.Replace("/","\\");
                        sw.WriteLine(arg);
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
        public List<UnityEngine.Object> GetSelectedObjectsAsList()
        {
            List<UnityEngine.Object> selection =new List<UnityEngine.Object>();
            foreach (var item in Selection.objects)
            {
                selection.Add(item);
            }
            return selection;
        }
        public List<string> RoboCopyDependenciesFiles(string buildType)
        {
            
            List<string> dependencies =  GetFolderDependencies(GetSelectedObjectsAsList());
            List<string> commands = new List<string>();
            foreach (var itemPath in dependencies)
            {
                var itemDirectory = Path.GetDirectoryName(itemPath);
                commands.Add("robocopy "+"\""+userSettings.projectWinLoc+"/"+itemDirectory+"\""+" "+"\""+userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType+"/"+itemDirectory+"\""+" /MIR");

                
            }
            foreach (var item in Selection.objects)
            {
                var directoryName = Path.GetDirectoryName(AssetDatabase.GetAssetPath(item));
                Debug.Log(directoryName);
                //var bundlesParentFolder = System.IO.Directory.GetParent(directoryName);
                //Debug.Log(bundlesParentFolder.Name);
                commands.Add("robocopy "+"\""+userSettings.projectWinLoc+"/"+directoryName +"\""+" "+ "\""+userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType+"/" +directoryName +"\""+"  *.meta /MIR");
                //commands.Add("robocopy "+userSettings.projectWinLoc+"/"+bundlesParentFolder +" "+ userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType+"/" +bundlesParentFolder +"  *.meta /MIR");
            }
            
            return commands;
        }

        private List<string> GetFolderDependencies(List<UnityEngine.Object> goIN) // this is a test of the export package idea can probobly delete
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

        public void ClearDependenciesCache(bool hardReset)
        {
            if (Directory.Exists(userSettings.projectWinLoc+"/IFXBuildToolProjects/"))
            {
                if (hardReset)
                {
                    Directory.Delete(userSettings.projectWinLoc+"/IFXBuildToolProjects/",true);
                    EditorUtility.DisplayDialog("Cache Cleared",
                    "Cache Fully cleared", "OK");
                }
                else
                {
                    if (Directory.Exists(userSettings.projectWinLoc+"/IFXBuildToolProjects/Android/Assets"))
                    {
                        Directory.Delete(userSettings.projectWinLoc+"/IFXBuildToolProjects/Android/Assets",true);
                    }
                    if (Directory.Exists(userSettings.projectWinLoc+"/IFXBuildToolProjects/iOS/Assets"))
                    {
                        Directory.Delete(userSettings.projectWinLoc+"/IFXBuildToolProjects/iOS/Assets",true);
                    }
                    
                    
                    EditorUtility.DisplayDialog("Cache Cleared",
                    "Cache cleared", "OK");
                }
                
                
            }
            else
            {
                Debug.Log("Cache Directory not found");
            }
            
        }
    }
 }