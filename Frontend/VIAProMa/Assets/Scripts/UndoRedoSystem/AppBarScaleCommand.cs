using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppBarScaleCommand : ICommand
{

    private Vector3 startScale;
    private Vector3 endScale;
    private GameObject currentObject;
    private i5.VIAProMa.UI.AppBar.AppBarPlacer appBarPlacer;

    public AppBarScaleCommand(Vector3 pStartScale, i5.VIAProMa.UI.AppBar.AppBarPlacer pAppBarPlacer)
    {
        startScale = pStartScale;
        appBarPlacer = pAppBarPlacer;
    }


    public void Execute()
    {
        startScale = appBarPlacer.TargetBoundingBox.Target.transform.localScale;
    }

    public void Undo()
    {
        endScale = appBarPlacer.TargetBoundingBox.Target.transform.localScale;
        appBarPlacer.TargetBoundingBox.Target.transform.localScale = startScale;
    }

    public void Redo()
    {
        appBarPlacer.TargetBoundingBox.Target.transform.localScale = endScale;
    }

}
