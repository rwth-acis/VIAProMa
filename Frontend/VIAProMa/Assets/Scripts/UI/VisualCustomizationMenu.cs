using i5.VIAProMa.SaveLoadSystem.Core;
using i5.VIAProMa.UI.InputFields;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace i5.VIAProMa.UI
{
    public class VisualCustomizationMenu : MonoBehaviour, IWindow
        {

        [SerializeField] private List<String> themes;
        [SerializeField] private Interactable RightButton;
        [SerializeField] private Interactable LeftButton;
        [SerializeField] private TextMeshPro ThemeLabel;

        public bool WindowEnabled { get; set; }

        public bool WindowOpen
        {
            get => gameObject.activeSelf;
        }

        public event EventHandler WindowClosed;

        private void Awake()
        {
            
        }

        private void Start()
        {
           
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