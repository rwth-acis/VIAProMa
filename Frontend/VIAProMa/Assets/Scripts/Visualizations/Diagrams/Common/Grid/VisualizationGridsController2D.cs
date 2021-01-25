using i5.VIAProMa.Utilities;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.Common.Grid
{
    public class VisualizationGridsController2D : MonoBehaviour
    {
        [SerializeField] private GridController xPos;
        [SerializeField] private GridController xNeg;
        [SerializeField] private GridController yPos;
        [SerializeField] private GridController yNeg;

        [SerializeField] private Vector3 size = Vector3.one;

        private void Awake()
        {
            if (xPos == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(xPos));
            }
            if (xNeg == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(xNeg));
            }
            if (yPos == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(yPos));
            }
            if (yNeg == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(yNeg));
            }
        }

        public Vector2 Size
        {
            get => size;
            set
            {
                size = value;
            }
        }

        public void UpdateGridPlanes()
        {
            PositionGridPlanes();
            ScaleGrid();
        }

        private void PositionGridPlanes()
        {
            PositionGridPlane(xPos.transform, Vector3.right);
            PositionGridPlane(xNeg.transform, Vector3.left);
            PositionGridPlane(yPos.transform, Vector3.up);
            PositionGridPlane(yNeg.transform, Vector3.down);
        }

        private void PositionGridPlane(Transform target, Vector3 axis)
        {
            Vector3 targetPos = 0.5f * Vector3.Scale(Size, axis);
            target.localPosition = targetPos;
        }

        private void ScaleGrid()
        {
            ScaleGridPlane(xPos.transform);
            ScaleGridPlane(xNeg.transform);
            ScaleGridPlane(yPos.transform);
            ScaleGridPlane(yNeg.transform);
        }

        private void ScaleGridPlane(Transform target)
        {
            Vector3 targetSize = target.localRotation * Size;
            targetSize.x = Mathf.Abs(targetSize.x);
            targetSize.y = Mathf.Abs(targetSize.y);
            target.localScale = targetSize;
        }

        public void Setup(Vector2Int cellCount, Vector3 size)
        {
            Size = size;
            UpdateGridPlanes();
            xPos.Setup(new Vector2Int(1, cellCount.y), new Vector2(0.5f, size.y));
            xNeg.Setup(new Vector2Int(1, cellCount.y), new Vector2(0.5f, size.y));
            yPos.Setup(new Vector2Int(cellCount.x, 1), new Vector2(size.x, 0.5f));
            yNeg.Setup(new Vector2Int(cellCount.x, 1), new Vector2(size.x, 0.5f));
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                UpdateGridPlanes();
            }
        }
    }
}