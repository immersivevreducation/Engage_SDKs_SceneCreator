using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace AssetBundles
{
    public class BuildScript
    {
        public static string overloadedDevelopmentServerURL = "";

        public static void BuildAssetBundles()
        {
            // Choose the output path according to the build target.
            string outputPath = Path.Combine(Utility.AssetBundlesOutputPath, Utility.GetPlatformName());
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            //@TODO: use append hash... (Make sure pipeline works correctly with it.)
            AssetBundleManifest mani = BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);

            foreach (string bund in mani.GetAllAssetBundles())
            {
                Debug.Log("Saving " + bund);
                try
                {
                    //StreamReader theReader = new StreamReader(outputPath + "/" + bund + ".manifest", Encoding.Default);
                    //string line = "";
                    //string crc = "";
                    //using (theReader)
                    //{
                    //    do
                    //    {
                    //        line = theReader.ReadLine();
                    //        if (line != null)
                    //        {
                    //            if (line.Contains("CRC"))
                    //            {
                    //                line = line.Replace("CRC:", "");
                    //                line = line.Replace(" ", "");
                    //                crc = line;
                    //            }
                    //        }
                    //    }
                    //    while (line != null);
                    //    theReader.Close();
                    //}
                    //if (!string.IsNullOrEmpty(crc))
                    //{
                    //File.WriteAllText(outputPath + "/" + bund + "_info", crc, Encoding.Default);

                    //lzip.compress_File(1, outputPath + "/" + bund + ".engagebundle", outputPath + "/" + bund, false, "_data");
                    //lzip.compress_File(1, outputPath + "/" + bund + ".engagebundle", outputPath + "/" + bund + "_info", true, "_info");
                    if (File.Exists(Path.Combine(outputPath, bund + ".engagebundle")))
                        File.Delete(Path.Combine(outputPath, bund + ".engagebundle"));

                    File.Move(Path.Combine(outputPath, bund), Path.Combine(outputPath, bund + ".engagebundle"));
                    //File.Delete(outputPath + "/" + bund + "_info");
                    Debug.Log("Saved " + bund + " successfully");
                    //}
                    //else
                    //{
                    //    Debug.Log("Problem Evaluating CRC " + bund);
                    //}
                }
                catch (System.Exception e)
                {
                    Debug.Log("Problem compressing bundle " + e);
                }
            }
        }

        public static void WriteServerURL()
        {
            string downloadURL;
            if (string.IsNullOrEmpty(overloadedDevelopmentServerURL) == false)
            {
                downloadURL = overloadedDevelopmentServerURL;
            }
            else
            {
                IPHostEntry host;
                string localIP = "";
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
                downloadURL = "http://" + localIP + ":7888/";
            }

            string assetBundleManagerResourcesDirectory = "Assets/AssetBundleManager/Resources";
            string assetBundleUrlPath = Path.Combine(assetBundleManagerResourcesDirectory, "AssetBundleServerURL.bytes");
            Directory.CreateDirectory(assetBundleManagerResourcesDirectory);
            File.WriteAllText(assetBundleUrlPath, downloadURL);
            AssetDatabase.Refresh();
        }

        public static void BuildPlayer()
        {
            var outputPath = EditorUtility.SaveFolderPanel("Choose Location of the Built Game", "", "");
            if (outputPath.Length == 0)
                return;

            string[] levels = GetLevelsFromBuildSettings();
            if (levels.Length == 0)
            {
                Debug.Log("Nothing to build.");
                return;
            }

            string targetName = GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget);
            if (targetName == null)
                return;

            // Build and copy AssetBundles.
            BuildScript.BuildAssetBundles();
            WriteServerURL();

            BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
            BuildPipeline.BuildPlayer(levels, outputPath + targetName, EditorUserBuildSettings.activeBuildTarget, option);
        }

        public static void BuildStandalonePlayer()
        {
            var outputPath = EditorUtility.SaveFolderPanel("Choose Location of the Built Game", "", "");
            if (outputPath.Length == 0)
                return;

            string[] levels = GetLevelsFromBuildSettings();
            if (levels.Length == 0)
            {
                Debug.Log("Nothing to build.");
                return;
            }

            string targetName = GetBuildTargetName(EditorUserBuildSettings.activeBuildTarget);
            if (targetName == null)
                return;

            // Build and copy AssetBundles.
            BuildScript.BuildAssetBundles();
            BuildScript.CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, Utility.AssetBundlesOutputPath));
            AssetDatabase.Refresh();

            BuildOptions option = EditorUserBuildSettings.development ? BuildOptions.Development : BuildOptions.None;
            BuildPipeline.BuildPlayer(levels, outputPath + targetName, EditorUserBuildSettings.activeBuildTarget, option);
        }

        public static string GetBuildTargetName(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                    return "/test.apk";
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return "/test.exe";
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSX:
                    return "/test.app";
                //case BuildTarget.WebPlayer:
                //case BuildTarget.WebPlayerStreamed:
                case BuildTarget.WebGL:
                    return "";
                // Add more build targets for your own.
                default:
                    Debug.Log("Target not implemented.");
                    return null;
            }
        }

        static void CopyAssetBundlesTo(string outputPath)
        {
            // Clear streaming assets folder.
            FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath);
            Directory.CreateDirectory(outputPath);

            string outputFolder = Utility.GetPlatformName();

            // Setup the source folder for assetbundles.
            var source = Path.Combine(Path.Combine(System.Environment.CurrentDirectory, Utility.AssetBundlesOutputPath), outputFolder);
            if (!System.IO.Directory.Exists(source))
                Debug.Log("No assetBundle output folder, try to build the assetBundles first.");

            // Setup the destination folder for assetbundles.
            var destination = System.IO.Path.Combine(outputPath, outputFolder);
            if (System.IO.Directory.Exists(destination))
                FileUtil.DeleteFileOrDirectory(destination);

            FileUtil.CopyFileOrDirectory(source, destination);
        }

        static string[] GetLevelsFromBuildSettings()
        {
            List<string> levels = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
            {
                if (EditorBuildSettings.scenes[i].enabled)
                    levels.Add(EditorBuildSettings.scenes[i].path);
            }

            return levels.ToArray();
        }
    }
}