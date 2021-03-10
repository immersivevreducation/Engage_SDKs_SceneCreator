using UnityEditor;
using UnityEngine;
using System;
using System.IO;
public class CreateSDKUpdate : EditorWindow
{
    string  projectFolder = Application.dataPath.Replace("/Assets", "");
 
    [MenuItem("ENGAGE/Create SDK Update")]
    static void CreateUpdateZip()
    {
        string currentSDKVersion = null;
        int versionNumber = 0;
        string path1 = Application.dataPath+@"/ENGAGE_CreatorSDK/SDKUpdateVersion.txt";
        if (File.Exists(path1))
        {
            currentSDKVersion = System.IO.File.ReadAllText(path1);
        }
        if (string.IsNullOrEmpty(currentSDKVersion))
        {
            using (StreamWriter writer = new StreamWriter(path1))  
            {  
                writer.Write("0");  
            }
        }
        else
        {
            versionNumber = Int32.Parse(currentSDKVersion);
            versionNumber +=1;
            using (StreamWriter writer = new StreamWriter(path1))  
            {  
                writer.Write(versionNumber.ToString());  
            }
        }
            
        
        lzip.compressDir(Application.dataPath+"\\ENGAGE_CreatorSDK",1,Application.dataPath.Replace("/Assets", "")+"/CreatorSDKUpdate.zip");
        Debug.Log("Update Created");
    }
}




