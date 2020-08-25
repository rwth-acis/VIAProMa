using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ConnectionCurve : MonoBehaviour
{

    public GameObject start;
    public GameObject goal;

    public string startID = "";
    public string goalID = "";

    public bool isTemp { get; set; }
    public LineRenderer lineRenderer;
    LineController lineController;
    public bool isMarked { get; set; }
    Gradient defaultColor;
    // Start is called before the first frame update


    void Start()
    {
        var view = GetComponent<PhotonView>();
        if (PhotonNetwork.InRoom && view.InstantiationData != null)
        {
            int startID = (int)view.InstantiationData[0];
            int goalID = (int)view.InstantiationData[1];
            start = PhotonNetwork.GetPhotonView(startID).transform.root.gameObject;
            goal = PhotonNetwork.GetPhotonView(goalID).transform.root.gameObject; 
        }
        lineController = GameObject.Find("LineController(Clone)").GetComponent<LineController>();
        defaultColor = lineRenderer.colorGradient;
        lineController.curves.Add(this);
    }

    public void Update()
    {
        var view = GetComponent<PhotonView>();
        if (startID != "" && goalID != "")
        {
            start = SaveLoadManager.Instance.GetRegisterdGameobject(startID);
            goal = SaveLoadManager.Instance.GetRegisterdGameobject(goalID);
            object[] data = new object[2];
            data[0] = start.GetComponent<PhotonView>().InstantiationId;
            data[1] = goal.GetComponent<PhotonView>().InstantiationId;
            view.InstantiationData = data;
        }
        if (start == null && goal == null)
        {
            int startID = (int)view.InstantiationData[0];
            int goalID = (int)view.InstantiationData[1];
            start = PhotonNetwork.GetPhotonView(startID).transform.root.gameObject;
            goal = PhotonNetwork.GetPhotonView(goalID).transform.root.gameObject;
        }
        //enabled = false;
    }

    [PunRPC]
    public void SetColor(float[] color1Arr, float[] color2Arr)
    {
        Color color1 = new Color(color1Arr[0], color1Arr[1], color1Arr[2], color1Arr[3]);
        Color color2 = new Color(color2Arr[0], color2Arr[1], color2Arr[2], color2Arr[3]);
        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color1, 0.0f), new GradientColorKey(color2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
    }

    [PunRPC]
    public void ResetColor()
    {
        lineRenderer.colorGradient = defaultColor;
    }

    private void OnDestroy()
    {
        lineController.curves.Remove(this);
    }
}
