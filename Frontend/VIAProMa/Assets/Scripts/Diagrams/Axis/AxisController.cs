using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisController : MonoBehaviour
{
    [SerializeField] GameObject labelPrefab;
    [SerializeField] private bool isHorizontal;
    private float axisMin;
    private float axisMax;

    private List<TextMesh> labelInstances;

    public Axis Axis { get; set; }

    public float Length
    {
        get
        {
            return transform.localScale.y;
        }
        set
        {
            transform.localScale = new Vector3(
                transform.localScale.x,
                value,
                transform.localScale.z
                );
        }
    }

    public bool IsHorizontal { get { return isHorizontal; } }

    public float AxisMin
    {
        get
        {
            return axisMin;
        }
    }

    public float AxisMax
    {
        get
        {
            return axisMax;
        }
    }


    private void Awake()
    {
        if (labelPrefab == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(labelPrefab));
            return;
        }

        TextMesh textMesh = labelPrefab.GetComponent<TextMesh>();
        if (textMesh == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(TextMesh), labelPrefab);
        }
        labelInstances = new List<TextMesh>();
    }

    public void VisualizeAxis(float labelDensity, Transform parent)
    {
        if (Axis == null)
        {
            return;
        }

        ClearExistingLabels();

        if (Axis.Type == AxisType.NUMERIC)
        {
            AxisConfiguration best = ExtendedWilkinson.PerformExtendedWilkinson(Length, IsHorizontal, labelDensity, Axis.DataMin, Axis.DataMax, out axisMin, out axisMax);
            RealizeConfiguration(best, parent);
        }
        else
        {
            List<AxisConfiguration> confs = AxisConfiguration.GeneratePossibleConfigurations(Axis.Labels);
            float bestScore;
            AxisConfiguration best = AxisConfiguration.OptimizeLegibility(Axis.Labels, IsHorizontal, confs, Length, 20, 100, out bestScore);
            axisMin = 0;
            axisMax = Axis.Labels.Count - 1;
            RealizeConfiguration(best, parent);
        }
    }

    private void RealizeConfiguration(AxisConfiguration conf, Transform parent)
    {
        float relativeStepSize = Length / (conf.Labels.Count - 1);
        for (int i = 0; i < conf.Labels.Count; i++)
        {
            TextMesh instantiatedLabel = Instantiate(labelPrefab).GetComponent<TextMesh>();
            instantiatedLabel.name = "Label " + transform.name;
            instantiatedLabel.transform.parent = parent; // set parent after instantiation so that it has the correct world-size independent of the parent's size
            instantiatedLabel.text = conf.Labels[i];
            instantiatedLabel.fontSize = conf.FontSize;
            instantiatedLabel.transform.localPosition = transform.localRotation * new Vector3(0, i * relativeStepSize, 0);
            if (transform.localEulerAngles.x == 90)
            {
                instantiatedLabel.transform.Rotate(0, 90f, 0);
            }
            if (!conf.HorizontalTextOrientation)
            {
                instantiatedLabel.transform.Rotate(new Vector3(0, 0, -90));
                instantiatedLabel.anchor = TextAnchor.MiddleLeft;
            }
            else
            {
                instantiatedLabel.anchor = TextAnchor.UpperCenter;
            }
            labelInstances.Add(instantiatedLabel);
        }
        TextMesh instantiatedTitle = Instantiate(labelPrefab).GetComponent<TextMesh>();
        instantiatedTitle.text = Axis.Title;
        instantiatedTitle.fontSize = conf.FontSize + 5;
        instantiatedTitle.transform.localPosition = transform.localRotation * new Vector3(0, relativeStepSize * conf.Labels.Count, 0);
        if (transform.localEulerAngles.x == 90)
        {
            instantiatedTitle.transform.Rotate(0, 90f, 0);
        }
        if (IsHorizontal)
        {
            if (transform.localEulerAngles.x == 90)
            {
                instantiatedTitle.anchor = TextAnchor.MiddleRight;
            }
            else
            {
                instantiatedTitle.anchor = TextAnchor.MiddleLeft;
            }
        }
        else
        {
            instantiatedTitle.anchor = TextAnchor.LowerCenter;
        }
    }

    private void ClearExistingLabels()
    {
        for (int i = 0; i < labelInstances.Count; i++)
        {
            Destroy(labelInstances[i].gameObject);
        }
        labelInstances.Clear();
    }

}
