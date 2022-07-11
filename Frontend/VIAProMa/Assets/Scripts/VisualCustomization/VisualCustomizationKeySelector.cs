using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

public class VisualCustomizationKeySelector : MonoBehaviour
{
    [SerializeField] private Interactable nextButton;
    [SerializeField] private Interactable prevButton;

    public void Activate()
    {
        nextButton.IsEnabled = true;
        prevButton.IsEnabled = true;
    }
    
    public void Deactivate()
    {
        nextButton.IsEnabled = false;
        prevButton.IsEnabled = false;
    }
}
