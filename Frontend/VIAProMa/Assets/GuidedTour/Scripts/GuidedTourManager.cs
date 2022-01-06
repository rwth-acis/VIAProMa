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
                SelectNextTask();
            }
        }

        internal void SkipSection()
        {
            if (ActiveSection == null)
            {
                throw new Exception("Tour has already been finished");
            }

            // For the remaining tasks in the section
            for (int task = taskIndex; task < ActiveSection.Tasks.Count; task++)
            {
                ActiveSection.Tasks[task].SkipTask();
            }

            taskIndex = ActiveSection.Tasks.Count;
            // Because of setting taskIndex, SelectNextTask() will chose the next section
            // Since ActiveTask has not been touched, SelectNextTask() will also deactivate it
            SelectNextTask();
        }

        private void SelectNextTask()
        {
            if (ActiveTask != null)
            {
                ActiveTask.Active = false;
                // To Do: Disable visual component for ActiveTask
            }

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
                    ActiveSection = null;
                    ActiveTask = null;
                    return;
                }
            }

            ActiveTask = ActiveSection.Tasks[taskIndex];
            ActiveTask.Active = true;
            // To Do: Enable visual component for ActiveTask

            Debug.Log("Selected next task: " + ActiveTask.Name);
        }
    }
}
