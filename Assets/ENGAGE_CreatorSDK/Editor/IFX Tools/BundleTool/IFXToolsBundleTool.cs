using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using systemDebug = System.Diagnostics;
using Process = System.Diagnostics.Process;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;
using IFXToolSM = IFXTools.IFXToolsStaticMethods;


 /////////////TO-DO////
 // if scenes are bing built change over all the auto git stuff paths to the scene versions

 namespace IFXTools{

     public  class BundleBuildSettings
     {
        public List<Object> selectedBundles {get; set;}
        public bool buildQACheckOverride {get; set;}
        public string gitCommitMessage {get; set;}
        public bool windowsBuildYesNo {get; set;}
        public bool androidBuildYesNo {get; set;}
        public bool iOSBuildYesNo {get; set;}
        public bool autoGitYesNo {get; set;}
        
     }
    public class IFXBundleTools : EditorWindow
    {
              
        IFXToolsQualityCheckTool qaTool;
        IFXToolsUserSettings userSettings;
        List<Object> selectedBundles {get; set;}
        string bundleBuildLog {get; set;}
        string bundleBuildLogWindows {get; set;}
        //string bundleBuildLogAndroid {get; set;}
        //string bundleBuildLogiOS {get; set;}
        string bundleBuildLogGIT {get; set;}
        
        //System.Text.StringBuilder bundleBuildLog;
        public IFXBundleTools()
        {
  
        }
        public void Init(IFXToolsUserSettings userSettingsIN)
        {
            userSettings = userSettingsIN;
        }
        
               
        bool passedQualityCheck;
        
        public string buildingStatus;
        
        List<string> bundlesBuiltWin = new List<string>();
        List<string> bundlesBuiltAndroid = new List<string>();
        List<string> bundlesBuiltiOS = new List<string>();
        
       
        public void BuildSelectedBundle(BundleBuildSettings buildSettings)
        {   
            //check for module folders        
            if (buildSettings.androidBuildYesNo)
            {
                if (IFXToolSM.CheckBuildModuleInstalled("AndroidPlayer")==false)
                {
                    EditorUtility.DisplayDialog("WARNING!", "Android Build module not installed!", "OK");
                    return;                          
                }
                
            }
            if (buildSettings.iOSBuildYesNo)
            {
                if (IFXToolSM.CheckBuildModuleInstalled("iOSSupport")==false)
                {
                    EditorUtility.DisplayDialog("WARNING!", "iOS Build module not installed!", "OK");
                    return;                          
                }
            }
            passedQualityCheck = true;
            
            //local Cdn paths//To enable swaping from ifx to scenes
            string cdnLocalWinLoc = userSettings.cdnWinIFXLoc;
            string cdnLocalAndroidLoc = userSettings.cdnAndroidIFXLoc;
            string cdnLocaliOSLoc = userSettings.cdniOSIFXLoc;
            for (int i = 0; i < buildSettings.selectedBundles.Count; i++)
            {
                if(IFXToolSM.DoesSelectedFolderContainFileType(buildSettings.selectedBundles[i], "*.unity") == true)
                {
                    cdnLocalWinLoc = userSettings.cdnWinSceneLoc;
                    cdnLocalAndroidLoc = userSettings.cdnAndroidSceneLoc;
                    cdnLocaliOSLoc = userSettings.cdniOSSceneLoc;
                }          
            }  
            


            if (buildSettings.autoGitYesNo)
            {
                IFXToolSM.GitPull(userSettings.cdnProjectPath);       
            }
            bundlesBuiltWin.Clear();
            bundlesBuiltAndroid.Clear();
            bundlesBuiltiOS.Clear();
            /////////////////////////////////////////////////////////////^Set Up ^///////////////////////////
            
            if (buildSettings.selectedBundles !=null)
            {                            
                IFXToolSM.ClearAllAssetLabelsInProject();
                
                for (int i = 0; i < buildSettings.selectedBundles.Count; i++)
                {
                    //re add asset labels based on folders names to selected folders
                    SetAssetLabelToFolderName(buildSettings.selectedBundles[i]);
                    
                    
                }                  
                
                //Checks for bad components
                if (!buildSettings.buildQACheckOverride)
                {
                    
                    qaTool = (IFXToolsQualityCheckTool)ScriptableObject.CreateInstance(typeof(IFXToolsQualityCheckTool));
                    qaTool.Init(this);
                    bool qaCheck = qaTool.BundleQualityCheck(buildSettings); //pass QA true or false
                    if (!qaCheck)
                    {
                        passedQualityCheck = false;
                    }
                    
                }
            ///////////////////////////////////////////////////////Passed QA Check - Build bundles////////////////////
                if (passedQualityCheck | buildSettings.buildQACheckOverride)
                {
                    buildSettings.buildQACheckOverride = false;                   
                    buildingStatus = "Building Bundles";
                    buildBundlesAsync();
                }
            }
            else
            {
                Debug.Log("Nothing Selected - Select the folder you want build first");
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

            void SetUpGitPathsForCreatedFiles()
            {
                for (int i = 0; i < buildSettings.selectedBundles.Count; i++)
                {
                    //Check if any of the selected folders have scenes. 
                    bool isScene = IFXToolSM.DoesSelectedFolderContainFileType(buildSettings.selectedBundles[i], "*.unity");                    

                    if (isScene == true)
                    {
                        Debug.Log("scene found in bundle: "+buildSettings.selectedBundles[i].name);
                        if (buildSettings.androidBuildYesNo)
                        {
                            bundlesBuiltAndroid.Add(userSettings.cdnAndroidSceneLoc + "/" + buildSettings.selectedBundles[i].name + ".engagebundle");
                            bundlesBuiltAndroid.Add(userSettings.cdnAndroidSceneLoc + "/" + buildSettings.selectedBundles[i].name + ".manifest");
                        }
                        if (buildSettings.iOSBuildYesNo)
                        {
                            bundlesBuiltiOS.Add(userSettings.cdniOSSceneLoc + "/" + buildSettings.selectedBundles[i].name + ".engagebundle");
                            bundlesBuiltiOS.Add(userSettings.cdniOSSceneLoc + "/" + buildSettings.selectedBundles[i].name + ".manifest");
                        }
                        if (buildSettings.windowsBuildYesNo)
                        {
                            bundlesBuiltWin.Add(userSettings.cdnWinSceneLoc + "/" + buildSettings.selectedBundles[i].name + ".engagebundle");
                            bundlesBuiltWin.Add(userSettings.cdnWinSceneLoc + "/" + buildSettings.selectedBundles[i].name + ".manifest");
                        }
                    }
                    else
                    {
                        if (buildSettings.androidBuildYesNo)
                        {
                            bundlesBuiltAndroid.Add(userSettings.cdnAndroidIFXLoc + "/" + buildSettings.selectedBundles[i].name + ".engagebundle");
                            bundlesBuiltAndroid.Add(userSettings.cdnAndroidIFXLoc + "/" + buildSettings.selectedBundles[i].name + ".manifest");
                        }
                        if (buildSettings.iOSBuildYesNo)
                        {
                            bundlesBuiltiOS.Add(userSettings.cdniOSIFXLoc + "/" + buildSettings.selectedBundles[i].name + ".engagebundle");
                            bundlesBuiltiOS.Add(userSettings.cdniOSIFXLoc + "/" + buildSettings.selectedBundles[i].name + ".manifest");
                        }
                        if (buildSettings.windowsBuildYesNo)
                        {
                            bundlesBuiltWin.Add(userSettings.cdnWinIFXLoc + "/" + buildSettings.selectedBundles[i].name + ".engagebundle");
                            bundlesBuiltWin.Add(userSettings.cdnWinIFXLoc + "/" + buildSettings.selectedBundles[i].name + ".manifest");
                        }
                    }
                    
                }
            }
            void GitPushBundlesToCDN()
            {
                
                List<string> commands= new List<string>();
                //string listOfBundles = string.Join(" ", bundlesBuiltWin);
                
                commands.Add("cd /D "+"\""+userSettings.cdnProjectPath+"\"");            

                foreach (var item in bundlesBuiltWin)
                {
                    commands.Add("git add "+"\""+item+"\"");
                }
                foreach (var item in bundlesBuiltAndroid)
                {
                    commands.Add("git add "+"\""+item+"\"");
                }
                foreach (var item in bundlesBuiltiOS)
                {
                    commands.Add("git add "+"\""+item+"\"");
                }
                commands.Add("git commit -m "+"\""+buildSettings.gitCommitMessage+"\"");
                commands.Add("git push");
                
                IFXToolSM.RunCMD(commands);
            }

            async Task buildBundlesAsync()
            {
                
                SetUpGitPathsForCreatedFiles();

                // if true build windows 
                if (buildSettings.windowsBuildYesNo)
                {
                    
                    IFXToolSM.DeleteFolderContents(userSettings.projectWinLoc + "/AssetBundles/Windows"); //clears out old bundles                
                    //Build the bundle
                    AssetBundles.BuildScript.BuildAssetBundles();       
                    //Copy bundles to cdn
                    if (!string.IsNullOrEmpty(cdnLocalWinLoc) && userSettings.CTMode())
                    {                        
                        IFXToolSM.CopyFolderContents(userSettings.projectWinLoc + "/AssetBundles/Windows", cdnLocalWinLoc);                        
                    }                          
                }
                
                
                
                List<Task<string>> BundleBuildTasks = new List<Task<string>>();
                // if true build android 
                if (buildSettings.androidBuildYesNo)
                {
                    IFXToolSM.DeleteFolderContents(userSettings.projectAndroidLoc + "/AssetBundles/Android"); //clears out old bundles
                    IFXToolSM.DeleteFolderContents(userSettings.projectWinLoc + "/AssetBundles/Android"); //clears out old bundles
                    string androidBuildPath = IFXToolSM.CreateBatchCMDSFile("Android", IFXToolSM.SyncUnityProjects("Android",userSettings.projectWinLoc), CreateAndroidBatchFile(buildSettings, cdnLocalAndroidLoc));
                    //RunBuildFileAsync(androidBuildPath, "Android");
                    BundleBuildTasks.Add(Task.Run(() => RunBatchFileAsync(androidBuildPath)));
                    //Debug.Log(androidBuild.Output);


                }
                // if true build iOS 
                if (buildSettings.iOSBuildYesNo)
                {
                    IFXToolSM.DeleteFolderContents(userSettings.projectiOSLoc + "/AssetBundles/iOS"); //clears out old bundles
                    IFXToolSM.DeleteFolderContents(userSettings.projectWinLoc + "/AssetBundles/iOS"); //clears out old bundles
                    string iOSBuildPath = IFXToolSM.CreateBatchCMDSFile("iOS", IFXToolSM.SyncUnityProjects("iOS",userSettings.projectWinLoc), CreateiOSBatchFile(buildSettings, cdnLocaliOSLoc));
                    //RunBuildFileAsync(iOSBuildPath, "iOS");
                    BundleBuildTasks.Add(Task.Run(() => RunBatchFileAsync(iOSBuildPath)));
                    // Git stuff handled in batch file!

                }
               
                Debug.Log("Waiting for all build tasks to finish");
                var results = await Task.WhenAll(BundleBuildTasks);
                Debug.Log("all build tasks now finished");
                /////////////////////////////CDN Push////////////////////////              
                
                buildingStatus = "Finished Building";
                if (buildSettings.autoGitYesNo)
                {
                    GitPushBundlesToCDN();
                }
               
               BuildDebugChecks(results);  
               ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////                

                void BuildDebugChecks(string[] buildLogsIN)
                {
                    StringBuilder bundleBuildLogFull = new StringBuilder();
                    bundleBuildLogFull.Append(System.Environment.NewLine + " //////////////////////////////////////////USER SETTINGS///////////////////////////////////");
                    bundleBuildLogFull.Append(System.Environment.NewLine + userSettings.cdnProjectPath + " -CDN Project path");
                    bundleBuildLogFull.Append(System.Environment.NewLine + userSettings.cdnWinIFXLoc + " -CDN project path Windows");
                    bundleBuildLogFull.Append(System.Environment.NewLine + userSettings.cdnAndroidIFXLoc + " -CDN project path Android");
                    bundleBuildLogFull.Append(System.Environment.NewLine + userSettings.cdniOSIFXLoc + " -CDN project path iOS");
                    bundleBuildLogFull.Append(System.Environment.NewLine + userSettings.projectWinLoc + " -Project path Windows");
                    bundleBuildLogFull.Append(System.Environment.NewLine + userSettings.projectAndroidLoc + " -Project path Android");
                    bundleBuildLogFull.Append(System.Environment.NewLine + userSettings.projectiOSLoc + " -Project path iOS");
                    bundleBuildLogFull.Append(System.Environment.NewLine + userSettings.unityEXELoc + " -Unity EXE");                    
                    bundleBuildLogFull.Append(System.Environment.NewLine + userSettings.thumbnailSavePath + " -Thumbnail save Path");
                    


                    bundleBuildLogFull.Append(System.Environment.NewLine + " //////////////////////////////////////////BUNDLES BUILT///////////////////////////////////");
                    bool windowsBuildSuccess = true;
                    bool androidBuildSuccess = true;
                    bool iOSBuildSuccess = true;

                    if (buildSettings.windowsBuildYesNo)
                    {
                        windowsBuildSuccess = false;
                        foreach (var item in bundlesBuiltWin)
                        {
                            //Debug.Log(item);
                            string fileName = Path.GetFileName(item);
                            string BundleBuiltPath = userSettings.projectWinLoc+"/AssetBundles/Windows/"+fileName;
                            Debug.Log(BundleBuiltPath);
                            if (File.Exists(BundleBuiltPath))
                            {
                                bundleBuildLogFull.Append(System.Environment.NewLine + BundleBuiltPath + " : Bundle Built successfully");
                                windowsBuildSuccess = true;
                            }
                            else
                            {
                                bundleBuildLogFull.Append(System.Environment.NewLine + BundleBuiltPath + " :  Bundle Build Failed");
                                windowsBuildSuccess = false;
                            }
                        }
                        Debug.Log("Build  Windows "+windowsBuildSuccess);
                    }
                        
                    if (buildSettings.androidBuildYesNo)
                    {
                        androidBuildSuccess =false;
                        foreach (var item in bundlesBuiltAndroid)
                        {
                            string fileName = Path.GetFileName(item);
                            string BundleBuiltPath = userSettings.projectWinLoc+"/AssetBundles/Android/"+fileName;
                            Debug.Log(BundleBuiltPath);
                            if (File.Exists(BundleBuiltPath))
                            {
                                bundleBuildLogFull.Append(System.Environment.NewLine + BundleBuiltPath + " : Bundle Built successfully");
                                androidBuildSuccess = true;
                            }
                            else
                            {
                                bundleBuildLogFull.Append(System.Environment.NewLine + BundleBuiltPath + " :  Bundle Build Failed");
                                androidBuildSuccess = false;
                            }
                        }
                        Debug.Log("Build  Android "+androidBuildSuccess);
                    }
                     if (buildSettings.iOSBuildYesNo)
                    {
                        iOSBuildSuccess = false;
                        foreach (var item in bundlesBuiltiOS)
                        {
                            string fileName = Path.GetFileName(item);
                            string BundleBuiltPath = userSettings.projectWinLoc+"/AssetBundles/iOS/"+fileName;
                            Debug.Log(BundleBuiltPath);
                            if (File.Exists(BundleBuiltPath))
                            {
                                bundleBuildLogFull.Append(System.Environment.NewLine + BundleBuiltPath + " : Bundle Built successfully");
                                iOSBuildSuccess = true;
                            }
                            else
                            {
                                bundleBuildLogFull.Append(System.Environment.NewLine + BundleBuiltPath + " :  Bundle Build Failed");
                                iOSBuildSuccess = false;
                            }
                        }
                        Debug.Log("Build success iOS "+iOSBuildSuccess);
                    }
                    if (userSettings.CTMode())
                    {
                        bundleBuildLogFull.Append(System.Environment.NewLine + " //////////////////////////////////////////BUNDLES TO CDN///////////////////////////////////");
                        bool windowsBuildMoved = false;
                        foreach (var item in bundlesBuiltWin)
                        {
                            Debug.Log(item);
                            if (File.Exists(item))
                            {
                                bundleBuildLogFull.Append(System.Environment.NewLine + item + " :  Moved to CDN successfully");
                                windowsBuildMoved = true;
                            }
                            else
                            {
                                bundleBuildLogFull.Append(System.Environment.NewLine + item + " :  Moved to CDN Failed");
                                windowsBuildMoved = false;
                            }
                        }
                        Debug.Log("Build Moved Win "+windowsBuildMoved);

                        
                        foreach (var item in bundlesBuiltAndroid)
                        {
                            Debug.Log(item);
                            if (File.Exists(item))
                            {
                                bundleBuildLogFull.Append(System.Environment.NewLine + item + " : Moved to CDN successfully");
                                
                            }
                            else
                            {
                                bundleBuildLogFull.Append(System.Environment.NewLine + item + " :  Moved to CDN Failed");
                                
                            }
                        }
                        Debug.Log("Build Moved Android "+androidBuildSuccess);

                        
                        foreach (var item in bundlesBuiltiOS)
                        {
                            Debug.Log(item);
                            if (File.Exists(item))
                            {
                                bundleBuildLogFull.Append(System.Environment.NewLine + item + " : Moved to CDN successfully");
                                
                            }
                            else
                            {
                                bundleBuildLogFull.Append(System.Environment.NewLine + item + " : Moved to CDN Failed");
                                
                            }
                        }
                        Debug.Log("Build Moved iOS "+iOSBuildSuccess);
                    }
                    



                    bundleBuildLogFull.Append(System.Environment.NewLine + " //////////////////////////////////////////WINDOWS BUILD LOGS///////////////////////////////////");
                    bundleBuildLogFull.Append( System.Environment.NewLine + bundleBuildLogWindows);
                    foreach (var item in buildLogsIN)
                    {
                        bundleBuildLogFull.Append(System.Environment.NewLine + " //////////////////////////////////////////BUILD LOG///////////////////////////////////");
                        bundleBuildLogFull.Append(System.Environment.NewLine + item);
                        
                        //Debug.Log(item);
                    }
                    
                    
                    

                    if (userSettings.CTMode())
                    {
                        bundleBuildLogFull.Append(System.Environment.NewLine + " //////////////////////////////////////////GIT LOGS///////////////////////////////////");
                        bundleBuildLogFull.Append(System.Environment.NewLine + bundleBuildLogGIT);
                    }
                    
                    
                    System.IO.File.WriteAllText(userSettings.projectWinLoc + "/Bundle Build Debug Log.txt", bundleBuildLogFull.ToString());
                    
                    if (windowsBuildSuccess && androidBuildSuccess && iOSBuildSuccess)
                    {
                        EditorUtility.DisplayDialog("Complete", " Bundles built", "Ok");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Failed", " theres was an issue building the bundle - check logs", "Ok");
                    }
                }
            }
        }
        
        Task<string> RunBatchFileAsync(string path)
        {
            
            var tcs = new TaskCompletionSource<string>();
            string output="Failed to retrive output from batch!";
            FileInfo info = new FileInfo(path);
            ProcessStartInfo startInfo = new ProcessStartInfo(info.FullName);
            if (userSettings.debugMode ==false)
            {
                startInfo.WindowStyle = systemDebug.ProcessWindowStyle.Hidden;
                startInfo.CreateNoWindow =true;
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardOutput = true;
            }
            
            
            
            var process = new Process();           
            process.EnableRaisingEvents=true;           
           
            process.Exited += (sender, args) =>
            {
                tcs.SetResult(output+"ExitCode: "+process.ExitCode);                
                process.Dispose();
            };
            process.StartInfo=startInfo;
            process.Start();
            output = System.Environment.NewLine + process.StandardOutput.ReadToEnd();
            
            return tcs.Task;            
        }
        
        
        public List<string> CreateAndroidBatchFile(BundleBuildSettings buildSettings, string cdnLocalLoc)
        {
            List<string> commands = new List<string>();
            foreach (var command in IFXToolSM.RoboCopyDependenciesFiles("Android",userSettings.projectWinLoc, buildSettings.selectedBundles))
            {
                commands.Add(command);
            }           
            commands.Add("\""+userSettings.unityEXELoc+"\" -quit -batchmode -buildTarget \"Android\" -projectPath \""+userSettings.projectAndroidLoc+"\" -executeMethod AssetBundles.BuildScript.BuildAssetBundles");
            commands.Add("robocopy "+"\""+userSettings.projectWinLoc +"/IFXBuildToolProjects/Android/AssetBundles/Android"+"\""+" "+"\""+userSettings.projectWinLoc+"/AssetBundles/Android"+"\"");
            if (cdnLocalLoc !=null)
            {
                if (userSettings.cdnAndroidIFXLoc != "" && userSettings.CTMode())
                {
                    commands.Add("robocopy "+"\""+userSettings.projectAndroidLoc+"/AssetBundles/Android"+"\""+" "+"\""+cdnLocalLoc+"\"");
                }
            }
            if (userSettings.debugMode ==true)
            {
                commands.Add("PAUSE");
            }
            return commands;
            
        }
        public List<string> CreateiOSBatchFile(BundleBuildSettings buildSettings, string cdnLocalLoc)
        {
            List<string> commands = new List<string>();            
            foreach (var command in IFXToolSM.RoboCopyDependenciesFiles("iOS",userSettings.projectWinLoc, buildSettings.selectedBundles))
            {
                commands.Add(command);
            }                      
            commands.Add("\""+userSettings.unityEXELoc+"\" -quit -batchmode -buildTarget \"iOS\" -projectPath \""+userSettings.projectiOSLoc+"\" -executeMethod AssetBundles.BuildScript.BuildAssetBundles");
            commands.Add("robocopy "+"\""+userSettings.projectWinLoc +"/IFXBuildToolProjects/iOS/AssetBundles/iOS"+"\""+" "+"\""+userSettings.projectWinLoc+"/AssetBundles/iOS"+"\"");
            if (cdnLocalLoc !=null)
            {
                if (userSettings.cdnAndroidIFXLoc != "" && userSettings.CTMode())
                {
                    commands.Add("robocopy "+"\""+userSettings.projectiOSLoc+"/AssetBundles/iOS"+"\""+" "+"\""+cdnLocalLoc+"\"");
                }
            }
            if (userSettings.debugMode ==true)
            {
                commands.Add("PAUSE");
            }
            return commands;
            
        }
        async public Task GitCommitChangesToRepo(List<Object> selectedBundleIN,string gitCommitM)
        {

            AssetDatabase.SaveAssets();//saves assets so all meta files show up
            List<string> commands = new List<string>();
            List<string> filesToAdd = new List<string>();
            filesToAdd.AddRange(IFXToolSM.GetFolderDependencies(selectedBundleIN));

            commands.Add("cd /D "+"\""+userSettings.projectWinLoc+"\"");
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
            string gitCommitBatch = IFXToolSM.CreateBatchCMDSFile("GitCommitToRepo",commands);
            var gitAddResults=RunBatchFileAsync(gitCommitBatch);
            var results = await Task.WhenAll(gitAddResults);
            System.IO.File.WriteAllText(userSettings.projectWinLoc+"/Git changes push Debug Log.txt", results[0]);

        }
        
    }
 }