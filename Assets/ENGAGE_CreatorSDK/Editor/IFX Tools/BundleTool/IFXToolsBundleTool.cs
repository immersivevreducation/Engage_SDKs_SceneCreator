using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using systemDebug = System.Diagnostics;
using Process = System.Diagnostics.Process;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;


 /////////////TO-DO////
 // Create log file for all build processes

 namespace IFXTools{

     public interface IBundleToolsParentWindow
     {
        bool buildQACheckOverride {get; set;}
        string gitCommitM {get; set;}
        bool windowsBuildYesNo {get; set;}
        bool androidBuildYesNo {get; set;}
        bool autoGitYesNo {get; set;}

        bool bundlesBuilding{get; set;}//Only used to update quicktools ui label for building

        
     }
    public class IFXBundleTools : EditorWindow
    {
              
        IFXToolsQualityCheckTool qaTool;
        IFXToolsUserSettings userSettings;
        IBundleToolsParentWindow parentWindow;
        public List<Object> selectedBundles {get; set;}
        string bundleBuildLog {get; set;}
        string bundleBuildLogWindows {get; set;}
        //string bundleBuildLogAndroid {get; set;}
        //string bundleBuildLogiOS {get; set;}
        string bundleBuildLogGIT {get; set;}
        
        //System.Text.StringBuilder bundleBuildLog;
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
                    parentWindow.bundlesBuilding =true;

                    //This part isn't auto "add"ing the files to a git comit. this part just makes the varable to hold all the paths
                    buildBundlesAsync(windowsYes, androidYes, iOSYes, autoGitYes);
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

            async Task buildBundlesAsync(bool winYes, bool andYes, bool iYes, bool GitYes)
            {
                
                SetUpGitPathsForCreatedFiles(winYes, andYes, iYes);
                
                List<Task<string>> BundleBuildTasks = new List<Task<string>>();
                // if true build android 
                if (andYes)
                {
                    DeleteFolderContents(userSettings.projectAndroidLoc + "/AssetBundles/Android"); //clears out old bundles
                    string androidBuildPath = CreateBatchCMDSFile("Android", SyncUnityProjects("Android"), CreateAndroidBatchFile(autoGitYesNo));
                    //RunBuildFileAsync(androidBuildPath, "Android");
                    BundleBuildTasks.Add(Task.Run(() => RunBatchFileAsync(androidBuildPath)));
                    //Debug.Log(androidBuild.Output);


                }
                // if true build iOS 
                if (iYes)
                {
                    DeleteFolderContents(userSettings.projectiOSLoc + "/AssetBundles/iOS"); //clears out old bundles
                    string iOSBuildPath = CreateBatchCMDSFile("iOS", SyncUnityProjects("iOS"), CreateiOSBatchFile(autoGitYesNo));
                    //RunBuildFileAsync(iOSBuildPath, "iOS");
                    BundleBuildTasks.Add(Task.Run(() => RunBatchFileAsync(iOSBuildPath)));
                    // Git stuff handled in batch file!

                }
                // if true build windows 
                if (winYes)
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
                    // if (autoGitYesNo)
                    // {
                    //     GitPull();
                    //     GitPushWin();
                    // }
                }

                /////////////////////////////CDN Push////////////////////////
                
                var results = await Task.WhenAll(BundleBuildTasks);
                
                if (GitYes)
                {
                    GitPushBundlesToCDN();
                }
                if (BuildDebugChecks(results,winYes,andYes,iYes,autoGitYesNo))
                {
                    EditorUtility.DisplayDialog("Complete", " Bundles built", "Ok");
                }
                else
                {
                    EditorUtility.DisplayDialog("Failed", " theres was an issue building the bundle - check logs", "Ok");
                }
                parentWindow.bundlesBuilding =false;

                bool BuildDebugChecks(string[] buildLogsIN,bool buildWindowsYes,bool buildAndroidYes, bool BuildiOSYes,bool autoGit )
                {
                    StringBuilder bundleBuildLogFull = new StringBuilder();
                    bundleBuildLogFull.Append(System.Environment.NewLine + " //////////////////////////////////////////USER SETTINGS///////////////////////////////////");
                    bundleBuildLogFull.Append(System.Environment.NewLine + userSettings.cdnProjectPath + " CDN Project path");
                    bundleBuildLogFull.Append(System.Environment.NewLine + userSettings.cdnWinLoc + " CDN project path Windows");
                    bundleBuildLogFull.Append(System.Environment.NewLine + userSettings.cdnAndroidLoc + " CDN project path Android");
                    bundleBuildLogFull.Append(System.Environment.NewLine + userSettings.cdniOSLoc + " CDN project path iOS");
                    bundleBuildLogFull.Append(System.Environment.NewLine + userSettings.projectWinLoc + " Project path Windows");
                    bundleBuildLogFull.Append(System.Environment.NewLine + userSettings.projectAndroidLoc + " Project path Android");
                    bundleBuildLogFull.Append(System.Environment.NewLine + userSettings.projectiOSLoc + " Project path iOS");
                    bundleBuildLogFull.Append(System.Environment.NewLine + userSettings.unityEXELoc + " Unity EXE");                    
                    bundleBuildLogFull.Append(System.Environment.NewLine + userSettings.thumbnailSavePath + " Thumbnail save Path");
                    


                    bundleBuildLogFull.Append(System.Environment.NewLine + " //////////////////////////////////////////BUNDLES BUILT///////////////////////////////////");
                    bool windowsBuildSuccess = true;
                    bool androidBuildSuccess = true;
                    bool iOSBuildSuccess = true;
                    if (buildWindowsYes)
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
                        Debug.Log("Build success Windows "+windowsBuildSuccess);
                    }
                        
                    if (buildAndroidYes)
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
                        Debug.Log("Build success Android "+androidBuildSuccess);
                    }
                     if (BuildiOSYes)
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

                        bool androidBuildMoved = false;
                        foreach (var item in bundlesBuiltAndroid)
                        {
                            Debug.Log(item);
                            if (File.Exists(item))
                            {
                                bundleBuildLogFull.Append(System.Environment.NewLine + item + " : Moved to CDN successfully");
                                androidBuildMoved = true;
                            }
                            else
                            {
                                bundleBuildLogFull.Append(System.Environment.NewLine + item + " :  Moved to CDN Failed");
                                androidBuildMoved = false;
                            }
                        }
                        Debug.Log("Build Moved Android "+androidBuildSuccess);

                        bool iOSBuildMoved = false;
                        foreach (var item in bundlesBuiltiOS)
                        {
                            Debug.Log(item);
                            if (File.Exists(item))
                            {
                                bundleBuildLogFull.Append(System.Environment.NewLine + item + " : Moved to CDN successfully");
                                iOSBuildMoved = true;
                            }
                            else
                            {
                                bundleBuildLogFull.Append(System.Environment.NewLine + item + " : Moved to CDN Failed");
                                iOSBuildMoved = false;
                            }
                        }
                        Debug.Log("Build Moved iOS "+iOSBuildSuccess);
                    }
                    



                    bundleBuildLogFull.Append(System.Environment.NewLine + " //////////////////////////////////////////WINDOWS BUILD LOGS///////////////////////////////////");
                    bundleBuildLogFull.Append( System.Environment.NewLine + bundleBuildLogWindows);
                    foreach (var item in buildLogsIN)
                    {
                        bundleBuildLogFull.Append(System.Environment.NewLine + " //////////////////////////////////////////ANDROID BUILD LOGS///////////////////////////////////");
                        bundleBuildLogFull.Append(System.Environment.NewLine + item);
                        bundleBuildLogFull.Append(System.Environment.NewLine + " //////////////////////////////////////////iOS BUILD LOGS///////////////////////////////////");
                        bundleBuildLogFull.Append(System.Environment.NewLine + item);
                        Debug.Log(item);
                    }
                    
                    
                    

                    if (userSettings.CTMode())
                    {
                        bundleBuildLogFull.Append(System.Environment.NewLine + " //////////////////////////////////////////GIT LOGS///////////////////////////////////");
                        bundleBuildLogFull.Append(System.Environment.NewLine + bundleBuildLogGIT);
                    }
                    
                    
                    System.IO.File.WriteAllText(userSettings.projectWinLoc + "/Bundle Build Debug Log.txt", bundleBuildLogFull.ToString());
                    
                    if (windowsBuildSuccess && androidBuildSuccess && iOSBuildSuccess)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        // async Task RunBuildFile(string path,string BuildType)
        // {
        //     int exitcode = await RunBatchFile(path);
        //     EditorUtility.DisplayDialog("Complete", BuildType+" Bundle built", "Ok");
            
        //     Debug.Log(exitcode);
        // }
        Task<string> RunBatchFileAsync(string path)
        {
            
            var tcs = new TaskCompletionSource<string>();
            string output="Failed to retrive output from batch!";
            FileInfo info = new FileInfo(path);
            ProcessStartInfo startInfo = new ProcessStartInfo(info.FullName);
            // if (!userSettings.debugMode)
            // {
            //     startInfo.WindowStyle = systemDebug.ProcessWindowStyle.Hidden;
            // }
            startInfo.WindowStyle = systemDebug.ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow =true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            // startInfo.RedirectStandardInput = true;
            
            
            var process = new Process();
            
            // {
            //     StartInfo = { FileName = info.FullName,WindowStyle = systemDebug.ProcessWindowStyle.Hidden, CreateNoWindow = true,},
            //     EnableRaisingEvents = true
                
            // };
            process.EnableRaisingEvents=true;
            // process.OutputDataReceived += new systemDebug.DataReceivedEventHandler((sender, e) =>
            // {
            //     //Debug.Log("Got to here____2");
            //     // Prepend line numbers to each line of the output.
            //     if (!System.String.IsNullOrEmpty(e.Data))
            //     {
            //         Debug.Log(e.Data);// to see what happens
            //         // parse e.Data here
            //     }
            // });
            // process.OutputDataReceived += SortOutputHandler;
           
            process.Exited += (sender, args) =>
            {
                tcs.SetResult(output+"ExitCode: "+process.ExitCode);
                //bundleBuildLog = string.Join("--------------------------------------"+System.Environment.NewLine, process.StandardOutput.ReadToEnd());
                process.Dispose();
            };
            process.StartInfo=startInfo;
            Debug.Log("Got to here____");
            process.Start();
            output = System.Environment.NewLine + process.StandardOutput.ReadToEnd();
            // while (!process.StandardOutput.EndOfStream) {
            //     //bundleBuildLog.Append(System.Environment.NewLine+process.StandardOutput);
            //     output = System.Environment.NewLine + process.StandardOutput.ReadToEnd();
            // } //kinda worked!

            //_________________________________
            // process.ErrorDataReceived += (s, e) => 
            // {
            //     bundleBuildLog.Append(System.Environment.NewLine+e.Data);
            //     //process.CancelOutputRead();
            // };
            // process.OutputDataReceived += (s, e) => 
            // {
            //     bundleBuildLog.Append(System.Environment.NewLine+e.Data);
            //     //process.CancelOutputRead();
            // };
            // process.Exited += (sender, args) =>
            // {
            //     tcs.SetResult(process.ExitCode);
            //     //bundleBuildLog = string.Join("--------------------------------------"+System.Environment.NewLine, process.StandardOutput.ReadToEnd());
            //     process.Dispose();
            // };
            // process.StartInfo=startInfo;
            // Debug.Log("Got to here____");
            // process.Start();
            // process.BeginOutputReadLine();
            // ________________________________________________
            // while (!process.StandardOutput.EndOfStream) {
                
            //  //bundleBuildLog = string.Join("--------------------------------------"+System.Environment.NewLine, process.StandardOutput.ReadToEnd());
            // } //kinda worked!


            //process.BeginOutputReadLine();
            // process.BeginErrorReadLine();

             //process.WaitForExit();
            

            //Debug.Log("Got to here____");
            //process.WaitForExit();

            
            //bundleBuildLog = string.Join("--------------------------------------"+System.Environment.NewLine, output);
            // if (process.HasExited)            
            // {
            //     tcs.SetResult(process.ExitCode);
            //     process.Dispose();
            // };
            return tcs.Task;
            // FileInfo info = new FileInfo(path);                   
            // Task<ProcessAsyncHelper.ProcessResult> androidBuild = await ProcessAsyncHelper.RunProcessAsync(info.FullName,"",1);
            
            
            // Debug.Log(androidBuild.Result.Output);
            // Debug.Log(androidBuild.Result.Error);
            // //await Task.Run(System.Diagnostics.Process.Start(info.FullName));

            // Debug.Log(path+" has finished");
        }
        // private  void SortOutputHandler(object sendingProcess, systemDebug.DataReceivedEventArgs outLine)
        // {
        //     bundleBuildLog = string.Join(System.Environment.NewLine,outLine.Data);
        //     Debug.Log(bundleBuildLog);
        //     // Collect the sort command output.
        //     // if (outLine.Data != null || outLine.Data == "")
        //     // {              
        //     //     // Add the text to the collected output.
        //     //     bundleBuildLog = string.Join(System.Environment.NewLine,outLine.Data);
        //     //     Debug.Log($"\nError stream: {bundleBuildLog}");
        //     // }
        // }
        
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
            
            // if (autoGitYesNo && userSettings.CTMode())
            // {
            //     //Start of git bit
            //     //string listOfBundles = string.Join(" ", bundlesBuiltAndroid);
            //     //commands = new string[8]{ "cd "+userSettings.cdnWinLoc, "cd ..", "cd ..", "cd ..", "cd ..","cd ..", "git add "+listOfBundles,"git commit -m "+gitCommitM};
            //     commands.Add("cd /D "+"\""+userSettings.cdnProjectPath+"\""); 
            //     commands.Add("git stash");
            //     commands.Add("git pull");
            //     commands.Add("git stash pop");
            //     foreach (var item in bundlesBuiltAndroid)
            //     {
            //         commands.Add("git add "+"\""+item+"\"");
            //     }
            //     commands.Add("git commit -m "+"\""+gitCommitM+"_Android"+"\"");
            //     commands.Add("git push");
            // }
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
            
            // if (autoGitYesNo && userSettings.CTMode())
            // {
            //     //Start of git bit
            //     //string listOfBundles = string.Join(" ", bundlesBuiltiOS);
            //     //commands = new string[8]{ "cd "+userSettings.cdnWinLoc, "cd ..", "cd ..", "cd ..", "cd ..","cd ..", "git add "+listOfBundles,"git commit -m "+gitCommitM};                
            //     commands.Add("cd /D "+"\""+userSettings.cdnProjectPath+"\""); 
            //     commands.Add("git stash");
            //     commands.Add("git pull");
            //     commands.Add("git stash pop");
            //     foreach (var item in bundlesBuiltiOS)
            //     {
            //         commands.Add("git add "+"\""+item+"\"");
            //     }
                
            //     commands.Add("git commit -m "+"\""+gitCommitM+"_iOS"+"\"");
            //     commands.Add("git push");
            // }
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
            
            commands.Add("mkdir "+"\""+userSettings.projectWinLoc.Replace("/","\\")+"\\IFXBuildToolProjects\\"+buildType+"\\AssetBundles\\"+buildType+"\"");
            commands.Add("robocopy "+"\""+userSettings.projectWinLoc+"/Assets/ENGAGE_CreatorSDK"+"\""+ " " +"\""+userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType+"/Assets/ENGAGE_CreatorSDK"+"\""+" "+" /MIR /XD "+"\""+userSettings.projectWinLoc+"/IFXBuildToolProjects"+"\"");
            commands.Add("robocopy "+"\""+userSettings.projectWinLoc+"/ProjectSettings"+"\""+" "+"\""+userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType+"/ProjectSettings"+"\""+ " /MIR /XD "+"\""+userSettings.projectWinLoc+"/IFXBuildToolProjects"+"\""); 
            
                       
            // if (buildType == "Android")
            // {
            //     userSettings.projectAndroidLoc = userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType;
            //     userSettings.SaveUserSettings();
            // }
            // if (buildType == "iOS")
            // {
            //     userSettings.projectiOSLoc = userSettings.projectWinLoc+"/IFXBuildToolProjects/"+buildType;
            //     userSettings.SaveUserSettings();
            // }

            return commands;
            
        }
        async public Task GitCommitChangesToRepo(List<Object> selectedBundleIN,string gitCommitM)
        {
            List<string> commands = new List<string>();
            List<string> filesToAdd = new List<string>();
            filesToAdd.AddRange(GetFolderDependencies(selectedBundleIN));

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
            string gitCommitBatch = CreateBatchCMDSFile("GitCommitToRepo",commands);
            var gitAddResults=RunBatchFileAsync(gitCommitBatch);
            var results = await Task.WhenAll(gitAddResults);
            System.IO.File.WriteAllText(userSettings.projectWinLoc+"/Git changes push Debug Log.txt", results[0]);

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
            writer.WriteLine("TIMEOUT 1");
            if (userSettings.debugMode)
            {
                writer.WriteLine("PAUSE");
            }
            writer.Close();
            return TempCMDBatchPath;
        }
        void RunCMD(List<string> arguments)
        {
            
            
            Process cmd = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            if (!userSettings.debugMode)
            {
                info.WindowStyle = systemDebug.ProcessWindowStyle.Hidden;
                info.CreateNoWindow = true;
            }
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
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
            // Synchronously read the standard output of the process.
            string output = cmd.StandardOutput.ReadToEnd(); 
            bundleBuildLogGIT = bundleBuildLogGIT+"----------------------------------------"+System.Environment.NewLine+output;
            
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