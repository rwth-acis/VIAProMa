using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VIAProMa.Visualizations.Diagrams;
using i5.VIAProMa.Visualizations.Common.Data.DataSets;

namespace i5.VIAProMa.Visualizations.Poll
{
    /**
     * Sets up Barchart Component for Poll visualization
     */
    public class PollBarVisualization : MonoBehaviour
    {
        private Barchart2DLabeled barChart;
        
        void Awake()
        {
            barChart = GetComponent<Barchart2DLabeled>();
        }

        public void Setup(string[] answers, int[] results, string[] voterLists)
        {
            DataSet dataset = new DataSet();
            List<string> answerAxis = new List<string>();
            List<float> resultAxis = new List<float>();
            List<string> voterAxis = new List<string>();
            List<Color> colors = new List<Color>();
            Debug.Log("Answers length " + answers.Length + "  results: " + results.Length);

            for (int i = 0; i < results.Length; i++)
            {
                answerAxis.Add(answers[i]);
                resultAxis.Add((float)results[i]);
                if (voterLists != null) voterAxis.Add(voterLists[i]);
                Debug.Log("Add data point " + answers[i] + ": " + results[i]);
                colors.Add(UnityEngine.Random.ColorHSV());
            }

            TextDataColumn answerColumn = new TextDataColumn(answerAxis);
            answerColumn.Title = "";
            dataset.DataColumns.Add(answerColumn);
            NumericDataColumn resultColumn = new NumericDataColumn(resultAxis);
            resultColumn.Title = "";
            dataset.DataColumns.Add(resultColumn);
            TextDataColumn voterColumn = new TextDataColumn(voterAxis);
            voterColumn.Title = "";
            if (voterLists != null) dataset.DataColumns.Add(voterColumn);
            dataset.DataPointColors = colors;
			barChart.DataSet = dataset;
			barChart.UpdateDiagram();
        }
    }
}