using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using HoloToolkit.Unity;
using Photon.Pun;
public class VIAProMaMenuActions : MonoBehaviour
{
    public void DoubleSize(BaseInputEventData eventData)
    {
        GameObject target = eventData.InputSource.Pointers[0]?.Result?.CurrentPointerTarget;
        if (target != null)
        {
            target.transform.root.localScale *= 2;
        }
    }

    public void HalveeSize(BaseInputEventData eventData)
    {
        GameObject target = eventData.InputSource.Pointers[0]?.Result?.CurrentPointerTarget;
        if (target != null)
        {
            target.transform.root.localScale /= 2;
        }
    }


    /// <summary>
    /// Main function for tool to remove visualisations.
    /// </summary>
    /// <param name="eventData"></param>
    public void Remove(BaseInputEventData eventData)
    {
        GameObject target = GetVisualisationFromInputSource(eventData.InputSource);
        if (target == null)
        {
            return;
        }
        if (target.GetComponentInChildren<PhotonView>() != null)
        {
            PhotonNetwork.Destroy(target);
        }
        else
        {
            Destroy(target);
        }
    }

    //Adjust Tool

    BoundingBoxStateController[] boundingBoxStateControllers;

    public void StartAdjusting()
    {
        boundingBoxStateControllers = FindObjectsOfType<BoundingBoxStateController>();
        foreach (var boundingbox in boundingBoxStateControllers)
        {
            boundingbox.BoundingBoxActive = true;
        }
    }

    public void StopAdjusting()
    {
        foreach (var boundingbox in boundingBoxStateControllers)
        {
            if (boundingbox != null)
            {
                boundingbox.BoundingBoxActive = false;
            }
        }
    }

    public void OpenConfigurationWindow(BaseInputEventData eventData)
    {
        GameObject target = GetVisualisationFromInputSource(eventData.InputSource, new Type[] {typeof(ConfigurationWindow)}, true, false);
        if (target != null)
        {
            ConfigurationWindow configurationWindow = target.transform.GetComponentInChildren<ConfigurationWindow>(true);
            if (configurationWindow != null)
            {
                configurationWindow.Open();
                configurationWindow.transform.LookAt(Camera.main.transform);
                //Set the x and y rotation to 0 and flip it around because with LookAt, the prefab faces away from the camera and is tilted strangly
                Vector3 rotation = configurationWindow.transform.eulerAngles;
                configurationWindow.transform.SetPositionAndRotation(eventData.InputSource.Pointers[0].Position, Quaternion.Euler(0,180+rotation.y,0));
            }
        }
    }

    public void UndoToolAction()
    {
        Debug.Log("Undo");
    }

    public void RedoToolAction()
    {
        Debug.Log("Redo");
    }

    private GameObject GetVisualisationFromInputSource(IMixedRealityInputSource source, Type[] typesToExclude = null, bool checkAbove = false, bool checkBelow = false)
    {
        foreach (var pointer in source.Pointers)
        {
            GameObject target = pointer.Result?.CurrentPointerTarget;

            //If wished, check if any of the children of the target is of a type that should be excluded
            if (typesToExclude != null && checkBelow)
            {
                foreach (Type type in typesToExclude)
                {
                    if (target.GetComponentInChildren(type,true) != null)
                    {
                        return null;
                    }
                }
            }

            if (target != null)
            {
                while (target != null && !IsVisualisation(target))
                {
                    //If wished, check if the current object (i.e. a object above in the hirachy of the original target) is of a type that should be excluded
                    if (typesToExclude != null && checkAbove)
                    {
                        foreach (Type type in typesToExclude)
                        {
                            if (target.GetComponent(type) != null)
                            {
                                return null;
                            } 
                        }
                    }

                    target = target.transform.parent?.gameObject;
                }
                if (target != null && IsVisualisation(target))
                {
                    return target;
                }
            }
        }
        return null;
    }

    bool IsVisualisation(GameObject gameObject)
    {
        return gameObject.GetComponent<Visualization>() != null;
    }
}
