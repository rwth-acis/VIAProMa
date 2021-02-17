using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class ActionHelperFunctions : MonoBehaviour
{

    public static GameObject GetVisualisationFromGameObject(GameObject gameObject, Type[] typesToExclude = null, bool checkAbove = false, bool checkBelow = false)
    {
        //If wished, check if any of the children of the target is of a type that should be excluded
        if (typesToExclude != null && checkBelow)
        {
            foreach (Type type in typesToExclude)
            {
                if (gameObject.GetComponentInChildren(type, true) != null)
                {
                    return null;
                }
            }
        }

        if (gameObject != null)
        {
            while (gameObject != null && !IsVisualisation(gameObject))
            {
                //If wished, check if the current object (i.e. a object above in the hirachy of the original target) is of a type that should be excluded
                if (typesToExclude != null && checkAbove)
                {
                    foreach (Type type in typesToExclude)
                    {
                        if (gameObject.GetComponent(type) != null)
                        {
                            return null;
                        }
                    }
                }

                gameObject = gameObject.transform.parent?.gameObject;
            }
            if (gameObject != null && IsVisualisation(gameObject))
            {
                return gameObject;
            }
        }
        return null;
    }

    public static bool IsVisualisation(GameObject gameObject)
    {
        return gameObject.GetComponent<Visualization>() != null;
    }

    public static ViveWandVirtualTool GetVirtualToolFromPointer(IMixedRealityPointer pointer)
    {
        return pointer.Controller.Visualizer.GameObjectProxy.GetComponentInChildren<ViveWandVirtualTool>();
    }
}
