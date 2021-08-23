using UnityEngine;
using Photon.Pun;
using i5.VIAProMa.WebConnection;

public class EvaluationTestscript : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update


    private void Start()
    {
        ConnectionManager.Instance.BackendAddress = "http://cloud17.dbis.rwth-aachen.de";
    }

    override public void OnJoinedLobby()
    {
        Debug.Log("Connecting to room");
        PhotonNetwork.JoinOrCreateRoom("SebEvalRoom", null, null);
    }
}
