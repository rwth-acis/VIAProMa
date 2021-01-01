using i5.VIAProMa.DataModel;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.Visualizations.Common;
using i5.VIAProMa.Visualizations.Common.Data.DataSets;
using i5.VIAProMa.WebConnection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.CommitStatistics
{
    public class CommitStatisticsVisualizer : MonoBehaviour
    {
        private i5.VIAProMa.Visualizations.Common.Diagram diagram;

        public string Owner { get; set; } = "";

        public string Repository { get; set; } = "";

        public event EventHandler ConfigurationChanged;

        private void Awake()
        {
            diagram = GetComponent<i5.VIAProMa.Visualizations.Common.Diagram>();
        }

        /// <summary>
        /// Converts punch card data from GitHub to a generic data set which can be inserted into diagrams
        /// </summary>
        /// <returns>The async task which returns the data set containing the GitHub punch card data</returns>
        private async Task<DataSet> GitHubPunchCardToDataSet()
        {
            ApiResult<PunchCardEntry[]> res = await GitHub.GetGitHubPunchCard(Owner, Repository);
            if (res.HasError)
            {
                return null;
            }

            DataSet dataset = new DataSet();
            List<string> weekDayAxis = new List<string>();
            List<float> hourAxis = new List<float>();
            List<float> amountAxis = new List<float>();
            List<Color> colors = new List<Color>();

            for (int i = 0; i < res.Value.Length; i++)
            {
                weekDayAxis.Add(res.Value[i].DayOfWeek.ToString());
                hourAxis.Add(res.Value[i].hour);
                amountAxis.Add(res.Value[i].numberOfCommits);
                colors.Add(UnityEngine.Random.ColorHSV());
            }


            TextDataColumn weekDayColumn = new TextDataColumn(weekDayAxis);
            weekDayColumn.Title = "Weekday";
            dataset.DataColumns.Add(weekDayColumn);
            NumericDataColumn amountColumn = new NumericDataColumn(amountAxis);
            amountColumn.Title = "Commits";
            dataset.DataColumns.Add(amountColumn);
            NumericDataColumn hourColumn = new NumericDataColumn(hourAxis);
            hourColumn.Title = "Hour";
            dataset.DataColumns.Add(hourColumn);
            dataset.DataPointColors = colors;

            return dataset;
        }

        public async void UpdateView()
        {
            if (string.IsNullOrWhiteSpace(Owner) || string.IsNullOrWhiteSpace(Repository))
            {
                return;
            }

            DataSet dataset = await GitHubPunchCardToDataSet();
            if (dataset != null)
            {
                diagram.DataSet = dataset;
                diagram.UpdateDiagram();
            }
        }
    }
}