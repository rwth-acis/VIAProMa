using i5.VIAProMa.UI.InputFields;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Voice.PUN.UtilityScripts;
using System;
using UnityEngine;

namespace i5.VIAProMa.UI
{
    public class ImportModelMenu : MonoBehaviour, IWindow
    {
        [SerializeField] private InputField searchField;
        [SerializeField] private Interactable loginButton;
        [SerializeField] private Interactable searchUpButton;
        [SerializeField] private Interactable searchDownButton;
        [SerializeField] private Interactable importedObjectsUpButton;
        [SerializeField] private Interactable importedObjectsDownButton;
        

        public bool WindowEnabled { get; set; }

        public bool WindowOpen
        {
            get => gameObject.activeSelf;
        }

        public event EventHandler WindowClosed;

        private void Awake()
        {
            if (searchField == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(searchField));
            }
            else
            {
                searchField.TextChanged += OnQueryChanged;
            }
            if (loginButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(loginButton));
            }
            if (searchUpButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(searchUpButton));
            }
            if (searchDownButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(searchDownButton));
            }
            if (importedObjectsUpButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(importedObjectsUpButton));
            }
            if (importedObjectsDownButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(importedObjectsDownButton));
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        private void OnQueryChanged(object sender, EventArgs e)
        {
            bool validInput = !string.IsNullOrWhiteSpace(searchField.Text);
            if (validInput) Debug.Log("Import Menu Search Field: " + searchField.Text);
        }

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
    }
}
