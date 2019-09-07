using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace i5.ViaProMa.Visualizations.Common
{

    public class AxisController : MonoBehaviour
    {
        [SerializeField] private float length = 1f;
        [SerializeField] private GameObject labelPrefab;
        [SerializeField] private TextMeshPro titleLabel;
        [SerializeField] private Transform axisTransform;

        public float labelDensity = 5f;

        private List<TextMeshPro> labelMeshes;

        public IAxis Axis { get; private set; }

        public float Length
        {
            get => length;
            set
            {
                length = value;
            }
        }

        private void Awake()
        {
            labelMeshes = new List<TextMeshPro>();
        }

        public void Setup(IAxis axis, float length)
        {
            Axis = axis;
            Length = length;

            UpdateAxis();
        }

        private void UpdateAxis()
        {
            axisTransform.localScale = new Vector3(
                length,
                axisTransform.localScale.y,
                axisTransform.localScale.z
                );


            IDisplayAxis displayAxis = ExtendedWilkinson.PerformExtendedWilkinson(Axis, Length, labelDensity, out float axisMin, out float axisMax);

            UpdateLabels(displayAxis);

            // position title
            titleLabel.text = displayAxis.Title;
            titleLabel.transform.localPosition = new Vector3(1.1f * length, 0, 0);
        }

        private void UpdateLabels(IDisplayAxis displayAxis)
        {
            for (int i = 0; i < displayAxis.Labels.Count; i++)
            {
                if (i >= labelMeshes.Count) // there is no text mesh for this label yet => create a new one
                {
                    GameObject newLabel = Instantiate(labelPrefab, transform);
                    TextMeshPro labelMesh = newLabel.GetComponent<TextMeshPro>();
                    labelMeshes.Add(labelMesh);
                }

                float posFraction = (float)i / (displayAxis.Labels.Count -1);
                SetupTextMesh(labelMeshes[i], displayAxis.Labels[i], displayAxis.FontSize, displayAxis.HorizontalAlignment, posFraction);
            }
            // destroy unused textmeshes
            // meshes 0 to axis.Labels.Count - 1 are used; all others need to be deleted
            for (int i = displayAxis.Labels.Count; i < labelMeshes.Count; i++)
            {
                Destroy(labelMeshes[i].gameObject);
            }
            int diff = labelMeshes.Count - displayAxis.Labels.Count;
            labelMeshes.RemoveRange(displayAxis.Labels.Count, diff);
        }

        private void SetupTextMesh(TextMeshPro textMesh, string text, float fontSize, bool horizontal, float posFraction)
        {
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.transform.localPosition = new Vector3(posFraction * length, 0, 0);
        }
    }
}
