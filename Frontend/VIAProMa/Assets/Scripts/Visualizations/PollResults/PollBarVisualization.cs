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
        private Barchart2D barChart;
        
        void Awake()
        {
            barChart = GetComponent<Barchart2D>();
        }

        public void Setup(string[] answers, int[] results)
        {
            DataSet dataset = new DataSet();
            List<string> answerAxis = new List<string>();
            List<float> resultAxis = new List<float>();
            List<Color> colors = new List<Color>();
            Debug.Log("Answers length " + answers.Length + "  results: " + results.Length);

            for (int i = 0; i < results.Length; i++)
            {
                answerAxis.Add(answers[i]);
                answerAxis.Add(answers[i]);
                resultAxis.Add((float)results[i]);
                resultAxis.Add(0f + i*0.001f);
                Debug.Log("Add data point " + answers[i] + ": " + results[i]);
                colors.Add(UnityEngine.Random.ColorHSV());
            }


            TextDataColumn answerColumn = new TextDataColumn(answerAxis);
            answerColumn.Title = "Answers";
            dataset.DataColumns.Add(answerColumn);
            NumericDataColumn resultColumn = new NumericDataColumn(resultAxis);
            resultColumn.Title = "Results";
            dataset.DataColumns.Add(resultColumn);
            dataset.DataPointColors = colors;

            barChart.DataSet = dataset;
            barChart.UpdateDiagram();
        }
    }

}