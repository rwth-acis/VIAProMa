using System;
using i5.Toolkit.Core.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace i5.VIAProMa.UI
{
    /// <summary>
    /// UI controller and actions for the visual customization menu
    /// </summary>
    public class VisualCustomizationMenu : MonoBehaviour, IWindow
    {
        [SerializeField] private InteractableToggleCollection themeToggles;
        
        public bool WindowEnabled { get; set; }

        public bool WindowOpen => gameObject.activeSelf;

        public event EventHandler WindowClosed;
        
        private void Awake()
        {
            if (themeToggles == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(themeToggles));
            }
        }
        
        public void Close()
        {
            gameObject.SetActive(false);
            WindowClosed?.Invoke(this, EventArgs.Empty);
        }

        public void Open()
        {
            gameObject.SetActive(true);
            Initialize();
        }

        public void Open(Vector3 position, Vector3 eulerAngles)
        {
            Open();
            transform.localPosition = position;
            transform.localEulerAngles = eulerAngles;
        }

        private void Initialize()
        {
            //TODO
        }
        
        public void OnThemeChanged()
        {
            //TODO
            Debug.Log(themeToggles.CurrentIndex);
            switch (themeToggles.CurrentIndex)
            {
                case 0:
                    VisualCustomizationManager.SwitchStyle("Shelf","Default","Default");
                    break;
                case 1:
                    VisualCustomizationManager.SwitchStyle("Shelf","Alternative","Default");
                    break;
                case 2:
                    VisualCustomizationManager.SwitchStyle("Shelf","Default","Red");
                    break;
                case 3:
                    VisualCustomizationManager.SwitchStyle("Shelf","Alternative","Red");
                    break;
            }
        }
    }
}