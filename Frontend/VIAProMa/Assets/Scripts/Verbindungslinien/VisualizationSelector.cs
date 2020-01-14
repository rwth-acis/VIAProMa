﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class VisualizationSelector : MonoBehaviour, IMixedRealityPointerHandler
{
    public LineDrawLogic linedrawscript;

    /// <summary>
    /// Called by the Mixed Reality Toolkit if the object was clicked
    /// If the LineDraw mode is active, the object is saved as either start or destination.
    /// </summary>
    /// <param name="eventData">The event data of the interaction</param>
    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (linedrawscript.isLineModeActivated)
        {
            if (!linedrawscript.oneSelected)
            {
                if (linedrawscript.start != null && linedrawscript.start.GetComponent<IssueSelector>() != null)
                {
                    linedrawscript.start.GetComponent<IssueSelector>().backgroundRenderer.material.color = linedrawscript.start.GetComponent<IssueSelector>().originalRendererColor;
                }
                linedrawscript.start = gameObject;
                linedrawscript.oneSelected = true;
            }
            else
            {
                if (linedrawscript.destination != null && linedrawscript.destination.GetComponent<IssueSelector>() != null)
                {
                    linedrawscript.destination.GetComponent<IssueSelector>().backgroundRenderer.material.color = linedrawscript.destination.GetComponent<IssueSelector>().originalRendererColor;
                }
                linedrawscript.destination = gameObject;
                linedrawscript.oneSelected = false;
            }
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
        linedrawscript = GameObject.FindGameObjectWithTag("LineDraw").GetComponent<LineDrawLogic>();
    }
}
