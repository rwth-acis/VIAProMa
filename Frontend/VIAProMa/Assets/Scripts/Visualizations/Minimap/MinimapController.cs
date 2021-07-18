using System;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.Minimap
{
    public class MinimapController : MonoBehaviour, IVisualizationVisualController, IColorChangeable
    {
        [Tooltip("All of the minimap items sit on this surface")] [SerializeField]
        private Transform minimapSurface;

        [Tooltip("Reference to the bounding box of the minimap")] [SerializeField]
        private BoundingBox boundingBox;

        [SerializeField] [Tooltip("The legend shows the current minimap scale")]
        private GameObject minimapLegend;

        // keep track of the original size so we can't get any smaller than it
        private Vector2 surfaceMinSize;
        private Vector3 lastPointerPosPos;
        private Vector3 lastPointerPosNeg;

        [Header("Handles")] [SerializeField] private Transform handleLeft;
        [SerializeField] private Transform handleRight;
        [SerializeField] private Transform handleTop;
        [SerializeField] private Transform handleBottom;

        // defines the extent of the mini objects displayed on top of the surface
        [Header("Minimap Extents")] [Tooltip("The bottom left corner of the minimap")] [SerializeField]
        private GameObject minCorner;

        [Tooltip("The top right (above) corner of the minimap")] [SerializeField]
        private GameObject maxCorner;

        // Used to resize the minimap from the handles
        // The Y-component here actually refers to the Z-axis because the minimap is placed
        // laying down on the Z-axis
        private Vector2 size;

        // Ref to minimap surface renderer
        private Renderer backgroundRenderer;

        private BoxCollider boundingBoxCollider;
        private BoundingBoxStateController boundingBoxStateController;


        public float Width
        {
            get => minimapSurface.localScale.x;
            set
            {
                // adjust both axes to keep square ratio
                size.x = Mathf.Max(value, surfaceMinSize.x);
                size.y = Mathf.Max(value, surfaceMinSize.y);
                UpdateSize();
            }
        }

        public float Height
        {
            get => minimapSurface.localScale.z;
            set
            {
                // adjust both axes to keep square ratio
                size.y = Mathf.Max(value, surfaceMinSize.y);
                size.x = Mathf.Max(value, surfaceMinSize.x);
                UpdateSize();
            }
        }
        
        // don't add a title because it looks confusing when displaying items
        // over it
        public string Title { get; set; } = "Unnamable Minimap";

        public Color Color
        {
            get => backgroundRenderer.material.color;
            set
            {
                backgroundRenderer.material.color = value;
                float h, s, v;
                Color.RGBToHSV(value, out h, out s, out v);
                var legend = minimapLegend.GetComponent<ScaleLegendController>();
                if (legend)
                {
                    legend.Color = Color.HSVToRGB(h, s, Mathf.Clamp01(v - 0.2f));
                }
            }
        }


        private void Awake()
        {
            backgroundRenderer = minimapSurface.gameObject.GetComponent<Renderer>();

            boundingBoxCollider = boundingBox?.gameObject.GetComponent<BoxCollider>();
            if (boundingBoxCollider is null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(BoxCollider), boundingBox?.gameObject);
            }

            boundingBoxStateController = boundingBox?.gameObject.GetComponent<BoundingBoxStateController>();
            if (boundingBoxStateController == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(BoundingBoxStateController),
                    boundingBox?.gameObject);
            }


            size = new Vector2(minimapSurface.localScale.x, minimapSurface.localScale.z);
            // record the initial size so we don't go smaller than this
            surfaceMinSize = new Vector2(minimapSurface.localScale.x, minimapSurface.localScale.z);
            UpdateSize();
        }


        private void OnEnable()
        {
            boundingBoxStateController.BoundingBoxStateChanged += OnBoundingBoxStateChanged;
        }

        private void OnDisable()
        {
            boundingBoxStateController.BoundingBoxStateChanged -= OnBoundingBoxStateChanged;
        }


        private void OnBoundingBoxStateChanged(object sender, EventArgs e)
        {
            handleLeft.gameObject.SetActive(!boundingBoxStateController.BoundingBoxActive);
            handleRight.gameObject.SetActive(!boundingBoxStateController.BoundingBoxActive);
            handleTop.gameObject.SetActive(!boundingBoxStateController.BoundingBoxActive);
            handleBottom.gameObject.SetActive(!boundingBoxStateController.BoundingBoxActive);
        }


        /// <summary>
        /// Update the size/scale of the objects in the prefab
        /// </summary>
        private void UpdateSize()
        {
            minimapSurface.localScale = new Vector3(
                size.x,
                minimapSurface.localScale.y,
                size.y);


            // put the minimap legend in the upper-right quadrant
            minimapLegend.transform.localScale = new Vector3(
                size.x,
                minimapLegend.transform.localScale.y,
                size.y);

            minimapLegend.transform.localPosition = new Vector3(
                size.x / 2f + minimapLegend.transform.localScale.x / 10f,
                0.01f,
                (size.y / 2f) - minimapLegend.transform.localScale.z / 20f
            );

            handleLeft.localPosition = new Vector3(
                -size.x / 2f,
                0f,
                0f
            );

            handleRight.localPosition = new Vector3(
                size.x / 2f,
                0f,
                0f
            );

            handleTop.localPosition = new Vector3(
                0f,
                0f,
                size.y / 2f
            );

            handleBottom.localPosition = new Vector3(
                0f,
                0f,
                -size.y / 2f
            );


            boundingBoxCollider.size = new Vector3(
                minimapSurface.localScale.x + 0.5f,
                boundingBoxCollider.size.y,
                boundingBoxCollider.size.z);

            minCorner.transform.localPosition = new Vector3(
                -size.x / 2f,
                0f,
                -size.y / 2f);


            maxCorner.transform.localPosition = new Vector3(
                size.x / 2f,
                0.4f,
                size.y / 2f);
        }
    }
}