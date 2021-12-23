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
        [SerializeField] public string Name { get; internal set; }
        [SerializeField] public string Description { get; internal set; }
        [SerializeField] public bool Active { get; internal set; }

        internal AbstractTourTask(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        void Start()
        {

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
