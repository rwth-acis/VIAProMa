using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuidedTour
{
    /**
     * <summary>
     * SimpleTourTasks repents a TourTasks which contains a name, a description and has a simple "continue" button 
     * to advance to the next task.
     * </summary>
     */
    public class SimpleTourTask : AbstractTourTask
    {
        private bool done;

        /**
         * <summary>
         * This method is called by the VisualTourComponent when the button is pressed.
         * </summary>
         */
        public void OnAction()
        {
            manager.OnTaskDone();
        }

        internal override void SkipTask()
        {
            // Nothing to do here
        }

    }
}