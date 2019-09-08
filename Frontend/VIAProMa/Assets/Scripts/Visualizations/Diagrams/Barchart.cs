using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace i5.ViaProMa.Visualizations.Diagrams
{
    public class Barchart : i5.ViaProMa.Visualizations.Common.Diagram
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

                Vector3 barPosition = Vector3.Scale(Size, new Vector3(xInUnitSpace, 0, zInUnitSpace));
                GameObject barObj = Instantiate(barPrefab, contentParent);
                barObj.transform.localPosition = barPosition;

                float barThicknessX = 0.9f / xAxisController.LabelCount;
                float barThicknessZ = 0.9f / zAxisController.LabelCount;

                barObj.transform.localScale = Vector3.Scale(Size, new Vector3(barThicknessX, yInUnitSpace, barThicknessZ));

                if (i < DataSet.DataPointColors.Count)
                {
                    barObj.GetComponentInChildren<Renderer>().material.color = DataSet.DataPointColors[i];
                }
            }
        }
    }
}
