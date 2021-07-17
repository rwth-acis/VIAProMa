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
        [SerializeField] private Transform background;
        [SerializeField] private Transform scaleStick;
        [SerializeField] private TextMeshPro scaleRatioText;
        private float scaleRatio;

        private float stickInitialSizeX;

        public void Awake()
        {
            stickInitialSizeX = scaleStick.localScale.x;
        }

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
            scaleRatioText.SetText(String.Format("{0:f2}x", scaleRatio));
        }

        private void UpdateScaleStick()
        {
            scaleStick.localScale = new Vector3(
                scaleRatio * stickInitialSizeX,
                scaleStick.localScale.y,
                scaleStick.localScale.z);
        }
    }
}