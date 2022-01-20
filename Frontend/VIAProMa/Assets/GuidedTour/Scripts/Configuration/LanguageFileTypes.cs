using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuidedTour
{
    [System.Serializable]
    public class LanguageRoot
    {
        public string defaultLanguage;
        public List<LanguageEntry> entries;
    }

    [System.Serializable]
    public class LanguageEntry
    {
        public string name;
        public string translation;
    }

}