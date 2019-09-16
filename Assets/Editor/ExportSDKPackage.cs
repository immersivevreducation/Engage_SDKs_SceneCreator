using UnityEngine;
using System.Collections;
using UnityEditor;
 
public static class ExportWithLayers {
	 
	    [MenuItem("ENGAGE/Export SDK with tags and physics layers")]
	    public static void ExportPackage()
	    {
		        string[] projectContent = new string[] {"Assets/Engage_SceneCreator","Assets/Standard Assets","ProjectSettings/TagManager.asset"};
				AssetDatabase.ExportPackage(projectContent, "Engage_SceneCreatorSDK.unitypackage", ExportPackageOptions.Recurse); 
		        Debug.Log("Project Exported");
		    }
	 
}