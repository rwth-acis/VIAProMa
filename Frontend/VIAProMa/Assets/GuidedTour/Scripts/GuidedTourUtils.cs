using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuidedTour
{
    public class GuidedTourUtils
    {
        /**
         * <summary>
         * Creates a <cref>UnlinkedTourTask</cref> for every task in the configuration file. This tasks will later 
         * be mapped to the actual task instances.
         * </summary>
         * <param name="sections">The list of tour section to add the tasks to</param>
         * <param name="root">The configuration file root object</param>
         */
        public static void CreateTasks(List<TourSection> sections, ConfigRootEntry root)
        {
            foreach (TourSectionEntry s in root.sections)
            {
                TourSection section = new TourSection(s.sectionName);
                sections.Add(section);
                foreach (TaskEntry t in s.tasks)
                {
                    AbstractTourTask task = new UnlinkedTourTask(t);
                    task.Id = t.id;
                    SetAttributes(t, task);
                    section.Tasks.Add(task);
                }
            }
        }

        /**
         * <summary>
         * This method is called by the GuidedTourManager if an <cref>UnlinkedTourTask</cref> is next in the tour. 
         * The method will search the scene for the real task.
         * </summary>
         * <param name="task">The placeholder task</param>
         * <returns>The true task or null if no such task with that id exists</returns>
         */
        public static AbstractTourTask LinkTask(UnlinkedTourTask task)
        {
            foreach (AbstractTourTask t in Resources.FindObjectsOfTypeAll<AbstractTourTask>())
            {
                if (t.Id.Equals(task.Id) && t.GetType() != typeof(UnlinkedTourTask))
                {
                    SetAttributes(task.Entry, t);
                    return t;
                }
            }

            return null;
        }

        private static Dictionary<string, AbstractTourTask> MapTasksById()
        {
            Dictionary<string, AbstractTourTask> map = new Dictionary<string, AbstractTourTask>();
            foreach (AbstractTourTask task in Resources.FindObjectsOfTypeAll<AbstractTourTask>())
            {
                map.Add(task.Id, task);
            }

            return map;
        }

        private static void SetAttributes(TaskEntry t, AbstractTourTask task)
        {
            task.Name = t.name;
            task.Description = t.description;
            task.ActionName = t.action;
        }
    }
}
