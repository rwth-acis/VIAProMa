﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public Axis axis;

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            axis.TargetLength = 2f;
            //List<string> labels = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            //axis.SetStringLabels(labels);

            axis.SetNumbericLabels(0, 10);

            ////Bounds b = new Bounds();
            ////b.Encapsulate(new Vector3(0, 1, 0));
            ////b.Encapsulate(new Vector3(0, -1, 0));

            ////Debug.Log(b);

            //Debug.Log("Test routine started");
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


            ////List<Vector3> points = new List<Vector3>();
            ////points.Add(new Vector3(0, 0, 0));
            ////points.Add(new Vector3(0, 1, 0));
            ////points.Add(new Vector3(1, 1, 1));
            ////points.Add(new Vector3(0, 0, 1));
            ////points.Add(new Vector3(1, 0, 0));

            //scatterplot.Points = points;
        }
    }
}
