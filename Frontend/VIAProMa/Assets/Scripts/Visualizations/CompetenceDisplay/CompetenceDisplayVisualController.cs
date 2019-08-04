using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompetenceDisplayVisualController : MonoBehaviour, IVisualizationVisualController
{
    [Header("References")]
    [SerializeField] private TextLabel titleLabel;
    [SerializeField] private GameObject userBadgePrefab;
    [SerializeField] private GridObjectCollection gridObjectCollection;
    [Header("Values")]
    [SerializeField] private float maxSize = 2f;

    private string title;
    private Vector3 userBadgeSize;

    public string Title
    {
        get => title;
        set
        {
            title = value;
            titleLabel.Text = title;
        }
    }

    public float MaxSize
    {
        get => maxSize;
        set
        {
            maxSize = value;
        }
    }

    public List<UserScore> Scores { get; set; }

    private void Awake()
    {
        if (userBadgePrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(userBadgePrefab));
        }
        UserDataDisplay udd = userBadgePrefab?.GetComponent<UserDataDisplay>();
        if (udd == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(UserDataDisplay), userBadgePrefab);
        }
        Renderer userBadgeRenderer = udd.GetComponent<Renderer>();
        userBadgeSize = userBadgeRenderer.bounds.size;
        if (gridObjectCollection == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(gridObjectCollection));
        }
    }

    public void DisplayCompetences()
    {
        foreach(Transform previouslyInstantiated in gridObjectCollection.transform)
        {
            // must change their activity to false because otherwise the gridObjectCollection does not notice the change
            previouslyInstantiated.gameObject.SetActive(false);
            Destroy(previouslyInstantiated.gameObject);
        }
        //gridObjectCollection.UpdateCollection();

        float maxScore = 0f;
        
        for (int i=0;i<Scores.Count;i++)
        {
            maxScore = Mathf.Max(maxScore, Scores[i].Score);
        }

        for (int i=0;i<Scores.Count;i++)
        {
            GameObject userBadgeInstance = Instantiate(userBadgePrefab, gridObjectCollection.transform);
            UserDataDisplay userDataDisplay = userBadgeInstance.GetComponent<UserDataDisplay>();
            userDataDisplay.Setup(Scores[i].User);
            userBadgeInstance.transform.localScale = Scores[i].Score / maxScore * maxSize / userBadgeSize.x * Vector3.one;
        }

        gridObjectCollection.UpdateCollection();
    }

    private void DetermineSizes()
    {
        titleLabel.
    }
}
