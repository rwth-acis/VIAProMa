using i5.VIAProMa.DataModel.API;
using i5.VIAProMa.UI.ListView.Issues;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.KanbanBoard
{
    public class KanbanBoardColumnVisualController : MonoBehaviour, IVisualizationVisualController, IColorChangeable
    {
        [Header("UI Elements")]
        [SerializeField] private Transform background;
        [SerializeField] private Transform header;
        [SerializeField] private Transform headerBackground;
        [SerializeField] private TextMeshPro headerTitle;
        [SerializeField] private Interactable upButton;
        [SerializeField] private Interactable downButton;
        [SerializeField] private ObjectGrid grid;
        [SerializeField] private BoundsControl boundsControl;
        [SerializeField] private Transform handleLeft;
        [SerializeField] private Transform handleRight;
        [SerializeField] private Transform handleTop;
        [SerializeField] private Transform handleBottom;

        [Header("References")]
        [SerializeField] private GameObject issueCardPrefab;


        [Header("Values")]
        [SerializeField] private float gap = 0.01f;

        private List<Issue> issues;
        private Vector2 issueCardSize;

        private Vector2 size;
        private Vector2Int gridSize;

        private Renderer backgroundRenderer;
        private Renderer headerBackgroundRenderer;
        private BoxCollider upButtonCollider;
        private BoxCollider downButtonCollider;

        private BoxCollider boundingboxCollider;
        private BoundingBoxStateController boundingBoxStateController;

        private List<IssueDataDisplay> issueCards;

        private int page = 0;
        private int firstItemOnPage = 0;

        public int Page
        {
            get => page;
            set
            {
                SetPage(value);
            }
        }

        public float Width
        {
            get => background.localScale.x;
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
                // do not allow heights which are lower than the size of the header background and the two page buttons
                size.y = Mathf.Max(headerBackground.localScale.y + upButtonCollider.size.y + downButtonCollider.size.y, value);
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
                SetPage(0);
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
                //headerBackgroundRenderer.material.color = new Color(
                //    Mathf.Clamp01(value.r - 0.5f),
                //    Mathf.Clamp01(value.g - 0.5f),
                //    Mathf.Clamp01(value.b - 0.5f));
                float h, s, v;
                Color.RGBToHSV(value, out h, out s, out v);
                headerBackgroundRenderer.material.color = Color.HSVToRGB(h, s, Mathf.Clamp01(v - 0.2f));
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

            if (upButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(upButton));
            }
            else
            {
                upButtonCollider = upButton.gameObject.GetComponent<BoxCollider>();
            }

            if (downButton == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(downButton));
            }
            else
            {
                downButtonCollider = downButton.gameObject.GetComponent<BoxCollider>();
            }

            if (grid == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(grid));
            }
            if (boundsControl == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(boundsControl));
            }

            boundingboxCollider = boundsControl?.gameObject.GetComponent<BoxCollider>();
            if (boundingboxCollider == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(BoxCollider), boundsControl?.gameObject);
            }

            boundingBoxStateController = boundsControl?.gameObject.GetComponent<BoundingBoxStateController>();
            if (boundingBoxStateController == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(BoundingBoxStateController), boundsControl?.gameObject);
            }

            if (handleLeft == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(handleLeft));
            }
            if (handleRight == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(handleRight));
            }
            if (handleTop == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(handleTop));
            }
            if (handleBottom == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(handleBottom));
            }

            if (issueCardPrefab == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(issueCardPrefab));
            }

            backgroundRenderer = background.gameObject.GetComponent<Renderer>();
            headerBackgroundRenderer = headerBackground.gameObject.GetComponent<Renderer>();

            BoxCollider issueCardColl = issueCardPrefab.GetComponentInChildren<BoxCollider>();
            issueCardSize = Vector2.Scale(issueCardColl.transform.localScale, issueCardColl.size) + (gap / 2f * Vector2.one);

            issueCards = new List<IssueDataDisplay>();
            issues = new List<Issue>();

            size = new Vector2(background.localScale.x, background.localScale.y);
            grid.CellSize = issueCardSize;
            grid.Centered = true;
            UpdateSize();
        }

        private void OnEnable()
        {
            boundingBoxStateController.BoundingBoxStateChanged += OnBoundingBoxStateChanged;
        }

        private void OnDisable()
        {
            boundingBoxStateController.BoundingBoxStateChanged -= OnBoundingBoxStateChanged;
        }

        private void OnBoundingBoxStateChanged(object sender, EventArgs e)
        {
            handleLeft.gameObject.SetActive(!boundingBoxStateController.BoundingBoxActive);
            handleRight.gameObject.SetActive(!boundingBoxStateController.BoundingBoxActive);
            handleTop.gameObject.SetActive(!boundingBoxStateController.BoundingBoxActive);
            handleBottom.gameObject.SetActive(!boundingBoxStateController.BoundingBoxActive);
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

            // position up button underneath header
            upButton.transform.localPosition = new Vector3(
                0,
                size.y / 2f - headerBackground.localScale.y - upButtonCollider.size.y / 2f,
                upButton.transform.localPosition.z);

            // position down button above end of column
            downButton.transform.localPosition = new Vector3(
                0,
                -size.y / 2f + downButtonCollider.size.y / 2f,
                downButton.transform.localPosition.z);

            grid.transform.localPosition = new Vector3(
                0,
                -headerBackground.localScale.y / 2f,
                grid.transform.localPosition.z);

            handleLeft.localPosition = new Vector3(
                -size.x / 2f,
                0f,
                0.01f
                );
            handleRight.localPosition = new Vector3(
                size.x / 2f,
                0f,
                0.01f
                );
            handleTop.localPosition = new Vector3(
                0f,
                size.y / 2f,
                0.01f
                );
            handleBottom.localPosition = new Vector3(
                0f,
                -size.y / 2f,
                0.01f
                );

            boundingboxCollider.size = 1.01f * background.localScale;

            DetermineGridSize();
            UpdateVisuals();
        }

        private void DetermineGridSize()
        {
            gridSize.x = (int)(size.x / issueCardSize.x);
            gridSize.y = (int)((size.y - headerBackground.localScale.y - upButtonCollider.size.y - downButtonCollider.size.y) / issueCardSize.y);
        }

        private void UpdateVisuals()
        {
            if (issues == null)
            {
                return;
            }
            // first create missing issue cards
            int issuesToInstantiate = Mathf.Min(ItemsPerPage, issues.Count) - issueCards.Count;
            for (int i = 0; i < issuesToInstantiate; i++)
            {
                GameObject issueCardObj = Instantiate(issueCardPrefab, grid.transform);
                IssueDataDisplay dataDisplay = issueCardObj.GetComponent<IssueDataDisplay>();
                if (dataDisplay == null)
                {
                    SpecialDebugMessages.LogComponentNotFoundError(this, nameof(IssueDataDisplay), issueCardObj);
                }
                else
                {
                    issueCards.Add(dataDisplay);
                }
            }

            // go over all issue card object and fill them with issue data or deactivate them
            for (int i = 0; i < issueCards.Count; i++)
            {
                int issueItemIndex = firstItemOnPage + i;
                if (issueItemIndex < issues.Count && i < ItemsPerPage)
                {
                    issueCards[i].gameObject.SetActive(true);
                    if (issueCards[i].Content != issues[issueItemIndex])
                    {
                        issueCards[i].Setup(issues[issueItemIndex]);
                    }
                }
                else
                {
                    issueCards[i].gameObject.SetActive(false);
                }
            }

            // update grid
            grid.Columns = gridSize.x;
            grid.UpdateGrid();
        }

        public void PageUp()
        {
            SetPage(page - 1);
            UpdateVisuals();
        }

        public void PageDown()
        {
            SetPage(page + 1);
            UpdateVisuals();
        }

        private void SetPage(int pageNumber)
        {
            page = Mathf.Clamp(pageNumber, 0, issues.Count / ItemsPerPage);
            firstItemOnPage = page * ItemsPerPage;
            CheckButtonStates();
        }

        private void CheckButtonStates()
        {
            upButton.IsEnabled = (page > 0);
            downButton.IsEnabled = (page < issues.Count / ItemsPerPage);
        }
    }
}