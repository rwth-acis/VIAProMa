using Microsoft.MixedReality.Toolkit.UI;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppBarSpanwer : Spawner
{
    [SerializeField] protected BoundingBox targetBoundingBox;

    protected override void Awake()
    {
        if (targetBoundingBox == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(targetBoundingBox));
        }

        base.Awake();
    }

    protected override void Setup()
    {
        base.Setup();
        AppBarPlacer placer = instance.GetComponent<AppBarPlacer>();
        if (placer == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(AppBarPlacer), instance);
        }
        placer.TargetBoundingBox = targetBoundingBox;
        AppBarActions actions = instance.GetComponent<AppBarActions>();
        if (actions == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(AppBarActions), instance);
        }
        PhotonView photonView = targetBoundingBox.Target.GetComponent<PhotonView>();
        actions.TargetNetworked = (photonView != null);
    }
}
