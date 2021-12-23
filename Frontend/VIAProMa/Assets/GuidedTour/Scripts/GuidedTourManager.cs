using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuidedTour
{
    /**
     * <summary>
     * The GuidedTourManager holds the TourSection which hold the TourTasks. The Manager is responsible for 
     * managing the state of the tasks and for managing the config file(s).
     * </summary>
     */
    public class GuidedTourManager : MonoBehaviour
    {
        public List<TourSection> Sections { get; private set; }
        private ConfigFile configFile = new ConfigFile("Assets/GuidedTour/Configuration/GuidedTour.json");

        void Start()
        {
            Sections = new List<TourSection>();
            // Debug: Add 2 Tour Sections as long as the config file logic is not implemented 
            Sections.Add(new TourSection("Section 1"));
            Sections.Add(new TourSection("Section 2"));
        }

        void Update()
        {

        }
    }
}
