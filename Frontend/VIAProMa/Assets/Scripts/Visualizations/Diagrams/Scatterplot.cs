using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.ViaProMa.Visualizations.Common;

namespace i5.ViaProMa.Visualizations.Diagrams
{
    public class Scatterplot : i5.ViaProMa.Visualizations.Common.Diagram
    {
        [SerializeField] private GameObject pointPrefab;

        protected override void Awake()
        {
            base.Awake();
            if (pointPrefab == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(pointPrefab));
            }
        }

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
                sphereObj.transform.localScale = 0.04f * Vector3.one;
            }
        }
    }
}
