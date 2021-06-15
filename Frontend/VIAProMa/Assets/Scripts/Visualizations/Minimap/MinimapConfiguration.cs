using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i5.VIAProMa.Visualizations.ColorConfigWindow;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.Minimap
{
    /// <summary>
    /// Configuration window for the minimap
    /// </summary>
    public class MinimapConfiguration : ConfigurationWindow
    {
        [Tooltip("The UI which handles the minimap")]
        [SerializeField] private ConfigurationIssueSelectionUI issueSelectionUI;

        public override bool WindowEnabled
        {
            get => base.WindowEnabled;
            set
            {
                base.WindowEnabled = value;
                issueSelectionUI.UIEnabled = value;
            }
        }

        protected override void Awake()
        {

        }

        public override void Open()
        {

        }
    }
}
