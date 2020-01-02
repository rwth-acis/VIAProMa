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
    /// True, if the LineDraw Mode is active
    /// </summary>
    private bool isLineModeActivated;

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
        gameObject.SetActive(transform.parent.parent.parent.parent.GetComponent<FoldController>().MenuOpen);
    }

    /// <summary>
    /// Is called when the LineDraw Button is clicked. Enables/Disables the LineDrawingMode and switches the displayed text accordingly.
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
        }
        isLineModeActivated = !isLineModeActivated;
    }
}
