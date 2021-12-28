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
        /**
         * <summary>
         * The identifier of this task used to reference the task by the configuration file
         * </summary>
         */
        [SerializeField] private string _id;
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
        /**
         * <summary>
         * The name of this task. Set by the configuration file
         * </summary>
         */
        public string Name { get; internal set; }
        /**
         * <summary>
         * The description of this task. Set by the configuration file
         * </summary>
         */
        public string Description { get; internal set; }
        /**
         * <summary>
         * Denotes whether the task is active or not. Only one task is active at a time. As long as the task is not active, the 
         * task might block input by the user regarding this task.
         * </summary>
         */
        public bool Active { get; internal set; }
        /**
         * <summary>
         * The description of the action which is performed by the user. Set by the configuration file
         * </summary>
         */
        public string ActionName { get; internal set; }

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
        
        /**
         * <summary>
         * This method is called when the task should be skipped. The underlying task is responsible for 
         * performing the task the user would have done by itself.
         * </summary>
         */
        internal abstract void SkipTask();
    }
}
