using i5.ViaProMa.Visualizations.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Test script which controls the scatter plot and fills it with test data
/// </summary>
public class Grade_Scatterplot : MonoBehaviour
{
    public i5.ViaProMa.Visualizations.Common.Diagram diagram;
    public Vector3 size;

    /// <summary>
    /// Fills the diagram with test data
    /// </summary>
    private void Start()
    {
        diagram.Size = size;

        i5.ViaProMa.Visualizations.Common.DataSet dataset = new i5.ViaProMa.Visualizations.Common.DataSet();
        dataset.DataColumns.Add(new TextDataColumn(new List<string>() { "a", "b", "c", "d", "e" }));
        dataset.DataColumns.Add(new NumericDataColumn(new List<float>() { 1, 2, 10, 4, 5 }));
        dataset.DataColumns.Add(new NumericDataColumn(new List<float>() { 0, 1, 2, 3, 4 }));
        dataset.DataColumns.Add(new NumericDataColumn(new List<float>() { 1, 2, 3, 2, 0.5f}));
        dataset.DataPointColors = new List<Color>() { Color.red, Color.white, Color.yellow, Color.blue, Color.green };
        diagram.DataSet = dataset;

        diagram.UpdateDiagram();
    }

    /// <summary>
    /// Updates the diagram if F5 was pressed
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            diagram.Size = size;
            diagram.UpdateDiagram();
        }
    }
}
