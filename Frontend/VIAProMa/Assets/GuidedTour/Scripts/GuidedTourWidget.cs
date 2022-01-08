﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using TMPro;

namespace GuidedTour
{

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

        internal void UpdateTask(AbstractTourTask task)
        {
            currentTask = task; 
            if (task == null)
            {
                headline.text = "Completed Tour";
                hintText.text = "You are finished!";
                return;
            }

            headline.text = task.Name;
            hintText.text = task.Description;

            if (task.GetType() == typeof(SimpleTourTask))
            { 
                continueButton.SetActive(true);
            }
            else
            {
                continueButton.SetActive(false);
            }
        }

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

        public void OnContinueClicked() 
        {
            SimpleTourTask sts = (SimpleTourTask) currentTask;
            sts.OnAction(); // --> Bind on button (task.ActionName as text)
        }

    }
}