using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.ViaProMa.Visualizations.Common
{
    public class Diagram : MonoBehaviour
    {
        [SerializeField] protected AxisController xAxisController;
        [SerializeField] protected AxisController yAxisController;
        [SerializeField] protected AxisController zAxisController;
        [SerializeField] private VisualizationGridsController gridsController;

        [SerializeField] protected Transform contentParent;

        public Vector3 Size { get; set; } = Vector3.one;

        public IAxis XAxis { get; private set; }

        public IAxis YAxis { get; private set; }

        public IAxis ZAxis { get; private set; }

        public DataSet DataSet { get; set; }

        protected virtual void Awake()
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
            if (contentParent == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(contentParent));
            }
        }

        public virtual void UpdateDiagram()
        {
            UpdateGridAxes();
        }

        protected virtual void ClearContent()
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
        }

        protected void UpdateGridAxes()
        {
            XAxis = DataSet.DataColumns[0].GenerateAxis();
            YAxis = DataSet.DataColumns[1].GenerateAxis();
            ZAxis = DataSet.DataColumns[2].GenerateAxis();
            xAxisController.Setup(XAxis, Size.x);
            yAxisController.Setup(YAxis, Size.y);
            zAxisController.Setup(ZAxis, Size.z);
            xAxisController.transform.localPosition = -Size / 2f;
            yAxisController.transform.localPosition = -Size / 2f;
            zAxisController.transform.localPosition = new Vector3(Size.x, -Size.y, -Size.z) / 2f;

            gridsController.Setup(new Vector3Int(
                AxisToGridAmount(xAxisController),
                AxisToGridAmount(yAxisController),
                AxisToGridAmount(zAxisController))
                , Size);

            contentParent.localPosition = -Size / 2f;
        }

        protected static float FractionInUnitSpace(float numericValue, i5.ViaProMa.Visualizations.Common.AxisController axisController)
        {
            if (axisController.NumericAxisMin == axisController.NumericAxisMax)
            {
                Debug.LogError("Axis Min and Max may not be the same");
                return 0;
            }
            return (numericValue - axisController.NumericAxisMin) / (axisController.NumericAxisMax - axisController.NumericAxisMin);
        }

        private static int AxisToGridAmount(AxisController axisController)
        {
            if (axisController.ticksInCells)
            {
                return axisController.LabelCount;
            }
            else
            {
                return axisController.LabelCount - 1;
            }
        }
    }
}
