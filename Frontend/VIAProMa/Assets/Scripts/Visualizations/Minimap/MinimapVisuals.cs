using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i5.VIAProMa.Visualizations;
using UnityEditor.IMGUI.Controls;

namespace i5.VIAProMa.Visualizations.Minimap
{
    /// <summary>
    /// Interface class which describes minimap visuals:
    /// Shows which items are on the minimap and if they are highlighted as important
    /// </summary>
    public class MinimapVisuals : IVisualizationVisualController
    {
        /// <summary>
        /// Gets/sets the items displayed on the minimap
        /// </summary>
        Visualization[] MinimapItems { get; set; }

        /// <summary>
        /// Gets/sers the highlighted items
        /// </summary>
        Visualization[] HighlightedItems { get; set; }

        public string Title { get; set; }

        private void Awake()
        {

        }

        private void UpdateVisuals()
        {

        }
    }
}
