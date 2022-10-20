using i5.VIAProMa.DataDisplays;
using i5.VIAProMa.UI.ListView.Core;
using i5.VIAProMa.Utilities;
using System.Linq;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.Competence
{
    public class UserScoreDisplay : DataDisplay<UserScore>
    {
        [SerializeField] private GameObject scoreBarPrefab;
        [SerializeField] private Transform scoreBarsParent;
        [SerializeField] private TextLabel nameLabel;
        [SerializeField] private Color[] colors = new Color[numberOfScoredProperties];

        private const int numberOfScoredProperties = 4;

        private UserDataDisplay userDisplay;
        private CompetenceBarController[] scoreBars = new CompetenceBarController[numberOfScoredProperties];

        public float BarLength { get; set; } = 1f;

        public float MaxSize { get; set; }

        public float MaxScore { get; set; }

        private void Awake()
        {
            userDisplay = GetComponentInChildren<UserDataDisplay>();
            if (userDisplay == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(UserDataDisplay), gameObject);
            }
            if (scoreBarPrefab == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(scoreBarPrefab));
            }
            CompetenceBarController cbc = scoreBarPrefab?.GetComponent<CompetenceBarController>();
            if (cbc == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(CompetenceBarController), scoreBarPrefab);
            }
            if (scoreBarsParent == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(scoreBarsParent));
            }
            if (nameLabel == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(nameLabel));
            }
            if (colors.Length != numberOfScoredProperties)
            {
                Debug.Log("Number of colors for the user score bars does not match number of tracked properties. Change the number of defined colors for the bars to " + numberOfScoredProperties, gameObject);
                return;
            }

            for (int i = 0; i < numberOfScoredProperties; i++)
            {
                GameObject scoreBarInstance = Instantiate(scoreBarPrefab, scoreBarsParent);
                CompetenceBarController scoreBar = scoreBarInstance.GetComponent<CompetenceBarController>();
                scoreBar.Color = colors[i];
                scoreBarInstance.SetActive(false);
                scoreBars[i] = scoreBar;
            }
        }

        public override void Setup(UserScore content)
        {
            base.Setup(content);
            userDisplay.Setup(content.User);
        }

        public override void UpdateView()
        {
            base.UpdateView();
            // show user image in userDisplay
            userDisplay.UpdateView();

            if (!string.IsNullOrEmpty(content.User.FirstName) || !string.IsNullOrEmpty(content.User.LastName))
            {
                nameLabel.Text = (content.User.FirstName + " " + content.User.LastName).Trim();
            }
            else
            {
                nameLabel.Text = content.User.UserName;
            }

            nameLabel.transform.localPosition = new Vector3(0f, nameLabel.transform.localPosition.y, nameLabel.Width / 2f + 0.02f);

            // adapt the size of the user display to the score
            float userDisplaySize = Content.Score / MaxScore * MaxSize;
            transform.localScale = userDisplaySize * Vector3.one;

            if (scoreBars.Length != numberOfScoredProperties)
            {
                Debug.LogError("User Score Display does not have the right amount of score bars.", gameObject);
                return;
            }

            // store amount of scored issues in array
            int[] issueAmount = new int[numberOfScoredProperties];
            issueAmount[0] = Content.CreatedIssuesCount;
            issueAmount[1] = Content.CommentedIssuesCount;
            issueAmount[2] = Content.DevelopedIssuesCount;
            issueAmount[3] = Content.ClosedIssuesCount;

            // count all issues
            int overallCount = issueAmount.Sum();

            // the bar start is the starting depth for each bar
            // it will be summed up with the lengths of previous bars in order to get the start point for the next bare
            float barStart = 0f;

            // set the scoreBars for each amount of issues
            for (int i = 0; i < numberOfScoredProperties; i++)
            {
                float barLength = CalculateBarLength(issueAmount[i], overallCount);
                scoreBars[i].Length = barLength;
                scoreBars[i].Thickness = 0.2f;
                scoreBars[i].transform.localPosition = new Vector3(0, -scoreBars[i].Height / 2f, barStart);
                scoreBars[i].gameObject.SetActive(issueAmount[i] > 0);
                scoreBars[i].Text = GetBarText(i, issueAmount[i]);
                barStart += barLength;
            }
        }

        /// <summary>
        /// Converts an amount of issues to a length for the bar
        /// </summary>
        /// <param name="issueAmount">The amount of issues to visualize</param>
        private float CalculateBarLength(int issueAmount, int overallCount)
        {
            return ((float)issueAmount) / overallCount * BarLength;
        }

        private string GetBarText(int propertyIndex, int issueAmount)
        {
            bool singleIssue = issueAmount == 1;
            switch (propertyIndex)
            {
                case 0:
                    return "Created " + issueAmount + (singleIssue ? " issue" : " issues");
                case 1:
                    return "Commented " + issueAmount + (singleIssue ? " issue" : " issues");
                case 2:
                    return "Develops " + issueAmount + (singleIssue ? " issue" : " issues");
                case 3:
                    return "Realized " + issueAmount + (singleIssue ? " issue" : " issues");
                default:
                    Debug.Log("Tried to get competence score bar with propertyIndex out of bounds", gameObject);
                    return "ERROR";
            }
        }
    }
}