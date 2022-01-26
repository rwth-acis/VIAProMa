using UnityEngine;
using TMPro;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.Visualizations.ProgressBars;
using System.Collections.Generic;

namespace GuidedTour
{
    public class SectionBoard : MonoBehaviour
    {
        [SerializeField] private GuidedTourManager guidedTourManager;
        [SerializeField] private ProgressBarController progressBar;
        [SerializeField] private TextMeshPro displayText;

        internal Dictionary<string, int> secsToTaskCount = new Dictionary<string, int>();
        internal int totalTasks = 0;

        void Start()
        {
            foreach (TourSection sec in guidedTourManager.Sections)
            {
                int taskCount = 0;
                foreach (AbstractTourTask t in sec.Tasks)
                {
                    taskCount++;
                    totalTasks++;
                }
                secsToTaskCount.Add(sec.Name, taskCount);
            }
            updateSectionBoard();
        }

        private void Awake()
        {
            if (guidedTourManager == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(guidedTourManager));
            }
            if (displayText == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(displayText));
            }
            if (progressBar == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(progressBar));
            }
        }

        /**
         * <summary>
         * This method calls the GuidedTourManagers skipSection() function when the Button on the Section Board is pressed
         * </summary>
         */
        public void skipSection()
        {
            guidedTourManager.SkipSection();
        }

        /**
         * <summary>
         * This Method updates the Visualization of the Progress Bar and Section Board
         * </summary>
         */
        internal void updateSectionBoard()
        {
            int currentTaskIterator = 0;
            int test = 0;
            foreach (TourSection sec in guidedTourManager.Sections)
            {
                if (sec.Name == guidedTourManager.ActiveSection.Name)
                {
                    foreach (AbstractTourTask t in sec.Tasks)
                    {
                        currentTaskIterator++;
                        if (t.Id == guidedTourManager.ActiveTask.Id)
                        {
                            test = currentTaskIterator;
                            break;
                        }
                    }
                }
            }
            int currentSecTaskCount = 0;
            if (secsToTaskCount.ContainsKey(guidedTourManager.ActiveSection.Name))
            {
                secsToTaskCount.TryGetValue(guidedTourManager.ActiveSection.Name, out currentSecTaskCount);
            }
            if (totalTasks > 0)
            {
                progressBar.PercentageDone = (float)guidedTourManager.totalTasksDone / totalTasks;
            }
            displayText.text = guidedTourManager.ActiveSection.Name + " (" + test + "/" + currentSecTaskCount + ")";
        }
    }
}
