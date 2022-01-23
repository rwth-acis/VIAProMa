using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GuidedTour
{
    /**
     * <summary>
     * The LanguageFile translate the strings in the GuidedTour configuration 
     * </summary>
     */
    public class LanguageFile
    {
        private readonly string fileName;
        private readonly Dictionary<string, string> map = new Dictionary<string, string>();
        private LanguageRoot root;

        public LanguageFile(string fileName)
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
            root = JsonUtility.FromJson<LanguageRoot>(reader.ReadToEnd());
            reader.Close();

            Map();
        }

        private void Map()
        {
            if (root.defaultLanguage == null)
            {
                throw new Exception("The default language cannot be null");
            }

            foreach (LanguageEntry e in root.entries)
            {
                map.Add(e.name, e.translation);
            }
        }

        public string GetTranslation(string name, string language)
        {
            string str;
            map.TryGetValue(name + ":" + language, out str);
            if (str == null)
            {
                map.TryGetValue(name + ":" + root.defaultLanguage, out str);
                if (str == null)
                    throw new Exception("No translation for name " + name);
            }

            return str;
        }
    }

}


