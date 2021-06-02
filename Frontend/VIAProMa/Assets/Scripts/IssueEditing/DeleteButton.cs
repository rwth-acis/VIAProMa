using TMPro;
using UnityEngine;
using i5.VIAProMa.Shelves.IssueShelf;
using Org.Requirements_Bazaar.API;
using Microsoft.MixedReality.Toolkit.Input;
using System;


public class DeleteButton : MonoBehaviour, IMixedRealityInputHandler
{
    [SerializeField] GameObject indicator;

    private ProjectTracker projectTracker;
    private IssuesLoader issueLoader;
    [HideInInspector] public int requirementID;

    // Timer parameters
    bool holding = false;
    float timer = 0;
    float timeUntilHold = 1f;

    public void Start()
    {
        issueLoader = GameObject.FindObjectOfType<IssuesLoader>();
        projectTracker = GameObject.FindObjectOfType<ProjectTracker>();
    }

    // On Input, start timer until the issue is deleted or the input has stopped, size of indicator is adjusted to delta time
    public void Update()
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

    // Called when the delete button on the issue bar is pressed
    public async void DeleteRequirement()
    {
        projectTracker.OnlastDeletedChanged(requirementID);
        await RequirementsBazaarManager.DeleteRequirement(requirementID);
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
