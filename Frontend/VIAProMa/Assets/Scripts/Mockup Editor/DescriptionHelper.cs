using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
