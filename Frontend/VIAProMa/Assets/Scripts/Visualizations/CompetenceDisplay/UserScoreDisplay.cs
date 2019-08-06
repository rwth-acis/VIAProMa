using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserScoreDisplay : DataDisplay<UserScore>
{
    [SerializeField] private GameObject scoreBarPrefab;
    [SerializeField] private Transform scoreBarsParent;
    [SerializeField] private Color[] colors = new Color[numberOfScoredProperties];

    private const int numberOfScoredProperties = 4;

    private UserDataDisplay userDisplay;
    private Vector3 initialUserDisplaySize;
    private GameObject[] scoreBars = new GameObject[numberOfScoredProperties];

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
        else
        {
             initialUserDisplaySize = userDisplay.GetComponent<Renderer>().bounds.size;
        }
        if (scoreBarPrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(scoreBarPrefab));
        }
        if (scoreBarsParent == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(scoreBarsParent));
        }
        if (colors.Length != numberOfScoredProperties)
        {
            Debug.Log("Number of colors for the user score bars does not match number of tracked properties. Change the number of defined colors for the bars to " + numberOfScoredProperties, gameObject);
            return;
        }

        for (int i=0;i<numberOfScoredProperties;i++)
        {
            GameObject scoreBarInstance = Instantiate(scoreBarPrefab, scoreBarsParent);
            Renderer rend = scoreBarInstance.GetComponent<Renderer>();
            rend.material.color = colors[i];
            scoreBarInstance.SetActive(false);
            scoreBars[i] = (scoreBarInstance);
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

        // adapt the size of hte user display to the score
        float userDisplaySize = Content.Score / MaxScore * MaxSize / initialUserDisplaySize.x;
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
        for (int i=0;i<numberOfScoredProperties;i++)
        {
            float barLength = CalculateBarLength(issueAmount[i], overallCount);
            scoreBars[i].transform.localScale = new Vector3(1, 1, barLength);
            scoreBars[i].transform.localPosition = new Vector3(0, 0, barStart);
            scoreBars[i].gameObject.SetActive(issueAmount[i] > 0);
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
}
