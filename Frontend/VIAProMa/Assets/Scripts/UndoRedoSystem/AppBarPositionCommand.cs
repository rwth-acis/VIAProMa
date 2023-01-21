using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppBarPositionCommand : ICommand
{
    private Vector3 startPosition;
    private Vector3 endPosition;
    private GameObject currentObject;
    private i5.VIAProMa.UI.AppBar.AppBarPlacer appBarPlacer;
    //appBarPlacer = currentObject.GetComponent<i5.VIAProMa.UI.AppBar.AppBarPlacer>();

    public AppBarPositionCommand(Vector3 pStartPosition, i5.VIAProMa.UI.AppBar.AppBarPlacer pAppBarPlacer)
    {
        startPosition = pStartPosition;
        appBarPlacer = pAppBarPlacer;
    }


    public void Execute()
    {
        startPosition = appBarPlacer.TargetBoundingBox.Target.transform.localPosition;
    }

    public void Undo()
    {
        endPosition = appBarPlacer.TargetBoundingBox.Target.transform.localPosition;
        appBarPlacer.TargetBoundingBox.Target.transform.localPosition = startPosition;

    }

    public void Redo()
    {
        appBarPlacer.TargetBoundingBox.Target.transform.localPosition = endPosition;
    }

}
