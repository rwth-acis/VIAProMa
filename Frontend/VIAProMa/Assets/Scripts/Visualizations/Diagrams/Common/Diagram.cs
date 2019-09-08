using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.ViaProMa.Visualizations.Common
{
    public class Diagram : MonoBehaviour
    {
        [SerializeField] private AxisController xAxisController;
        [SerializeField] private AxisController yAxisController;
        [SerializeField] private AxisController zAxisController;
        [SerializeField] private VisualizationGridsController gridsController;

        public Vector3 Size { get; set; }

        public IAxis XAxis { get; private set; }

        public IAxis YAxis { get; private set; }

        public IAxis ZAxis { get; private set; }

        private void Awake()
        {
            if (xAxisController == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(xAxisController));
            }
            if (yAxisController == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(yAxisController));
            }
            if (zAxisController == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(zAxisController));
            }
            if (gridsController == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(gridsController));
            }
        }

        public void UpdateGridAxes()
        {
            xAxisController.Setup(XAxis, Size.x);
            yAxisController.Setup(YAxis, Size.y);
            zAxisController.Setup(ZAxis, Size.z);
            xAxisController.transform.localPosition = -Size / 2f;
            yAxisController.transform.localPosition = -Size / 2f;
            zAxisController.transform.localPosition = new Vector3(Size.x, -Size.y, -Size.z) / 2f;
            gridsController.Setup(new Vector3Int(xAxisController.LabelCount-1, yAxisController.LabelCount-1, zAxisController.LabelCount-1), Size);
        }
    }
}
