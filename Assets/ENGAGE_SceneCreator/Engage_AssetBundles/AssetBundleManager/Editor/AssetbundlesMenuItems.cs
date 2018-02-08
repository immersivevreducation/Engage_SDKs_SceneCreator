using UnityEngine;
using UnityEditor;
using System.Collections;

namespace AssetBundles
{
	public class AssetBundlesMenuItems
	{
		[MenuItem ("Assets/AssetBundles/Build Engage AssetBundles")]
		static public void BuildAssetBundles ()
		{
			BuildScript.BuildAssetBundles();
		}
	}
}