using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuidedTour
{
    /**
     * <summary>
     * UnlinkedTourTask is a placeholder for a task which will be loaded when the tasks gets activated.
     * </summary>
     */
    public class UnlinkedTourTask : AbstractTourTask
    {
        internal TaskEntry Entry { get; private set; }

        internal UnlinkedTourTask(TaskEntry entry)
        {
            Entry = entry;
        }

        /*
         * This point should never be reached because if the ActiveTask becomes an 
         * UnlinkedTourTask the GuidedTourManager will relink it to the real task
         */
        internal override void SkipTask()
        {
            throw new InvalidOperationException("Cannot call SkipTask() on an unlinked tour task");
        }
    }
}