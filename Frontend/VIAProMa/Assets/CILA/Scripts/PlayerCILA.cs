using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;
public class PlayerCILA : MonoBehaviour
{
    public static PlayerCILA instance;
    PhotonView photonView;
    public GameObject requestGO;
    Player requester;
    PhotonMessageInfo photonMessageInfo;
    // Start is called before the first frame update
    void Start()
    {
        requestGO.SetActive(false);
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
            instance = this;
    }
    private void OnEnable()
    {
        requestGO.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
    }

    [PunRPC]
    public void NotifRequest()
    {
        print(photonMessageInfo.Sender);
        requestGO.SetActive(true);
        requester = photonMessageInfo.Sender;
        print(requester);
    }

    [PunRPC]
    public void RequestChart()
    {
        Player target = photonView.Owner;
        var ownId = PhotonNetwork.LocalPlayer.ActorNumber;
        print(target.UserId + " " + ownId);
        photonView.RPC("NotifRequest", target);
    }
    [PunRPC]
    public void Approve()
    {
        requestGO.SetActive(false);
        string emailstudent = StatementManager.instance.nameStudent;
        bool isStudent = StatementManager.instance.isStudent;
        instance.photonView.RPC("Result", requester, emailstudent, isStudent);
    }
    [PunRPC]
    public void Reject()
    {
        requestGO.SetActive(false);
        instance.photonView.RPC("Result", requester, "rejected");
    }
    [PunRPC]
    public void Result(string result, bool isstudent)
    {
        print(result);
        if (result != "rejected")
        {
            if (isstudent)
                StatementManager.instance.studentData(result);
            else
            {
                StatementManager.instance.ManagerData();
            }
        }
    }
}
