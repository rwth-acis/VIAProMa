using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IssueDataDisplay))]
public class IssueCardSynchronizer : TransformSynchronizer
{
    private IssueDataDisplay issueDataDisplay;

    private void Awake()
    {
        issueDataDisplay = GetComponent<IssueDataDisplay>();
    }

    private async void Start()
    {
        // in case that the card was already setup, e.g. in the local instance => do not setup again
        if (issueDataDisplay.Content != null)
        {
            Debug.Log("Card was already setup");
            return;
        }

        if (photonView.CreatorActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            Debug.Log("Card created by local player; will not set up");
            return;
        }

        int issueId;

        if (photonView.InstantiationData.Length == 1) // requirements bazaar => only id was sent
        {
            issueId = (int)photonView.InstantiationData[0];
            ApiResult<Issue> result = await RequirementsBazaar.GetRequirement(issueId);
            if (result.Successful)
            {
                issueDataDisplay.Setup(result.Value);
            }
            else
            {
                Debug.LogError("Card synchronizer could not fetch requirement with id " + issueId + ": " + result.ErrorMessage);
            }
        }
        else if (photonView.InstantiationData.Length == 2) // GitHub => id and project id was sent
        {
            issueId = (int)photonView.InstantiationData[0];
            int projectId = (int)photonView.InstantiationData[1];
            ApiResult<Issue> result = await GitHub.GetIssue(projectId, issueId);
            if (result.Successful)
            {
                issueDataDisplay.Setup(result.Value);
            }
            else
            {
                Debug.LogError("Card synchronizer could not fetch GitHub issue of project " 
                    + projectId + " and id " + issueId + ": " + result.ErrorMessage);
            }
        }
        else
        {
            Debug.Log("Unexpected number of instantiation data on issue");
            return;
        }
    }
}
