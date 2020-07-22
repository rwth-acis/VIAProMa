using i5.ViaProMa.Visualizations.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// Test script which controls the scatter plot and fills it with test data
/// </summary>
public class ScatterVisualizer : MonoBehaviour
{
    private i5.ViaProMa.Visualizations.Common.Diagram diagram;
    public string name {get; set;} = "";
    private Vector3 size = Vector3.one;
    public TextAsset jsonFile;
    public TextLabel textLabel;
    public event EventHandler ConfigurationChanged;
    
    private void Awake()
    {
        diagram = GetComponent<i5.ViaProMa.Visualizations.Common.Diagram>();
        name = PhotonNetwork.NickName;
    }

    private async Task<i5.ViaProMa.Visualizations.Common.DataSet> JsonFileToDataSet()
    {
        diagram.Size = size;
        jsonFile = (TextAsset)Resources.Load(name, typeof(TextAsset));

        if (jsonFile==null)
        {
            Debug.Log("failed to get the file");
            return null;
        }

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
            colors.Add(UnityEngine.Random.ColorHSV());

            xValues.Add(assignment.name);
            yValues.Add(assignment.score);
            zValues.Add("second semester");
            colors.Add(UnityEngine.Random.ColorHSV());
        }

        TextDataColumn xColumn = new TextDataColumn(xValues);
        NumericDataColumn yColumn = new NumericDataColumn(yValues);
        TextDataColumn zColumn = new TextDataColumn(zValues);

        dataSet.DataColumns.Add(xColumn);
        dataSet.DataColumns.Add(yColumn);
        dataSet.DataColumns.Add(zColumn);
        dataSet.DataPointColors = colors;

        return dataSet;
    }

    public async void UpdateView()
    {
        i5.ViaProMa.Visualizations.Common.DataSet dataset = await JsonFileToDataSet();
        if (dataset != null)
        {
            textLabel.Text = name + ", assignments";
            diagram.DataSet = dataset;
            diagram.UpdateDiagram();
        }
    }
}
