using i5.ViaProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionLinesMenu : MonoBehaviour, IWindow
{

    /// <summary>
    /// Referencing the initial color of Lines
    /// </summary>
    [SerializeField] private Color defaultColor;

    ///<summary>
    ///Referencing the Color Chooser
    /// </summary>
    [SerializeField] private ConfigurationColorChooser colorChooser;

    /// <summary>
    /// Referencing the caption of the line draw button
    /// </summary>
    [SerializeField] private GameObject optionWindow;

    /// <summary>
    /// Referencing the caption of the delete button
    /// </summary>
    [SerializeField] private TextMeshPro deleteCaption;

    /// <summary>
    /// Referencing the LineDraw Button
    /// </summary>
    [SerializeField] private Interactable lineDrawButton;

    /// <summary>
    /// Referencing the DeleteAllLines Button
    /// </summary>
    [SerializeField] private Interactable deleteAllLinesButton;

    ///<summary>
    ///Referencing the DeleteSpecificLines Button
    /// </summary>
    [SerializeField] private Interactable deleteSpecificLinesButton;

    /// <summary>
    /// Referencing the ThickLine Button
    /// </summary>
    [SerializeField] private Interactable thickLineButton;

    ///<summary>
    ///Referencing the ThinLine Button
    /// </summary>
    [SerializeField] private Interactable thinLineButton;

    /// <summary>
    /// The LineRenderer Prefab to be instantiated
    /// </summary>
    [SerializeField] private GameObject lineRendererPrefab;

    /// <summary>
    /// Reference to the currently selected Width Button
    /// </summary>
    private Interactable _highlightedWidthButton;
    public Interactable HighightedWidthButton
    {
        get { return _highlightedWidthButton; }
        set { _highlightedWidthButton = value; }
    }


    /// <summary>
    /// True, if the LineDraw Mode is active
    /// </summary>
    private bool _isLineModeActivated;
    public bool IsLineModeActivated
    {
        get { return _isLineModeActivated; }
    }

    /// <summary>
    /// True, if the LineDelete Mode is active
    /// </summary>
    private bool _isDeleteLineModeActivated;
    public bool IsDeleteLineModeActivated
    {
        get { return _isDeleteLineModeActivated; }
    }

    /// <summary>
    /// True, if the one start object has been selected
    /// </summary>
    private bool _oneSelected;
    public bool OneSelected
    {
        get { return _oneSelected; }
        set { _oneSelected = value; }
    }

    /// <summary>
    /// The start object of the line
    /// </summary>
    private GameObject _startObject;
    public GameObject StartObject
    {
        get { return _startObject; }
        set { _startObject = value; }
    }

    /// <summary>
    /// The destination object of the line
    /// </summary>
    private GameObject _destinationObject;
    public GameObject DestinationObject
    {
        get { return _destinationObject; }
        set { _destinationObject = value; }
    }

    /// <summary>
    /// Currently selected color
    /// </summary>
    private Color _chosenColor;
    public Color ChosenColor
    {
        get { return _chosenColor; }
        set { _chosenColor = value; }
    }

    /// <summary>
    /// Currently selected line width
    /// </summary>
    private bool _isThick;
    public bool IsThick
    {
        get { return _isThick; }
        set { _isThick = value; }
    }

    public bool WindowEnabled { get; set; }

    public bool WindowOpen
    {
        get => gameObject.activeSelf;
    }

    public event EventHandler WindowClosed;


    public void Close()
    {
        colorChooser.ColorChosen -= ColorChosenEventHandler;
        gameObject.SetActive(false);
        WindowClosed?.Invoke(this, EventArgs.Empty);
    }

    public void Open()
    {
        colorChooser.ColorChosen += ColorChosenEventHandler;
        gameObject.SetActive(true);
    }

    public void Open(Vector3 position, Vector3 eulerAngles)
    {
        Open();
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;
    }

    /// <summary>
    /// Start with the buttons invisible and the LineDraw Mode is deactivated.
    /// </summary>
    void Start()
    {
        optionWindow.SetActive(false);
        _highlightedWidthButton = thinLineButton;
        _highlightedWidthButton.Enabled = false;

        _isLineModeActivated = false;
        _isDeleteLineModeActivated = false;
        _isThick = false;
        _chosenColor = defaultColor;
    }



    /// <summary>
    /// A line is initiated with the correct values.
    /// </summary>
    public void SetLine()
    {
        GameObject lineRenderer = Instantiate(lineRendererPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        LineRenderer lineRendererComponent = lineRenderer.GetComponent<LineRenderer>();
        lineRendererComponent.SetPosition(0, _startObject.transform.position);
        lineRendererComponent.SetPosition(1, _destinationObject.transform.position);
        UpdatePosition positionScript = lineRenderer.GetComponent<UpdatePosition>();
        positionScript.StartObject = _startObject;
        positionScript.DestinationObject = _destinationObject;
        lineRenderer.GetComponent<Renderer>().material.SetColor("_Color", _chosenColor);
        if (_isThick)
        {
            lineRendererComponent.startWidth = 0.04f;
            lineRendererComponent.endWidth = 0.04f;
        }
        else
        {
            lineRendererComponent.startWidth = 0.01f;
            lineRendererComponent.endWidth = 0.01f;
        }
    }


    /// <summary>
    /// Is called when the LineDraw Button is clicked. The additional window containing color and width options is opened.
    /// </summary>
    public void EnterLineDrawMode()
    {
        if (_isDeleteLineModeActivated)
        {
            return;
        }
        optionWindow.SetActive(true);
        _isLineModeActivated = true;
    }


    /// <summary>
    /// Is called when the Draw Line Button is clicked. A Line in the chosen color and width is instantianted between the two selected objects.
    /// </summary>
    public void LeaveLineDrawMode()
    {
        if (_isDeleteLineModeActivated)
        {
            return;
        }
        optionWindow.SetActive(false);
        if (_startObject == null || _destinationObject == null)
        {
            _isLineModeActivated = !_isLineModeActivated;
            return;
        }
        IssueSelector startIssueSelector = _startObject.GetComponent<IssueSelector>();
        if (startIssueSelector != null)
        {
            startIssueSelector.backgroundRenderer.material.color = startIssueSelector.originalRendererColor;
        }
        IssueSelector destinationIssueSelector = _destinationObject.GetComponent<IssueSelector>();
        if (destinationIssueSelector != null)
        {
            destinationIssueSelector.backgroundRenderer.material.color = destinationIssueSelector.originalRendererColor;
        }
        if (_startObject.GetComponent<VisualizationSelector>() != null)
        {
            _startObject.transform.Find("HighlightingCube").gameObject.SetActive(false);
        }
        if (_destinationObject.GetComponent<VisualizationSelector>() != null)
        {
            _destinationObject.transform.Find("HighlightingCube").gameObject.SetActive(false);
        }
        if (_startObject != null && _destinationObject != null)
        {
            SetLine();
        }
        _startObject = null;
        _destinationObject = null;
        _isLineModeActivated = false;
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
        if (_isLineModeActivated)
        {
            return;
        }
        if (_isDeleteLineModeActivated)
        {
            deleteCaption.SetText("Enter Single Delete");
            lineDrawButton.Enabled = true;
            if (_startObject == null || _destinationObject == null)
            {
                _isDeleteLineModeActivated = !_isDeleteLineModeActivated;
                return;
            }
            IssueSelector startIssueSelector = _startObject.GetComponent<IssueSelector>();
            if (startIssueSelector != null)
            {
                startIssueSelector.backgroundRenderer.material.color = startIssueSelector.originalRendererColor;
            }
            IssueSelector destinationIssueSelector = _destinationObject.GetComponent<IssueSelector>();
            if (destinationIssueSelector != null)
            {
                destinationIssueSelector.backgroundRenderer.material.color = destinationIssueSelector.originalRendererColor;
            }
            if (_startObject.GetComponent<VisualizationSelector>() != null)
            {
                _startObject.transform.Find("HighlightingCube").gameObject.SetActive(false);
            }
            if (_destinationObject.GetComponent<VisualizationSelector>() != null)
            {
                _destinationObject.transform.Find("HighlightingCube").gameObject.SetActive(false);
            }
            if (_startObject != null && _destinationObject != null)
            {
                GameObject[] lines = GameObject.FindGameObjectsWithTag("Line");
                foreach
                (GameObject line in lines)
                {
                    LineRenderer lineRendererComponent = line.GetComponent<LineRenderer>();
                    if ((lineRendererComponent.GetPosition(0) == _startObject.transform.position
                        && lineRendererComponent.GetPosition(1) == _destinationObject.transform.position) ||
                        (lineRendererComponent.GetPosition(0) == _startObject.transform.position
                        && lineRendererComponent.GetPosition(1) == _destinationObject.transform.position))
                    {
                        Destroy(line);
                    }
                }
            }
        }
        else
        {
            deleteCaption.SetText("Delete Line");
            lineDrawButton.Enabled = false;
        }
        _isDeleteLineModeActivated = !_isDeleteLineModeActivated;
    }

    /// <summary>
    /// Called by the Line Width buttons OnClick
    /// </summary>
    public void SwitchWidth(bool newIsThick)
    {
        //Reset highlighted button
        _highlightedWidthButton.Enabled = true;
        _isThick = newIsThick;

        //Highlight the button that was now selected
        if (newIsThick)
        {
            _highlightedWidthButton = thickLineButton;
            _highlightedWidthButton.Enabled = false;
        }
        else
        {
            _highlightedWidthButton = thinLineButton;
            _highlightedWidthButton.Enabled = false;
        }
    }


    void ColorChosenEventHandler(Color color)
    {
        ChosenColor = color;
    }
}
