using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IntTriple;
using UnityEditor;

public class LineControllScriptFrameShare : MonoBehaviour
{
    public float stepSize = 1;
    public GameObject startObject;
    public GameObject goalObject;
    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    public int maxProcessingTimePerFrame = 25;
    public bool testMode = true;


    GameObject boundContainerStart;
    GameObject boundContainerEnd;

    //TODO delet
    public GameObject[] testObject;

    enum PathAlgorithm
    {
        AStar,
        Greedy,
        HPA
    }

    // Start is called before the first frame update
    void Start()
    {
        //Main line renderer
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.1f;

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;

        boundContainerStart = GenerateCurveBoundingContainer(startObject);
        boundContainerEnd = GenerateCurveBoundingContainer(goalObject);
    }
    // Update is called once per frame
    void Update()
    {      
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        Vector3[] curve = JoinedCurveGeneration.start(startObject.transform.position, goalObject.transform.position, boundContainerStart, boundContainerEnd, stepSize);
        lineRenderer.positionCount = curve.Length;
        lineRenderer.SetPositions(curve);
    }

    GameObject GenerateCurveBoundingContainer(GameObject objectWithBound)
    {
        BoxCollider curveBoundingBox = objectWithBound.transform.Find("Bounding Box").gameObject.GetComponent<BoxCollider>();

        GameObject boundContainer = new GameObject();
        boundContainer.transform.parent = objectWithBound.transform;
        boundContainer.transform.localPosition = Vector3.zero;
        boundContainer.layer = 6;

        boundContainer.AddComponent<BoxCollider>();
        BoxCollider boundingboxOnOtherLayer = boundContainer.GetComponent<BoxCollider>();
        boundingboxOnOtherLayer.name = "CurveBoundingBox";
        boundingboxOnOtherLayer.size = curveBoundingBox.size + new Vector3(0.2f, 0.2f, 0.2f);
        boundingboxOnOtherLayer.center = curveBoundingBox.center;

        return boundContainer;
    }

   
}
