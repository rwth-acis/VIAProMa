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
    public Vector3 size = Vector3.one;
    public TextAsset jsonFile;

    /// <summary>
    /// Fills the diagram with test data
    /// </summary>
    private void Start()
    {
        diagram.Size = size;
        Information MentorData = JsonUtility.FromJson<Information>(jsonFile.text);
        i5.ViaProMa.Visualizations.Common.DataSet dataSet = new i5.ViaProMa.Visualizations.Common.DataSet();
        List<string> xValues = new List<string>();
        List<float> yValues = new List<float>();
        List<string> zValues = new List<string>();
        List<Color> colors = new List<Color>();

        foreach (Assignment assignment in MentorData.assignments)
        {
            xValues.Add(assignment.name);
            yValues.Add(assignment.score);
            zValues.Add("first semester");
            colors.Add(Random.ColorHSV());

            xValues.Add(assignment.name);
            yValues.Add(assignment.score);
            zValues.Add("second semester");
            colors.Add(Random.ColorHSV());
        }

        TextDataColumn xColumn = new TextDataColumn(xValues);
        NumericDataColumn yColumn = new NumericDataColumn(yValues);
        TextDataColumn zColumn = new TextDataColumn(zValues);

        dataSet.DataColumns.Add(xColumn);
        dataSet.DataColumns.Add(yColumn);
        dataSet.DataColumns.Add(zColumn);
        dataSet.DataPointColors = colors;

        /*
        dataset.DataColumns.Add(new TextDataColumn(new List<string>() { "a", "b", "c", "d", "e" }));
        dataset.DataColumns.Add(new NumericDataColumn(new List<float>() { 1, 2, 10, 4, 5 }));
        dataset.DataColumns.Add(new NumericDataColumn(new List<float>() { 0, 1, 2, 3, 4 }));
        dataset.DataColumns.Add(new NumericDataColumn(new List<float>() { 1, 2, 3, 2, 0.5f}));
        dataset.DataPointColors = new List<Color>() { Color.red, Color.white, Color.yellow, Color.blue, Color.green };
        */


        diagram.DataSet = dataSet;
        diagram.UpdateDiagram();
    }
}
