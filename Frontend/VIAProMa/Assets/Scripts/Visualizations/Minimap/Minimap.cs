using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i5.Toolkit.Core.Utilities;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.Minimap
{
    /// <summary>
    /// Abstractor visualization logic for minimap
    /// </summary>
    [RequireComponent(typeof(MinimapController))]
    public class Minimap : Visualization
    {
        /// <summary>
        /// Initialize the minimap component
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            Title = "Sample Minimap";
            visualController = GetComponent<MinimapController>();
        }

        public override void UpdateView()
        {
            base.UpdateView();
        }
    }
}
