using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ConnectionCurve : MonoBehaviour
{

    public GameObject start { get; set; }
    public GameObject goal { get; set; }
    public LineRenderer lineRenderer { get; set; }
    public bool isNetworked { get; set; }
    // Start is called before the first frame update
    void Start()
    {

    }

    public ConnectionCurve Setup(GameObject start, GameObject goal, GameObject LineController, Color color1, Color color2)
    {
        this.start = start;
        this.goal = goal;
        //lineObject.transform.parent = LineController.transform;
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.025f;

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color1, 0.0f), new GradientColorKey(color2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
        return this;
    }

    public ConnectionCurve Setup(GameObject start, GameObject goal, GameObject LineController)
    {
        return Setup(start,goal,LineController, Color.green, Color.green);
    }
}
