using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;

/// <summary>
/// A collection of helper functions for virtual tool actions
/// </summary>
public class ActionHelperFunctions : MonoBehaviour
{
    /// <summary>
    /// Iterates upwards in the hirachy of gameObject and returns the first GameObject that has the Visualization script attached. Returns null if nothing is found.
    /// </summary>
    /// <param name="gameObject"></param> The gameObject on which the search should start
    /// <param name="typesToExclude"></param> Can be used as a filter. A GameObject can be ignored if it has a object contianed in typesToExclude above or below it
    /// <param name="checkAbove"></param> Check above in the hirachy for filterd types
    /// <param name="checkBelow"></param>Check below in the hirachy for filterd types
    /// <returns></returns>
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

    /// <summary>
    /// Is the gameObject a visualistaion?
    /// </summary>
    /// <param name="gameObject"></param> The object to be checked
    /// <returns></returns>
    public static bool IsVisualisation(GameObject gameObject)
    {
        return gameObject.GetComponent<Visualization>() != null;
    }


    /// <summary>
    /// Get the virtual tool that is attached to the pointer
    /// </summary>
    /// <param name="pointer"></param>
    /// <returns></returns>
    public static ViveWandVirtualTool GetVirtualToolFromPointer(IMixedRealityPointer pointer)
    {
        return pointer.Controller.Visualizer.GameObjectProxy.GetComponentInChildren<ViveWandVirtualTool>();
    }

    /// <summary>
    /// Rotates the object on the XZ plane towards the main camera and sets it to new position
    /// </summary>
    /// <param name="objectToRotate"></param> The object to move and rotate
    /// <param name="newPosition"></param> The new position
    public void RotateToCameraOnXZPlane(GameObject objectToRotate, Vector3 newPosition)
    {
        objectToRotate.transform.LookAt(Camera.main.transform);
        //Set the x and y rotation to 0 and flip it around because with LookAt, the prefab faces away from the camera and is tilted strangly
        Vector3 rotation = objectToRotate.transform.eulerAngles;
        objectToRotate.transform.SetPositionAndRotation(newPosition, Quaternion.Euler(0, 180 + rotation.y, 0));
    }



    //public Material highlightMaterial;
    //public Material wireframeMaterial;
    //Material previousMaterial;
    //public void HighlightBoudningBox(FocusEventData data)
    //{
    //    BoundingBox box = GetVisualisationFromGameObject(data.NewFocusedObject)?.GetComponentInChildren<BoundingBox>();
    //    if (box != null && GetVisualisationFromGameObject(data.NewFocusedObject) != null)
    //    {
    //        previousMaterial = box.BoxMaterial;
    //        box.BoxMaterial = highlightMaterial;
    //        box.ShowWireFrame = true;
    //        box.WireframeMaterial = wireframeMaterial;
    //        //Debug.Log("Highlight " + box.ToString());
    //    }
    //}

    //public void DeHighlightBoundingBox(FocusEventData data)
    //{
    //    BoundingBox box = GetVisualisationFromGameObject(data.OldFocusedObject)?.GetComponentInChildren<BoundingBox>();
    //    if (box != null)
    //    {
    //        box.BoxMaterial = previousMaterial;
    //        box.ShowWireFrame = false;
    //        //Debug.Log("Dehighlight " + box.ToString());
    //    }
    //    else
    //    {
    //        //Debug.Log("Dehighlight no box found");
    //    }
    //}

    

}
