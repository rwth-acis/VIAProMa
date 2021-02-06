using System.Collections;
using System.Collections.Generic;
using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.UI.ListView.Issues;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.UI.InputFields;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.StickyNote
{
    public class StickyNoteVisualController : MonoBehaviour, IVisualizationVisualController
    {
        [Header("UI Elements")]
        [SerializeField] private InputField inputField;
        [SerializeField] private GameObject background;
        [SerializeField] private GameObject title;
        [SerializeField] private GameObject dash;
        [SerializeField] private Interactable pinButton;
        [SerializeField] private Interactable clearButton;
        [SerializeField] private Interactable editButton;
        [SerializeField] private Interactable colorButton;
        [SerializeField] private Renderer colorTag;
        [SerializeField] private ColorCycler colorCycler;


        public string Text
            {
                get => inputField.Text;
                set
                {
                    inputField.Text = value;
                }
            }
        
        public string Title
        {
            get => "Note";
            set
            {
            }
        }

        public string ColorTag
        {
            get {
                if (colorTag.material.color == Color.red) return "red";
                else if (colorTag.material.color == Color.yellow) return "yellow";
                else if (colorTag.material.color == Color.green) return "green";
                else if (colorTag.material.color == Color.cyan) return "cyan";
                else if (colorTag.material.color == Color.blue) return "blue";
                else if (colorTag.material.color == Color.magenta) return "magenta";
                else if (colorTag.material.color == Color.black) return "black";
                else if (colorTag.material.color == Color.grey) return "grey";
                else return "white";
            }
            set {
                    if (value == "red") colorCycler.colorSet(Color.red);
                    else if (value == "yellow") colorCycler.colorSet(Color.yellow);
                    else if (value == "green") colorCycler.colorSet(Color.green);
                    else if (value == "cyan") colorCycler.colorSet(Color.cyan);
                    else if (value == "blue") colorCycler.colorSet(Color.blue);

                    else if (value == "magenta") colorCycler.colorSet(Color.magenta);
                    else if (value == "black") colorCycler.colorSet(Color.black);
                    else if (value == "grey") colorCycler.colorSet(Color.grey);
                    else colorCycler.colorSet(Color.white);
                    
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
                if (pinButton == null)
                {
                    SpecialDebugMessages.LogMissingReferenceError(this, nameof(colorButton));
                }
                if (clearButton == null)
                {
                    SpecialDebugMessages.LogMissingReferenceError(this, nameof(clearButton));
                }
                if (editButton == null)
                {
                    SpecialDebugMessages.LogMissingReferenceError(this, nameof(editButton));
                }
                if (colorTag == null)
                {
                    SpecialDebugMessages.LogMissingReferenceError(this, nameof(colorTag));
                }
            }

    }

 
}
