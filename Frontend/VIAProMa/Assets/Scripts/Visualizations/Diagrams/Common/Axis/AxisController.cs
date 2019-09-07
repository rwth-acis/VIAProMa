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

        private List<TextMeshPro> labelMeshes;

        public IAxis Axis { get; set; }

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

        private void SetupTextMesh(TextMeshPro textMesh, string text, float fontSize, bool horizontal)
        {
            textMesh.text = text;
            textMesh.fontSize = fontSize;
        }

        private void UpdateAxis()
        {
            axisTransform.localScale = new Vector3(
                transform.localScale.x,
                transform.localScale.y,
                length
                );


            IDisplayAxis displayAxis = ExtendedWilkinson.PerformExtendedWilkinson(Axis, Length, 1f, out float axisMin, out float axisMax);

            UpdateLabels(displayAxis);

            // position title
        }

        private void UpdateLabels(IDisplayAxis axis)
        {
            for (int i = 0; i < axis.Labels.Count; i++)
            {
                if (i >= labelMeshes.Count) // there is no text mesh for this label yet => create a new one
                {
                    GameObject newLabel = Instantiate(labelPrefab, transform);
                    TextMeshPro labelMesh = newLabel.GetComponent<TextMeshPro>();
                    labelMeshes.Add(labelMesh);
                }

                SetupTextMesh(labelMeshes[i], axis.Labels[i], axis.FontSize, axis.HorizontalAlignment);
            }
            // destroy unused textmeshes
            // meshes 0 to axis.Labels.Count - 1 are used; all others need to be deleted
            for (int i = axis.Labels.Count; i < labelMeshes.Count; i++)
            {
                Destroy(labelMeshes[i].gameObject);
            }
            int diff = labelMeshes.Count - axis.Labels.Count;
            labelMeshes.RemoveRange(axis.Labels.Count, diff);
        }
    }
}
