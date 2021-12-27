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


        void Start()
        {

        }

        void Update()
        {

        }

        internal override bool IsTaskDone()
        {
            return true; // To Do: Listen for "continue" button
        }
    }
}