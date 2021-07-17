using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.Visualizations.Minimap;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace i5.VIAProMa.Visualizations.Minimap
{
    public class MinimapController : MonoBehaviour, IVisualizationVisualController
    {
        [Tooltip("All of the minimap items sit on this surface")] [SerializeField]
        private Transform minimapSurface;
        [Tooltip("Reference to the bounding box of the minimap")] [SerializeField]
        private BoundingBox boundingBox;

        [SerializeField] [Tooltip("The legend shows the current minimap scale")]
        private Transform minimapLegend;

        private Vector2 surfaceMinSize;

        private Vector3 lastPointerPosPos;
        private Vector3 lastPointerPosNeg;

        [Header("Handles")] 
        [SerializeField] private Transform handleLeft;
        [SerializeField] private Transform handleRight;
        [SerializeField] private Transform handleTop;
        [SerializeField] private Transform handleBottom;

        [SerializeField] private GameObject minCorner;
        [SerializeField] private GameObject maxCorner;

        private string title = "Unnamable Minimap";

        // Used to resize the minimap from the handles
        // The Y-component here actually refers to the Z-axis because the minimap is placed laying down on the Z-axis
        private Vector2 size;
        private Renderer backgroundRenderer;
        private Renderer headerBackgroundRenderer;

        private BoxCollider boundingBoxCollider;
        private BoundingBoxStateController boundingBoxStateController;

        public Visualization[] HighlightedItems { get; set; }


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

        public string Title
        {
            get => title;
            set => title = value;
        }

        public Color Color
        {
            get => backgroundRenderer.material.color;
            set
            {
                Debug.Log("Setting color");
                backgroundRenderer.material.color = value;
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

        public void StartResizing(Vector3 pointerPosition, bool handleOnPositiveCap)
        {
            if (handleOnPositiveCap)
            {
                lastPointerPosPos = pointerPosition;
            }
            else
            {
                lastPointerPosNeg = pointerPosition;
            }
        }

        private float ProjectOnRight(Vector3 vector, Vector3 position)
        {
            Vector3 delta = vector - position;
            return Vector3.Dot(transform.right, delta);
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


        private void UpdateSize()
        {
            minimapSurface.localScale = new Vector3(
                size.x,
                minimapSurface.localScale.y,
                size.y);


            // put the minimap legend in the upper-right quadrant
            minimapLegend.localScale = new Vector3(
                size.x,
                minimapLegend.localScale.y,
                size.y);

            minimapLegend.localPosition = new Vector3(
                size.x / 2f + minimapLegend.localScale.x / 10f,
                0.01f,
                (size.y / 2f) - minimapLegend.localScale.z / 20f
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