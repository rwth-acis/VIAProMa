using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Component that is attached to the Description Window, updates the description text
/// </summary>
public class DescriptionHelper : MonoBehaviour
{
    public TMP_Text label;

    /// <summary>
    /// Updates the descriptiontext
    /// </summary>
    /// <param name="pLabel"></param>
    public void UpdateLabel(string pLabel)
    {
        label.text = pLabel;
    }
}
