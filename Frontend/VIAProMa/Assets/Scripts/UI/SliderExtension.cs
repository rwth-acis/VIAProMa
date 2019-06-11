using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(PinchSlider))]
public class SliderExtension : MonoBehaviour
{
    [SerializeField] TextMeshPro thumbValueLabel;
    [SerializeField] TextMeshPro minValueLabel;
    [SerializeField] TextMeshPro maxValueLabel;

    public bool roundToInt = true;

    public int minValue;
    public int maxValue;

    private PinchSlider slider;

    public float Value
    {
        get
        {
            return (maxValue - minValue) * slider.SliderValue + minValue;
        }
        set
        {
            value = Mathf.Clamp(value, minValue, maxValue); // make sure that the value is in range
            slider.SliderValue = (value - minValue) / (maxValue - minValue);
        }
    }

    public int ValueInt
    {
        get
        {
            return Mathf.RoundToInt(Value);
        }
        set
        {
            Value = value;
        }
    }

    private void Awake()
    {
        slider = GetComponent<PinchSlider>();

        if (thumbValueLabel == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(thumbValueLabel));
        }
        if (minValueLabel == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(minValueLabel));
        }
        if (maxValueLabel == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(maxValueLabel));
        }

        slider.OnValueUpdated.AddListener(UpdateDisplay);
    }

    private void UpdateDisplay(SliderEventData data)
    {
        if (roundToInt)
        {
            thumbValueLabel.text = ValueInt.ToString();
        }
        else
        {
            thumbValueLabel.text = Value.ToString("0.0");
        }

        minValueLabel.text = minValue.ToString();
        maxValueLabel.text = maxValue.ToString();
    }

    private void OnDisable()
    {
        slider.OnValueUpdated.RemoveListener(UpdateDisplay);
    }
}
