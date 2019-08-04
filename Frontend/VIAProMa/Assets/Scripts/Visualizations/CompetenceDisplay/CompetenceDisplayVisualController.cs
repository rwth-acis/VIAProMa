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
        else
        {
            gridObjectCollection.CellWidth = maxSize;
            gridObjectCollection.CellHeight = maxSize;
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

        float maxScore = 0f;
        float maxDiff = 0f;
        
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
            if (i > 0)
            {
                float diff = Mathf.Abs(Scores[i].Score - Scores[i - 1].Score);
                maxDiff = Mathf.Max(maxDiff, diff);
            }
        }

        DetermineSizes();

        gridObjectCollection.UpdateCollection();
    }

    private void DetermineSizes()
    {
        Vector2 cellSize = new Vector2(gridObjectCollection.CellWidth, gridObjectCollection.CellHeight); // size of the cells
        float targetCirumference = Scores.Count * cellSize.magnitude;
        float targetRadius = targetCirumference / (2f * Mathf.PI);
        //targetRadius = Mathf.Max(cellSize.magnitude, targetRadius);
        targetRadius = Mathf.Max(maxSize, targetRadius);
        gridObjectCollection.Radius = targetRadius;

        titleLabel.MaxWidth = targetRadius * 0.8f;
        titleLabel.MaxHeight = targetRadius * 0.8f;
    }
}
