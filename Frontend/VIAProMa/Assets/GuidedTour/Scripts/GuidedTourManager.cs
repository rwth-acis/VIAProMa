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
        /**
         * The current active task
         */
        public AbstractTourTask ActiveTask { get; private set; }
        public TourSection ActiveSection { get; private set; }
        public List<TourSection> Sections { get; private set; }

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
        private NotificationWidget notifications;

        [Tooltip("The current language (same identifier for the language as in the language file)")]
        [SerializeField]
        private string language = "en";
        [Tooltip("The reference to the section board")]
        [SerializeField]
        private float notificationTime = 5;

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

            ActiveTask.State = AbstractTourTask.TourTaskState.ACTIVE;
            if (ActiveTask.IsTaskDone())
            {
                notifications.ShowMessage("Completed Task " + languageFile.GetTranslation(ActiveTask.Name, language), notificationTime);
                OnTaskCompleted();
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

            StartCoroutine(SkipSectionCoroutine());
        }

        private IEnumerator SkipSectionCoroutine()
        {
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

        private void OnTaskUpdate()
        {
            if (ActiveTask != null)
            {
                sectionBoard.updateSectionBoard();
                widget.UpdateTask(ActiveTask, languageFile, language);
                ActiveTask.OnTaskActivation(indicatorArrow);
                ActiveTask.State = AbstractTourTask.TourTaskState.ACTIVE;
            }
            else
            {
                sectionBoard.progressBar.PercentageDone = 1;
                widget.UpdateTask(null, languageFile, language);
            }
        }

        private void OnTaskCompleted()
        {
            totalTasksDone++;
            ActiveTask.State = AbstractTourTask.TourTaskState.COMPLETED;
            ActiveTask.OnTaskDeactivation(indicatorArrow);
        }

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

        private void CheckTourNotFinished()
        {
            if (ActiveSection == null)
            {
                throw new InvalidOperationException("Tour has already been finished");
            }
        }
    }
}
