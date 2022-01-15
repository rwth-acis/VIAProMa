using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuidedTour
{
    /**
     * <summary>
     * The GuidedTourManager holds the TourSection which hold the TourTasks. The Manager is responsible for 
     * managing the state of the tasks and for managing the config file(s).
     * </summary>
     */
    public class GuidedTourManager : MonoBehaviour
    {
        public AbstractTourTask ActiveTask { get; private set; }
        public TourSection ActiveSection { get; private set; }
        public List<TourSection> Sections { get; private set; }

        [SerializeField] private TextPlacer textPlacer;
        [SerializeField] private GuidedTourWidget widget; 

        private ConfigFile configFile = new ConfigFile("Assets/GuidedTour/Configuration/GuidedTour.json");
        private int sectionIndex = 0;
        private int taskIndex = -1;

        private void Awake()
        {
            Sections = new List<TourSection>();

            configFile.LoadConfig();
            GuidedTourUtils.LinkTasks(Sections, configFile.Root);

            ActiveSection = Sections[0];
            SelectNextTask();
        }

        void Update()
        {
            if (ActiveTask == null)
                return;

            if (ActiveTask.IsTaskDone())
            {
                Debug.Log("Finished task: " + ActiveTask.Name);
                ActiveTask.State = AbstractTourTask.TourTaskState.COMPLETED;
                SelectNextTask();
            }
        }


        /**
         * <summary>
         * This method skips the current section. For each remaining task of the section the SkipTask() Method is called.
         * If this section was not the last, the next section will begin with its first task.
         * <exception cref="InvalidOperationException">If there is no task left in this tour</exception>
         * </summary>
         */
        internal void SkipSection()
        {
            CheckTourNotFinished();

            // For the remaining tasks in the section
            for (int task = taskIndex; task < ActiveSection.Tasks.Count; task++)
            {
                ActiveSection.Tasks[task].SkipTask();
                ActiveSection.Tasks[task].State = AbstractTourTask.TourTaskState.COMPLETED;
            }

            taskIndex = ActiveSection.Tasks.Count;
            // Because of setting taskIndex, SelectNextTask() will chose the next section
            SelectNextTask();
        }

        /**
         * <summary>
         * This method skips the current task. If there are reaming tasks in the tour, the next task will be choosen.
         * <exception cref="InvalidOperationException">If there is no task left in this tour</exception>
         * </summary>
         */
        internal void SkipTask()
        {
            CheckTourNotFinished();

            ActiveTask.SkipTask();
            ActiveTask.State = AbstractTourTask.TourTaskState.COMPLETED;
            SelectNextTask();
        }

        private void SelectNextTask()
        {
            taskIndex++;
            if (taskIndex >= ActiveSection.Tasks.Count) // Section finished
            {
                sectionIndex++;
                if (sectionIndex < Sections.Count) // More sections left
                {
                    Debug.Log("Completed Section: " + ActiveSection.Name);
                    ActiveSection = Sections[sectionIndex];
                    Debug.Log("Next Section: " + ActiveSection.Name);

                    taskIndex = 0;
                }
                else // Finished with the complete tour
                {
                    Debug.Log("- Tour completed -");
                    widget.UpdateTask(null);
                    ActiveSection = null;
                    ActiveTask = null;
                    return;
                }
            }

            ActiveTask = ActiveSection.Tasks[taskIndex];
            ActiveTask.State = AbstractTourTask.TourTaskState.ACTIVE;
            widget.UpdateTask(ActiveTask);
            textPlacer.drawSectionBoard();

            Debug.Log("Selected next task: " + ActiveTask.Name);
        }

        private void CheckTourNotFinished()
        {
            if (ActiveSection == null)
            {
                throw new InvalidOperationException("Tour has already been finished");
            }
        }
    }
}
