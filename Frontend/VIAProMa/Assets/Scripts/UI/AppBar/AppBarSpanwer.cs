using Microsoft.MixedReality.Toolkit.UI;
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
    }
}
