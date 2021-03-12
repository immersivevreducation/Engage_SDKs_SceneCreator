using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Process = System.Diagnostics.Process;
using ProcessStartInfo = System.Diagnostics.ProcessStartInfo;


public class CreatorSDKUpdateHandler : EditorWindow
{
    string  projectFolder; 
    public string updateSDkPath; // if changing value change in the duplicates window too!
    private void OnEnable() 
    {
        projectFolder = Application.dataPath.Replace("/Assets", "");
        updateSDkPath = Application.dataPath.Replace("/Assets", "")+@"\SdkUpdate"; // if changing value change in the duplicates window too!
    }
    
    public static void SDKUpdate(string projectFolder, string updateSDkPath )
    {
        if (!Directory.Exists(updateSDkPath))
        {
           Directory.CreateDirectory(updateSDkPath);
        }
        
        string currentSDkPath = Application.dataPath+"\\ENGAGE_CreatorSDK";
        
        int [] progress = new int[100];
        string updateZipPath =projectFolder+"/CreatorSDKUpdate.zip";
        if (File.Exists(updateZipPath))
        {
            lzip.decompress_File(updateZipPath, updateSDkPath, progress);
            Debug.Log("Decompressed");
        }
        else
        {
            EditorUtility.DisplayDialog("Update Failed", "Could not find Update Zip File", "OK", "");
            Debug.Log("Could not find Update Zip File, cannot continue.");
            return;
        }
        
        
        List<FileInfo> duplicatefileNamesFound = DuplicateFileFinder(updateSDkPath,Application.dataPath);
        if (duplicatefileNamesFound.Count >0)
        {
            SDKUpdateDuplicateFilesWindow SDKUpdateDuplicateFilesWindow = GetWindow<SDKUpdateDuplicateFilesWindow>();
            SDKUpdateDuplicateFilesWindow.titleContent = new GUIContent("Conflicting SDK Files Helper");
            SDKUpdateDuplicateFilesWindow.minSize = new Vector2(400,400);
            SDKUpdateDuplicateFilesWindow.duplicatefileNames = duplicatefileNamesFound;
            SDKUpdateDuplicateFilesWindow.Show();
        }
        else
        {
            PerformUpdateOverwriteToLocalSDK(updateSDkPath);
            
        }
        if (Directory.Exists(updateSDkPath))
        {
            DirectoryInfo UpdateDir = new DirectoryInfo(updateSDkPath);
            UpdateDir.Delete();
        }
         EditorUtility.DisplayDialog("Update Complete", "Update Complete", "OK", "");
        
    }
    public static void PerformUpdateOverwriteToLocalSDK(string updatePath)
    {
        RunBatchFile(CreateBatchCMDSFile(updatePath+"\\SDKUpdate.bat", CreateSDKCopyBatchFile(updatePath)));
    }   
    static List<string> CreateSDKCopyBatchFile(string pathToNewSDKFolder)
    {
        List<string> commands = new List<string>();
        

        commands.Add("robocopy "+"\""+pathToNewSDKFolder+"\" "+Application.dataPath+"/ENGAGE_CreatorSDK /MIR");
        commands.Add("DEL /Q /S "+"\""+pathToNewSDKFolder+"\" ");
        Debug.Log("BatchFile created at: "+pathToNewSDKFolder);
        return commands;
    }
    public static string CreateBatchCMDSFile(string pathforBatch,List<string> input,List<string> input2=null,List<string> input3=null)
        {

            List<string> commandsList = new List<string>();
            if (input != null)
            {
                commandsList.AddRange(input);
            }
            if (input2 != null)
            {
                commandsList.AddRange(input2);
            }
            if (input3 != null)
            {
                commandsList.AddRange(input3);
            }            
            StreamWriter writer = new StreamWriter(pathforBatch, false);
            foreach (string cmd in commandsList)
            {
                writer.WriteLine(cmd);
            }
            writer.WriteLine("TIMEOUT 1");
            writer.Close();
            return pathforBatch;
        }
    public static List<System.IO.FileInfo> DuplicateFileFinder(string sourceFolder,string targetFolder)
    {
        List<FileInfo> duplicatefileNames = new List<FileInfo>();
        DirectoryInfo SDKUpdate = new DirectoryInfo(sourceFolder);  
        DirectoryInfo SDKOld = new DirectoryInfo(targetFolder);  

        // Take a snapshot of the file system.  
        FileInfo[] SDKUpdateList = SDKUpdate.GetFiles("*.*", System.IO.SearchOption.AllDirectories);
        FileInfo[] SDKOldList = SDKOld.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

        //Filter out items that paths have engage sdk in them, since we will be overwriting the sdk anyway.
        List<FileInfo> filteredSDKOldList = new List<FileInfo>();
        foreach (var item in SDKOldList)
        {
            if (!item.FullName.Contains("\\ENGAGE_CreatorSDK\\") && !item.FullName.Contains(".meta")) //also filter out .meta becasue they can be assumed to exist and therefore no need to clutter ui
            {
                filteredSDKOldList.Add(item);
            }
            //Debug.Log("SDK Old LISt: "+item.Name); 
        }   
        foreach (var itemUpdateLsit in SDKUpdateList)
        {
            foreach (var itemOldList in filteredSDKOldList)
            {
               if (IdenticalFileName(itemUpdateLsit,itemOldList))
                {
                    duplicatefileNames.Add(itemOldList);
                } 
            }
        }    
       return duplicatefileNames;
    }
    public static bool IdenticalFileName(FileInfo f1, FileInfo f2)  
    {  
        return (f1.Name == f2.Name);  
    } 
   static void RunBatchFile(string path)
    {
        FileInfo info = new FileInfo(path);
        ProcessStartInfo startInfo = new ProcessStartInfo(info.FullName);       
        startInfo.CreateNoWindow = true;
        
        var process = new Process();           
        process.EnableRaisingEvents=true;           
        
        
        process.StartInfo=startInfo;
        process.Start();
            
    }

}

public class SDKUpdateDuplicateFilesWindow : EditorWindow
{
    public List<FileInfo> duplicatefileNames = new List<FileInfo>();
    Vector2 scrollPos;
    private void OnGUI() 
        {
            DuplicateFilesUI();
            this.Repaint();
        }
        void DuplicateFilesUI()
        {
            Rect groupRect=new Rect(5, 25, Screen.width-20, Screen.height-10);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(groupRect.width), GUILayout.Height(groupRect.height-175));
            EditorGUILayout.LabelField("The following Assets have duplicate names with assets found within the SDK Update");
            EditorGUILayout.LabelField("Duplicate scripts can cause errors and duplicate assets can cause confusion");    
            EditorGUILayout.LabelField("This tool facilitates easy deletion of duplicate assets");    
            EditorGUILayout.LabelField("but deletion can not be undone so please carefully check before deleting");                                                        
            foreach (FileInfo item in duplicatefileNames)
            {                
                GUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Name: "+item.Name, EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Path: "+item.FullName);
                if (GUILayout.Button("Open in explorer"))
                {
                     Process.Start(System.IO.Path.GetDirectoryName(item.FullName));                                  
                }
                
                if (GUILayout.Button("Delete File"))
                {
                    item.Delete();
                    duplicatefileNames.Remove(item);
                    //asume meta exists and delete too
                    FileInfo itemMeta = new FileInfo(item.FullName+".meta");
                    itemMeta.Delete();                                 
                }

                GUILayout.EndVertical();
                GUILayout.Label(" ");
            }

            GUILayout.Label(" ");
            GUILayout.Label(" ");                    
            EditorGUILayout.EndScrollView(); 

            
            if (GUILayout.Button("Continue Update"))
            {
                    CreatorSDKUpdateHandler.PerformUpdateOverwriteToLocalSDK(Application.dataPath.Replace("/Assets", "")+@"\SdkUpdate");
                    this.Close();                               
            }
            if (GUI.Button(new Rect(10, groupRect.height - 75, groupRect.width, 50), "Cancel"))
            {
                this.Close();
            }        
    
        }
}




