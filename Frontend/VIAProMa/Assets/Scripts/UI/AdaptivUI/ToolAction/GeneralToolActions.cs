using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.UI;

/// <summary>
/// A collection of ToolActions that are helpfull for multiple tools
/// </summary>
public class GeneralToolActions : ActionHelperFunctions
{
    #region IconOverVisualisation

    public GameObject highlightSprite;
    GameObject instantiatedIcon;

    /// <summary>
    /// Spawns the icon from the currently selected tool over the boundingbox of the visualisation the tool just started pointing at
    /// </summary>
    /// <param name="data"></param> The data from the corresponding focus event
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

    /// <summary>
    /// Updates position and rotation of the currently spawned icon over the visualisation, the tool currently points at
    /// </summary>
    /// <param name="data"></param> The data from the corresponding focus event
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

    /// <summary>
    /// Destroys the instantiated icon, that was spawned by SpawnCurrentIconOverVisualisation()
    /// </summary>
    public void DestroyCurrentIconOverVisualisation()
    {
        Destroy(instantiatedIcon);
    }

    #endregion

    #region BoundingBox

    BoundingBoxStateController[] boundingBoxStateControllers;

    /// <summary>
    /// Activates the BoundingBox from all visualisations in the scene, but deactivates the manipulation handler. This makes ot easier to hit most visualisations with the pointer.
    /// </summary>
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

    /// <summary>
    /// Deactivates all BoundingBoxes, that were activated by OpenBoudningBoxWithoutManipulation() and activates there manipulation handler again
    /// </summary>
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

    #endregion

    #region Descritption Texts

    /// <summary>
    /// Activates the button description texts on the tool, that is saved in data
    /// </summary>
    /// <param name="data"></param> The data from the corresponding input event
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
    #endregion
}
