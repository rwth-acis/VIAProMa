using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace ConfigLoadData
{

    public class LoadData : MonoBehaviour
    {
        /// <summary>
        /// Method to laod Data from JSON Config file
        /// <returns>an object which contains config data as variables.</returns>
        /// </summary>
        public static Config LoadConfig()
        {
            string configFilePath = System.IO.Path.Combine(Application.streamingAssetsPath,"DeepLink","config.json");
            string configData = System.IO.File.ReadAllText(configFilePath);
            Config config = JsonUtility.FromJson<Config>(configData);
            return config;
        }

        [System.Serializable]
        public class Config
        {
            public string scheme;
            public string invitePath;
            public string paramName_roomName;
        }
    }
}