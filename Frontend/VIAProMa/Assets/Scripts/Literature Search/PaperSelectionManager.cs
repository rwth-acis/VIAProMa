using HoloToolkit.Unity;
using i5.VIAProMa.IssueSelection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.LiteratureSearch
{
    public class PaperSelectionManager : Singleton<PaperSelectionManager>
    {
        /// <summary>
        /// The list of selected papers
        /// </summary>
        public List<Paper> SelectedPapers { get; private set; }

        /// <summary>
        /// Event which is invoked if the selection mode is changed, i.e. if the selection mode is started or ended
        /// </summary>
        public event EventHandler SelectionModeChanged;
        /// <summary>
        /// Event which is invoked if a paper was selected or deselected
        /// The paper and it was selected or deselected can be found in the event arguments
        /// </summary>
        public event EventHandler<SelectionChangedArgs<Paper>> PaperSelectionChanged;

        /// <summary>
        /// True if the selection mode is currently active and the user can select papers
        /// </summary>
        public bool SelectionModeActive
        {
            get; private set;
        }

        /// <summary>
        /// Initializes the component
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            SelectedPapers = new List<Paper>();
            SelectionModeActive = false;
        }

        /// <summary>
        /// Starts the selection mode with an empty initial list of selected papers
        /// </summary>
        public void StartSelectionMode()
        {
            StartSelectionMode(new List<Paper>());
        }

        /// <summary>
        /// Starts the selection mode with the given papers as the initial list
        /// </summary>
        /// <param name="selectedPapers">The list of papers which are already selected</param>
        public void StartSelectionMode(List<Paper> selectedPapers)
        {
            SelectedPapers = selectedPapers;
            SelectionModeActive = true;
            SelectionModeChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Ends the selectoin mode
        /// </summary>
        /// <returns>The list of papers which were selected by the user</returns>
        public List<Paper> EndSelectionMode()
        {
            SelectionModeActive = false;
            SelectionModeChanged?.Invoke(this, EventArgs.Empty);
            return SelectedPapers;
        }

        /// <summary>
        /// Selects the given paper
        /// Selection Mode should be set to true; otherwise, the method has no effect
        /// </summary>
        /// <param name="paper">The paper to select</param>
        public void SetSelected(Paper paper)
        {
            if (SelectionModeActive)
            {
                SelectedPapers.Add(paper);
                PaperSelectionChanged?.Invoke(this, new SelectionChangedArgs<Paper>(paper, true));
            }
        }

        /// <summary>
        /// Unselects the given paper.
        /// </summary>
        /// <param name="paper"></param>
        public void SetDeselected(Paper paper)
        {
            if (SelectionModeActive)
            {
                bool removeSuccessful = SelectedPapers.Remove(paper);
                if (removeSuccessful)
                {
                    PaperSelectionChanged?.Invoke(this, new SelectionChangedArgs<Paper>(paper, false));
                }
            }
        }

        /// <summary>
        /// Checks whether the given paper is selected.
        /// </summary>
        /// <param name="paper"></param>
        /// <returns></returns>
        public bool IsSelected(Paper paper)
        {
            return SelectedPapers.Contains(paper);
        }
    }

}
