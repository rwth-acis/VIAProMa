using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpecialDebugMessages
{
    public static void LogComponentMissingReferenceError(MonoBehaviour thisComponent, string referenceName)
    {
        Debug.LogError("Error: Reference In Inspector not Set Up\n" + thisComponent.GetType().Name + " is missing a reference for " + referenceName + ". Set it up in the inspector", thisComponent);
    }

    public static void LogComponentNotFoundError(MonoBehaviour thisComponent, string searchedComponent, GameObject target)
    {
        Debug.LogError("Error: Component of type " + searchedComponent + " not found\n" + thisComponent.GetType().Name + " looked for the component " + searchedComponent + " on the GameObject " + target.name + " but could not find it. Maybe you need to add it to " + target.name + "?", thisComponent);
    }
}
