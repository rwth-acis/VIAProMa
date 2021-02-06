using i5.VIAProMa.Utilities;
using UnityEngine;
using TMPro;

namespace i5.VIAProMa.Visualizations.Diagrams
{
    /**
     * Manages Label above a bar in a bar chart
     */
    public class LabeledBar : MonoBehaviour
    {
        [SerializeField] private GameObject labelMarker;

        private void Awake()
        {
            if (labelMarker == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(labelMarker));
            }
        }

        private void OnEnable()
        {
            UpdateScale();
            labelMarker?.SetActive(false);
        }

        private void UpdateScale()
        {
            if (labelMarker)
                labelMarker.transform.localScale = new Vector3(1.0f/labelMarker.transform.parent.lossyScale.x, 1.0f/labelMarker.transform.parent.lossyScale.y, 1.0f/labelMarker.transform.parent.lossyScale.z); // No 0 scale expected
        }

        public void SetLabel(string content)
        {
            TextMeshPro label = labelMarker?.GetComponentInChildren<TextMeshPro>();
            if (label)
            {
                label.text = content;
                label.gameObject.SetActive(true);
            }
            UpdateScale();
        }
    }
}
