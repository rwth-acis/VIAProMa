using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuidedTour
{
    /**
     * <summary>
     * The GuidedTourManager holds the TourSection which hold the TourTasks. The Manager is responsible for 
     * managing the state of the tasks and for managing the configuration file(s).
     * </summary>
     */
    public class GuidedTourManager : MonoBehaviour
    {
        /**
         * <summary>The current active task or null if the tour is finished</summary>
         */
        public AbstractTourTask ActiveTask { get; private set; }
        /**
         * <summary>The current active tour section or null if the tour is finished</summary>
         */
        public TourSection ActiveSection { get; private set; }
        /**
         * <summary>The list of sections in the tor</summary>
         */
        public List<TourSection> Sections { get; private set; }

        [Header("References")]
        [Tooltip("The reference to the section board")]
        [SerializeField]
        private SectionBoard sectionBoard;

        [Tooltip("The reference to the widget")]
        [SerializeField]
        private GuidedTourWidget widget;

        [Tooltip("The reference to the indicator arrow")]
        [SerializeField]
        private GameObject indicatorArrow;

        [Tooltip("The reference to the notification widget")]
        [SerializeField]
        internal NotificationWidget notifications;

        [Header("Settings")]
        [Tooltip("The current language (same identifier for the language as in the language file)")]
        [SerializeField]
        private string language = "en";
        [Tooltip("The time the notifications will be shown in seconds")]
        [SerializeField]
        internal float notificationTime = 5;

        [Header("Configuration files")]
        [SerializeField]
        private TextAsset configFileLocation;
        [SerializeField]
        private TextAsset languageFileLocation;

        private ConfigFile configFile;
        private LanguageFile languageFile;
        internal int sectionIndex = 0;
        internal int taskIndex = -1;
        internal int totalTasksDone = 0;

        private void Awake()
        {
            configFile = new ConfigFile(configFileLocation);
            languageFile = new LanguageFile(languageFileLocation);

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

            ActiveTask.State = AbstractTourTask.TourTaskState.ACTIVE; // remove
        }

        /**
         * <summary>Called by active task when the task is done</summary>
         */ 
        internal void OnTaskDone()
        {
            notifications.ShowMessage("Completed Task " + languageFile.GetTranslation(ActiveTask.Name, language), notificationTime);
            notifications.PlaySuccessSound();
            OnTaskCompleted();
            SelectNextTask();
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

            StartCoroutine(SkipSectionCoroutine());
        }

        // As coroutine to yield in between to let animations play
        private IEnumerator SkipSectionCoroutine()
        {
            // Make time 20x faster
            Time.timeScale = 20;
            // For the remaining tasks in the section
            for (int task = taskIndex; task < ActiveSection.Tasks.Count; task++)
            {
                ActiveTask = ActiveSection.Tasks[task];
                LinkCurrentTask();
                OnTaskUpdate();

                yield return new WaitForSeconds(5);

                ActiveTask.SkipTask();
                OnTaskCompleted();
            }

            // Reset time
            Time.timeScale = 1;

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
            OnTaskCompleted();
            SelectNextTask();
        }

        // Selects the next task and sets ActiveTask and ActiveSection
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

                    ActiveSection = null;
                    ActiveTask = null;
                    OnTaskUpdate();
                    return;
                }
            }

            ActiveTask = ActiveSection.Tasks[taskIndex];
            LinkCurrentTask();
            OnTaskUpdate();


            Debug.Log("Selected next task: " + ActiveTask.Name);
        }

        // Called when a new task is selected. ActiveTask is now the new task
        private void OnTaskUpdate()
        {
            if (ActiveTask != null)
            {
                sectionBoard.updateSectionBoard();
                widget.UpdateTask(ActiveTask, languageFile, language);
                ActiveTask.OnTaskLinked(this);
                ActiveTask.OnTaskActivation(indicatorArrow);
                ActiveTask.State = AbstractTourTask.TourTaskState.ACTIVE;
            }
            else
            {
                sectionBoard.progressBar.PercentageDone = 1;
                widget.UpdateTask(null, languageFile, language);
            }
        }

        // Called when a task is finished (also when it is skipped). ActiveTask is the old task
        private void OnTaskCompleted()
        {
            totalTasksDone++;
            ActiveTask.State = AbstractTourTask.TourTaskState.COMPLETED;
            ActiveTask.OnTaskDeactivation(indicatorArrow);
        }

        // Links ActiveTask with the real task in the scene, if it is an UnlinkedTourTask
        private void LinkCurrentTask()
        {
            if (ActiveTask.GetType() == typeof(UnlinkedTourTask))
            {
                string id = ActiveTask.Id;
                ActiveTask = ActiveSection.Tasks[taskIndex] = GuidedTourUtils.LinkTask((UnlinkedTourTask)ActiveTask);
                if (ActiveTask == null)
                {
                    throw new Exception("No task with the id \"" + id + "\" is in the scene");
                }
            }
        }

        // If the tour is finished, the method throws an InvalidOperationException
        private void CheckTourNotFinished()
        {
            if (ActiveSection == null)
            {
                throw new InvalidOperationException("Tour has already been finished");
            }
        }
    }
}
