#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class PrefabCollectionBuilder : IPreprocessBuildWithReport
{
    public int callbackOrder
    {
        get
        {
            return 0;
        }
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        //PrefabResourceCollection.FindNetworkPrefabsInResources();
    }
}
#endif
