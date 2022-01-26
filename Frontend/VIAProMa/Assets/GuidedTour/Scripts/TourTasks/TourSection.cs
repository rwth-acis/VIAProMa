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
    public class TourSection
    {
        public string Name { get; internal set; }
        public List<AbstractTourTask> Tasks { get; private set; } 

        internal TourSection(string name)
        {
            this.Name = name;
            Tasks = new List<AbstractTourTask>();
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
