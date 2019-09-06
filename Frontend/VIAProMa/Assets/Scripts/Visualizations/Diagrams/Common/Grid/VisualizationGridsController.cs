using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.ViaProMa.Visualizations.Common
{
    public class VisualizationGridsController : MonoBehaviour
    {
        [SerializeField] private GridController xPos;
        [SerializeField] private GridController xNeg;
        [SerializeField] private GridController yPos;
        [SerializeField] private GridController yNeg;
        [SerializeField] private GridController zPos;
        [SerializeField] private GridController zNeg;

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
            if (zPos == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(zPos));
            }
            if (zNeg == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(zNeg));
            }
        }

        public Vector3 Size
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
            PositionGridPlane(zPos.transform, Vector3.forward);
            PositionGridPlane(zNeg.transform, Vector3.back);
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
            ScaleGridPlane(zPos.transform);
            ScaleGridPlane(zNeg.transform);
        }

        private void ScaleGridPlane(Transform target)
        {
            Vector3 targetSize = target.localRotation * Size;
            targetSize.x = Mathf.Abs(targetSize.x);
            targetSize.y = Mathf.Abs(targetSize.y);
            targetSize.z = Mathf.Abs(targetSize.z);
            target.localScale = targetSize;
        }

        public void Setup(Vector3Int cellCount, Vector3 size)
        {
            Size = size;
            UpdateGridPlanes();
            xPos.Setup(new Vector2Int(cellCount.z, cellCount.y), new Vector2(size.z, size.y));
            xNeg.Setup(new Vector2Int(cellCount.z, cellCount.y), new Vector2(size.z, size.y));
            yPos.Setup(new Vector2Int(cellCount.x, cellCount.z), new Vector2(size.x, size.z));
            yNeg.Setup(new Vector2Int(cellCount.x, cellCount.z), new Vector2(size.x, size.z));
            zPos.Setup(new Vector2Int(cellCount.x, cellCount.y), new Vector2(size.x, size.y));
            zNeg.Setup(new Vector2Int(cellCount.x, cellCount.y), new Vector2(size.x, size.y));
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