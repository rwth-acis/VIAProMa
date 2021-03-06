﻿using i5.VIAProMa.Utilities;
using i5.VIAProMa.Visualizations.Diagrams.Common.Axes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.Diagrams
{
    public class Barchart : i5.VIAProMa.Visualizations.Common.Diagram
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
            int minColumnLength = Mathf.Min(DataSet.DataColumns[0].ValueCount, DataSet.DataColumns[1].ValueCount, DataSet.DataColumns[2].ValueCount);

            for (int i = 0; i < minColumnLength; i++)
            {
                float xInUnitSpace = FractionInUnitSpace(DataSet.DataColumns[0].GetFloatValue(i), xAxisController);
                float yInUnitSpace = FractionInUnitSpace(DataSet.DataColumns[1].GetFloatValue(i), yAxisController);
                float zInUnitSpace = FractionInUnitSpace(DataSet.DataColumns[2].GetFloatValue(i), zAxisController);

                xInUnitSpace = CorrectForTicksInCells(xInUnitSpace, xAxisController);
                yInUnitSpace = CorrectForTicksInCells(yInUnitSpace, yAxisController);
                zInUnitSpace = CorrectForTicksInCells(zInUnitSpace, zAxisController);

                Vector3 barPosition = Vector3.Scale(Size, new Vector3(xInUnitSpace, 0, zInUnitSpace));
                GameObject barObj = Instantiate(barPrefab, contentParent);
                barObj.transform.localPosition = barPosition;

                float barThicknessX = 0.8f / (xAxisController.NumericAxisMax - xAxisController.NumericAxisMin +1);
                float barThicknessZ = 0.8f / (zAxisController.NumericAxisMax - zAxisController.NumericAxisMin +1);

                barObj.transform.localScale = Vector3.Scale(Size, new Vector3(barThicknessX, Mathf.Max(yInUnitSpace, 0.001f), barThicknessZ));

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
