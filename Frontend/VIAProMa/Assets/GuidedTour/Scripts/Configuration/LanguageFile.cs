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
        private readonly TextAsset file;
        private readonly Dictionary<string, string> map = new Dictionary<string, string>();
        private LanguageRoot root;

        public LanguageFile(TextAsset file)
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
            root = JsonUtility.FromJson<LanguageRoot>(file.text);

            Map();
        }

        // Map the identifiers to their translation for lookup speed
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

        /**
         * <summary>Get the translation for an identifier</summary>
         * <param name="name">The identifier for the translation</param>
         * <param name="language">The language tag, as it is used in the language file</param>
         */
        public string GetTranslation(string name, string language)
        {
            if (root == null)
            {
                throw new InvalidOperationException("Language file has not been loaded yet");
            }

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


