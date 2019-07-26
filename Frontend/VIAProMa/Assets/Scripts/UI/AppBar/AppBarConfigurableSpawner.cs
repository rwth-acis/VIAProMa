using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppBarConfigurableSpawner : AppBarSpanwer
{
    [SerializeField] private GameObject configurationWindow;

    private IWindow configurationWindowInterface;

    protected override void Awake()
    {
        if (configurationWindow == null)
        {
            SpecialDebugMessages.LogMissingReferenceError(this, nameof(configurationWindow));
        }
        configurationWindowInterface = configurationWindow?.GetComponent<IWindow>();
        if (configurationWindowInterface == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(IWindow), configurationWindow);
        }

        base.Awake();
    }

    protected override void Setup()
    {
        base.Setup();
        AppBarConfiguration configurationActions = instance.GetComponent<AppBarConfiguration>();
        configurationActions.ConfigurationWindow = configurationWindowInterface;
    }

    private void OnValidate()
    {
        if (configurationWindow != null)
        {
            configurationWindowInterface = configurationWindow?.GetComponent<IWindow>();
            if (configurationWindowInterface == null)
            {
                configurationWindow = null;
            }
        }
    }
}
