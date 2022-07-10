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
    public class VisualCustomizationMenu : MonoBehaviour, IWindow
    {
        [SerializeField] private Interactable previousButton;
        [SerializeField] private Interactable nextButton;
        [SerializeField] private Interactable addButton;
        [SerializeField] private List<VisualThemeItem> visualThemeItems;
        [SerializeField] private VisualThemeEditorMenu editorMenu;
        
        public bool WindowEnabled { get; set; }
        private List<VisualCustomizationTheme> loadedThemes;
        private int page;
        
        public bool WindowOpen => gameObject.activeSelf;

        public event EventHandler WindowClosed;

        public void Close()
        {
            gameObject.SetActive(false);
            editorMenu.Close();
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
            var allThemes = new List<VisualCustomizationTheme>();
            allThemes.AddRange(themes);
            allThemes.AddRange(customThemes);
            loadedThemes = allThemes;
            
            GoToPage(0);
        }

        public void ReloadThemes()
        {
            var themes = VisualCustomizationManager.GetDefaultThemes();
            var customThemes = VisualCustomizationManager.GetCustomThemes();
            var allThemes = new List<VisualCustomizationTheme>();
            allThemes.AddRange(themes);
            allThemes.AddRange(customThemes);
            loadedThemes = allThemes;

            if (loadedThemes.Count / visualThemeItems.Count < page)
            {
                page = 0;
            }
            
            GoToPage(page);
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
                    visualThemeItems[index].Setup(loadedThemes[themeIndex]);
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

        [ContextMenu("Deactivate")]
        public void Deactivate()
        {
            foreach (var visualThemeItem in visualThemeItems)
            {
                visualThemeItem.Deactivate();
            }
            
            addButton.IsEnabled = false;
        }
        
        [ContextMenu("Activate")]
        public void Activate()
        {
            foreach (var visualThemeItem in visualThemeItems)
            {
                visualThemeItem.Activate();
            }

            addButton.IsEnabled = true;
        }

        public void OpenEditor()
        {
            editorMenu.Open();
            var newTheme = new VisualCustomizationTheme(VisualCustomizationManager.GetDefaultTheme().styleSelections)
                {
                    name = ""
                };
            editorMenu.LoadTheme(newTheme);
        }
        
        public void OpenEditor(VisualCustomizationTheme theme)
        {
            editorMenu.Open();
            editorMenu.LoadTheme(theme);
        }
    }
}