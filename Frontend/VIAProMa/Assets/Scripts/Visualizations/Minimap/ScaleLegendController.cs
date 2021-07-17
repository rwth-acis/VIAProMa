using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace i5.VIAProMa.Visualizations.Minimap
{
    /// <summary>
    /// Manages the width of the legends and updates the
    /// scale ratio text accordingly
    /// </summary>
    class ScaleLegendController : MonoBehaviour
    {
        [SerializeField] private TextMeshPro scaleRatioText;
        [SerializeField] private float scaleRatio;
        [SerializeField] private Transform scaleStick;

        public float Scale
        {
            set
            {
                scaleRatio = value;
                UpdateScaleRatioText();
                UpdateScaleStick();
            }
            get => scaleRatio;
        }

        private void UpdateScaleRatioText()
        {
            scaleRatioText.SetText(String.Format("{1}x", scaleRatio));
        }

        private void UpdateScaleStick()
        {

        }
    }
}
