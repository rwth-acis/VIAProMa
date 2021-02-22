using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.UI;


public class GeneralToolActions : ActionHelperFunctions
{
    public GameObject highlightSprite;
    GameObject instantiatedIcon;
    public void SpawnCurrentIconOverVisualisation(FocusEventData data)
    {
        if (GetVisualisationFromGameObject(data.NewFocusedObject) != null)
        {
            if (instantiatedIcon != null)
            {
                //This shouldn't happen, but just to be sure
                Destroy(instantiatedIcon);
            }
            instantiatedIcon = Instantiate(highlightSprite);
            instantiatedIcon.GetComponentInChildren<Image>().sprite = GetVirtualToolFromPointer(data.Pointer).currentEntry.iconTool;
        }
    }

    public void UpdateCurrentIconOverVisualisation(FocusEventData data)
    {
        if (instantiatedIcon != null)
        {
            GameObject target = GetVisualisationFromGameObject(data.NewFocusedObject);
            Collider collider = target?.GetComponentInChildren<BoundingBox>()?.GetComponent<Collider>();
            if (collider != null)
            {
                Vector3 maxpoint = collider.bounds.max;
                Vector3 minpoint = collider.bounds.min;
                minpoint.y = maxpoint.y;
                Vector3 topMiddle = minpoint + 0.5f * (maxpoint - minpoint);
                topMiddle.y += 0.3f;
                RotateToCameraOnXZPlane(instantiatedIcon, topMiddle);
            }
        }
    }

    public void DestroyCurrentIconOverVisualisation()
    {
        Destroy(instantiatedIcon);
    }

    BoundingBoxStateController[] boundingBoxStateControllers;

    public void OpenBoudningBoxWithoutManipulation()
    {
        boundingBoxStateControllers = FindObjectsOfType<BoundingBoxStateController>();
        foreach (var boundingbox in boundingBoxStateControllers)
        {
            if (GetVisualisationFromGameObject(boundingbox.gameObject) != null)
            {
                boundingbox.BoundingBoxActive = true;
                boundingbox.manipulationHandler.enabled = false;
            }
        }
    }

    public void CloseBoudningBoxWithoutManipulation()
    {
        foreach (var boundingbox in boundingBoxStateControllers)
        {
            if (boundingbox != null)
            {
                boundingbox.BoundingBoxActive = false;
                boundingbox.manipulationHandler.enabled = true;
            }
        }
    }

    public void ActivateDesciptionTexts(BaseInputEventData data)
    {
        GameObject tool = data.InputSource.Pointers[0].Controller.Visualizer.GameObjectProxy;
        GameObject buttonDescriptons = tool.transform.Find("ButtonDescriptions")?.gameObject;
        buttonDescriptons.SetActive(true);
    }

    public void DeactivateDesciptionTexts(BaseInputEventData data)
    {
        GameObject tool = data.InputSource.Pointers[0].Controller.Visualizer.GameObjectProxy;
        GameObject buttonDescriptons = tool.transform.Find("ButtonDescriptions")?.gameObject;
        buttonDescriptons.SetActive(false);
    }
}
