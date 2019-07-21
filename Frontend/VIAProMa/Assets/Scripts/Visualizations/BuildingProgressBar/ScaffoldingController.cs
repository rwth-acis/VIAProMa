using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaffoldingController : MonoBehaviour
{
    [SerializeField] private Renderer outerScaffolding;
    [SerializeField] private Renderer innerScaffolding;
    [SerializeField] private Transform innerOccluder;
    [SerializeField] private ObjectArray floors;

    public float tilingFactor = 50;
    public float gapBetweenLayers = 0.008f;

    private Vector3 localSize;

    public Vector3 LocalSize
    {
        get => localSize;
        set
        {
            localSize = value;
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
        if (floors == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(floors));
        }
    }

    private void UpdateScale()
    {
        localSize = new Vector3(
            Mathf.Max(2 * gapBetweenLayers, localSize.x),
            Mathf.Max(2 * gapBetweenLayers, localSize.y),
            Mathf.Max(2 * gapBetweenLayers, localSize.z)
            );

        Vector2 tiling = new Vector2(
            Mathf.Max(localSize.x, localSize.z),
            localSize.y
            );

        outerScaffolding.transform.localScale = localSize;
        innerScaffolding.transform.localScale = new Vector3(localSize.x - gapBetweenLayers, localSize.y - gapBetweenLayers, localSize.z - gapBetweenLayers);
        innerOccluder.localScale = new Vector3(localSize.x - 2 * gapBetweenLayers, localSize.y - 2 * gapBetweenLayers, localSize.z - 2 * gapBetweenLayers);

        outerScaffolding.material.mainTextureScale = tilingFactor * tiling;
        innerScaffolding.material.mainTextureScale = tilingFactor * tiling;
    }

    private void SetFloorVisibility()
    {

    }
}
