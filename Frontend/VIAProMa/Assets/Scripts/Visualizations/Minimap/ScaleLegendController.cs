using TMPro;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.Minimap
{
    /// <summary>
    /// Manages the width of the legends and updates the
    /// scale ratio text accordingly
    /// </summary>
    class ScaleLegendController : MonoBehaviour, IColorChangeable
    {
        [Header("Prefab Sub-Objects")]
        [SerializeField] private GameObject background;
        [SerializeField] private Transform scaleStick;
        [SerializeField] private TextMeshPro scaleRatioText;

        private float scaleRatio;
        // keep track of initial size
        private float stickInitialSizeX;
        private Renderer backgroundRenderer;

        public void Awake()
        {
            stickInitialSizeX = scaleStick.localScale.x;
            backgroundRenderer = background.GetComponent<Renderer>();
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

        public Color Color
        {
            get => backgroundRenderer.material.color;
            set => backgroundRenderer.material.color = value;
        }

        /// <summary>
        /// Updates the scale text
        /// </summary>
        private void UpdateScaleRatioText()
        {
            scaleRatioText.SetText($"{scaleRatio:f2}x");
        }

        /// <summary>
        /// Updates the sclae stick to one unit of the minimap
        /// </summary>
        private void UpdateScaleStick()
        {
            scaleStick.localScale = new Vector3(
                scaleRatio * stickInitialSizeX,
                scaleStick.localScale.y,
                scaleStick.localScale.z);
        }
    }
}