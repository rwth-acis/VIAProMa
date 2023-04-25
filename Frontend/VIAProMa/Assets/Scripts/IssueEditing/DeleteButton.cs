using TMPro;
using UnityEngine;
using i5.VIAProMa.Shelves.IssueShelf;
using Org.Requirements_Bazaar.API;
using Microsoft.MixedReality.Toolkit.Input;
using System;
using i5.VIAProMa.Login;
using i5.VIAProMa.DataModel.API;
using System.Collections.Generic;

/// <summary>
/// Contains the functionalities of the delete button for issue cards
/// </summary>
public class DeleteButton : IssueButton, IMixedRealityInputHandler
{
    [SerializeField] GameObject indicator;

    private ProjectTracker projectTracker;
    private IssuesLoader issueLoader;

    // Timer parameters
    bool holding = false;
    float timer = 0;
    float timeUntilHold = 1f;

    /// <summary>
    /// Determines whether the issue card is a child of the issue shelf
    /// </summary>
    public bool BelongsToShelf
    {
        get
        {
            return this.transform.parent.GetComponentInParent<IssuesLoader>() != null;
        }
    }

    /// <summary>
    /// Sets up the shelf and disables delete buttons for freely placed issue cards
    /// </summary>
    public void Start()
    {
        if (!BelongsToShelf)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            issueLoader = GameObject.FindObjectOfType<IssuesLoader>();
            projectTracker = GameObject.FindObjectOfType<ProjectTracker>();
            Setup(new List<DataSource>() { DataSource.GITHUB, DataSource.REQUIREMENTS_BAZAAR });
        }
    }

    /// <summary>
    /// On Input, start timer until the issue is deleted or the input has stopped, size of indicator is adjusted to delta time
    /// </summary>
    private void Update()
    {
        if (holding)
        {
            timer += Time.deltaTime;
            if(timer > timeUntilHold)
            {
                DeleteRequirement();
                timer = 0;
                indicator.transform.localScale = new Vector3(4.5f, 4.5f, 0);     
            }
        }
        indicator.transform.localScale = new Vector3(4.5f * timer, 4.5f * timer, 0);
    }

    // Called when the delete button on the issue card is pressed, deletes the issue card
    public async void DeleteRequirement()
    {
        projectTracker.OnlastDeletedChanged(resourceID);
        await RequirementsBazaarManager.DeleteRequirement(resourceID);
        issueLoader.LoadContent();
    }

    public void OnInputUp(InputEventData eventData)
    {
        holding = false;
        timer = 0;
        indicator.transform.localScale = new Vector3(4.5f, 4.5f, 0);
    }

    public void OnInputDown(InputEventData eventData)
    {
        holding = true;
        indicator.transform.localScale = new Vector3(0, 0, 0);
    }
}
