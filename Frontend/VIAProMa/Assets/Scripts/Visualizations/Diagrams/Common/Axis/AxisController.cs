﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.Diagrams.Common.Axes
{
    /// <summary>
    /// Controls the visual representation of an axis
    /// </summary>
    public class AxisController : MonoBehaviour
    {
        [Tooltip("The length of the axids")]
        [SerializeField] private float length = 1f;
        [Tooltip("The label prefab which is used for the axis labels")]
        [SerializeField] private GameObject labelPrefab;
        [Tooltip("The label instance which should be filled with the title of the axis")]
        [SerializeField] private TextMeshPro titleLabel;
        [Tooltip("The transform of the axis itself")]
        [SerializeField] private Transform axisTransform;

        /// <summary>
        /// Describes how many labels should be placed within one unit
        /// </summary>
        public float labelDensity = 5f;
        /// <summary>
        /// The type of the axis
        /// </summary>
        public AxisType axisType;
        /// <summary>
        /// If set to true, the ticks are placed at the center of a cell, if false, they are placed on the borders of the cells
        /// </summary>
        public bool ticksInCells;

        private List<TextMeshPro> labelMeshes;

        /// <summary>
        /// The axis which should be shown
        /// </summary>
        public IAxis Axis { get; private set; }

        /// <summary>
        /// The numeric value of the minimum on the axis
        /// </summary>
        public float NumericAxisMin { get; private set; }

        /// <summary>
        /// The numeric value of the maximum on the axis
        /// </summary>
        public float NumericAxisMax { get; private set; }

        /// <summary>
        /// The length of the axis in Unity units
        /// </summary>
        public float Length
        {
            get => length;
            set
            {
                length = value;
            }
        }

        /// <summary>
        /// The amount of labels which are placed on the axis
        /// </summary>
        public int LabelCount
        {
            get => labelMeshes.Count;
        }

        /// <summary>
        /// Initializes the component
        /// </summary>
        private void Awake()
        {
            labelMeshes = new List<TextMeshPro>();
        }

        /// <summary>
        /// Sets the given axis and length
        /// Optimizes the tick display in order to show the axis
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="length"></param>
        public void Setup(IAxis axis, float length)
        {
            Axis = axis;
            Axis.Horizontal = (axisType != AxisType.Y_AXIS);
            Length = length;

            UpdateAxis();
        }

        /// <summary>
        /// Sets the length on the axis transform, optimizes the tick placement and fills the labels with text
        /// </summary>
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

        /// <summary>
        /// Updates the tick labels to fit with the selected step size
        /// </summary>
        /// <param name="displayAxis"></param>
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

                float posFraction;
                if (ticksInCells)
                {
                    float prevPoint = (float)i / displayAxis.Labels.Count;
                    float nextPoint = (float)(i + 1) / displayAxis.Labels.Count;
                    posFraction = (prevPoint + nextPoint) / 2f;
                }
                else
                {
                    posFraction = (float)i / (displayAxis.Labels.Count - 1);
                }
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

        /// <summary>
        /// Sets up the given text mesh
        /// </summary>
        /// <param name="textMesh">The text mesh to set up</param>
        /// <param name="text">The text which should be displayed</param>
        /// <param name="fontSize">The font size which the text mesh should have</param>
        /// <param name="horizontalAlignment">If true, the text mesh will be horizontal</param>
        /// <param name="posFraction">The percentage which describes where on the axis the label is</param>
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
