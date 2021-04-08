using i5.VIAProMa.Utilities;
using i5.VIAProMa.Visualizations.Diagrams.Common.Axes;
using i5.VIAProMa.Visualizations.Common.Data.DataSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.Diagrams
{
    /**
     * A 2D Barchart that uses the third data column to label the other two if barPrefab is a LabeledBar
     */
    public class Barchart2DLabeled : i5.VIAProMa.Visualizations.Common.Diagram2D
    {
        [SerializeField] private GameObject barPrefab;

        protected override void Awake()
        {
            base.Awake();
            if (barPrefab == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(barPrefab));
            }
        }

        public override void UpdateDiagram()
        {
            base.UpdateDiagram();
            // axes and grid are already set up at this point

            ClearContent();

            if (DataSet.DataColumns.Count == 0)
            {
                Debug.LogError("Cannot visualize empty data set");
                return;
            }
            int minColumnLength = Mathf.Min(DataSet.DataColumns[0].ValueCount, DataSet.DataColumns[1].ValueCount);

            for (int i = 0; i < minColumnLength; i++)
            {
                float xInUnitSpace = FractionInUnitSpace(DataSet.DataColumns[0].GetFloatValue(i), xAxisController);
                float yInUnitSpace = FractionInUnitSpace(DataSet.DataColumns[1].GetFloatValue(i), yAxisController);

                xInUnitSpace = CorrectForTicksInCells(xInUnitSpace, xAxisController);
                yInUnitSpace = CorrectForTicksInCells(yInUnitSpace, yAxisController);

                Vector3 barPosition = Vector3.Scale(Size, new Vector3(xInUnitSpace, 0, 0.25f));
                GameObject barObj = Instantiate(barPrefab, contentParent);
                barObj.transform.localPosition = barPosition;

                float barThicknessX = 0.8f / (xAxisController.NumericAxisMax - xAxisController.NumericAxisMin +1);
                float barThicknessZ = 0.4f;

                barObj.transform.localScale = Vector3.Scale(Size, new Vector3(barThicknessX, Mathf.Max(yInUnitSpace, 0.001f), barThicknessZ));

                if (DataSet.DataColumns.Count > 2 && DataSet.DataColumns[2] is TextDataColumn && DataSet.DataColumns[2].ValueCount > i)
                { // Set label if separate label column is supplied
                    LabeledBar label = barObj.GetComponent<LabeledBar>();
                    if (label) label.SetLabel((DataSet.DataColumns[2] as TextDataColumn).Values[i]);
                }

                if (i < DataSet.DataPointColors.Count)
                {
                    barObj.GetComponentInChildren<Renderer>().material.color = DataSet.DataPointColors[i];
                }
            }
        }

        private float CorrectForTicksInCells(float inUnitSpace, AxisController axisController)
        {
            if (axisController.ticksInCells)
            {
                float rangeSize = axisController.NumericAxisMax - axisController.NumericAxisMin+1;
                float scaleFactor = (rangeSize - 1) / rangeSize;
                return inUnitSpace * scaleFactor + 1f / (2f * rangeSize);
            }
            else
            {
                return inUnitSpace;
            }
        }
    }
}
