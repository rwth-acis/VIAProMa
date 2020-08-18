using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ConnectionCurve : MonoBehaviour
{

    public GameObject start { get; }
    public GameObject goal { get; set; }
    public LineRenderer lineRenderer { get; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Setup(GameObject start, GameObject goal, GameObject LineController, Color color1, Color color2)
    {
        this.start = start;
        this.goal = goal;
        GameObject lineObject = null;
        if (PhotonNetwork.InRoom)
        {
            void callBack(GameObject test)
            {
                lineObject = test;
            }
            ResourceManager.Instance.SceneNetworkInstantiate(prefab, Vector3.zero, Quaternion.identity, callBack);
            photonView = lineObject.GetComponent<PhotonView>();
            lineRenderer = lineObject.GetComponent<LineRenderer>();
            photonView.RPC("AddConnectionCurve", RpcTarget.Others, start, goal);
        }
        else
        {
            lineObject = new GameObject();
            lineRenderer = lineObject.AddComponent<LineRenderer>();
        }
        //lineObject.transform.parent = LineController.transform;


        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.025f;

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color1, 0.0f), new GradientColorKey(color2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
    }

    public ConnectionCurve(GameObject start, GameObject goal, GameObject LineController, GameObject prefab, Color color1, Color color2)
    {
        this.start = start;
        this.goal = goal;
        GameObject lineObject = null;
        if (PhotonNetwork.InRoom)
        {
            void callBack(GameObject test)
            {
                lineObject = test;
            }
            ResourceManager.Instance.SceneNetworkInstantiate(prefab, Vector3.zero, Quaternion.identity, callBack);
            photonView = lineObject.GetComponent<PhotonView>();
            lineRenderer = lineObject.GetComponent<LineRenderer>();
            photonView.RPC("AddConnectionCurve", RpcTarget.Others, start, goal);
        }
        else
        {
            lineObject = new GameObject();
            lineRenderer = lineObject.AddComponent<LineRenderer>();
        }
        //lineObject.transform.parent = LineController.transform;


        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.025f;

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color1, 0.0f), new GradientColorKey(color2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;

        coroutineData = new CoroutineData();
    }
}
