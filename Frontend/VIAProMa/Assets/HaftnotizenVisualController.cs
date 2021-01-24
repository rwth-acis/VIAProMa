using System.Collections;
using System.Collections.Generic;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.UI.ListView.Issues;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.UI.InputFields;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

public class HaftnotizenVisualController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private InputField inputField;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject dash;
    [SerializeField] private Interactable pinButton;
    [SerializeField] private Interactable clearButton;
    [SerializeField] private Interactable editButton;


    public string Text
        {
            get => inputField.Text;
            set
            {
                inputField.Text = value;
            }
        }


    private void Awake()
        {
            if (inputField == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(inputField));
            }
            if (background == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(background));
            }
            if (title == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(title));
            }
            if (dash == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(dash));
            }
            if (pinButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(pinButton));
            }
            if (clearButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(clearButton));
            }
            if (editButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(editButton));
            }
        }

}

 