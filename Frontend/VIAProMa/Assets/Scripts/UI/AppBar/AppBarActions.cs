using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AppBarPlacer))]
public class AppBarActions : MonoBehaviour
{
    private AppBarPlacer appBarPlacer;

    private Vector3 startPosition;
    private Quaternion startRotation;

    public bool TargetNetworked { get; set; }

    private void Awake()
    {
        appBarPlacer = GetComponent<AppBarPlacer>();
        if (appBarPlacer == null)
        {
            SpecialDebugMessages.LogComponentNotFoundError(this, nameof(AppBarPlacer), gameObject);
        }
    }

    public void RemoveObject()
    {
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

    public void StartAdjustment()
    {
        startPosition = appBarPlacer.TargetBoundingBox.Target.transform.localPosition;
        startRotation = appBarPlacer.TargetBoundingBox.Target.transform.localRotation;
    }

    public void ResetAdjustment()
    {
        appBarPlacer.TargetBoundingBox.Target.transform.localPosition = startPosition;
        appBarPlacer.TargetBoundingBox.Target.transform.localRotation = startRotation;
    }
}
