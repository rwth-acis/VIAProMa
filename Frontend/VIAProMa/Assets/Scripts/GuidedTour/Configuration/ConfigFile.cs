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
        private readonly TextAsset file;

        /**
         * <summary>The root of the json file. Available after Load() has been called.</summary>
         */ 
        public ConfigRootEntry Root { get; private set; }

        public ConfigFile(TextAsset file)
        {
            this.file = file;
        }

        /**
         * <summary>
         * Load the JSON file specified in the constructor
         * </summary>
         */
        public void LoadConfig()
        {
            Root = JsonUtility.FromJson<ConfigRootEntry>(file.text);
        }

    }

}


