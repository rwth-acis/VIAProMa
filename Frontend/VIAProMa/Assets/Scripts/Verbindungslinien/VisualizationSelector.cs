using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class VisualizationSelector : MonoBehaviour, IMixedRealityPointerHandler
{
    ///public ConnectionLinesMenu linedrawscript;
    public WindowManager manager;

    /// <summary>
    /// Called by the Mixed Reality Toolkit if the object was clicked
    /// If the LineDraw mode is active, the object is saved as either start or destination.
    /// </summary>
    /// <param name="eventData">The event data of the interaction</param>
    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (manager.ConnectionLinesMenu.isLineModeActivated || manager.ConnectionLinesMenu.isDeleteLineModeActivated)
        {
            if (!manager.ConnectionLinesMenu.oneSelected)
            {
                if (manager.ConnectionLinesMenu.start != null && manager.ConnectionLinesMenu.start.GetComponent<IssueSelector>() != null)
                {
                    manager.ConnectionLinesMenu.start.GetComponent<IssueSelector>().backgroundRenderer.material.color = manager.ConnectionLinesMenu.start.GetComponent<IssueSelector>().originalRendererColor;
                }
                if (manager.ConnectionLinesMenu.start != null && manager.ConnectionLinesMenu.start.GetComponent<VisualizationSelector>() != null)
                {
                    manager.ConnectionLinesMenu.start.transform.Find("HighlightingCube").gameObject.SetActive(false);
                }
                manager.ConnectionLinesMenu.start = gameObject;
                manager.ConnectionLinesMenu.oneSelected = true;
            }
            else
            {
                if (manager.ConnectionLinesMenu.destination != null && manager.ConnectionLinesMenu.destination.GetComponent<IssueSelector>() != null)
                {
                    manager.ConnectionLinesMenu.destination.GetComponent<IssueSelector>().backgroundRenderer.material.color = manager.ConnectionLinesMenu.destination.GetComponent<IssueSelector>().originalRendererColor;
                }
                if (manager.ConnectionLinesMenu.destination != null && manager.ConnectionLinesMenu.destination.GetComponent<VisualizationSelector>() != null)
                {
                    manager.ConnectionLinesMenu.destination.transform.Find("HighlightingCube").gameObject.SetActive(false);
                }
                manager.ConnectionLinesMenu.destination = gameObject;
                manager.ConnectionLinesMenu.oneSelected = false;
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
