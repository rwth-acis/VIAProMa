using i5.VIAProMa.Utilities;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.BuildingProgressBar
{
    public class ScaffoldingController : MonoBehaviour
    {
        [SerializeField] private Renderer outerScaffolding;
        [SerializeField] private Renderer innerScaffolding;
        [SerializeField] private Transform innerOccluder;
        [SerializeField] private ObjectArray floors;
        [SerializeField] private GameObject crane;

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
            if (crane == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(crane));
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

            crane.transform.localPosition = new Vector3(0, localSize.y / 2f, 0);

            if (Mathf.Min(localSize.x, localSize.y, localSize.z) <= 0.1f) // deactivate crane on very small scaffolding to avoid ridiculous looks
            {
                crane.SetActive(false);
            }
            else
            {
                crane.SetActive(true);
            }
        }

        private void SetFloorVisibility()
        {

        }
    }
}