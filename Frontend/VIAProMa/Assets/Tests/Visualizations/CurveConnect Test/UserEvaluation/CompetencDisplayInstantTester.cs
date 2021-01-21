using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using i5.VIAProMa.Visualizations.Competence;
using i5.VIAProMa.DataModel.API;

public class CompetencDisplayInstantTester : MonoBehaviourPunCallbacks
{
    [Range(1, 10)]
    public int[] simulatedCompetences;
    public CompetenceDisplayVisualController competenceDisplayController;
    public string titel;

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            List<UserScore> scores = new List<UserScore>();
            for (int i = 0; i < simulatedCompetences.Length; i++)
            {
                UserScore score = new UserScore(new User(DataSource.GITHUB, -1, "User " + (i + 1), "User " + (i + 1), "", ""), 1, 0, 0, 0);
                Issue issue = new Issue(DataSource.GITHUB, -1, "", "", 0, null, IssueStatus.CLOSED, "", "", null, null);
                for (int j = 0; j < simulatedCompetences[i]; j++)
                {
                    score.AddCreatedIssue(issue);
                    score.AddCommentedIssue(issue);
                    score.AddDevelopedIssue(issue);
                    score.AddClosedDevelopedIssue(issue);
                }
                scores.Add(score);
            }

            if (titel != "")
            {
                competenceDisplayController.Title = titel;
            }
            competenceDisplayController.Scores = scores;
            competenceDisplayController.DisplayCompetences();
        }
        
    }
}
