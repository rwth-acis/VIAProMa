using System;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.SaveLoadSystem.Core
{
    [Serializable]
    public class SaveData
    {
        [SerializeField] private int appVersion;
        [SerializeField] private string projectVersion;
        [SerializeField] private List<SerializedObject> data;

        public int AppVersion
        {
            get { return appVersion; }
            set { appVersion = value; }
        }

        public List<SerializedObject> Data
        {
            get { return data; }
            set { data = value; }
        }

        public string ProjectVersion
        {
            get { return projectVersion; }
            set { projectVersion = value; }
        }

        public SaveData(int appVersion, Guid projectVersion)
        {
            this.appVersion = appVersion;
            this.projectVersion = projectVersion.ToString();
        }
    }
}