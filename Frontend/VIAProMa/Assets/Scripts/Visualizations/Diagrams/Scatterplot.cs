using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VIAProMa.Visualizations.Common;
using i5.VIAProMa.Utilities;

namespace i5.VIAProMa.Visualizations.Diagrams
{
    public class Scatterplot : i5.VIAProMa.Visualizations.Common.Diagram
    {
        [SerializeField] private GameObject pointPrefab;
        [SerializeField] private float sphereSize = 0.04f;

        protected override void Awake()
        {
            base.Awake();
            if (pointPrefab == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(pointPrefab));
            }
        }

        /// <summary>
        /// Updates the diagram
        /// Expected format for the data columns: data columns 0 - 2 are used to populate the axes and position the points
        /// data column 3 is optional; it is a scale factor which is multiplied with the usual size of the sphere
        /// </summary>
        public override void UpdateDiagram()
        {
            base.UpdateDiagram();
            // axes have already been set up

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

                Vector3 pointPosition = Vector3.Scale(Size, new Vector3(xInUnitSpace, yInUnitSpace, zInUnitSpace));
                GameObject sphereObj = Instantiate(pointPrefab, contentParent);
                sphereObj.transform.localPosition = pointPosition;

                float scaleFactor = 1f;
                if (DataSet.DataColumns.Count > 3 && i < DataSet.DataColumns[3].ValueCount) // size data are given for this point
                {
                    scaleFactor = DataSet.DataColumns[3].GetFloatValue(i);
                }

                sphereObj.transform.localScale = scaleFactor * sphereSize * Vector3.one;

                if (i < DataSet.DataPointColors.Count)
                {
                    sphereObj.GetComponent<Renderer>().material.color = DataSet.DataPointColors[i];
                }
            }
        }
    }
}
