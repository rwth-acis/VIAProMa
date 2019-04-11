using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    public Scatterplot scatterplot;

    private /*async*/ void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Test routine started");
            //ApiResult<PunchCardEntry[]> res = await BackendConnector.GetGitHubPunchCard("rwth-acis", "GaMR");
            //if (res.Successful)
            //{
            //    Debug.Log(res.Value.Length);
            //}

            List<Vector3> points = new List<Vector3>();
            points.Add(new Vector3(0, 0, 0));
            points.Add(new Vector3(0, 1, 0));
            points.Add(new Vector3(1, 1, 1));
            points.Add(new Vector3(0, 0, 1));
            points.Add(new Vector3(1, 0, 0));

            scatterplot.Points = points;
        }
    }
}
