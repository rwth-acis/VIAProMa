using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompetenceDisplayVisualController : MonoBehaviour, IVisualizationVisualController
{
    [Header("References")]
    [SerializeField] private GameObject userBadgePrefab;
    [SerializeField] private Transform userDisplayParent;
    [SerializeField] private GridObjectCollection gridObjectCollection;
    [Header("Values")]
    [SerializeField] private float maxSize = 2f;

    private string title;

    public string Title
    {
        get => title;
        set => title = value;
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
        if (userDisplayParent == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(userDisplayParent));
        }
    }

    public void DisplayCompetences()
    {
        for (int i=0;i<Scores.Count;i++)
        {
            GameObject userBadgeInstance = Instantiate(userBadgePrefab, userDisplayParent);
            UserDataDisplay userDataDisplay = userBadgeInstance.GetComponent<UserDataDisplay>();
            userDataDisplay.Setup(Scores[i].User);
            userBadgeInstance.transform.localScale = Scores[i].NormalizedScore * maxSize * Vector3.one;
        }
    }
}
