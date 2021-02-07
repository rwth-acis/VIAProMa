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
    /// <summary>
	/// Interface to set up Barchart2DLabeled for Poll visualization
	/// </summary>
    [RequireComponent(typeof(Barchart2DLabeled))]
    public class PollBarVisualization : MonoBehaviour
    {
        [SerializeField] private TextLabel questionLabel;
        private Barchart2DLabeled barChart;

        /// <summary>
        /// Event triggered when a update has been forced
        /// </summary>
        public event EventHandler PollVizUpdatedForced;

        /// <summary>
        /// ID of the currently displayed Poll
        /// </summary>
        /// <value></value>
        public int pollID { get; private set; }

        private void Awake()
        {
            barChart = GetComponent<Barchart2DLabeled>();
            if (questionLabel == null) SpecialDebugMessages.LogMissingReferenceError(this, nameof(questionLabel));
            PollHandler.Instance.PollToDisplayRecieved += CheckSetup;
        }

        private void OnDestroy()
        {
            PollHandler.Instance.PollToDisplayRecieved -= CheckSetup;
        }

        private void CheckSetup(object sender, int id)
        {
            if (id == pollID)
            { // PollHandler notifies us that our observed poll ID has been updated
                // (either newly synchronized or updated with new resulst)
                SetupPoll(id);
            }
        }

        /// <summary>
        /// Force update the poll visualizations, making sure all observers are notified of the change
        /// Used when poll entry changes without ID change         
        /// </summary>
        /// <param name="id">ID of the Poll to visualize</param>
        public void ForceUpdatePoll(int id)
        {
            SetupPoll(id);
            PollVizUpdatedForced?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Update poll visualization with new ID incase it is different
        /// </summary>
        /// <param name="id">ID of the Poll to visualize</param>
        public void UpdatePoll(int id)
        {
            if (pollID != id)
                SetupPoll(id);
        }

        /// <summary>
        /// Setup poll visualization with poll, updating even when the poll is already set up once
        /// </summary>
        /// <param name="id">ID of the Poll to visualize</param>
        public void SetupPoll(int id)
        {
            pollID = id;
            SerializablePoll poll = PollHandler.Instance.GetPollByID(pollID);
            if (poll == null)
            { // Wait for poll to be synchronized (CheckSetup event)
                return;
            }

            string[] voters = new string[poll.Answers.Length];
            for (int i = 0; i < voters.Length; i++)
            {
                if (poll.Flags.HasFlag(PollOptions.Public))
                {
                    voters[i] = poll.SerializeableSelection.Aggregate(new StringBuilder(), (sb, cur) =>
                    {
                        if (cur.Item2[i])
                            sb.Append(sb.Length == 0 ? "" : ", ").Append(cur.Item1);
                        return sb;
                    }).ToString();
                }
                else
                {
                    voters[i] = "Anonymous";
                }
            }
            SetupBarChart(poll.Question, poll.Answers, poll.AccumulatedResult, voters);
        }

        private void SetupBarChart(string question, string[] answers, int[] results, string[] voterLists)
        { // Setup barchart component
            DataSet dataset = new DataSet();
            List<string> answerAxis = new List<string>();
            List<float> resultAxis = new List<float>();
            List<string> voterAxis = new List<string>();
            List<Color> colors = new List<Color>();

            for (int i = 0; i < results.Length; i++)
            {
                answerAxis.Add(answers[i]);
                resultAxis.Add((float)results[i]);
                voterAxis.Add(voterLists[i]);
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
            dataset.DataColumns.Add(voterColumn);
            dataset.DataPointColors = colors;
            barChart.DataSet = dataset;
            barChart.UpdateDiagram();
            questionLabel.Text = question;
        }
    }
}