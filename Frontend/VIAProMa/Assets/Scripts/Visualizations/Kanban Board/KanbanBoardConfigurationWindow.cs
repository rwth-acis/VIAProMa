using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KanbanBoardConfigurationWindow : ConfigurationWindow
{
    [SerializeField] ConfigurationColorChooser colorChooser;

    public override bool WindowEnabled
    {
        get => base.WindowEnabled;
        set
        {
            base.WindowEnabled = value;
            colorChooser.UIEnabled = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        colorChooser.Setup(visualization);
    }
}
