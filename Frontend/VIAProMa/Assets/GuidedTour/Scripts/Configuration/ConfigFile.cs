using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GuidedTour
{
    /**
     * <summary>
     * The ConfigFile Class holds the logic to read JSON configuration file which is used to configure a guided tour.
     * </summary>
     */
    public class ConfigFile
    {
        private readonly string fileName;
        public ConfigRootEntry Root { get; private set; }

        public ConfigFile(string fileName)
        {
            this.fileName = fileName;
        }

        /**
         * <summary>
         * Load the JSON file specified in the constructor
         * </summary>
         */
        public void LoadConfig()
        {
            StreamReader reader = new StreamReader(fileName);
            Root = JsonUtility.FromJson<ConfigRootEntry>(reader.ReadToEnd());
            reader.Close();
        }

        /**
         * <summary>
         * Override the file with the current data of the root. Used for debugging purposes only
         * </summary>
         */
        private void WriteConfig()
        {
            StreamWriter writer = new StreamWriter(fileName);
            writer.Write(JsonUtility.ToJson(Root, true));
            writer.Close();
        }
    }

}


