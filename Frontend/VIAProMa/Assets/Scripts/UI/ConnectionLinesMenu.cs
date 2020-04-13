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
    /// Referencing the ThickLine Button
    /// </summary>
    [SerializeField] private GameObject thickLineButton;

    ///<summary>
    ///Referencing the ThinLine Button
    /// </summary>
    [SerializeField] private GameObject thinLineButton;

    ///<summary>
    ///Referencing the Color Chooser
    /// </summary>
    [SerializeField] private GameObject colorChooser;

    /// <summary>
    /// The LineRenderer Prefab to be instantiated
    /// </summary>
    [SerializeField] private GameObject lineRendererPrefab;


    /// <summary>
    /// Reference to the currently selected Width Button
    /// </summary>
    private GameObject _highlightedWidthButton;
    public GameObject HighightedWidthButton
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
        colorChooser.GetComponent<ConfigurationColorChooser>().ColorChosen -= ColorChosenEventHandler;
        gameObject.SetActive(false);
        WindowClosed?.Invoke(this, EventArgs.Empty);
    }

    public void Open()
    {
        colorChooser.GetComponent<ConfigurationColorChooser>().ColorChosen += ColorChosenEventHandler;
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
        colorChooser.SetActive(false);
        thickLineButton.SetActive(false);
        thinLineButton.SetActive(false);
        _highlightedWidthButton = thinLineButton.gameObject;
        _highlightedWidthButton.GetComponent<Interactable>().Enabled = false;

        _isLineModeActivated = false;
        _isDeleteLineModeActivated = false;
        _isThick = false;
        _chosenColor = defaultColor;
    }

    public void SetWindowToDrawMode()
    {
        caption.GetComponent<TextMeshPro>().SetText("Enter Line Draw");
        deleteSpecificLinesButton.GetComponent<Interactable>().Enabled = true;
        colorChooser.SetActive(false);
        thickLineButton.SetActive(false);
        thinLineButton.SetActive(false);
    }

    public void SetWindowToSelectionMode()
    {
        caption.GetComponent<TextMeshPro>().SetText("Draw Line");
        deleteSpecificLinesButton.GetComponent<Interactable>().Enabled = false;
        colorChooser.SetActive(true);
        thickLineButton.SetActive(true);
        thinLineButton.SetActive(true);
    }

   public void SetLine()
    {
        GameObject lineRenderer = Instantiate(lineRendererPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        lineRenderer.GetComponent<LineRenderer>().SetPosition(0, _startObject.transform.position);
        lineRenderer.GetComponent<LineRenderer>().SetPosition(1, _destinationObject.transform.position);
        lineRenderer.GetComponent<UpdatePosition>().StartObject = _startObject;
        lineRenderer.GetComponent<UpdatePosition>().DestinationObject = _destinationObject;
        lineRenderer.GetComponent<Renderer>().material.SetColor("_Color", _chosenColor);
        if (_isThick)
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


    /// <summary>
    /// Is called when the LineDraw Button is clicked. Enables/Disables the LineDrawingMode and switches the displayed text accordingly.
    /// When the Draw Line button is clicked, an empty gameobject containing the LineRenderer component is instantiated and start and
    /// destination position of the line set.
    /// </summary>
    public void SwitchLineDrawMode()
    {

        if (_isDeleteLineModeActivated)
        {
            return;
        }
        if (_isLineModeActivated)
        {
            SetWindowToDrawMode();

            if (_startObject == null || _destinationObject == null)
            {
                _isLineModeActivated = !_isLineModeActivated;
                return;
            }
            if (_startObject.GetComponent<IssueSelector>() != null)
            {
                _startObject.GetComponent<IssueSelector>().backgroundRenderer.material.color = _startObject.GetComponent<IssueSelector>().originalRendererColor;
            }
            if (_destinationObject.GetComponent<IssueSelector>() != null)
            {
                _destinationObject.GetComponent<IssueSelector>().backgroundRenderer.material.color = _destinationObject.GetComponent<IssueSelector>().originalRendererColor;
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
        }
        else
        {
            SetWindowToSelectionMode();
        }
        _isLineModeActivated = !_isLineModeActivated;
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
        if (_isLineModeActivated)
        {
            return;
        }
        if (_isDeleteLineModeActivated)
        {
            deleteCaption.GetComponent<TextMeshPro>().SetText("Enter Single Delete");
            lineDrawButton.GetComponent<Interactable>().Enabled = true;
            if (_startObject == null || _destinationObject == null)
            {
                _isDeleteLineModeActivated = !_isDeleteLineModeActivated;
                return;
            }
            if (_startObject.GetComponent<IssueSelector>() != null)
            {
                _startObject.GetComponent<IssueSelector>().backgroundRenderer.material.color = _startObject.GetComponent<IssueSelector>().originalRendererColor;
            }
            if (_destinationObject.GetComponent<IssueSelector>() != null)
            {
                _destinationObject.GetComponent<IssueSelector>().backgroundRenderer.material.color = _destinationObject.GetComponent<IssueSelector>().originalRendererColor;
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
                    if ((line.GetComponent<LineRenderer>().GetPosition(0) == _startObject.transform.position
                        && line.GetComponent<LineRenderer>().GetPosition(1) == _destinationObject.transform.position) ||
                        (line.GetComponent<LineRenderer>().GetPosition(0) == _startObject.transform.position
                        && line.GetComponent<LineRenderer>().GetPosition(1) == _destinationObject.transform.position))
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
        _isDeleteLineModeActivated = !_isDeleteLineModeActivated;
    }

    /// <summary>
    /// Called by the Line Width buttons OnClick
    /// </summary>
    public void SwitchWidth(bool newIsThick)
    {
        //Reset highlighted button
        _highlightedWidthButton.GetComponent<Interactable>().Enabled = true;
        _isThick = newIsThick;

        //Highlight the button that was now selected
        if (newIsThick)
        {
            _highlightedWidthButton = thickLineButton.gameObject;
            _highlightedWidthButton.GetComponent<Interactable>().Enabled = false;
        }
        else
        {
            _highlightedWidthButton = thinLineButton.gameObject;
            _highlightedWidthButton.GetComponent<Interactable>().Enabled = false;
        }
    }


    void ColorChosenEventHandler(Color color)
    {
        ChosenColor = color;
    }
}
