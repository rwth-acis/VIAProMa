using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AppBarPlacer))]
public class AppBarStateController : MonoBehaviour
{
    [SerializeField] private GameObject collapsedView;
    [SerializeField] private GameObject expandedView;
    [SerializeField] private GameObject adjustmentView;

    private AppBarState state = AppBarState.COLLAPSED;
    private BoundingBoxStateController boundingBoxStateController;

    public AppBarState State
    {
        get => state;
        set
        {
            state = value;
            UpdateView();
        }
    }

    private void Awake()
    {
        if (collapsedView == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(collapsedView));
        }
        if (expandedView == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(expandedView));
        }
        if (adjustmentView == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(adjustmentView));
        }
    }

    private void Start()
    {
        AppBarPlacer appBarPlacer = GetComponent<AppBarPlacer>();
        if (appBarPlacer == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(AppBarPlacer), gameObject);
        }
        boundingBoxStateController = appBarPlacer?.TargetBoundingBox?.gameObject.GetComponent<BoundingBoxStateController>();
        if (boundingBoxStateController == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(BoundingBoxStateController), appBarPlacer?.TargetBoundingBox.gameObject);
        }
    }

    private void UpdateView()
    {
        collapsedView.SetActive(state == AppBarState.COLLAPSED);
        expandedView.SetActive(state == AppBarState.EXPANDED);
        adjustmentView.SetActive(state == AppBarState.ADJUSTMENT_VIEW);

        if (state == AppBarState.ADJUSTMENT_VIEW)
        {
            boundingBoxStateController.BoundingBoxActive = true;
        }
        else
        {
            boundingBoxStateController.BoundingBoxActive = false;
        }
    }

    public void Collapse()
    {
        State = AppBarState.COLLAPSED;
    }

    public void Expand()
    {
        State = AppBarState.EXPANDED;
    }

    public void AdjustmentView()
    {
        State = AppBarState.ADJUSTMENT_VIEW;
    }
}

public enum AppBarState
{
    COLLAPSED, EXPANDED, ADJUSTMENT_VIEW
}