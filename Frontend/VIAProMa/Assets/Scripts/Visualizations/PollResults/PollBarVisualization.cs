using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using i5.VIAProMa.Visualizations.Diagrams;
using i5.VIAProMa.Visualizations.Common.Data.DataSets;
using i5.VIAProMa.Utilities;
using i5.VIAProMa.Multiplayer.Poll;

namespace i5.VIAProMa.Visualizations.Poll
{
    /**
     * Sets up Barchart Component for Poll visualization
     */
    [RequireComponent(typeof(Barchart2DLabeled))]
    public class PollBarVisualization : MonoBehaviour
    {
        [SerializeField] private TextLabel questionLabel;
        private Barchart2DLabeled barChart;

        public event EventHandler PollVizUpdated;

		public int pollIndex { get; private set; }
        
        void Awake()
        {
            barChart = GetComponent<Barchart2DLabeled>();
            if (questionLabel == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(questionLabel));
        }

        private void Setup(string question, string[] answers, int[] results, string[] voterLists)
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
            questionLabel.Text = question;
        }

		private void CheckSetup(object sender, int index)
        {
			if (index == pollIndex)
			{
				PollHandler.Instance.PollToDisplayRecieved -= CheckSetup;
				Setup(index);
			}
		}

		public void Setup(int id)
        {
			if (pollIndex != id)
				PollVizUpdated?.Invoke(this, EventArgs.Empty);
			pollIndex = id;
			SerializablePoll poll = PollHandler.Instance.GetPollAtIndex(pollIndex);
			if (poll == null)
			{ // wait for poll to be synchronized
				PollHandler.Instance.PollToDisplayRecieved += CheckSetup;
				return;
			}

			string[] voters = new string[poll.Answers.Length];
			for (int i = 0; i < voters.Length; i++)
			{
				if (poll.Flags.HasFlag(PollOptions.Public))
				{
					voters[i] = poll.SerializeableSelection.Aggregate(new StringBuilder(), (sb, cur) => {
						if (cur.Item2[i]) sb.Append(sb.Length == 0? "" : ", ").Append(cur.Item1);
						return sb;
					}).ToString();
				}
				else 
				{
					voters[i] = "Anonymous";
				}
			}
            Setup(poll.Question, poll.Answers, poll.AccumulatedResult, voters);
        }
    }
}