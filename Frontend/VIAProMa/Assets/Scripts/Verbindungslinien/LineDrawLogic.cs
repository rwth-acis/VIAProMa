﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;

public class LineDrawLogic : MonoBehaviour
{
    /// <summary>
    /// Referencing the caption of the line draw button
    /// </summary>
    public GameObject caption;

    /// <summary>
    /// Referencing the caption of the delete button
    /// </summary>
    public GameObject deleteCaption;

    /// <summary>
    /// Referencing the LineDraw Button
    /// </summary>
    public GameObject lineDrawButton;

    /// <summary>
    /// Referencing the DeleteAllLines Button
    /// </summary>
    public GameObject deleteAllLinesButton;

    ///<summary>
    ///Referencing the DeleteSpecifixLines Button
    /// </summary>
    public GameObject deleteSpecificLinesButton;
     
    /// <summary>
    /// The LineRenderer Prefab to be instantiated
    /// </summary>
    public GameObject lineRendererPrefab;

    /// <summary>
    /// True, if the LineDraw Mode is active
    /// </summary>
    [HideInInspector] public bool isLineModeActivated;

    /// <summary>
    /// True, if the LineDelete Mode is active
    /// </summary>
    [HideInInspector] public bool isDeleteLineModeActivated;

    /// <summary>
    /// True, if the one start object has been selected
    /// </summary>
    [HideInInspector] public bool oneSelected;

    /// <summary>
    /// The start object of the line
    /// </summary>
    [HideInInspector] public GameObject start;

    /// <summary>
    /// The destination object of the line
    /// </summary>
    [HideInInspector] public GameObject destination;

    /// <summary>
    /// Start with the buttons invisible and the LineDraw Mode is deactivated.
    /// </summary>
    void Start()
    {
        lineDrawButton.SetActive(false);
        deleteAllLinesButton.SetActive(false);
        deleteSpecificLinesButton.SetActive(false);
        isLineModeActivated = false;
        isDeleteLineModeActivated = false;
    }

    /// <summary>
    /// The LineDraw Button and the DeleteAllLines Button appear when the Menu is open and are deactivated when it is closed.
    /// </summary>
    void Update()
    {
        lineDrawButton.SetActive(transform.GetComponent<FoldController>().MenuOpen);
        deleteAllLinesButton.SetActive(transform.GetComponent<FoldController>().MenuOpen);
        deleteSpecificLinesButton.SetActive(transform.GetComponent<FoldController>().MenuOpen);
    }

    /// <summary>
    /// Is called when the LineDraw Button is clicked. Enables/Disables the LineDrawingMode and switches the displayed text accordingly.
    /// When the Draw Line button is clicked, an empty gameobject containing the LineRenderer component is instantiated and start and
    /// destination position of the line set.
    /// </summary>
    public void SwitchLineDrawMode()
    {
        if (isDeleteLineModeActivated)
        {
            return;
        }
        if (isLineModeActivated)
        {
            caption.GetComponent<TextMeshPro>().SetText("Enter Line Draw");
            deleteSpecificLinesButton.GetComponent<Interactable>().Enabled = true;
            if (start == null || destination == null)
            {
                isLineModeActivated = !isLineModeActivated;
                return;
            }
            if (start.GetComponent<IssueSelector>() != null)
            {
                start.GetComponent<IssueSelector>().backgroundRenderer.material.color = start.GetComponent<IssueSelector>().originalRendererColor;
            }
            if (destination.GetComponent<IssueSelector>() != null)
            {
                destination.GetComponent<IssueSelector>().backgroundRenderer.material.color = destination.GetComponent<IssueSelector>().originalRendererColor;
            }
            if (start != null && destination != null)
            {
                GameObject lineRenderer = Instantiate(lineRendererPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                lineRenderer.GetComponent<LineRenderer>().SetPosition(0, start.transform.position);
                lineRenderer.GetComponent<LineRenderer>().SetPosition(1, destination.transform.position);
                lineRenderer.GetComponent<UpdatePosition>().startObject = start;
                lineRenderer.GetComponent<UpdatePosition>().destinationObject = destination;
            }
            start = null;
            destination = null;
        }
        else
        {
            caption.GetComponent<TextMeshPro>().SetText("Draw Line");
            deleteSpecificLinesButton.GetComponent<Interactable>().Enabled = false;
        }
        isLineModeActivated = !isLineModeActivated;
        Debug.Log("Mode switched!");
    }

    /// <summary>
    /// Deletes all drawn lines in the scene. Is called when the "Delete All Lines" button is clicked.
    /// </summary>
    public void DeleteAllLines()
    {
        GameObject[] lines = GameObject.FindGameObjectsWithTag("Line");
        foreach
        (GameObject line in lines)
        {
            GameObject.Destroy(line);
            Debug.Log("At least one line deleted");
        }
        Debug.Log("All Lines deleted");
    }

    public void DeleteSpecificLine()
    {
        if (isLineModeActivated)
        {
            return;
        }
        if (isDeleteLineModeActivated)
        {
            deleteCaption.GetComponent<TextMeshPro>().SetText("Enter Single Delete");
            lineDrawButton.GetComponent<Interactable>().Enabled = true;
            if (start == null || destination == null)
            {
                isDeleteLineModeActivated = !isDeleteLineModeActivated;
                return;
            }
            if (start.GetComponent<IssueSelector>() != null)
            {
                start.GetComponent<IssueSelector>().backgroundRenderer.material.color = start.GetComponent<IssueSelector>().originalRendererColor;
            }
            if (destination.GetComponent<IssueSelector>() != null)
            {
                destination.GetComponent<IssueSelector>().backgroundRenderer.material.color = destination.GetComponent<IssueSelector>().originalRendererColor;
            }
            if (start != null && destination != null)
            {
                GameObject[] lines = GameObject.FindGameObjectsWithTag("Line");
                foreach
                (GameObject line in lines)
                {
                    if ((line.GetComponent<LineRenderer>().GetPosition(0) == start.transform.position 
                        && line.GetComponent<LineRenderer>().GetPosition(1) == destination.transform.position) ||
                        (line.GetComponent<LineRenderer>().GetPosition(0) == start.transform.position 
                        && line.GetComponent<LineRenderer>().GetPosition(1) == destination.transform.position))
                    {
                        Destroy(line);
                    }
                }
            }
        }
        else
        {
            deleteCaption.GetComponent<TextMeshPro>().SetText("Delete Line");
            lineDrawButton.GetComponent<Interactable>().Enabled = false;
        }
        isDeleteLineModeActivated = !isDeleteLineModeActivated;


        
    }
}
