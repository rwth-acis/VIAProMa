using i5.VIAProMa.Utilities;
using i5.VIAProMa.Visualizations.ColorConfigWindow;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.KanbanBoard
{
    [RequireComponent(typeof(ConfigurationIssueSelectionUI))]
    public class KanbanBoardConfigurationWindow : ConfigurationWindow
    {
        [SerializeField] private ConfigurationColorChooser colorChooser;

        private ConfigurationIssueSelectionUI issueSelection;

        public override bool WindowEnabled
        {
            get => base.WindowEnabled;
            set
            {
                base.WindowEnabled = value;
                issueSelection.UIEnabled = value;
                colorChooser.UIEnabled = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (colorChooser == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(colorChooser));
            }

            colorChooser.Setup(visualization);
            issueSelection = GetComponent<ConfigurationIssueSelectionUI>();
            issueSelection.Setup(visualization);
        }
    }
}