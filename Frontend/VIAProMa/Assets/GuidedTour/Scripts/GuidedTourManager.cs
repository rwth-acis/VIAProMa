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

        [SerializeField] private SectionBoard sectionBoard;
        [SerializeField] private string language = "en";
        [SerializeField] private float notificationTime = 5;
        [SerializeField] private GuidedTourWidget widget;
        [SerializeField] private GameObject indicatorArrow;
        [SerializeField] private NotificationWidget notifications;

        private ConfigFile configFile = new ConfigFile("Assets/GuidedTour/Configuration/GuidedTour.json");
        private LanguageFile languageFile = new LanguageFile("Assets/GuidedTour/Configuration/Languages.json");
        private int sectionIndex = 0;
        private int taskIndex = -1;

        private void Awake()
        {
            Sections = new List<TourSection>();

            languageFile.LoadConfig();
            configFile.LoadConfig();
            GuidedTourUtils.CreateTasks(Sections, configFile.Root);

            ActiveSection = Sections[0];
            SelectNextTask();
        }

        void Update()
        {
            if (ActiveTask == null)
                return;

            ActiveTask.State = AbstractTourTask.TourTaskState.ACTIVE;
            if (ActiveTask.IsTaskDone())
            {
                notifications.ShowMessage("Completed Task " + languageFile.GetTranslation(ActiveTask.Name, language), notificationTime);
                ActiveTask.State = AbstractTourTask.TourTaskState.COMPLETED;
                ActiveTask.OnTaskDeactivation(indicatorArrow);
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
            ActiveTask.OnTaskDeactivation(indicatorArrow);
            SelectNextTask();
        }

        private void SelectNextTask()
        {
            sectionBoard.currentTaskCount++;
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
                    sectionBoard.progressBar.PercentageDone = 1;
                    widget.UpdateTask(null, languageFile, language);
                    ActiveSection = null;
                    ActiveTask = null;
                    return;
                }
            }

            ActiveTask = ActiveSection.Tasks[taskIndex];
            if (ActiveTask.GetType() == typeof(UnlinkedTourTask))
            {
                string id = ActiveTask.Id;
                ActiveTask = ActiveSection.Tasks[taskIndex] = GuidedTourUtils.LinkTask((UnlinkedTourTask)ActiveTask);
                if (ActiveTask == null)
                {
                    throw new Exception("No task with the id \"" + id +"\" is in the scene");
                }
            }

            ActiveTask.State = AbstractTourTask.TourTaskState.ACTIVE;
            sectionBoard.updateSectionBoard();
            widget.UpdateTask(ActiveTask, languageFile, language);
            ActiveTask.OnTaskActivation(indicatorArrow);

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
