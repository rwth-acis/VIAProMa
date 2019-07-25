using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KanbanBoardColumnVisualController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Transform background;
    [SerializeField] private Transform header;
    [SerializeField] private Transform headerBackground;
    [SerializeField] private TextMeshPro headerTitle;
    [SerializeField] private IssueListView issueListView;


    private List<Issue> issues;
    private float issueCardHeight;

    public float Height
    {
        get => background.localScale.y;
        set
        {
            if (value >= headerBackground.localScale.y) // do not allow negative values or 0
            {
                SetHeight(value);
            }
        }
    }

    public List<Issue> Issues
    {
        get => issues;
        set
        {
            issues = value;
            UpdateVisuals();
        }
    }

    public string Title
    {
        get => headerTitle.text;
        set
        {
            headerTitle.text = value;
            headerTitle.gameObject.SetActive(!string.IsNullOrEmpty(value));
        }
    }

    private void Awake()
    {
        if (background == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(background));
        }
        if (header == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(header));
        }
        if (issueListView == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(issueListView));
        }
        else
        {
            BoxCollider sizeCollider = issueListView.ItemPrefab.GetComponentInChildren<BoxCollider>();
            if (sizeCollider == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(BoxCollider), issueListView.ItemPrefab);
            }
        }
    }

    private void SetHeight(float height)
    {
        background.localScale = new Vector3(
            background.localScale.x,
            height,
            background.localScale.z
            );
        header.localPosition = new Vector3(
            0,
            height / 2f - headerBackground.localScale.y / 2f, // position at top
            0);
        issueListView.transform.localPosition = new Vector3(
            0,
            height / 2 - headerBackground.localScale.y - 0.1f,
            0);
    }

    private void UpdateVisuals()
    {
        issueListView.Items = issues;
    }
}
