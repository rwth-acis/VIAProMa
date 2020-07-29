using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public static float stepSize = 1;
    public static int curveSegmentCount = 60;
    List<ConnectionCurve> curves;

    //Test
    public GameObject start;
    public GameObject goal;

    // Start is called before the first frame update
    void Start()
    {
        curves = new List<ConnectionCurve>();

        //Test
        AddConnectionCurve(start,goal);
    }

    // Update is called once per frame
    void Update()
    {
        foreach(ConnectionCurve connectionCurve in curves)
        {
            Vector3[] curve = JoinedCurveGeneration.start(connectionCurve.start, connectionCurve.goal, stepSize, curveSegmentCount);
            connectionCurve.lineRenderer.positionCount = curve.Length;
            connectionCurve.lineRenderer.SetPositions(curve);
        }
    }

    void AddConnectionCurve(GameObject start, GameObject goal)
    {
        curves.Add(new ConnectionCurve(start, goal, this.gameObject));
    }
}

public class ConnectionCurve
{
    public GameObject start { get; }
    public GameObject goal { get; }
    public LineRenderer lineRenderer { get; }

    public ConnectionCurve(GameObject start, GameObject goal, GameObject LineController)
    {
        this.start = start;
        this.goal = goal;

        GameObject lineObject = new GameObject();
        lineObject.transform.parent = LineController.transform;
        lineRenderer = lineObject.AddComponent<LineRenderer>();

        Color c1 = Color.yellow;
        Color c2 = Color.red;

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.1f;

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;

    }

    //GameObject GenerateCurveBoundingContainer(GameObject objectWithBound)
    //{
    //    BoxCollider curveBoundingBox = objectWithBound.transform.Find("Bounding Box").gameObject.GetComponent<BoxCollider>();

    //    GameObject boundContainer = new GameObject();
    //    boundContainer.transform.parent = objectWithBound.transform;
    //    boundContainer.transform.localPosition = Vector3.zero;
    //    boundContainer.layer = 6;

    //    boundContainer.AddComponent<BoxCollider>();
    //    BoxCollider boundingboxOnOtherLayer = boundContainer.GetComponent<BoxCollider>();
    //    boundingboxOnOtherLayer.name = "CurveBoundingBox";
    //    boundingboxOnOtherLayer.size = curveBoundingBox.size + new Vector3(0.2f, 0.2f, 0.2f);
    //    boundingboxOnOtherLayer.center = curveBoundingBox.center;

    //    return boundContainer;
    //}
}
