﻿using i5.VIAProMa.Visualizations.Common;
using i5.VIAProMa.Visualizations.Common.Data.DataSets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Test script which controls the scatter plot and fills it with test data
/// </summary>
public class ScatterPlotTestRunner : MonoBehaviour
{
    //public VisualizationGridsController gridController;

    //public Vector3 size = Vector3.one;
    //public Vector3Int count = Vector3Int.one;

    //// Update is called once per frame
    //void Update()
    //{
    //    gridController.Setup(count, size);
    //}

    public i5.VIAProMa.Visualizations.Common.Diagram diagram;
    public Vector3 size;

    /// <summary>
    /// Fills the diagram with test data
    /// </summary>
    private void Start()
    {
        diagram.Size = size;

        DataSet dataset = new DataSet();
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
