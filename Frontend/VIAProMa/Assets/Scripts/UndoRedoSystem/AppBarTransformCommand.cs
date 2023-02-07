using UnityEngine;

/// <summary>
/// Command for transforming an object where an AppBar object is attached to.
/// </summary>
public class AppBarTransformCommand : ICommand
{
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Quaternion startRotation;
    private Quaternion endRotation;
    private Vector3 startScale;
    private Vector3 endScale;

    private i5.VIAProMa.UI.AppBar.AppBarPlacer appBarPlacer;

    public AppBarTransformCommand(Vector3 pStartPosition, Quaternion pStartRotation, Vector3 pStartScale, i5.VIAProMa.UI.AppBar.AppBarPlacer pAppBarPlacer)
    {
        startPosition = pStartPosition;
        startRotation = pStartRotation;
        startScale = pStartScale;
        appBarPlacer = pAppBarPlacer;

        endPosition = appBarPlacer.TargetBoundingBox.Target.transform.localPosition;
        endRotation = appBarPlacer.TargetBoundingBox.Target.transform.localRotation;
        endScale = appBarPlacer.TargetBoundingBox.Target.transform.localScale;
    }

    /* -------------------------------------------------------------------------- */

    /// <summary>
    /// Executes the transformation of the object.
    /// </summary>
    public void Execute()
    {
        appBarPlacer.TargetBoundingBox.Target.transform.localPosition = endPosition;
        appBarPlacer.TargetBoundingBox.Target.transform.localRotation = endRotation;
        appBarPlacer.TargetBoundingBox.Target.transform.localScale = endScale;
    }

    /// <summary>
    /// Reverses the transformation of the object. 
    /// </summary>
    public void Undo()
    {
        endPosition = appBarPlacer.TargetBoundingBox.Target.transform.localPosition;
        endRotation = appBarPlacer.TargetBoundingBox.Target.transform.localRotation;
        endScale = appBarPlacer.TargetBoundingBox.Target.transform.localScale;
        appBarPlacer.TargetBoundingBox.Target.transform.localPosition = startPosition;
        appBarPlacer.TargetBoundingBox.Target.transform.localRotation = startRotation;
        appBarPlacer.TargetBoundingBox.Target.transform.localScale = startScale;
    }
}