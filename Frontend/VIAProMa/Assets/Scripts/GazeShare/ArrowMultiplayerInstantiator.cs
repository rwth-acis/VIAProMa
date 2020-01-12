using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class ArrowMultiplayerInstantiator : InstantiateArrows
{
    protected override void Update()
    {
        if (photonView.IsMine)
        {
            moveMyArrow();
        }
        else
        {
            moveOtherArrows();
        }
    }
}