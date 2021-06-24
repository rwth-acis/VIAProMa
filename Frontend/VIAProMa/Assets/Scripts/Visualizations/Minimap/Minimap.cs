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
        private MinimapVisuals minimapVisuals;

        protected override void Awake()
        {
            base.Awake();
            //minimapVisuals = GetComponent<MinimapVisuals>();
            //if (minimapVisuals is null)
            //{
            //    SpecialDebugMessages.LogComponentNotFoundError(this, nameof(MinimapVisuals), gameObject);
            //}

            ContentProvider = new SingleIssuesProvider();
            Title = "";
        }
    }
}
