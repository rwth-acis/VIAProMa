using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuidedTour
{
    [System.Serializable]
    public class ConfigRootEntry
    {
        public List<TourSectionEntry> sections = new List<TourSectionEntry>();
    }

    [System.Serializable]
    public class TourSectionEntry
    {
        public string sectionName;
        public List<TaskEntry> tasks = new List<TaskEntry>();
    }

    [System.Serializable]
    public class TaskEntry
    {
        public string id;
        public string name;
        public string description;
    }
}