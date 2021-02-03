using UnityEngine;

namespace i5.VIAProMa.Visualizations.Haftnotizen
{
    /// <summary>
    /// Logic of the Kanban Board Column
    /// </summary>
    [RequireComponent(typeof(HaftnotizenVisualController))]
    public class Haftnotizen : Visualization
    {
        /// <summary>
        /// Initializes the component
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            visualController = GetComponent<HaftnotizenVisualController>();
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
            //((HaftnotizenVisualController)visualController).Text = ContentProvider.Text;
            base.UpdateView();
        }
    }
}