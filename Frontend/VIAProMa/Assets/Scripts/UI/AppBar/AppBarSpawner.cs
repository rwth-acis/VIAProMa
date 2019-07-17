using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppBarSpawner : Spawner
{
    [SerializeField] private BoundingBox targetBoundingBox;
    [SerializeField] private GameObject configurationWindow;

    private IWindow configurationWindowInterface;

    protected override void Awake()
    {
        if (configurationWindow == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(configurationWindow));
        }
        configurationWindowInterface = configurationWindow.GetComponent<IWindow>();
        if (configurationWindowInterface == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(IWindow), configurationWindow);
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
        AppBarConfiguration configurationActions = instance.GetComponent<AppBarConfiguration>();
        configurationActions.ConfigurationWindow = configurationWindowInterface;
    }

    private void OnValidate()
    {
        configurationWindowInterface = configurationWindow.GetComponent<IWindow>();
        if (configurationWindowInterface == null)
        {
            configurationWindow = null;
        }
    }
}
