﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using TMPro;

namespace GuidedTour
{
    /**
     * <summary>
     * The GuidedTourWidget connects the visual component widget with the texts and buttons visible. It changes the texts with information from the GuidedTourManager
     * and provides information about the visibility of the continue button and the widget itself.
     * </summary>
     */

    public class GuidedTourWidget : MonoBehaviour
    {
      
        private AbstractTourTask currentTask = null;
        public GameObject widget;
        public TextMeshPro headline;
        public TextMeshPro hintText;
        public GameObject continueButton;

        public delegate void WidgetVisibleChangedAction(bool IsVisible);
        public static event WidgetVisibleChangedAction OnWidgetVisibleChanged;


        public void Start()
        {
            headline.text = "Hallo";
        }

        /**
     * <summary>
     * The UpdateTask method, updates the texts that are visible on the widget with the current task. The information about the task is provided by 
     * the GuidedTourManager that calls this function. It also sets the continue button, if it is necessary by identifying the type of the task. 
     * </summary>
     */

        internal void UpdateTask(AbstractTourTask task)
        {
            //check if you reached the end of the tour
            currentTask = task; 
            if (task == null)
            {
                headline.text = "Completed Tour";
                hintText.text = "You are finished!";
                return;
            }

            //set the texts with the information of the current task
            headline.text = task.Name;
            hintText.text = task.Description;

            //check the type of the task to identify if the continue button is needed
            if (task.GetType() == typeof(SimpleTourTask))
            { 
                continueButton.SetActive(true);
            }
            else
            {
                continueButton.SetActive(false);
            }
        }

        /**
     * <summary>
     * The WidgetVisible method provides a get and set method for the visability of the widget.
     * </summary>
     */

        public bool WidgetVisible
        {
            get { return widget.activeSelf; }
            set 
            {
                widget.SetActive(value);
                // Throw Event OnWidgetVisibleChanged (if there is a subscriber)
                if (OnWidgetVisibleChanged != null)
                    OnWidgetVisibleChanged(value);
            }
        }
        /**
     * <summary>
     * The OnContinueClicked method defines the behavior of the continue button. If it was clicked, it moves on to the next task and considers the current 
     * task as finished.
     * </summary>
     */

        public void OnContinueClicked() 
        {
            SimpleTourTask sts = (SimpleTourTask) currentTask;
            sts.OnAction(); // --> Bind on button 
        }

    }
}