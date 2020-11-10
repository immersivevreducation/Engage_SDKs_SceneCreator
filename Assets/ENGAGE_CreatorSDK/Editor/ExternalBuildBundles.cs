using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetBundles;
public class ExternalBuildBundles
{
    // Start is called before the first frame update
    static public void BuildAssetBundles ()
		{

			AssetBundles.BuildScript.BuildAssetBundles();
			//IFXTools.IFXBundleTools.ClearAllAssetLabelsInProject();
		}
}
