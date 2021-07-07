using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using i5.VIAProMa.Visualizations;

public class VisualisationNumerator : MonoBehaviour
{
    public List<Visualization> visualisations;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < visualisations.Count; i++)
        {
            visualisations[i].Title = "Visualisation " + i;
        }
    }
}
