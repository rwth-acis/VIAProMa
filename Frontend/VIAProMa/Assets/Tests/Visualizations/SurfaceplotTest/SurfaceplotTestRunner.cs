using i5.ViaProMa.Visualizations.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceplotTestRunner : MonoBehaviour
{
    public Surfaceplot surfaceplot;
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
        surfaceplot.Size = size;
        i5.ViaProMa.Visualizations.Common.DataSet dataSet = new i5.ViaProMa.Visualizations.Common.DataSet();
        //dataSet.DataColumns.Add(new TextDataColumn(new List<string>() { "a", "b", "c", "d" }));
        //dataSet.DataColumns.Add(new NumericDataColumn(new List<float>() { 0, 1, 2, 3 }));
        //dataSet.DataColumns.Add(new TextDataColumn(new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday" }));

        List<float> xValues = new List<float>();
        List<float> yValues = new List<float>();
        List<float> zValues = new List<float>();

        for (float x = 0; x < Mathf.PI; x+=0.1f)
        {
            for (float z = 0; z< Mathf.PI;z+=0.1f)
            {
                xValues.Add(x);
                zValues.Add(z);
                yValues.Add(Mathf.Sin(x) + Mathf.Cos(z));
            }
        }

        dataSet.DataColumns.Add(new NumericDataColumn(xValues));
        dataSet.DataColumns.Add(new NumericDataColumn(yValues));
        dataSet.DataColumns.Add(new NumericDataColumn(zValues));

        surfaceplot.DataSet = dataSet;
        surfaceplot.UpdateDiagram();
    }
}
