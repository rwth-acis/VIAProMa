using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i5.Toolkit.Core.Utilities;

namespace i5.VIAProMa.Visualizations.Minimap
{
    /// <summary>
    /// Abstractor visualization logic for minimap
    /// </summary>
    public class Minimap : Visualization
    {
        protected override void Awake()
        {
            base.Awake();
            Title = "Sample Minimap";
            visualController = GetComponent<MinimapController>();
        }
    }
}
