using i5.ViaProMa.UI;
using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class InformationButtonConfigurator : MonoBehaviour
{
    [SerializeField] private Interactable InformationButton;
    public bool status {get; set;} = true;
    //private PhotonView photonView;
    public event EventHandler ConfigurationChanged;

    private void Awake()
    {
        //photonView = GetComponent<PhotonView>();

        if (UserManager.Instance.UserRole == UserRoles.TUTOR)
        {
            status = false;
        }
    }

    private void Start()
    {
        /*
        if (photonView.Owner.NickName == PhotonNetwork.NickName)
        {
            InformationButtonSynchronizer synch = (InformationButtonSynchronizer) InformationButton.GetComponent(typeof(InformationButtonSynchronizer));
            synch.Initial();
        }
        */
    }

    public async void Update()
    {
        InformationButton.Enabled = status;
    }

}