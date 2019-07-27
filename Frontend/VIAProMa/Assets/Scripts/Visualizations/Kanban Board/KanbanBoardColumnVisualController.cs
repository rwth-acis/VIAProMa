using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KanbanBoardColumnVisualController : MonoBehaviour, IVisualizationVisualController
{
    [Header("UI Elements")]
    [SerializeField] private Transform background;
    [SerializeField] private Transform header;
    [SerializeField] private Transform headerBackground;
    [SerializeField] private TextMeshPro headerTitle;
    [SerializeField] private IssueListView issueListView;
    [SerializeField] private ObjectGrid grid;
    [SerializeField] private BoundingBox boundingBox;


    private List<Issue> issues;
    private Vector2 issueCardSize;

    private Vector2 size;
    private Vector2Int gridSize;

    private Renderer backgroundRenderer;
    private Renderer headerBackgroundRenderer;

    private BoxCollider boundingboxCollider;

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

    private int ItemsPerPage { get => gridSize.x * gridSize.y; }

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

    public Color Color
    {
        get => backgroundRenderer.material.color;
        set
        {
            backgroundRenderer.material.color = value;
            headerBackgroundRenderer.material.color = new Color(
                Mathf.Clamp01(value.r + 0.1f),
                Mathf.Clamp01(value.g + 0.1f),
                Mathf.Clamp01(value.b + 0.1f));
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
        if (boundingBox == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(boundingBox));
        }

        boundingboxCollider = boundingBox?.gameObject.GetComponent<BoxCollider>();
        if (boundingboxCollider == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(BoxCollider), boundingBox?.gameObject);
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

        boundingboxCollider.size = 1.01f * background.localScale;
        boundingBox.Refresh();


        UpdateGrid();
    }

    private void DetermineGridSize()
    {
        gridSize.x = (int) (size.x / issueCardSize.x);
        gridSize.y = (int)(size.y / issueCardSize.y);
    }

    private void UpdateGrid()
    {
        DetermineGridSize();
        grid.Columns = gridSize.x;
        grid.UpdateGrid();
    }

    private void UpdateVisuals()
    {
        issueListView.Items = issues.GetRange(0, ItemsPerPage);
        UpdateGrid();
    }
}
