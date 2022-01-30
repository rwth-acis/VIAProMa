using i5.VIAProMa.Shelves.IssueShelf;
using i5.VIAProMa.Shelves.ProjectLoadShelf;
using i5.VIAProMa.Shelves.Visualizations;
using i5.VIAProMa.UI.KeyboardInput;
using i5.VIAProMa.UI.ListView.Issues;
using i5.VIAProMa.UI.MainMenuCube;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VIAProMa.Visualizations;
using i5.VIAProMa.Visualizations.ProgressBars;
using System.Linq;
using i5.VIAProMa.Visualizations.BuildingProgressBar;
using i5.VIAProMa.Visualizations.Common;

namespace i5.VIAProMa.UI.SearchMenu
{
    public class SearchMenu : MonoBehaviour
    {
        private List<(string, TMP_Text)> autocompleteIssues;
        private List<(string, GameObject)> autocompleteVisualizations;

        public enum SearchTargets
        {
            MainMenu, IssueShelf, VisualizationShelf, ProjectShelf, IssueCard, Visualization
        }

        [SerializeField] private InteractableToggleCollection radialMenu;
        [SerializeField] private Keyboard keyboard;

        private SearchTargets currentTarget;

        private void OnEnable()
        {
            keyboard.InputFinished += OnKeyboardInputFinished;
            autocompleteIssues = CalculateAutocompleteIssues();
            autocompleteVisualizations = CalculateAutocompleteVisualizations();
        }

        private void OnDisable()
        {
            keyboard.InputFinished -= OnKeyboardInputFinished;
        }

        /// <summary>
        /// Either pings the object if there is only one or opens a keyboard for search.
        /// </summary>
        public void OnSelectRadial()
        {
            int index = radialMenu.CurrentIndex;
            if (radialMenu.ToggleList[index].enabled == false) return;

            currentTarget = (SearchTargets)index;
            switch (currentTarget)
            {
                case SearchTargets.MainMenu:
                case SearchTargets.IssueShelf:
                case SearchTargets.VisualizationShelf:
                case SearchTargets.ProjectShelf:
                    PingSingleton(currentTarget);
                    break;
                default:
                    OpenKeyboard(currentTarget);
                    break;
            }

        }

        /// <summary>
        /// Opens the keyboard with required autocomplete list
        /// </summary>
        /// <param name="target">Kind of Objects to add to autocomplete</param>
        public void OpenKeyboard(SearchTargets target)
        {
            if(target == SearchTargets.IssueCard)
            {
                List<string> options = autocompleteIssues.ConvertAll(x => x.Item1);
                keyboard.Open(transform.position - transform.forward, transform.rotation.eulerAngles, "", options);
            }
            if (target == SearchTargets.Visualization)
            {
                List<string> options = autocompleteVisualizations.ConvertAll(x => x.Item1);
                keyboard.Open(transform.position - transform.forward, transform.rotation.eulerAngles, "", options);
            }
        }

        public void CloseMenu()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Pings an object that is only once in the scene (singleton).
        /// </summary>
        /// <param name="target"></param>
        private void PingSingleton(SearchTargets target)
        {
            MonoBehaviour at = null;
            if (target == SearchTargets.MainMenu) at = FindObjectOfType<MainMenu>();
            if (target == SearchTargets.IssueShelf) at = FindObjectOfType<IssuesLoader>();
            if (target == SearchTargets.VisualizationShelf) at = FindObjectOfType<VisualizationShelf>();
            if (target == SearchTargets.ProjectShelf) at = FindObjectOfType<ProjectLoader>();

           
            if (at == null || at.gameObject.activeSelf == false)
            {
                AudioManager.instance.PlayErrorSound(transform.position);
                return;
            }

            AudioManager.instance.PlayerPingSound(at.gameObject.transform.position);
        }

        private List<(string, TMP_Text)> CalculateAutocompleteIssues()
        {
            List<(string, TMP_Text)> autocompleteOptions = new List<(string, TMP_Text)>();
            foreach (IssueDataDisplay issue in FindObjectsOfType<IssueDataDisplay>())
            {
                if (!issue.enabled) continue;
                var texts = issue.GetComponentsInChildren<TMP_Text>();
                TMP_Text title = texts[0].gameObject.name == "Title" ? texts[0] : texts[1];
                autocompleteOptions.Add((title.text, title));
                //autocompleteOptions.Add(issue)
            }
            return autocompleteOptions;
        }


        private List<(string, GameObject)> CalculateAutocompleteVisualizations()
        {
            List<(string, GameObject)> autocompleteOptions = new List<(string, GameObject)>();

            //Add all visualizations
            autocompleteOptions.AddRange(FindObjectsOfType<Visualization>().ToList().ConvertAll(x=>(x.gameObject.name, x.gameObject)));
            autocompleteOptions.AddRange(FindObjectsOfType<Diagram>().ToList().ConvertAll(x => (x.gameObject.name, x.gameObject)));

            return autocompleteOptions;
        }

        /// <summary>
        /// Handles the actual search for the object and pings it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyboardInputFinished(object sender, InputFinishedEventArgs e)
        {
            if (e.Aborted) return;
            if(currentTarget == SearchTargets.IssueCard)
            {
                TMP_Text tmpObject = autocompleteIssues.Find(x => x.Item1 == e.Text).Item2;
                if (tmpObject == null) AudioManager.instance.PlayErrorSound(transform.position);
                AudioManager.instance.PlayerPingSound(tmpObject.transform.position);
            }

            if(currentTarget == SearchTargets.Visualization)
            {
                GameObject go = autocompleteVisualizations.Find(x => x.Item1 == e.Text).Item2;
                if(go == null ) AudioManager.instance.PlayErrorSound(transform.position);
                AudioManager.instance.PlayerPingSound(go.transform.position);
            }
            
        }
    }
}
