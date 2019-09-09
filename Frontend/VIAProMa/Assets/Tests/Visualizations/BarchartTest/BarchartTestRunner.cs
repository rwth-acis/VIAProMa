using i5.ViaProMa.Visualizations.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarchartTestRunner : MonoBehaviour
{
    public i5.ViaProMa.Visualizations.Diagrams.Barchart barchart;
    public Vector3 size = Vector3.one;

    private void Start()
    {
        UpdateDiagram();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            UpdateDiagram();
        }
    }

    private void UpdateDiagram()
    {
        barchart.Size = size;

        List<string> days = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
        List<string> months = new List<string>() { "January", "February", "March", "April" };

        i5.ViaProMa.Visualizations.Common.DataSet dataSet = new i5.ViaProMa.Visualizations.Common.DataSet();
        List<string> xValues = new List<string>();
        List<float> yValues = new List<float>();
        List<string> zValues = new List<string>();
        List<Color> colors = new List<Color>();

        for (int i = 0; i < days.Count; i++)
        {
            for (int j = 0; j < months.Count; j++)
            {
                xValues.Add(days[i]);
                zValues.Add(months[j]);
                yValues.Add(Random.Range(0, 10));
                colors.Add(Random.ColorHSV());
            }
        }

        TextDataColumn xColumn = new TextDataColumn(xValues);
        NumericDataColumn yColumn = new NumericDataColumn(yValues);
        TextDataColumn zColumn = new TextDataColumn(zValues);

        dataSet.DataColumns.Add(xColumn);
        dataSet.DataColumns.Add(yColumn);
        dataSet.DataColumns.Add(zColumn);
        dataSet.DataPointColors = colors;

        barchart.DataSet = dataSet;
        barchart.UpdateDiagram();
    }
}
