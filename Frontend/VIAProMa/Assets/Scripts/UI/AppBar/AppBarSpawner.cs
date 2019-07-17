using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppBarSpawner : Spawner
{
    [SerializeField] private BoundingBox targetBoundingBox;

    protected override void Setup()
    {
        base.Setup();
        AppBarPlacer placer = instance.GetComponent<AppBarPlacer>();
        if (placer == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(AppBarPlacer), instance);
        }
        placer.TargetBoundingBox = targetBoundingBox;
    }
}
