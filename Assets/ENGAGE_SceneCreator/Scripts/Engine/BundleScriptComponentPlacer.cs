using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BundleScriptComponentPlacer : MonoBehaviour {

	public string bundleName="bundlename";
	[System.Serializable]
	public class ScriptPlacer
	{
		public string scriptName;
		public List<GameObject> attachToObject = new List<GameObject>();
		public ScriptPlacer(
			string scriptName,
			List<GameObject> attachToObject
		){
			this.scriptName = scriptName;
			this.attachToObject= attachToObject;
		}
	}
	public List<ScriptPlacer> placeScripts = new List<ScriptPlacer>();

}
