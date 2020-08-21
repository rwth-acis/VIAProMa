using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ConnectionCurve : MonoBehaviour
{

    public GameObject start { get; set; }
    public GameObject goal { get; set; }
    public bool isTemp { get; set; }
    public LineRenderer lineRenderer;
    LineController lineController;
    // Start is called before the first frame update


    void Start()
    {
        var view = GetComponent<PhotonView>();
        int startID = (int)view.InstantiationData[0];
        int goalID = (int)view.InstantiationData[1];
        start = PhotonNetwork.GetPhotonView(startID).transform.root.gameObject;
        goal = PhotonNetwork.GetPhotonView(goalID).transform.root.gameObject;
        lineController = GameObject.Find("LineController(Clone)").GetComponent<LineController>();

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.025f;

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.green, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;

        lineController.curves.Add(this);
    }

    [PunRPC]
    public ConnectionCurve SetColor(Vector4 color1, Vector4 color2)
    {
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

    private void OnDestroy()
    {
        lineController.curves.Remove(this);
    }
}
