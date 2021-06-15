using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i5.VIAProMa.Visualizations.Minimap;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.Minimap
{
    public class MinimapController : MonoBehaviour, IMinimapVisuals
    {
        private BoxCollider boundBoxCollider;

        [Tooltip("Reference to the title of the minimap")]
        private TextLabel textLabel;

        public string Title
        {
            get => textLabel.Text;
            set => textLabel.Text = value;
        }
        public Visualization[] MinimapItems { get; set; }
        public Visualization[] HighlightedItems { get; set; }

    }
}
