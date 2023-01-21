using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppBarRotationCommand : ICommand
{
    private Quaternion startRotation;
    private Quaternion endRotation;
    private GameObject currentObject;
    private i5.VIAProMa.UI.AppBar.AppBarPlacer appBarPlacer;

    public AppBarRotationCommand(Quaternion pStartRotation, i5.VIAProMa.UI.AppBar.AppBarPlacer pAppBarPlacer)
    {
        startRotation = pStartRotation;
        appBarPlacer = pAppBarPlacer;
    }


    public void Execute()
    {
        startRotation = appBarPlacer.TargetBoundingBox.Target.transform.localRotation;

    }

    public void Undo()
    {
        endRotation = appBarPlacer.TargetBoundingBox.Target.transform.localRotation;
        appBarPlacer.TargetBoundingBox.Target.transform.localRotation = startRotation;
    }

    public void Redo()
    {
        appBarPlacer.TargetBoundingBox.Target.transform.localRotation = endRotation;
    }

}
