using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LineDrawLogic : MonoBehaviour
{
    /// <summary>
    /// Referencing the caption of the button
    /// </summary>
    public GameObject caption;

    /// <summary>
    /// Referencing the LineDraw Button
    /// </summary>
    public GameObject lineDrawButton;

    /// <summary>
    /// The LineRenderer Prefab to be instantiated
    /// </summary>
    public GameObject lineRendererPrefab;

    /// <summary>
    /// True, if the LineDraw Mode is active
    /// </summary>
    [HideInInspector] public bool isLineModeActivated;

    /// <summary>
    /// True, if the one start object has been selected
    /// </summary>
    [HideInInspector] public bool oneSelected;

    /// <summary>
    /// The transform component of the start object
    /// </summary>
    [HideInInspector] public Transform start;

    /// <summary>
    /// The transform component of the destination object
    /// </summary>
    [HideInInspector] public Transform destination;

    /// <summary>
    /// Start with the button invisible and the LineDraw Mode is deactivated.
    /// </summary>
    void Start()
    {
        lineDrawButton.SetActive(false);
        isLineModeActivated = false;
    }

    /// <summary>
    /// The LineDraw Button appears when the Menu is open and is deactivated when it is closed.
    /// </summary>
    void Update()
    {
        lineDrawButton.SetActive(transform.GetComponent<FoldController>().MenuOpen);
    }

    /// <summary>
    /// Is called when the LineDraw Button is clicked. Enables/Disables the LineDrawingMode and switches the displayed text accordingly.
    /// When the Draw Line button is clicked, an empty gameobject containing the LineRenderer component is instantiated and start and
    /// destination position of the line set.
    /// </summary>
    public void SwitchLineDrawMode()
    {
        if(isLineModeActivated)
        {
            caption.GetComponent<TextMeshPro>().SetText("Enter Line Draw");
        }
        else
        {
            caption.GetComponent<TextMeshPro>().SetText("Draw Line");
            if(start != null && destination != null)
            {
                GameObject lineRenderer = Instantiate(lineRendererPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                lineRenderer.GetComponent<LineRenderer>().SetPosition(0, start.position);
                lineRenderer.GetComponent<LineRenderer>().SetPosition(1, destination.position);
            }
            start = null;
            destination = null;
        }
        isLineModeActivated = !isLineModeActivated;
    }
}
