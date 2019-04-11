using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpecialDebugMessages
{
    public static void LogComponentMissingReferenceError(string componentName, string referenceName, Object context)
    {
        Debug.LogError(componentName + " is missing a reference for " + referenceName + "\nSet it up in the inspector", context);
    }
}
