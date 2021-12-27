using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuidedTour
{
    public class GuidedTourUtils
    {
        public static void LinkTasks(List<TourSection> sections, ConfigRootEntry root)
        {
            Dictionary<string, AbstractTourTask> taskMap = MapTasksById();

            foreach (TourSectionEntry s in root.sections)
            {
                TourSection section = new TourSection(s.sectionName);
                sections.Add(section);
                foreach (TaskEntry t in s.tasks)
                {
                    AbstractTourTask task;
                    if (taskMap.TryGetValue(t.id, out task))
                    {
                        section.Tasks.Add(task);
                        SetAttributes(t, task);
                    }
                    else
                    {
                        throw new Exception("Unknown identifier \"" + t.id + "\" for a task");
                    }
                }
            }
        }

        private static Dictionary<string, AbstractTourTask> MapTasksById()
        {
            Dictionary<string, AbstractTourTask> map = new Dictionary<string, AbstractTourTask>();

            AbstractTourTask[] tasks = Resources.FindObjectsOfTypeAll<AbstractTourTask>();
            foreach (AbstractTourTask task in tasks)
            {
                map.Add(task.Id, task);
            }

            return map;
        }

        private static void SetAttributes(TaskEntry t, AbstractTourTask task)
        {
            task.Name = t.name;
            task.Description = t.description;
        }
    }
}
