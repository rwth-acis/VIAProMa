using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompetenceDisplay : Visualization
{
    public float creatorScore = 1f;
    public float commenterScore = 0.5f;
    public float developerScore = 3f;
    public float closedDeveloperScore = 5f;

    private Dictionary<string, UserScore> scores = new Dictionary<string, UserScore>();

    public override void UpdateView()
    {
        CalculateScore();

        base.UpdateView();
    }

    private void CalculateScore()
    {
        foreach (Issue issue in ContentProvider.Issues)
        {
            EnsureUserExistsInScores(issue.Creator);
            scores[issue.Creator.UserName].AddCreatedIssue(issue);

            foreach(User dev in issue.Developers)
            {
                EnsureUserExistsInScores(dev);
                if (issue.Status == IssueStatus.CLOSED)
                {
                    scores[dev.UserName].AddClosedDevelopedIssue(issue);
                }
                else
                {
                    scores[dev.UserName].AddDevelopedIssue(issue);
                }
            }
        }
    }

    private void EnsureUserExistsInScores(User user)
    {
        if (!scores.ContainsKey(user.UserName))
        {
            // create entry for user
            scores.Add(user.UserName, new UserScore(user, creatorScore, commenterScore, developerScore, closedDeveloperScore));
        }
    }
}
