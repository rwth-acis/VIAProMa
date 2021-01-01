using System;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.SaveLoadSystem.Core
{
    [Serializable]
    public class SaveData
    {
        [SerializeField] private int appVersion;
        [SerializeField] private List<SerializedObject> data;

        public int AppVersion
        {
            get
            {
                return appVersion;
            }

            set
            {
                appVersion = value;
            }
        }

        public List<SerializedObject> Data
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
            }
        }

        public SaveData(int appVersion)
        {
            this.appVersion = appVersion;
        }
    }
}