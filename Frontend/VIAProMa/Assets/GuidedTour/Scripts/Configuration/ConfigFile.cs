using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GuidedTour
{
    public class ConfigFile
    {
        private readonly string fileName;
        private ConfigRootEntry root;

        public ConfigFile(string fileName)
        {
            this.fileName = fileName;

            TourSectionEntry e = new TourSectionEntry();
            e.sectionName = "The section name";
            TaskEntry t = new TaskEntry();
            t.taskName = "The task name";
            e.tasks.Add(t);

            root = new ConfigRootEntry();
            root.sections.Add(e);
            root.sections.Add(e);
            WriteConfig();
        }

        public void LoadConfig()
        {

        }

        public void WriteConfig()
        {
            StreamWriter writer = new StreamWriter(fileName);
            writer.Write(JsonUtility.ToJson(root, true));
            writer.Close();
        }
    }

    [System.Serializable]
    class ConfigRootEntry
    {
        public List<TourSectionEntry> sections = new List<TourSectionEntry>();
    }

    [System.Serializable]
    class TourSectionEntry
    {
        public string sectionName;
        public List<TaskEntry> tasks = new List<TaskEntry>();
    }

    [System.Serializable]
    class TaskEntry
    {
        public string taskName;
    }
}


