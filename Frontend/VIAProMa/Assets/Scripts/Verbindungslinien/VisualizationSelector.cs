using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class VisualizationSelector : MonoBehaviour, IMixedRealityPointerHandler
{
    private WindowManager manager;

    /// <summary>
    /// Called by the Mixed Reality Toolkit if the object was clicked
    /// If the LineDraw mode is active, the object is saved as either start or destination.
    /// </summary>
    /// <param name="eventData">The event data of the interaction</param>
    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if(gameObject == manager.ConnectionLinesMenu.StartObject || gameObject == manager.ConnectionLinesMenu.DestinationObject)
        {
            return;
        }
        if (manager.ConnectionLinesMenu.IsLineModeActivated || manager.ConnectionLinesMenu.IsDeleteLineModeActivated)
        {
            if (!manager.ConnectionLinesMenu.OneSelected)
            {
                if (manager.ConnectionLinesMenu.StartObject != null && manager.ConnectionLinesMenu.StartObject.GetComponent<IssueSelector>() != null)
                {
                    //manager.ConnectionLinesMenu.StartObject.GetComponent<IssueSelector>().backgroundRenderer.material.color = manager.ConnectionLinesMenu.StartObject.GetComponent<IssueSelector>().originalRendererColor;
                    manager.ConnectionLinesMenu.StartObject.GetComponent<IssueSelector>().Selected = false;
                }
                    if (manager.ConnectionLinesMenu.StartObject != null && manager.ConnectionLinesMenu.StartObject.GetComponent<VisualizationSelector>() != null)
                {
                    manager.ConnectionLinesMenu.StartObject.transform.Find("HighlightingCube").gameObject.SetActive(false);
                }
                manager.ConnectionLinesMenu.StartObject = gameObject;
                manager.ConnectionLinesMenu.OneSelected = true;
            }
            else
            {
                if (manager.ConnectionLinesMenu.DestinationObject != null && manager.ConnectionLinesMenu.DestinationObject.GetComponent<IssueSelector>() != null)
                {
                    //manager.ConnectionLinesMenu.DestinationObject.GetComponent<IssueSelector>().backgroundRenderer.material.color = manager.ConnectionLinesMenu.DestinationObject.GetComponent<IssueSelector>().originalRendererColor;
                    manager.ConnectionLinesMenu.DestinationObject.GetComponent<IssueSelector>().Selected = false;
                }
                if (manager.ConnectionLinesMenu.DestinationObject != null && manager.ConnectionLinesMenu.DestinationObject.GetComponent<VisualizationSelector>() != null)
                {
                    manager.ConnectionLinesMenu.DestinationObject.transform.Find("HighlightingCube").gameObject.SetActive(false);
                }
                manager.ConnectionLinesMenu.DestinationObject = gameObject;
                manager.ConnectionLinesMenu.OneSelected = false;
            }
            transform.Find("HighlightingCube").gameObject.SetActive(true);
        }
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindWithTag("LineDraw").GetComponent<WindowManager>();
    }
}
