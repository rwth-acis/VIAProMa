using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.Common.Grid
{

    [RequireComponent(typeof(Renderer))]
    public class GridController : MonoBehaviour
    {
        [SerializeField] private Vector2 cellSize = Vector2.one;
        [SerializeField] private Vector2 offset = Vector2.zero;
        [SerializeField] private float wireThickness = 0.01f;

        [Tooltip("True for Unity's quads which face towards the z-axis in neutral rotation; False for flat planes which face towards the y-axis in neutral rotation")]
        public bool zForward = true;

        private Vector3 previousScale;
        private Renderer gridRenderer;

        public Vector2 CellSize
        {
            get => cellSize;
            set
            {
                cellSize = value;
            }
        }

        public Vector2 Offset
        {
            get => offset;
            set
            {
                offset = value;
            }
        }

        public float WireThickenss
        {
            get => wireThickness;
            set
            {
                wireThickness = value;
            }
        }

        private void Awake()
        {
            EnsureRenderer();
        }

        private void Start()
        {
            UpdateGrid();
        }

        private void Update()
        {
            if (transform.hasChanged)
            {
                transform.hasChanged = false;
                UpdateGrid();
            }
        }

        private void EnsureRenderer()
        {
            if (gridRenderer == null)
            {
                gridRenderer = GetComponent<Renderer>();
            }
        }

        public void UpdateGrid()
        {
            EnsureRenderer();

            float xScale;
            float yScale;

            xScale = transform.localScale.x;

            if (zForward)
            {
                yScale = transform.localScale.y;
            }
            else
            {
                yScale = transform.localScale.z;
            }

            if (gridRenderer != null && xScale != 0 && yScale != 0)
            {
                gridRenderer.material.SetFloat("_CellWidth", CellSize.x / xScale);
                gridRenderer.material.SetFloat("_CellHeight", CellSize.y / yScale);
                float wireThicknessX = wireThickness / xScale;
                float wireThicknessY = wireThickness / yScale;
                gridRenderer.material.SetFloat("_WireThicknessX", wireThicknessX);
                gridRenderer.material.SetFloat("_WireThicknessY", wireThicknessY);
                gridRenderer.material.SetFloat("_OffsetX", offset.x);
                gridRenderer.material.SetFloat("_OffsetY", offset.y);
            }
        }

        public void Setup(Vector2Int cellCount, Vector2 overallSize)
        {
            CellSize = new Vector2(overallSize.x / cellCount.x, overallSize.y / cellCount.y);
            transform.localScale = overallSize;
            UpdateGrid();
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                UpdateGrid();
            }
        }
    }
}
