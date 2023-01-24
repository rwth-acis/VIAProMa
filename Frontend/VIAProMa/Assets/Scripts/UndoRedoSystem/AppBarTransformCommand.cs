using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppBarTransformCommand : ICommand
{
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Quaternion startRotation;
    private Quaternion endRotation;
    private Vector3 startScale;
    private Vector3 endScale;

    private GameObject currentObject;
    private i5.VIAProMa.UI.AppBar.AppBarPlacer appBarPlacer;
    //appBarPlacer = currentObject.GetComponent<i5.VIAProMa.UI.AppBar.AppBarPlacer>();

    public AppBarTransformCommand(Vector3 pStartPosition, Quaternion pStartRotation, Vector3 pStartScale, i5.VIAProMa.UI.AppBar.AppBarPlacer pAppBarPlacer)
    {
        startPosition = pStartPosition;
        startRotation = pStartRotation;
        startScale = pStartScale;
        appBarPlacer = pAppBarPlacer;
    }


    public void Execute()
    {
        startPosition = appBarPlacer.TargetBoundingBox.Target.transform.localPosition;
    }

    public void Undo()
    {
        endPosition = appBarPlacer.TargetBoundingBox.Target.transform.localPosition;
        endRotation = appBarPlacer.TargetBoundingBox.Target.transform.localRotation;
        endScale = appBarPlacer.TargetBoundingBox.Target.transform.localScale;
        appBarPlacer.TargetBoundingBox.Target.transform.localPosition = startPosition;
        appBarPlacer.TargetBoundingBox.Target.transform.localRotation = startRotation;
        appBarPlacer.TargetBoundingBox.Target.transform.localScale = startScale;
    }

    public void Redo()
    {
        appBarPlacer.TargetBoundingBox.Target.transform.localPosition = endPosition;
        appBarPlacer.TargetBoundingBox.Target.transform.localRotation = endRotation;
        appBarPlacer.TargetBoundingBox.Target.transform.localScale = endScale;
    }
}
