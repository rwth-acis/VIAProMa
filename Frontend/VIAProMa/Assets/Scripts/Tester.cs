using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public Barchart barChart;
    public Vector3 boxSize = Vector3.one;

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Test routine started");



            //ApiResult<PunchCardEntry[]> res = await BackendConnector.GetGitHubPunchCard("rwth-acis", "GaMR");
            //if (res.HasError)
            //{
            //    return;
            //}
            //List<DataPoint> points = new List<DataPoint>();

            //Color[] colors = { Color.black, Color.blue, Color.cyan, Color.green, Color.magenta, Color.white, Color.yellow };

            //foreach (PunchCardEntry entry in res.Value)
            //{
            //    Color color = colors[entry.day];
            //    points.Add(new DataPoint(new Vector3(entry.day, entry.numberOfCommits, entry.hour), color));
            //}

            //Axis xAxis = new Axis() { Labels = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" }, Title = "Weekday" };
            //Axis yAxis = new Axis() { Title = "Number of Commits" };
            //Axis zAxis = new Axis() { Title = "Hour" };

            //DataSet set = new DataSet() { Points = points, XAxis = xAxis, YAxis = yAxis, ZAxis = zAxis };

            //barChart.BoxSize = boxSize;
            //barChart.DataSet = set;
        }
    }
}
