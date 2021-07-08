using UnityEngine;

namespace i5.VIAProMa.Visualizations.KanbanBoard
{
    /// <summary>
    /// Logic of the Kanban Board Column
    /// </summary>
    [RequireComponent(typeof(KanbanBoardColumnVisualController))]
    public class KanbanBoardColumn : Visualization
    {
        /// <summary>
        /// Initializes the component
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            visualController = GetComponent<KanbanBoardColumnVisualController>();
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
            ((KanbanBoardColumnVisualController)visualController).Issues = ContentProvider.Issues;
            base.UpdateView();
        }
    }
}