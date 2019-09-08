using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.ViaProMa.Visualizations.Common
{
    public class Diagram : MonoBehaviour
    {
        [SerializeField] private AxisController xAxis;
        [SerializeField] private AxisController yAxis;
        [SerializeField] private AxisController zAxis;
        [SerializeField] private VisualizationGridsController gridsController;

        public Vector3 Size { get; set; }

        private void Awake()
        {
            if (xAxis == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(xAxis));
            }
            if (yAxis == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(yAxis));
            }
            if (zAxis == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(zAxis));
            }
            if (gridsController == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(gridsController));
            }
        }

        public void UpdateGridAxes()
        {
            FloatAxis axis = new FloatAxis("Test", 0, 10);
            xAxis.Setup(axis, Size.x);
            yAxis.Setup(axis, Size.y);
            zAxis.Setup(axis, Size.z);
            xAxis.transform.localPosition = -Size / 2f;
            yAxis.transform.localPosition = -Size / 2f;
            zAxis.transform.localPosition = new Vector3(Size.x, -Size.y, -Size.z) / 2f;
            gridsController.Setup(new Vector3Int(xAxis.LabelCount-1, yAxis.LabelCount-1, zAxis.LabelCount-1), Size);
        }
    }
}
