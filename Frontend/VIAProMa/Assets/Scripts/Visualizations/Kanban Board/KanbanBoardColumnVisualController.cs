using Microsoft.MixedReality.Toolkit.Utilities;
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
    [SerializeField] private ObjectGrid grid;


    private List<Issue> issues;
    private Vector2 issueCardSize;

    private Vector2 size;
    private Vector2Int gridSize;

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
        if (grid == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(grid));
        }

        BoxCollider issueCardColl = issueListView.ItemPrefab.GetComponentInChildren<BoxCollider>();
        issueCardSize = Vector2.Scale(issueCardColl.transform.localScale, issueCardColl.size);

        size = new Vector2(background.localScale.x, background.localScale.y);
        grid.CellSize = issueCardSize;
        grid.Centered = true;
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

        grid.transform.localPosition = new Vector3(
            0,
            -headerBackground.localScale.y / 2f,
            grid.transform.localPosition.z);

        DetermineGridSize();
        grid.Columns = gridSize.x;
        grid.UpdateGrid();
    }

    private void DetermineGridSize()
    {
        gridSize.x = (int) (size.x / issueCardSize.x);
        gridSize.y = (int)(size.y / issueCardSize.y);
        Debug.Log(gridSize);
    }

    private void UpdateVisuals()
    {
        // TODO: only add so many items to issues that the grid is filled correctly
        issueListView.Items = issues;
    }
}
