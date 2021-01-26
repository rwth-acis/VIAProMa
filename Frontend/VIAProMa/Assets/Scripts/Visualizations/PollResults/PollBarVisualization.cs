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
        private Barchart2D barChart2D;
        private Barchart barChart;
        
        void Awake()
        {
            barChart2D = GetComponent<Barchart2D>();
            barChart = GetComponent<Barchart>();
        }

        public void Setup(string[] answers, int[] results)
        {
            DataSet dataset = new DataSet();
            List<string> answerAxis = new List<string>();
            List<float> resultAxis = new List<float>();
            List<float> nonceAxis = new List<float>();
            List<Color> colors = new List<Color>();
            Debug.Log("Answers length " + answers.Length + "  results: " + results.Length);

            for (int i = 0; i < results.Length; i++)
            {
                answerAxis.Add(answers[i]);
                resultAxis.Add((float)results[i]);
                nonceAxis.Add(0f);
                Debug.Log("Add data point " + answers[i] + ": " + results[i]);
                colors.Add(UnityEngine.Random.ColorHSV());
            }


            TextDataColumn answerColumn = new TextDataColumn(answerAxis);
            answerColumn.Title = "Answers";
            dataset.DataColumns.Add(answerColumn);
            NumericDataColumn resultColumn = new NumericDataColumn(resultAxis);
            resultColumn.Title = "Results";
            dataset.DataColumns.Add(resultColumn);
            
            NumericDataColumn nonceColumn = new NumericDataColumn(nonceAxis);	
            nonceColumn.Title = "Nothing";	
            if (barChart != null)
                dataset.DataColumns.Add(nonceColumn);
            
            dataset.DataPointColors = colors;

            if (barChart != null)
            {
                barChart.DataSet = dataset;
                barChart.UpdateDiagram();
            }
            else
            {
                barChart2D.DataSet = dataset;
                barChart2D.UpdateDiagram();
            }
        }
    }

}