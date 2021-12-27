using System;
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

            configFile.LoadConfig();
            GuidedTourUtils.LinkTasks(Sections, configFile.Root);

            foreach(TourSection s in Sections)
            {
                Debug.Log("== Section: " + s.Name);
                foreach(AbstractTourTask task in s.Tasks)
                {
                    Debug.Log("  = Task: " + task.Name);
                }
            }
        }

        void Update()
        {

        }
    }
}
