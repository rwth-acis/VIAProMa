using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuidedTour
{
    /**
     * <summary>
     * AbstractTourTask is the abstraction for all specific TourTaks. Every TourTasks has a name, a description 
     * and a state (active or inactive).
     * To Do:
     *  - Every Tour Task should have a reference to its graphical representation and vice versa
     * </summary>
     */
    public abstract class AbstractTourTask : MonoBehaviour
    {
        [SerializeField] private string _id;
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public bool Active { get; internal set; }

        void Start()
        {
            Id = _id;
        }

        /**
         * <summary>
         * This method is called on every frame by the TourManger whilst this task is active. As soon as this method returns true, 
         * this task is over and the TourManager selects the next tasks.
         * </summary>
         */
        internal abstract bool IsTaskDone();
    }
}
