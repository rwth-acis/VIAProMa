using i5.VIAProMa.Visualizations.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using i5.VIAProMa.Visualizations;
using i5.VIAProMa;
using System.Data;
using i5.VIAProMa.Visualizations.Common.Data.DataSets;
using DataSet = i5.VIAProMa.Visualizations.Common.Data.DataSets.DataSet;
/// <summary>
/// Test script which controls the scatter plot and fills it with test data
/// </summary>
public class ScatterVisualizer : MonoBehaviour
{
    public GameObject scatter;
    private Diagram diagram;
    public string name {get; set;} = "";
    private Vector3 size = Vector3.one;
    //public TextAsset jsonFile;
    public TextLabel textLabel;
    public event EventHandler ConfigurationChanged;
    private GameObject ScatterList;
    private LearningLocker locker;
    

    private void Awake()
    {
        locker = GetComponent<LearningLocker>();
        diagram = GetComponent<Diagram>();
        name = PhotonNetwork.NickName;

        if (UserManager.Instance.UserRole != UserRoles.TUTOR)
        {
            scatter.SetActive(false);
        }
    }

    private void Start()
    {
        ScatterList = GameObject.Find("ScatterList");
        scatter.transform.parent = ScatterList.transform;
    }

/*
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
*/
    private async Task<DataSet> JsonFileToDataSet()
    {
        diagram.Size = size;
        IInformation MentorData = await locker.GetInformation(name);
        DataSet dataSet = new DataSet();
        List<string> xValues = new List<string>();
        List<float> yValues = new List<float>();
        List<string> zValues = new List<string>();
        List<Color> colors = new List<Color>();
        int item = 0;

        if (MentorData.assignments.Length > 10)
        {
            int num = MentorData.assignments.Length;
            for (int i = num-10; i<num; i++)
            {
                item += 1;
                xValues.Add(item.ToString());
                yValues.Add(MentorData.assignments[i]*100);
                zValues.Add("first semester");
                colors.Add(UnityEngine.Random.ColorHSV());

                xValues.Add(item.ToString());
                yValues.Add(0);
                zValues.Add("second semester");
                colors.Add(UnityEngine.Random.ColorHSV());
            }
        }
        else
        {
            foreach (float score in MentorData.assignments)
            {
                item += 1;
                xValues.Add(item.ToString());
                yValues.Add(score*100);
                zValues.Add("first semester");
                colors.Add(UnityEngine.Random.ColorHSV());

                xValues.Add(item.ToString());
                yValues.Add(0);
                zValues.Add("second semester");
                colors.Add(UnityEngine.Random.ColorHSV());
            }
        }

        /*
        foreach (Assignment assignment in MentorData.assignments)
        {
            item += 1;
            xValues.Add(item.ToString());
            yValues.Add(assignment.score*10);
            zValues.Add("first semester");
            colors.Add(UnityEngine.Random.ColorHSV());

            xValues.Add(item.ToString());
            yValues.Add(0);
            zValues.Add("second semester");
            colors.Add(UnityEngine.Random.ColorHSV());
        }
        */

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
        DataSet dataset = await JsonFileToDataSet();
        
        if (name == PhotonNetwork.NickName)
        {
            scatter.SetActive(true);
        }

        if (dataset != null)
        {
            textLabel.Text = name + ", assignments";
            diagram.DataSet = dataset;
            diagram.UpdateDiagram();
        }
    }
}
