﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace InternalTools
{
    public static class PackageExporter
    {
        private const string releasesFolder = "Releases";

        [MenuItem("i5 Toolkit/Export All Packages")]
        public static void ExportPackages()
        {
            ExportCoreReleasePackage();
            ExportSampleReleasePackage();
        }

        [MenuItem("i5 Toolkit/Export Core Package")]
        public static void ExportCoreReleasePackage()
        {
            Debug.Log("Exporting core package...");
            List<string> exportFiles = new List<string>();
            string[] searchFolders =
            {
            "Assets/i5 Toolkit for Unity/Editor",
            "Assets/i5 Toolkit for Unity/Runtime",
            "Assets/i5 Toolkit for Unity/Tests/TestHelpers"
        };
            foreach (string guid in AssetDatabase.FindAssets("", searchFolders))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                // exclude obj folder with is created by the DocFX generator
                if (!path.EndsWith("/obj") && !path.Contains("/obj/"))
                {
                    exportFiles.Add(path);
                }
            }

            exportFiles.Add("Assets/i5 Toolkit for Unity/CHANGELOG.md");
            exportFiles.Add("Assets/i5 Toolkit for Unity/LICENSE.md");
            exportFiles.Add("Assets/i5 Toolkit for Unity/Logo.png");
            exportFiles.Add("Assets/i5 Toolkit for Unity/README.md");
            exportFiles.Add("Assets/i5 Toolkit for Unity/package.json");

            AssetDatabase.ExportPackage(exportFiles.ToArray(),
                $"{releasesFolder}/i5-Toolkit-for-Unity-v{Application.version}.unitypackage",
                ExportPackageOptions.Default);
            Debug.Log("Core package export finished");
        }

        [MenuItem("i5 Toolkit/Export Samples Package")]
        public static void ExportSampleReleasePackage()
        {
            Debug.Log("Exporting samples package...");
            List<string> exportFiles = new List<string>();
            string[] searchFolders =
            {
            "Assets/i5 Toolkit for Unity/Samples"
        };
            foreach (string guid in AssetDatabase.FindAssets("", searchFolders))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (!path.EndsWith("OpenID Connect Client Data.asset"))
                {
                    exportFiles.Add(path);
                }
            }
            AssetDatabase.ExportPackage(exportFiles.ToArray(),
               $"{releasesFolder}/i5-Toolkit-for-Unity-v{Application.version}-Examples.unitypackage",
               ExportPackageOptions.Default);
            Debug.Log("Samples package export finished");
        }
    }
}