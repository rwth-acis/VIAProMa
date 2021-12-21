using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuidedTour
{
    /**
     * <summary>
     * A TourSections contains a list of tasks. TourSections are used to split the tour into smaller sections and skip hole sections.
     * </summary>
     */
    public class TourSection : MonoBehaviour
    {
        public string Name { get; internal set; }
        public List<AbstractTourTask> Tasks { get; private set; } 

        internal TourSection(string name)
        {
            this.Name = name;
        }

        // Start is called before the first frame update
        void Start()
        {
            Tasks = new List<AbstractTourTask>();

            // Debug: Add 2 Tasks per TourSection as long as config logic is not implemented to load sections and tasks from file
            Tasks.Add(new SimpleTourTask("Test Task 1", "The description for test task 1"));
            Tasks.Add(new SimpleTourTask("Test Task 2", "The description for test task 2"));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
