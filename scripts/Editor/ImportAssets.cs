using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class VZImporter : EditorWindow
{
    static List<string> discover_assets(string folder, List<string> discovered_assets)
    {
        // base case: files
        foreach (string file in System.IO.Directory.GetFiles(folder))
            discovered_assets.Add(file);

        // recursive case: subfolders
        foreach (string subfolder in System.IO.Directory.GetDirectories(folder))
        {
            discover_assets(subfolder, discovered_assets);
        }
        return discovered_assets;
    }

    [MenuItem("VisionZero/Import Assets")]
    static void Import()
    {
        string path = EditorUtility.OpenFolderPanel("Select Containing Folder", "", "");
        Debug.Log("Selected Folder: " + path);
        //path = "\\\\NAS-SYNTHIA/synthia/VisionZero/bellevue/modelos/cars";

        // Import packages (needs to be refined, seems that only one package gets imported!)
        foreach (string folder in System.IO.Directory.GetDirectories(path))
        {
            //Debug.Log("Fichero: " + folder);
            foreach (string file in System.IO.Directory.GetFiles(folder))
            {
                file.Normalize();
                if (file.EndsWith(".unitypackage"))
                {
                    Debug.Log("Asset File: " + file);
                    AssetDatabase.ImportPackage(file, false);
                }
            }
        }
    }

    [MenuItem("VisionZero/Instantiate Assets")]
    static void Instantiate()
    {
        Debug.Log("Finding models ...");

        // Find models
        List<string> discovered_assets = new List<string>();
        discover_assets("Assets/", discovered_assets);
        foreach (string asset in discovered_assets)
            if (asset.EndsWith(".fbx"))
                Debug.Log("FOUND .FBX ASSET: " + asset);

        // Load and instantiate objects
        Vector3 origin = new Vector3(0, 0, 0);
        int instance = 0;
        foreach (string asset in discovered_assets)
            if (asset.EndsWith(".fbx"))
            {
                Debug.Log("LOADING .FBX ASSET: " + asset);
                Object prefab = AssetDatabase.LoadAssetAtPath(asset, typeof(GameObject));
                GameObject clone = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
                clone.transform.Rotate(new Vector3(0, 1, 0), -45);
                clone.transform.position = origin + new Vector3(0, 0, instance * 5);
                instance++;
            }
    }

    [MenuItem("VisionZero/Export Packages")]
    static void Export()
    {
        Debug.Log("Exporting packages ...");

        // Find models
        List<string> discovered_assets = new List<string>();
        discover_assets("Assets/", discovered_assets);
        foreach (string asset in discovered_assets)
            if (asset.EndsWith(".fbx"))
                Debug.Log("FOUND .FBX ASSET: " + asset);

        foreach (string asset in discovered_assets)
            if (asset.EndsWith(".fbx"))
            {
                AssetDatabase.ExportPackage(asset, 
                                            "VZ_"+asset+".unitypackage", 
                                            ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse | ExportPackageOptions.Interactive);
            }
    }
}