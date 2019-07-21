using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingProgressBarVisuals : MonoBehaviour, IProgressBarVisuals
{
    [SerializeField] private ScaffoldingController scaffoldingController;
    [SerializeField] private Transform clippingPlane;

    private BuildingSizeData buildingSizeData;
    private float percentageDone;
    private float percentageInProgress;

    public float PercentageDone
    {
        get => percentageDone;
        set
        {
            percentageDone = value;
            UpdateVisuals();
        }
    }
    public float PercentageInProgress
    {
        get => percentageInProgress;
        set
        {
            percentageInProgress = value;
            UpdateVisuals();
        }
    }

    private void Start()
    {
        buildingSizeData = GetComponentInChildren<BuildingSizeData>();
        if (buildingSizeData == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(BuildingSizeData), gameObject);
        }
    }

    private void UpdateVisuals()
    {
        percentageDone = Mathf.Clamp01(percentageDone);
        percentageInProgress = Mathf.Clamp(percentageInProgress, 0, 1 - percentageDone);

        clippingPlane.localPosition = new Vector3(0, percentageDone, 0);

        if (percentageDone >= 1)
        {
            scaffoldingController.gameObject.SetActive(false);
        }
        else
        {
            scaffoldingController.gameObject.SetActive(true);
            scaffoldingController.transform.localPosition = new Vector3(
            0,
            percentageDone + percentageInProgress / 2f,
            0
            );
            Vector2 scaffoldingSize = buildingSizeData.GetBuildingSize(percentageDone);
            scaffoldingController.LocalSize = new Vector3(
                scaffoldingSize.x,
                percentageInProgress,
                scaffoldingSize.y
                );
        }
    }
}
