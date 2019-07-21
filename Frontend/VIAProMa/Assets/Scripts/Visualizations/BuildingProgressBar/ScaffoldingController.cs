using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaffoldingController : MonoBehaviour
{
    [SerializeField] private Renderer outerScaffolding;
    [SerializeField] private Renderer innerScaffolding;
    [SerializeField] private Transform innerOccluder;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private ObjectArray floors;

    public float tilingFactor = 50;
    public float gapBetweenLayers = 0.008f;

    [SerializeField] private Vector3 size;

    public Vector3 Size
    {
        get => size;
        set
        {
            size = value;
            UpdateScale();
        }
    }

    private void Awake()
    {
        if (outerScaffolding == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(outerScaffolding));
        }
        if (innerScaffolding == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(innerScaffolding));
        }
        if (innerOccluder == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(innerOccluder));
        }
        if (floorPrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(floorPrefab));
        }
        if (floors == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(floors));
        }
    }

    private void UpdateScale()
    {
        size = new Vector3(
            Mathf.Max(0.011f, size.x),
            Mathf.Max(0.011f, size.y),
            Mathf.Max(0.011f, size.z)
            );

        Vector2 tiling = new Vector2(
            Mathf.Max(size.x, size.z),
            size.y
            );

        outerScaffolding.transform.localScale = size;
        innerScaffolding.transform.localScale = new Vector3(size.x - gapBetweenLayers, size.y - gapBetweenLayers, size.z - gapBetweenLayers);
        innerOccluder.localScale = new Vector3(size.x - 2 * gapBetweenLayers, size.y - 2 * gapBetweenLayers, size.z - 2 * gapBetweenLayers);

        outerScaffolding.material.mainTextureScale = tilingFactor * tiling;
        innerScaffolding.material.mainTextureScale = tilingFactor * tiling;
    }

    private void Update()
    {
        UpdateScale();
    }
}
