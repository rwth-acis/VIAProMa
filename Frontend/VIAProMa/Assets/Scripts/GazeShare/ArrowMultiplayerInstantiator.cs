using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;


public class ArrowMultiplayerInstantiator : InstantiateArrows
{
    protected override void Update()
    {
        if (photonView.IsMine)
        {
            moveMyArrow();
            setColorOfArrow();
        }
        else
        {
            moveOtherArrows();
            setColorOfArrow();
        }
    }
}