using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserScore
{
    public User User { get; private set; }

    private float creatorScore;
    private float commenterScore;
    private float developerScore;
    private float closedDeveloperScore;

    private List<Issue> createdIssues;
    private List<Issue> commentedIssues;
    private List<Issue> developedIssues;
    private List<Issue> closedDevelopedIssues;

    public float Score
    {
        get
        {
            return creatorScore * createdIssues.Count + commenterScore * commentedIssues.Count
                + developerScore * developedIssues.Count + closedDeveloperScore * closedDevelopedIssues.Count;
        }
    }

    public UserScore(User user, float creatorScore, float commenterScore, float developerScore, float closedDeveloperScore)
    {
        User = user;
        this.creatorScore = creatorScore;
        this.commenterScore = commenterScore;
        this.developerScore = developerScore;
        this.closedDeveloperScore = closedDeveloperScore;

        createdIssues = new List<Issue>();
        commentedIssues = new List<Issue>();
        developedIssues = new List<Issue>();
        closedDevelopedIssues = new List<Issue>();
    }

    public void AddCreatedIssue(Issue issue)
    {
        createdIssues.Add(issue);
    }

    public void AddCommentedIssue(Issue issue)
    {
        commentedIssues.Add(issue);
    }

    public void AddDevelopedIssue(Issue issue)
    {
        developedIssues.Add(issue);
    }

    public void AddClosedDevelopedIssue(Issue issue)
    {
        closedDevelopedIssues.Add(issue);
    }
}
