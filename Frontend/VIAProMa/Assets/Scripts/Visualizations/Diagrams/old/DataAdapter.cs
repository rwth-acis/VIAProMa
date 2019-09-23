using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Transforms data from one format to another
/// </summary>
public static class DataAdapter
{
    /// <summary>
    /// Converts punch card data from GitHub to a generic data set which can be inserted into diagrams
    /// </summary>
    /// <returns>The async task which returns the data set containing the GitHub punch card data</returns>
    public static async Task<DataSet> GitHubPunchCardToDataSet()
    {
        
        ApiResult<PunchCardEntry[]> res = await GitHub.GetGitHubPunchCard("rwth-acis", "GaMR");
        if (res.HasError)
        {
            return null;
        }

        List<DataPoint> points = new List<DataPoint>();

        Color[] colors = { Color.black, Color.blue, Color.cyan, Color.green, Color.magenta, Color.white, Color.yellow };

        foreach (PunchCardEntry entry in res.Value)
        {
            Color color = colors[entry.day];
            points.Add(new DataPoint(new Vector3(entry.day, entry.numberOfCommits, entry.hour), color));
        }

        Axis xAxis = new Axis() { Labels = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" }, Title = "Weekday" };
        Axis yAxis = new Axis() { Title = "Number of Commits" };
        Axis zAxis = new Axis() { Title = "Hour" };

        DataSet set = new DataSet() { Points = points, XAxis = xAxis, YAxis = yAxis, ZAxis = zAxis };

        return set;
    }
}
