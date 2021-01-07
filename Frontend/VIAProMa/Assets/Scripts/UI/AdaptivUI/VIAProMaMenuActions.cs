using System.Collections;
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
        GameObject target = GetVisualisationFromInputSource(eventData.InputSource);
        if (target != null)
        {
            ConfigurationWindow configurationWindow = target.transform.GetComponentInChildren<ConfigurationWindow>(true);
            if (configurationWindow != null)
            {
                configurationWindow.Open();
            }
        }
    }

    private GameObject GetVisualisationFromInputSource(IMixedRealityInputSource source)
    {
        foreach (var pointer in source.Pointers)
        {
            GameObject target = pointer.Result?.CurrentPointerTarget;
            if (target != null)
            {
                while (target != null && !IsVisualisation(target))
                {
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
