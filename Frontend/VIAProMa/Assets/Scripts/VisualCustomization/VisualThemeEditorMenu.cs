using System;
using System.Collections.Generic;
using System.Linq;
using i5.Toolkit.Core.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace i5.VIAProMa.UI
{
    /// <summary>
    /// UI controller and actions for the visual customization menu
    /// </summary>
    public class VisualThemeEditorMenu : MonoBehaviour, IWindow
    {
        [SerializeField] private Interactable previousButton;
        [SerializeField] private Interactable nextButton;
        [SerializeField] private List<VisualThemeItem> visualThemeItems;
        
        public bool WindowEnabled { get; set; }
        private List<VisualCustomizationTheme> loadedThemes;
        private int page;
        
        public bool WindowOpen => gameObject.activeSelf;

        public event EventHandler WindowClosed;

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

        private void Start()
        {
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
            var themes = VisualCustomizationManager.GetDefaultThemes();
            var customThemes = VisualCustomizationManager.GetCustomThemes();
            themes.AddRange(customThemes);
            loadedThemes = themes;
            
            GoToPage(0);
        }

        private void GoToPage(int pageNumber)
        {
            page = pageNumber;
            var startNumber = pageNumber * visualThemeItems.Count;

            for (var index = 0; index < visualThemeItems.Count; index++)
            {
                var visualThemeItem = visualThemeItems[index];
                var themeIndex = startNumber + index;

                if (themeIndex < loadedThemes.Count)
                {
                    visualThemeItems[index].gameObject.SetActive(true);
                    visualThemeItems[index].Setup(loadedThemes[themeIndex].name,true);
                }
                else
                {
                    visualThemeItems[index].gameObject.SetActive(false);
                }
            }
            
            previousButton.IsEnabled = pageNumber != 0;
            nextButton.IsEnabled = startNumber + visualThemeItems.Count < loadedThemes.Count;
        }

        public void NextPage()
        {
            GoToPage(page+1);
        }
        
        public void PreviousPage()
        {
            GoToPage(page-1);
        }
    }
}