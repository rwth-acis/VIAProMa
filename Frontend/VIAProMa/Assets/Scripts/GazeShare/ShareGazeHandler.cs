﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Monitors the user's share my gaze button
/// </summary>
public class ShareGazeHandler : MonoBehaviourPun, IPunObservable
{
    protected Vector3 far = new Vector3(0f, -10f, 0f);
    protected Vector3 aLittleLeftOpen = new Vector3(-0.05555f, 0f, 0f);
    protected Vector3 aLittleLeftClosed = new Vector3(0f, 0f, -0.05555f);
    protected Vector3 scaleFactor = new Vector3(0.5f, 0.5f, 0.5f);
    protected bool isSharing;
    protected bool targetIsSharing;

    public void Start()
    {
        isSharing = true;
    }

    protected GameObject[] getAllGameObjectsArrow()
    {
        GameObject[] arrayAll = GameObject.FindGameObjectsWithTag("arrow");
        return arrayAll;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.Serialize(ref isSharing);
        }
        else
        {
           //stream.Serialize(ref isSharing);
        }
    }

    /// <summary>
    /// Checks if left side of the main menu cube is opened or closed
    /// by looking at its rotation and sets the share button position
    /// correctly in either case, also sets the rotation, the right scale
    /// and updates the text on the button
    /// </summary>
    protected virtual void Update()
    {
        if (photonView.IsMine)
        {
            if (GameObject.Find("Left") != null)
            {
                if (GameObject.Find("Left").transform.rotation.y < -0.51)
                {
                    transform.position = GameObject.Find("No Gaze Button").transform.position + aLittleLeftOpen;
                }
                else
                {
                    transform.position = GameObject.Find("No Gaze Button").transform.position + aLittleLeftClosed;
                }
                transform.rotation = GameObject.Find("No Gaze Button").transform.rotation;
                transform.localScale = Vector3.Scale(GameObject.Find("No Gaze Button").transform.localScale, scaleFactor);
                setTextOfShareLabel();
            } else { transform.position = far; }
        }
        else
        {
            transform.position = far;
        }
    }

    public void toggleSharing()
    {
        foreach (GameObject arrow in getAllGameObjectsArrow())
        {
            if (arrow.GetComponent<InstantiateArrows>().photonView.OwnerActorNr == photonView.OwnerActorNr)
            {
                arrow.GetComponent<InstantiateArrows>().ToggleSharing();
                isSharing = arrow.GetComponent<InstantiateArrows>().sharing;
            }
        }
    }

    protected GameObject[] getShareGazeLabelObject()
    {
        GameObject[] shareLabelObject = GameObject.FindGameObjectsWithTag("shareLabel");
        return shareLabelObject;
    }

    protected void setTextOfShareLabel()
    {
        foreach (GameObject label in getShareGazeLabelObject())
        {
            if (isSharing == true)
            {
                label.GetComponent<TextMeshPro>().text = "Unshare my Gaze";
            }
            else
            {
                label.GetComponent<TextMeshPro>().text = "Share my Gaze";
            }
        }
    }
}
