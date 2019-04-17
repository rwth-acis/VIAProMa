using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axis : MonoBehaviour
{
    [SerializeField] GameObject labelPrefab;
    [SerializeField] private bool isHorizontal;

    private TextMesh textMesh;

    private float length;

    public float Length
    {
        get { return length; }
        set
        {
            length = value;
            if (value <= 0)
            {
                gameObject.SetActive(false);
                Debug.Log("deactivated gameobject", this);
            }
            else
            {
                Debug.Log("set length", this);
                gameObject.SetActive(true);
                transform.localScale = new Vector3(
                    transform.localScale.x,
                    value,
                    transform.localScale.z);
            }
        }
    }

    public float TargetLength { get; set; }

    public bool IsHorizontal { get { return isHorizontal; } }


    private void Awake()
    {
        if (labelPrefab == null)
        {
            SpecialDebugMessages.LogComponentMissingReferenceError(this, nameof(labelPrefab));
            return;
        }

        textMesh = labelPrefab.GetComponent<TextMesh>();
        if (textMesh == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(TextMesh), labelPrefab);
        }
    }


    public void SetNumbericLabels(float dataMin, float dataMax, float labelDensity, Transform parent)
    {
        ExtendedWilkinson wil = new ExtendedWilkinson();
        AxisConfiguration best = wil.PerformExtendedWilkinson(TargetLength, IsHorizontal, labelDensity, dataMin, dataMax);
        RealizeConfiguration(best, parent);
    }

    public void SetStringLabels(List<string> labels, Transform parent)
    {
        List<AxisConfiguration> confs =  AxisConfiguration.GeneratePossibleConfigurations(labels);
        float bestScore;
        AxisConfiguration best = AxisConfiguration.OptimizeLegibility(labels, IsHorizontal, confs, TargetLength, 20, 80, out bestScore);
        RealizeConfiguration(best, parent);
    }

    private void RealizeConfiguration(AxisConfiguration conf, Transform parent)
    {
        Length = TargetLength;
        float stepSize = Length / (conf.Labels.Count -1);
        for (int i=0; i<conf.Labels.Count;i++)
        {
            TextMesh instantiatedLabel = Instantiate(labelPrefab).GetComponent<TextMesh>();
            instantiatedLabel.transform.parent = parent; // set parent after instantiation so that it has the correct world-size independnet of the parent's size
            instantiatedLabel.text = conf.Labels[i];
            instantiatedLabel.fontSize = conf.FontSize;
            if (!conf.HorizontalTextOrientation)
            {
                instantiatedLabel.transform.Rotate(new Vector3(0, 0, -90));
                instantiatedLabel.anchor = TextAnchor.MiddleLeft;
            }
            else
            {
                instantiatedLabel.anchor = TextAnchor.UpperCenter;
            }
            instantiatedLabel.transform.position = new Vector3(i * stepSize, 0, 0);
        }
    }

}
