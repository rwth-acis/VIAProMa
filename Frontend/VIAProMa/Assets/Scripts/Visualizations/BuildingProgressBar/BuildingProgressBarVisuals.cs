using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Visual controller which steers the appearance of the building progress bar
/// </summary>
public class BuildingProgressBarVisuals : MonoBehaviour, IProgressBarVisuals
{ 
    [Tooltip("The controller for the scaffolding")]
    [SerializeField] private ScaffoldingController scaffoldingController;
    [Tooltip("The object which determines the height and orientation of the clipping plane which cuts the building in half")]
    [SerializeField] private ClippingPlane clippingPlane;
    [Tooltip("The list of building prefabs which can be selected")]
    [SerializeField] private GameObject[] buildingPrefabs;
    [Tooltip("The bounding box of this visualization")]
    [SerializeField] private BoundingBox boundingBox;
    [Tooltip("The label which displays the title of this visualization")]
    [SerializeField] private TextLabel titleLabel;

    private GameObject instantiatedBuilding;

    private BuildingSizeData buildingSizeData;
    private float percentageDone;
    private float percentageInProgress;

    private Renderer[] currentBuildingRenderers;
    private BoxCollider boundingBoxCollider;

    private int buildingModelIndex;

    /// <summary>
    /// Gets or sets the percentage which should be displayed as done, i.e. for which the building should be visible
    /// </summary>
    public float PercentageDone
    {
        get => percentageDone;
        set
        {
            percentageDone = value;
            UpdateVisuals();
        }
    }

    /// <summary>
    /// Gets or sets the percentage which should be displayed as in progress, i.e. for which the scaffolding should be visible
    /// </summary>
    public float PercentageInProgress
    {
        get => percentageInProgress;
        set
        {
            percentageInProgress = value;
            UpdateVisuals();
        }
    }

    /// <summary>
    /// The title of the visualization
    /// </summary>
    public string Title
    {
        get => titleLabel.Text;
        set => titleLabel.Text = value;
    }

    /// <summary>
    /// The model index of the selected building
    /// This acts as an ID by which the application can recognize the building which is shown on the visualization
    /// </summary>
    public int BuildingModelIndex
    {
        get => buildingModelIndex;
        set
        {
            buildingModelIndex = value;
            InstantiateBuilding(buildingModelIndex);
        }
    }

    /// <summary>
    /// Checks the component's setup and initializes it
    /// </summary>
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

        if (titleLabel == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(titleLabel));
        }

        buildingModelIndex = Random.Range(0, buildingPrefabs.Length);
        InstantiateBuilding(buildingModelIndex);
    }

    /// <summary>
    /// Places the building with the given index on the visualization
    /// The index is automatically clamped so that it is inside of the array of available buildings
    /// </summary>
    /// <param name="index">The index of the building</param>
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
        boundingBox.Refresh();

        titleLabel.MaxWidth = Mathf.Min(bounds.size.x, bounds.size.z);
    }

    /// <summary>
    /// Updates the heights based on the given percentage values
    /// </summary>
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
}
