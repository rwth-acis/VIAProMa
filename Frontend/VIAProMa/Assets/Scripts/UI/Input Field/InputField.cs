using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace i5.ViaProMa.UI
{

    public class InputField : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshPro contentField;

        [Header("Content")]
        [SerializeField] private string text = "Content";

        public event EventHandler TextChanged;

        private Interactable fieldButton;

        public string Text
        {
            get { return text; }
            set
            {
                if (text != value) // only do something if the text was actually changed (and not the same text entered again)
                {
                    text = value;
                    ApplyTextToDisplay();
                    TextChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public TextMeshPro ContentField { get => contentField; }

        public bool Enabled
        {
            get => fieldButton.Enabled;
            set => fieldButton.Enabled = value;
        }

        public List<string> AutocompleteOptions
        {
            get;set;
        }

        private void Awake()
        {
            if (contentField == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(contentField));
            }
            fieldButton = GetComponent<Interactable>();
            if (fieldButton == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(Interactable), gameObject);
            }
        }

        public void OnClick()
        {
            if (Keyboard.Instance == null)
            {
                Debug.LogError("There is no keyboard in the scene. Add the keyboard prefab somewhere in the scene to use InputFields.");
            }
            else
            {
                Keyboard.Instance.Open(transform.position - transform.forward * 0.05f, transform.eulerAngles, Text, AutocompleteOptions);
                Keyboard.Instance.InputFinished += OnKeyboardInputFinished;
            }
        }

        private void OnKeyboardInputFinished(object sender, InputFinishedEventArgs e)
        {
            Keyboard.Instance.InputFinished -= OnKeyboardInputFinished;
            if (!e.Aborted)
            {
                Text = e.Text;
            }
        }

        private void OnKeyboardTextChanged(object sender, EventArgs e)
        {
            Text = Keyboard.Instance.Text;
        }

        private void ApplyTextToDisplay()
        {
            if (contentField != null)
            {
                contentField.text = text;
            }
        }

        public void ResetText()
        {
            Text = "";
        }

        private void OnValidate()
        {
            ApplyTextToDisplay();
        }
    }
}