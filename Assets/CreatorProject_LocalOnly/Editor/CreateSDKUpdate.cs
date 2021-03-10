using UnityEditor;
using UnityEngine;
public class CreateSDKUpdate : EditorWindow
{
    string  projectFolder = Application.dataPath.Replace("/Assets", "");
 
    [MenuItem("ENGAGE/Create SDK Update")]
    static void CreateUpdateZip()
    {
        lzip.compressDir(Application.dataPath+"\\ENGAGE_CreatorSDK",1,Application.dataPath.Replace("/Assets", "")+"/CreatorSDKUpdate.zip");
        Debug.Log("Update Created");
    }
}




