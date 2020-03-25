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


    public bool WindowEnabled { get; set; }

    public bool WindowOpen
    {
        get => gameObject.activeSelf;
    }

    public event EventHandler WindowClosed;


    public void Close()
    {
        gameObject.SetActive(false);
        WindowClosed?.Invoke(this, EventArgs.Empty);
    }

    public void Open()
    {
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
        isLineModeActivated = false;
        isDeleteLineModeActivated = false;
    }
}
