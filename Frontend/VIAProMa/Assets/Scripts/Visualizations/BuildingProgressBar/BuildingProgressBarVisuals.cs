using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingProgressBarVisuals : MonoBehaviour, IProgressBarVisuals
{ 
    [SerializeField] private ScaffoldingController scaffoldingController;
    [SerializeField] private ClippingPlane clippingPlane;
    [SerializeField] private GameObject[] buildingPrefabs;
    [SerializeField] private BoundingBox boundingBox;

    private GameObject instantiatedBuilding;

    private BuildingSizeData buildingSizeData;
    private float percentageDone;
    private float percentageInProgress;

    private Renderer[] currentBuildingRenderers;
    private BoxCollider boundingBoxCollider;

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

    private void Awake()
    {
        if (scaffoldingController == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(scaffoldingController));
        }
        if (clippingPlane == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(clippingPlane));
        }
        if (buildingPrefabs.Length == 0)
        {
            SpecialDebugMessages.LogArrayInitializedWithSize0Warning(this, nameof(buildingPrefabs));
        }
        else
        {
            for (int i=0;i<buildingPrefabs.Length;i++)
            {
                if (buildingPrefabs[i] == null)
                {
                    SpecialDebugMessages.LogArrayMissingReferenceError(this, nameof(buildingPrefabs), i);
                }
            }
        }
        if (boundingBox == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(boundingBox));
        }
        boundingBoxCollider = boundingBox?.gameObject.GetComponent<BoxCollider>();
        if (boundingBoxCollider == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(BoxCollider), boundingBox.gameObject);
        }

        int buildingModelIndex = Random.Range(0, buildingPrefabs.Length);
        InstantiateBuilding(buildingModelIndex);
    }

    private void InstantiateBuilding(int index)
    {
        if (instantiatedBuilding != null)
        {
            for (int i=0;i<currentBuildingRenderers.Length;i++)
            {
                clippingPlane.RemoveRenderer(currentBuildingRenderers[i]);
            }
            Destroy(instantiatedBuilding);
        }

        index = Mathf.Clamp(index, 0, buildingPrefabs.Length - 1);
        instantiatedBuilding = Instantiate(buildingPrefabs[index], transform);

        buildingSizeData = instantiatedBuilding.GetComponent<BuildingSizeData>();
        if (buildingSizeData == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(BuildingSizeData), instantiatedBuilding);
        }
        currentBuildingRenderers = instantiatedBuilding.GetComponentsInChildren<Renderer>();
        for (int i=0;i<currentBuildingRenderers.Length;i++)
        {
            clippingPlane.AddRenderer(currentBuildingRenderers[i]);
        }

        // set up the bounding box
        // use the bounds of the buildingSizeData so that the scaffolding is also included in the bounding box
        Bounds bounds = buildingSizeData.GetBounds();
        boundingBoxCollider.center = bounds.center;
        boundingBoxCollider.size = bounds.size;
        boundingBox.RefreshDisplay();
    }

    private void UpdateVisuals()
    {
        if (buildingSizeData == null)
        {
            return;
        }

        percentageDone = Mathf.Clamp01(percentageDone);
        percentageInProgress = Mathf.Clamp(percentageInProgress, 0, 1 - percentageDone);

        float doneHeight = percentageDone * buildingSizeData.BuildingHeight;
        float progressHeight = percentageInProgress * buildingSizeData.BuildingHeight;

        clippingPlane.transform.localPosition = new Vector3(0, doneHeight, 0);

        if (percentageDone >= 1)
        {
            scaffoldingController.gameObject.SetActive(false);
        }
        else
        {
            scaffoldingController.gameObject.SetActive(true);
            scaffoldingController.transform.localPosition = new Vector3(
            0,
            doneHeight + progressHeight / 2f,
            0
            );
            Vector2 scaffoldingSize = buildingSizeData.GetBuildingSize(doneHeight);
            scaffoldingController.LocalSize = new Vector3(
                scaffoldingSize.x,
                progressHeight,
                scaffoldingSize.y
                );
        }
    }

    public void SetTitle(string title)
    {
    }
}
