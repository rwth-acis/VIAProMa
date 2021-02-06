using UnityEngine;

namespace i5.VIAProMa.Visualizations.StickyNote
{
    /// <summary>
    /// Logic of the Kanban Board Column
    /// </summary>
    [RequireComponent(typeof(StickyNoteVisualController))]
    public class StickyNote : Visualization
    {
        /// <summary>
        /// Initializes the component
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            visualController = GetComponent<StickyNoteVisualController>();
        }

        /// <summary>
        /// Sets the issues provider
        /// </summary>
        private void Start()
        {
            ContentProvider = new SingleIssuesProvider();
        }

        /// <summary>
        /// Updates the display: transfers the issues to the visual controller which displays them
        /// </summary>
        public override void UpdateView()
        {
            base.UpdateView();
        }
    }
}