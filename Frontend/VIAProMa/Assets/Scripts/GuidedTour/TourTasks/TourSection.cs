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
        /**
         * <summary>The name of the current section</summary>
         */ 
        public string Name { get; internal set; }
        /**
         * <summary>An ordered list with the tasks in the section</summary>
         */
        public List<AbstractTourTask> Tasks { get; private set; } 

        internal TourSection(string name)
        {
            this.Name = name;
            Tasks = new List<AbstractTourTask>();
        }
    }
}
