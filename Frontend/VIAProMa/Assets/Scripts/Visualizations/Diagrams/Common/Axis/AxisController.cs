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
        public AxisType axisType;

        private List<TextMeshPro> labelMeshes;

        public IAxis Axis { get; private set; }

        public float NumericAxisMin { get; private set; }

        public float NumericAxisMax { get; private set; }

        public float Length
        {
            get => length;
            set
            {
                length = value;
            }
        }

        public int LabelCount
        {
            get => labelMeshes.Count;
        }

        private void Awake()
        {
            labelMeshes = new List<TextMeshPro>();
        }

        public void Setup(IAxis axis, float length)
        {
            Axis = axis;
            Axis.Horizontal = (axisType != AxisType.Y_AXIS);
            Length = length;

            UpdateAxis();
        }

        private void UpdateAxis()
        {
            // scale axis: length
            axisTransform.localScale = new Vector3(
                length,
                axisTransform.localScale.y,
                axisTransform.localScale.z
                );

            IDisplayAxis displayAxis = ExtendedWilkinson.PerformExtendedWilkinson(Axis, Length, labelDensity, out float axisMin, out float axisMax);
            NumericAxisMin = axisMin;
            NumericAxisMax = axisMax;

            UpdateLabels(displayAxis);

            // position title
            titleLabel.fontSize = displayAxis.FontSize;
            titleLabel.text = displayAxis.Title;
            titleLabel.rectTransform.sizeDelta = titleLabel.GetPreferredValues(Mathf.Infinity, Mathf.Infinity);
            TextMeshPro lastLabel = labelMeshes[labelMeshes.Count - 1];
            Vector3 titlePosition = lastLabel.transform.localPosition;
            titlePosition.x += lastLabel.rectTransform.sizeDelta.x / 2f + titleLabel.rectTransform.sizeDelta.x / 2f + 0.01f;
            titleLabel.transform.localPosition = titlePosition;
            titleLabel.transform.up = Vector3.up;
            titleLabel.transform.forward = transform.forward;
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

                float posFraction = (float)i / (displayAxis.Labels.Count - 1);
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

        private void SetupTextMesh(TextMeshPro textMesh, string text, float fontSize, bool horizontalAlignment, float posFraction)
        {
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.rectTransform.sizeDelta = textMesh.GetPreferredValues(Mathf.Infinity, Mathf.Infinity);
            if (axisType == AxisType.X_AXIS)
            {
                textMesh.transform.localScale = Vector3.one;
            }
            else
            {
                textMesh.transform.localScale = new Vector3(-1, 1, 1);
            }
            Vector3 labelPosition = new Vector3(posFraction * length, 0, 0);
            float positioningDirection = -1f;
            if (axisType == AxisType.Y_AXIS)
            {
                positioningDirection = 1f;
            }
            if (horizontalAlignment)
            {
                labelPosition.y += positioningDirection * textMesh.rectTransform.sizeDelta.y / 2f;
            }
            else
            {
                labelPosition.y += positioningDirection * textMesh.rectTransform.sizeDelta.x / 2f;
            }
            textMesh.transform.localPosition = labelPosition;
            // keep the text horizontally
            textMesh.transform.up = Vector3.up;
            // keep the text looking into the forward direction
            textMesh.transform.forward = transform.forward;
        }
    }
}
