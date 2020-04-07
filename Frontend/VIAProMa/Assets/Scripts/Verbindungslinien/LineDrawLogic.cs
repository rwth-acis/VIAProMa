using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;

public class LineDrawLogic : MonoBehaviour
{
    /// <summary>
    /// Referencing the caption of the line draw button
    /// </summary>
    [SerializeField] private GameObject caption;

    /// <summary>
    /// Referencing the caption of the delete button
    /// </summary>
    [SerializeField] private GameObject deleteCaption;

    /// <summary>
    /// Referencing the LineDraw Button
    /// </summary>
    [SerializeField] private GameObject lineDrawButton;

    /// <summary>
    /// Referencing the DeleteAllLines Button
    /// </summary>
    [SerializeField] private GameObject deleteAllLinesButton;

    ///<summary>
    ///Referencing the DeleteSpecificLines Button
    /// </summary>
    [SerializeField] private GameObject deleteSpecificLinesButton;

    /// <summary>
    /// The LineRenderer Prefab to be instantiated
    /// </summary>
    [SerializeField] private GameObject lineRendererPrefab;

    /// <summary>
    /// True, if the LineDraw Mode is active
    /// </summary>
    private bool isLineModeActivated;

    /// <summary>
    /// True, if the LineDelete Mode is active
    /// </summary>
    private bool isDeleteLineModeActivated;

    /// <summary>
    /// True, if the one start object has been selected
    /// </summary>
    private bool oneSelected;

    /// <summary>
    /// The start object of the line
    /// </summary>
    private GameObject start;

    /// <summary>
    /// The destination object of the line
    /// </summary>
    private GameObject destination;

    /// <summary>
    /// Reference to the customization menu
    /// </summary>
    [SerializeField] private GameObject cMenu;

    /// <summary>
    /// Currently selected color
    /// </summary>
    private Color curColor;

    /// <summary>
    /// Currently selected line width
    /// </summary>
    private bool isThick;

    /// <summary>
    /// The button of the currently selected Color
    /// </summary>
    private GameObject highlightedColorButton;

    /// <summary>
    /// The button of the currently selected Width
    /// </summary>
    private GameObject highlightedWidthButton;

    /// <summary>
    /// Start with the buttons invisible and the LineDraw Mode is deactivated.
    /// </summary>
    void Start()
    {
        isLineModeActivated = false;
        isDeleteLineModeActivated = false;
        cMenu.SetActive(false);
        curColor = Color.red;
        isThick = false;
        highlightedColorButton = cMenu.transform.Find("RedButton").gameObject;
        highlightedColorButton.GetComponent<Interactable>().Enabled = false;
        highlightedWidthButton = cMenu.transform.Find("ThinButton").gameObject;
        highlightedWidthButton.GetComponent<Interactable>().Enabled = false;
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
            //Close the customization menu
            cMenu.SetActive(false);

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
            if (start.GetComponent<VisualizationSelector>() != null)
            {
                start.transform.Find("HighlightingCube").gameObject.SetActive(false);
            }
            if (destination.GetComponent<VisualizationSelector>() != null)
            {
                destination.transform.Find("HighlightingCube").gameObject.SetActive(false);
            }
            if (start != null && destination != null)
            {
                GameObject lineRenderer = Instantiate(lineRendererPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                lineRenderer.GetComponent<LineRenderer>().SetPosition(0, start.transform.position);
                lineRenderer.GetComponent<LineRenderer>().SetPosition(1, destination.transform.position);
                lineRenderer.GetComponent<UpdatePosition>().StartObject = start;
                lineRenderer.GetComponent<UpdatePosition>().DestinationObject = destination;
                lineRenderer.GetComponent<Renderer>().material.SetColor("_Color",curColor);
                if (isThick)
                {
                    lineRenderer.GetComponent<LineRenderer>().startWidth = 0.04f;
                    lineRenderer.GetComponent<LineRenderer>().endWidth = 0.04f;
                }
                else
                {
                    lineRenderer.GetComponent<LineRenderer>().startWidth = 0.01f;
                    lineRenderer.GetComponent<LineRenderer>().endWidth = 0.01f;
                }
            }
            start = null;
            destination = null;
        }
        else
        {
            //Open the customization menu
            cMenu.SetActive(true);

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
            if (start.GetComponent<VisualizationSelector>() != null)
            {
                start.transform.Find("HighlightingCube").gameObject.SetActive(false);
            }
            if (destination.GetComponent<VisualizationSelector>() != null)
            {
                destination.transform.Find("HighlightingCube").gameObject.SetActive(false);
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

    /// <summary>
    /// Called by the Line Width buttons OnClick
    /// </summary>
    public void SwitchWidth(bool newIsThick)
    {
        //Reset highlighted button
        highlightedWidthButton.GetComponent<Interactable>().Enabled = true;
        isThick = newIsThick;

        //Highlight the button that was now selected
        if(newIsThick)
        {
            highlightedWidthButton = cMenu.transform.Find("ThickButton").gameObject;
            highlightedWidthButton.GetComponent<Interactable>().Enabled = false;
        }
        else
        {
            highlightedWidthButton = cMenu.transform.Find("ThinButton").gameObject;
            highlightedWidthButton.GetComponent<Interactable>().Enabled = false;
        }
    }

    /// <summary>
    /// Called by the Color Switch buttons OnClick
    /// </summary>
    public void SwitchColor(int newColor)
    {
        //Reset highlighted button
        highlightedColorButton.GetComponent<Interactable>().Enabled = true;

        switch(newColor)
        {
            case 1:
                curColor = Color.red;
                highlightedColorButton = cMenu.transform.Find("RedButton").gameObject;
                highlightedColorButton.GetComponent<Interactable>().Enabled = false;
                break;
            case 2:
                curColor = Color.yellow;
                highlightedColorButton = cMenu.transform.Find("YellowButton").gameObject;
                highlightedColorButton.GetComponent<Interactable>().Enabled = false;
                break;
            case 3:
                curColor = Color.white;
                highlightedColorButton = cMenu.transform.Find("WhiteButton").gameObject;
                highlightedColorButton.GetComponent<Interactable>().Enabled = false;
                break;
            case 4:
                curColor = Color.green;
                highlightedColorButton = cMenu.transform.Find("GreenButton").gameObject;
                highlightedColorButton.GetComponent<Interactable>().Enabled = false;
                break;
            case 5:
                curColor = Color.blue;
                highlightedColorButton = cMenu.transform.Find("BlueButton").gameObject;
                highlightedColorButton.GetComponent<Interactable>().Enabled = false;
                break;
            case 6:
                curColor = Color.black;
                highlightedColorButton = cMenu.transform.Find("BlackButton").gameObject;
                highlightedColorButton.GetComponent<Interactable>().Enabled = false;
                break;
        }
    }
}
