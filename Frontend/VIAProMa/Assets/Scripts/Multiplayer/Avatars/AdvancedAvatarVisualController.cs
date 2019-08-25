using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedAvatarVisualController : MonoBehaviourPun
{
    /// <summary>
    /// Initializes the component
    /// Deactivates the avatar's visual representation if it is the own avatar
    /// If the avatar represents a remote player, it displays the player's name
    /// </summary>
    private void Start()
    {
        if (photonView.IsMine)
        {
            //SetVisibility(false);
        }
        else
        {
            SetVisibility(true);
        }
    }

    private void SetVisibility(bool visibility)
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(visibility);
        }
    }
}
