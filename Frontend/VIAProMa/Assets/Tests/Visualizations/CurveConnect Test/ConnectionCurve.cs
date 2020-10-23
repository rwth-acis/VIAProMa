using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Threading.Tasks;

/// <summary>
/// An object to display and manage 3D curves.
/// </summary>
public class ConnectionCurve : MonoBehaviour
{

    public GameObject start;
    public GameObject goal;

    //Serializer IDs. When the curve was not instatiated through the save laod manager, they are always empty.
    public string startID = "";
    public string goalID = "";

    //True, when the curve has another color than the default color
    public bool isMarked { get; set; }

    public LineRenderer lineRenderer;
    Gradient defaultColor;

    void Start()
    {
        //When the IDs are empty, the curve was not instantiated through the SaveLoadManager
        if (startID == "" && goalID == "")
        {
            var view = GetComponent<PhotonView>();
            //When the client is in a room and the instatiation data is not null, the curve was instantiated with the Photon Instantiation and the instantiation data has to be resolved
            if (PhotonNetwork.InRoom && view.InstantiationData != null)
            {
                int startID = (int)view.InstantiationData[0];
                int goalID = (int)view.InstantiationData[1];
                start = PhotonNetwork.GetPhotonView(startID).transform.gameObject;
                goal = PhotonNetwork.GetPhotonView(goalID).transform.gameObject;
            }
            defaultColor = lineRenderer.colorGradient;
            ConnectionCurveManager.Instance.curves.Add(this);
        }
        else
        {
            ResolveSerializerID();
        }
    }

    /// <summary>
    /// Resolves the saved serializer IDs into a start and goal object. It then creates a new curve with the correct objects as instantiation data and then destroyes itself.
    /// </summary>
    private async void ResolveSerializerID()
    {
        GameObject startObject;
        GameObject goalObject;
        bool resolved = false;
        //It can take some time, until the save load manager can resolve the IDs
        do
        {
            startObject = SaveLoadManager.Instance.GetRegisterdGameobject(startID);
            goalObject = SaveLoadManager.Instance.GetRegisterdGameobject(goalID);
            if (startObject == null || goalObject == null)
            {
                await Task.Yield();
            }
            else
            {
                resolved = true;
            }
        }
        while (!resolved);
        object[] data = new object[2];
        data[0] = startObject.GetComponent<PhotonView>().ViewID;
        data[1] = goalObject.GetComponent<PhotonView>().ViewID;
        ResourceManager.Instance.SceneNetworkInstantiate(ConnectionCurveManager.Instance.curveConnectPrefab, Vector3.zero, Quaternion.identity, (x) => { }, data);
        PhotonNetwork.Destroy(GetComponent<PhotonView>());
    } 

    /// <summary>
    /// Sets the color gradient of the line renderer to a gradient from color1Arr to color2Arr. Both need to be provided in the format: (r,g,b,a).
    /// </summary>
    [PunRPC]
    public void SetColor(float[] color1Arr, float[] color2Arr)
    {
        //Using the Color class for color1Arr and color2Arr is not possible, because Color is not supported by photon.
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

    /// <summary>
    /// Sets the color gradient of the line renderer to the default color gradient. The default color gradient is the gradient, the curve was initalised with.
    /// </summary>
    [PunRPC]
    public void ResetColor()
    {
        lineRenderer.colorGradient = defaultColor;
    }

    /// <summary>
    /// Sets the goal the object with the Photon ID viewID.
    /// </summary>
    [PunRPC]
    public void SetGoal(int viewID)
    {
        goal = PhotonNetwork.GetPhotonView(viewID).gameObject;
    }

    private void OnDestroy()
    {
        //Curves remove themself from the curve watch list
        ConnectionCurveManager.Instance?.curves.Remove(this);
    }
}
