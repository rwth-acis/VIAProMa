using i5.ViaProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputField))]
public class DeleteInputFieldContent : MonoBehaviour
{
    private InputField inputField;

    public void DeleteContent()
    {
        inputField.Text = "";
    }
}
