using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompetenceDisplayTestRunner : MonoBehaviour
{
    [Range(1,10)]
    public int[] simulatedCompetences;
    public CompetenceDisplayVisualController competenceDisplayController;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            List<UserScore> scores = new List<UserScore>();
            for (int i = 0; i < simulatedCompetences.Length; i++)
            {
                UserScore score = new UserScore(new User(DataSource.GITHUB, -1, "User " + i, "User " + i, "", ""), 1, 0, 0, 0);
                Issue issue = new Issue(DataSource.GITHUB, -1, "", "", 0, null, IssueStatus.CLOSED, "", "", null, null);
                for (int j = 0; j < simulatedCompetences[i]; j++)
                {
                    score.AddCreatedIssue(issue);
                }
                scores.Add(score);
            }

            competenceDisplayController.Scores = scores;
            competenceDisplayController.DisplayCompetences();
        }
    }
}
