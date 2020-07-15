using i5.ViaProMa.Visualizations.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;

public class BarchartVisualizer : MonoBehaviour
{
    private i5.ViaProMa.Visualizations.Common.Diagram barchart;
    public string name { get; set; } = "";
    //public string name { get; set; } = PhotonNetwork.NickName;
    private Vector3 size = Vector3.one;
    public TextAsset jsonFile;
    public event EventHandler ConfigurationChanged;
    private PhotonView photonView;
    private void Awake()
    {
        barchart = GetComponent<i5.ViaProMa.Visualizations.Common.Diagram>();
        name = PhotonNetwork.NickName;
        //name = photonView.Owner.NickName;
        Debug.Log("barchart visualizer:"+name);
    }

    private async Task<i5.ViaProMa.Visualizations.Common.DataSet> JsonFileToDataSet()
    {
        barchart.Size = size;
        Debug.Log(name);
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
            barchart.DataSet = dataset;
            barchart.UpdateDiagram();
        }
    }
}
