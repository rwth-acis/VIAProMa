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
            Vector3 currentHitPosition = MixedRealityToolkit.InputSystem.GazeProvider.HitPosition;
            transform.position = currentHitPosition + up;
            transform.rotation = rot;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, lerpSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, lerpSpeed * Time.deltaTime);
        }
    }
}