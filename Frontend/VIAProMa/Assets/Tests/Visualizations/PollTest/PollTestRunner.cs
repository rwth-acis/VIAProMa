using i5.VIAProMa.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PollTestRunner : MonoBehaviour
{
    public Vector3 position;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log("Connecting to room", gameObject);
                PhotonNetwork.JoinOrCreateRoom("PollTest", null, null);
            }
        }
        else if (Input.GetKeyDown(KeyCode.F5))
        {
            if (PhotonNetwork.InRoom)
            {
                Debug.Log("Creating Poll");
                WindowManager.Instance.PollMenu.Open(position, Vector3.zero);
            }
        }
    }
}
