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

        internal override bool IsTaskDone()
        {
            throw new InvalidOperationException("Cannot call IsTaskDone() on an unlinked tour task");
        }

        internal override void SkipTask()
        {
            throw new InvalidOperationException("Cannot call SkipTask() on an unlinked tour task");
        }

        internal override void OnTaskActivation(GameObject indicatorArrow) {

        }
        internal override void OnTaskDeactivation(GameObject indicatorArrow) { }
    }
}