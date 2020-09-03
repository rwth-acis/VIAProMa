using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the application logic and actions which are triggered if the buttons on the app bar are pressed
/// </summary>
[RequireComponent(typeof(AppBarPlacer))]
public class AppBarActions : MonoBehaviour
{
    private AppBarPlacer appBarPlacer;

    public GameObject lineControllerPrefab;
    private LineController lineController;
    public GameObject test1;
    public GameObject test2;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 startScale;

    /// <summary>
    /// True if the target to which the app bar belongs is a networed object (with a PhotonView)
    /// </summary>
    public bool TargetNetworked { get; set; }

    /// <summary>
    /// Checks the component's setup and initializes it
    /// </summary>
    private void Awake()
    {
        appBarPlacer = GetComponent<AppBarPlacer>();
        if (appBarPlacer == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(AppBarPlacer), gameObject);
        }

        GameObject lineObject = GameObject.Find("LineController(Clone)");
        if (lineObject == null)
            lineController = Instantiate(lineControllerPrefab).GetComponent<LineController>();
        else
            lineController = lineObject.GetComponent<LineController>();
    }

    /// <summary>
    /// Destroys the object (either networked or not based on the setting TargetNetworked)
    /// This also destroys the bounding box and finally the app bar
    /// </summary>
    public void RemoveObject()
    {
        //Remove the curves connected with the object
        lineController.DeleteCurves(appBarPlacer.TargetBoundingBox.Target);

        if (TargetNetworked)
        {
            PhotonNetwork.Destroy(appBarPlacer.TargetBoundingBox.Target);
        }
        else
        {
            Destroy(appBarPlacer.TargetBoundingBox.Target);
        }

        // check if the bounding box still exists (in this case it was not a child of the target gameobject)
        if (appBarPlacer.TargetBoundingBox == null)
        {
            Destroy(appBarPlacer.TargetBoundingBox.gameObject);
        }

        // finally also destroy the app bar
        Destroy(gameObject);
    }

    /// <summary>
    /// Puts the app bar into placement mode and stores the current position, rotation and scale for the reset option
    /// </summary>
    public void StartAdjustment()
    {
        startPosition = appBarPlacer.TargetBoundingBox.Target.transform.localPosition;
        startRotation = appBarPlacer.TargetBoundingBox.Target.transform.localRotation;
        startScale = appBarPlacer.TargetBoundingBox.Target.transform.localScale;
    }

    /// <summary>
    /// Resets the target object to its position, rotation and scale to the status when the adjustment was started
    /// </summary>
    public void ResetAdjustment()
    {
        appBarPlacer.TargetBoundingBox.Target.transform.localPosition = startPosition;
        appBarPlacer.TargetBoundingBox.Target.transform.localRotation = startRotation;
        appBarPlacer.TargetBoundingBox.Target.transform.localScale = startScale;
    }

    public void Connect()
    {
        //lineController.SendMessage("StartConnecting", appBarPlacer.TargetBoundingBox.Target);
        lineController.ChangeState(LineController.State.connecting, appBarPlacer.TargetBoundingBox.Target, gameObject.GetComponent<AppBarStateController>());
    }

    public void Disconnect()
    {
        //lineController.SendMessage("StartDisconnecting");
        lineController.ChangeState(LineController.State.disconnecting);
    }
}
