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
    private Vector2 issueCardSize;

    private Vector2 size;

    public float Width
    {
        get => background.localScale.y;
        set
        {
            size.x = Mathf.Max(issueCardSize.x, value); // do not allow values which are smaller than the issueCard
            UpdateSize();
        }
    }

    public float Height
    {
        get => background.localScale.y;
        set
        {
            size.y = Mathf.Max(headerBackground.localScale.y, value); // do not allow negative values or 0
            UpdateSize();
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

        size = new Vector2(background.localScale.x, background.localScale.y);
        UpdateSize();
    }

    private void UpdateSize()
    {
        // the background has the whole size
        background.localScale = new Vector3(
            size.x,
            size.y,
            background.localScale.z
            );

        // the header should be at the top and adapts to the width
        headerBackground.localScale = new Vector3(
            size.x,
            headerBackground.localScale.y,
            headerBackground.localScale.z
            );
        header.localPosition = new Vector3(
            0,
            size.y / 2f - headerBackground.localScale.y / 2f, // position at top
            0);
        headerTitle.rectTransform.sizeDelta = new Vector2(size.x, headerBackground.localScale.y);

        // the list view should be underneath the header
        issueListView.transform.localPosition = header.localPosition - new Vector3(0, header.localScale.y / 2f + issueCardSize.y / 2f, 0);
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
