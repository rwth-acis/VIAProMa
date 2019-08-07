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
    //private Vector3 userBadgeSize;

    public string Title
    {
        get => title;
        set
        {
            title = value;
            if (string.IsNullOrEmpty(title))
            {
                titleLabel.Text = "Contributions";
            }
            else
            {
                titleLabel.Text = title;
            }
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
        
        for (int i=0;i<Scores.Count;i++)
        {
            maxScore = Mathf.Max(maxScore, Scores[i].Score);
        }

        for (int i=0;i<Scores.Count;i++)
        {
            GameObject userBadgeInstance = Instantiate(userBadgePrefab, gridObjectCollection.transform);
            UserScoreDisplay disp = userBadgeInstance.GetComponent<UserScoreDisplay>();
            disp.MaxScore = maxScore;
            disp.MaxSize = maxSize;
            disp.BarLength = 1f;
            disp.Setup(Scores[i]);
        }

        DetermineSizes();

        gridObjectCollection.UpdateCollection();
    }

    private void DetermineSizes()
    {
        Vector2 cellSize = new Vector2(gridObjectCollection.CellWidth, gridObjectCollection.CellHeight); // size of the cells
        float targetCirumference = Scores.Count * cellSize.magnitude;
        float targetRadius = targetCirumference / (2f * Mathf.PI);
        targetRadius = Mathf.Max(maxSize, targetRadius);
        gridObjectCollection.Radius = targetRadius;

        titleLabel.MaxWidth = targetRadius * 0.8f;
        titleLabel.MaxHeight = targetRadius * 0.8f;
    }
}
